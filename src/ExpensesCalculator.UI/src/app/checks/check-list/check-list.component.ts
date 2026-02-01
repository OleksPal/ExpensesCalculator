import { Component, Input, Output, EventEmitter, OnInit, OnChanges, OnDestroy, SimpleChanges, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ModalWindowComponent } from '../../shared/modal-window/modal-window.component';
import { ValidationErrors, parseValidationErrors } from '../../shared/models/validation-errors.model';
import { ChecksService, Check } from '../../services/checks.service';
import { ItemListComponent } from '../../items/item-list/item-list.component';
import { CheckSumUpdateService } from '../../services/check-sum-update.service';
import { DayExpensesTotalSumUpdateService } from '../../services/day-expenses-total-sum-update.service';
import { ToastService } from '../../services/toast.service';
import { FormValidationService } from '../../services/form-validation.service';
import { FilterBarComponent, FilterOption } from '../../shared/filter-bar/filter-bar.component';
import { Subscription } from 'rxjs';
import { TourAnchorNgBootstrapDirective } from 'ngx-ui-tour-ng-bootstrap';

declare var bootstrap: any;

@Component({
  selector: 'app-check-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalWindowComponent, TranslatePipe, ItemListComponent, FilterBarComponent, TourAnchorNgBootstrapDirective],
  templateUrl: './check-list.component.html',
  styleUrl: './check-list.component.css'
})
export class CheckListComponent implements OnInit, OnChanges, OnDestroy {
  @Input() dayExpensesId!: string;
  @Input() participants: string[] = [];
  @Input() scrollToCheckId?: string;
  @Output() checksLoaded = new EventEmitter<void>();

  // Data properties
  checksList: Check[] = [];
  filteredChecksList: Check[] = [];
  expandedCheckIds: Set<string> = new Set();

  // Modal properties
  modalInstance: any;
  currentModalContent: 'add' | 'edit' | 'delete' = 'add';
  modalTitle: string = '';

  // Form properties
  id = '';
  location = '';
  payer = '';
  currentCheckTotalSum = 0;

  // Validation properties
  formErrors: ValidationErrors = {};
  formValidated = false;

  // Filter and sort properties
  filterText = '';
  filterCriteria: string = 'Location';
  filterOptions: FilterOption[] = [
    { value: 'Location', labelKey: 'CHECKS.FILTER.LOCATION' },
    { value: 'Payer', labelKey: 'CHECKS.FILTER.PAYER' },
    { value: 'Sum', labelKey: 'CHECKS.FILTER.SUM' }
  ];
  sortColumn: 'location' | 'totalSum' | 'payer' = 'totalSum';
  sortOrder: 'asc' | 'desc' = 'desc';

  // UI state properties
  isLoading = false;

  // Subscription for check sum updates
  private checkSumUpdateSub?: Subscription;

  // Scroll to check flag
  private shouldScrollToCheck = false;

  // Inject services using inject() for better SSR compatibility
  private checkSumUpdateService = inject(CheckSumUpdateService);
  private dayExpensesTotalSumUpdateService = inject(DayExpensesTotalSumUpdateService);

  constructor(
    private checksService: ChecksService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService,
    private formValidationService: FormValidationService
  ) {}

  ngOnInit(): void {
    if (this.dayExpensesId) {
      this.loadChecks();
    }

    // Check if we should scroll to a specific check
    if (this.scrollToCheckId) {
      this.shouldScrollToCheck = true;
    }

    // Subscribe to check sum updates
    this.checkSumUpdateSub = this.checkSumUpdateService.checkSumUpdates.subscribe(
      (update) => this.onCheckSumUpdated(update)
    );
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['dayExpensesId'] && !changes['dayExpensesId'].firstChange) {
      this.loadChecks();
    }
  }

  ngOnDestroy(): void {
    if (this.checkSumUpdateSub) {
      this.checkSumUpdateSub.unsubscribe();
    }
  }

  // Data loading methods
  loadChecks(): void {
    if (!this.dayExpensesId) return;

    this.isLoading = true;
    this.checksService.getAllDayExpensesChecks(this.dayExpensesId).subscribe({
      next: (data) => {
        this.checksList = data;
        this.applyLocalFiltering();
        this.isLoading = false;

        // Emit event to parent to re-initialize tour
        this.checksLoaded.emit();

        // Scroll to check if needed
        if (this.shouldScrollToCheck && this.scrollToCheckId) {
          this.scrollToCheck(this.scrollToCheckId);
          this.shouldScrollToCheck = false;
        }
      },
      error: (err) => {
        console.error('Error loading checks:', err);
        this.isLoading = false;
      }
    });
  }

  // Local filtering and sorting methods
  onFilterChange(filterText: string): void {
    this.filterText = filterText;
    this.applyLocalFiltering();
  }

  changeFilterCriteria(criteria: string): void {
    this.filterCriteria = criteria;
    this.applyLocalFiltering();
  }

  sortChecks(column: 'location' | 'totalSum' | 'payer'): void {
    if (this.sortColumn === column) {
      this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortOrder = 'asc';
    }
    this.applyLocalSorting();
  }

  applyLocalFiltering(): void {
    const searchTerm = this.filterText.toLowerCase().trim();

    if (!searchTerm) {
      this.filteredChecksList = [...this.checksList];
    } else {
      this.filteredChecksList = this.checksList.filter(check => {
        if (this.filterCriteria === 'Location') {
          return check.location.toLowerCase().includes(searchTerm);
        } else if (this.filterCriteria === 'Payer') {
          return check.payer.toLowerCase().includes(searchTerm);
        } else if (this.filterCriteria === 'Sum') {
          return check.totalSum.toFixed(2).includes(searchTerm);
        }
        return false;
      });
    }

    this.applyLocalSorting();
  }

  applyLocalSorting(): void {
    this.filteredChecksList.sort((a, b) => {
      let valueA: any = a[this.sortColumn];
      let valueB: any = b[this.sortColumn];

      // Handle string comparison for location and payer
      if (typeof valueA === 'string') {
        valueA = valueA.toLowerCase();
        valueB = valueB.toLowerCase();
      }

      if (valueA < valueB) return this.sortOrder === 'asc' ? -1 : 1;
      if (valueA > valueB) return this.sortOrder === 'asc' ? 1 : -1;
      return 0;
    });
  }

  // Row expansion methods
  toggleCheckExpansion(id: string): void {
    const collapseElement = document.getElementById('collapse' + id);
    if (collapseElement) {
      const bsCollapse = new bootstrap.Collapse(collapseElement, { toggle: false });

      if (this.expandedCheckIds.has(id)) {
        this.expandedCheckIds.delete(id);
        bsCollapse.hide();
      } else {
        this.expandedCheckIds.add(id);
        bsCollapse.show();
      }
    }
  }

  isCheckExpanded(id: string): boolean {
    return this.expandedCheckIds.has(id);
  }

  // Modal management methods
  openModal(type: 'add' | 'edit' | 'delete', id: string = ''): void {
    this.currentModalContent = type;
    this.modalTitle = this.translate.instant(`CHECKS.MODAL.${type.toUpperCase()}_TITLE`);

    const modalElement = document.getElementById('checksModal');
    if (!modalElement) return;

    if (type === 'add') {
      this.clearFormData();
    } else if (id) {
      // Load check data from local array (no server call needed)
      const check = this.checksList.find(c => c.id === id);
      if (check) {
        this.id = check.id;
        this.location = check.location;
        this.payer = check.payer;
        this.currentCheckTotalSum = check.totalSum;
      }
    }

    if (!this.modalInstance) {
      this.modalInstance = new bootstrap.Modal(modalElement, {
        backdrop: 'static',
        keyboard: false
      });
    }

    this.formErrors = {};
    this.formValidated = false;

    this.modalInstance.show();
  }

  hideModal(): void {
    if (this.modalInstance) {
      this.modalInstance.hide();
      this.formErrors = {};
      this.formValidated = false;
      this.clearFormData();
    }
  }

  clearFormData(): void {
    this.id = '';
    this.location = '';
    this.payer = '';
    this.currentCheckTotalSum = 0;
  }

  // CRUD operations
  validateCheckForm(): boolean {
    this.formErrors = {};
    this.formValidated = true;

    this.formErrors = this.formValidationService.validateCheckForm(this.location, this.payer);

    return !this.formValidationService.hasErrors(this.formErrors);
  }

  createCheck(): void {
    if (!this.validateCheckForm()) return;
    this.formValidated = true;

    this.checksService.createCheck(this.location, this.payer, this.dayExpensesId).subscribe({
      next: () => {
        this.hideModal();
        this.loadChecks();
        this.toastService.success(
          this.translate.instant('CHECKS.TOAST.SUCCESS'),
          this.translate.instant('CHECKS.TOAST.CREATE_SUCCESS')
        );
      },
      error: error => {
        this.formErrors = parseValidationErrors(error);
        this.formValidated = true;
        if (Object.keys(this.formErrors).length === 0 || this.formErrors['general']) {
          const errorMessage = this.formErrors['general'] || error?.error?.message || error?.message || this.translate.instant('CHECKS.TOAST.CREATE_ERROR');
          this.toastService.error(
            this.translate.instant('CHECKS.TOAST.ERROR'),
            this.translateBackendError(errorMessage)
          );
        }
      }
    });
  }

  editCheck(): void {
    if (!this.validateCheckForm()) return;
    this.formValidated = true;

    this.checksService.editCheck(this.id, this.location, this.payer, this.dayExpensesId).subscribe({
      next: () => {
        this.hideModal();
        this.loadChecks();
        this.toastService.success(
          this.translate.instant('CHECKS.TOAST.SUCCESS'),
          this.translate.instant('CHECKS.TOAST.EDIT_SUCCESS')
        );
      },
      error: error => {
        this.formErrors = parseValidationErrors(error);
        this.formValidated = true;
        if (Object.keys(this.formErrors).length === 0 || this.formErrors['general']) {
          const errorMessage = this.formErrors['general'] || error?.error?.message || error?.message || this.translate.instant('CHECKS.TOAST.EDIT_ERROR');
          this.toastService.error(
            this.translate.instant('CHECKS.TOAST.ERROR'),
            this.translateBackendError(errorMessage)
          );
        }
      }
    });
  }

  deleteCheck(): void {
    this.checksService.deleteCheck(this.id).subscribe({
      next: () => {
        this.hideModal();
        this.loadChecks();
        this.toastService.success(
          this.translate.instant('CHECKS.TOAST.SUCCESS'),
          this.translate.instant('CHECKS.TOAST.DELETE_SUCCESS')
        );
      },
      error: error => {
        console.log(error);
        const errorMessage = error?.error?.message || error?.message || this.translate.instant('CHECKS.TOAST.DELETE_ERROR');
        this.toastService.error(
          this.translate.instant('CHECKS.TOAST.ERROR'),
          this.translateBackendError(errorMessage)
        );
      }
    });
  }

  // Helper methods
  translateBackendError(errorMessage: string): string {
    if (!errorMessage) return '';

    const errorMap: Record<string, string> = {
      'Invalid data': 'CHECKS.BACKEND_ERRORS.INVALID_DATA',
      'Unauthorized': 'CHECKS.BACKEND_ERRORS.UNAUTHORIZED'
    };

    const translationKey = errorMap[errorMessage];
    if (translationKey) {
      return this.translate.instant(translationKey);
    }

    for (const [key, value] of Object.entries(errorMap)) {
      if (errorMessage.toLowerCase().includes(key.toLowerCase())) {
        return this.translate.instant(value);
      }
    }

    return errorMessage;
  }

  getIconOrderClass(column: string): string {
    if (this.sortColumn !== column) {
      return 'bi bi-funnel-fill ps-1';
    }
    return this.sortOrder === 'asc' ? 'bi bi-arrow-up ps-1' : 'bi bi-arrow-down ps-1';
  }

  // Handle check sum updates from item-list component
  onCheckSumUpdated(event: { checkId: string, newSum: number }): void {
    const check = this.checksList.find(c => c.id === event.checkId);
    if (check) {
      check.totalSum = event.newSum;
      // Re-apply filtering and sorting to update the display
      this.applyLocalFiltering();
      // Manually trigger change detection
      this.cdr.detectChanges();

      // Calculate and emit day expenses total sum
      const dayExpensesTotalSum = this.checksList.reduce((sum, c) => sum + c.totalSum, 0);
      this.dayExpensesTotalSumUpdateService.emitDayExpensesTotalSumUpdate(this.dayExpensesId, dayExpensesTotalSum);
    }
  }

  // Scroll to specific check
  private scrollToCheck(checkId: string): void {
    setTimeout(() => {
      // Expand the check if not already expanded
      if (!this.expandedCheckIds.has(checkId)) {
        this.toggleCheckExpansion(checkId);
      }

      // Wait for expansion animation then scroll
      setTimeout(() => {
        const checkElement = document.querySelector(`[data-check-id="${checkId}"]`) as HTMLElement;
        if (checkElement) {
          // Get element position
          const elementRect = checkElement.getBoundingClientRect();
          const absoluteElementTop = elementRect.top + window.scrollY;
          // Scroll with offset to show header/previous row (approximately 80px above)
          window.scrollTo({
            top: absoluteElementTop - 80,
            behavior: 'smooth'
          });
        }
      }, 400);
    }, 200);
  }
}

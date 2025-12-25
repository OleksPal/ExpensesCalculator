import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExpensesService, DayExpenses } from '../../services/expenses.service';
import { ModalWindowComponent } from "../../shared/modal-window/modal-window.component";
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import flatpickr from 'flatpickr';
import { Ukrainian } from 'flatpickr/dist/l10n/uk';

declare var bootstrap: any;

@Component({
  selector: 'app-day-expenses-list',
  standalone: true,
  imports: [FormsModule, CommonModule, ModalWindowComponent, TranslatePipe],
  providers: [DatePipe],
  templateUrl: './day-expenses-list.component.html',
  styleUrl: './day-expenses-list.component.css'
})
export class DayExpensesListComponent implements OnInit, AfterViewInit, OnDestroy {
  private flatpickrInitialized = false;
  private flatpickrInstance: any;
  private modalFlatpickrInstance: any;
  private settingDatesFromApi = false;
  private langChangeSub!: Subscription;

  private langToLocale(lang: string): string {
    return lang === 'ua' ? 'uk' : 'en';
  }

  ngAfterViewInit() {
    this.currentLocale = this.langToLocale(this.translate.currentLang);
    this.tryInitializeFlatpickr();
    this.initializeTooltips();

    this.langChangeSub = this.translate.onLangChange.subscribe((event) => {
      this.currentLocale = this.langToLocale(event.lang);
      this.destroyTooltips();
      setTimeout(() => this.initializeTooltips(), 0);

      if (this.flatpickrInstance) {
        this.flatpickrInstance.set('locale', {
          ...(event.lang === 'ua' ? Ukrainian : {}),
          rangeSeparator: ' - '
        });
        if (this.fromDate && this.toDate) {
          this.setFlatpickrDates(this.fromDate, this.toDate);
        }
      }

      if (this.modalFlatpickrInstance) {
        this.modalFlatpickrInstance.set('locale', {
          ...(event.lang === 'ua' ? Ukrainian : {})
        });
      }
    });
  }

  initializeTooltips() {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltipTriggerList.forEach(tooltipTriggerEl => {
      new bootstrap.Tooltip(tooltipTriggerEl, {
        html: true
      });
    });
  }

  destroyTooltips() {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltipTriggerList.forEach(tooltipTriggerEl => {
      const existing = bootstrap.Tooltip.getInstance(tooltipTriggerEl);
      if (existing) existing.dispose();
    });
  }

  ngOnDestroy(): void {
    if (this.flatpickrInstance) {
      this.flatpickrInstance.destroy();
    }
    this.destroyModalFlatpickr();
    if (this.langChangeSub) {
      this.langChangeSub.unsubscribe();
    }
  }

  firstDateRangeChange: boolean = true;

  // Flatpickr
  tryInitializeFlatpickr() {
    if (this.flatpickrInitialized) return;
    
    const element = document.getElementById('dateRangeInput');
    const calendarButton = document.getElementById('calendarButton');

    if (element) {
      this.flatpickrInstance = flatpickr(element, {
        mode: 'range',
        dateFormat: 'Y-m-d',
        altInput: true,
        altFormat: 'M d, Y',
        locale: {
          ...(this.translate.currentLang === 'ua' ? Ukrainian : {}),
          rangeSeparator: ' - '
        },

        onReady: (selectedDates, dateStr, instance) => {
          if (instance.altInput) {
            instance.altInput.style.width = '180px';
            instance.altInput.style.height = '37px';
            instance.altInput.style.outline = 'none';
            instance.altInput.style.boxShadow = 'none';
            instance.altInput.style.color = 'white';
            instance.altInput.style.cursor = 'pointer';
            instance.altInput.value = 'All';
          }

          if (calendarButton) {
            calendarButton.addEventListener('click', () => {
              instance.open();
            });
          }
        },

        onChange: (dates: Date[]) => {
          if (this.settingDatesFromApi) return;

          if (dates.length === 2) {
            this.fromDate = this.formatFlatPickrDate(dates[0]);
            this.toDate = this.formatFlatPickrDate(dates[1]);

            this.loadExpenses();
          }
        }
      });
      this.flatpickrInitialized = true;
    }
  }

  setFlatpickrDates(fromDate: string, toDate: string): void {
    if (this.flatpickrInstance) {
      try {
        this.settingDatesFromApi = true;
        const startDate = new Date(fromDate);
        const endDate = new Date(toDate);
        this.flatpickrInstance.setDate([startDate, endDate], true);
      } catch (error) {
        console.error('Error setting flatpickr dates:', error);
      } finally {
        this.settingDatesFromApi = false;
      }
    }
  }

  initModalFlatpickr(readonly: boolean = false) {
    this.destroyModalFlatpickr();

    const element = document.getElementById('modalDateInput');
    if (!element) return;

    this.modalFlatpickrInstance = flatpickr(element, {
      dateFormat: 'Y-m-d',
      altInput: true,
      altFormat: 'M d, Y',
      locale: {
        ...(this.translate.currentLang === 'ua' ? Ukrainian : {})
      },
      defaultDate: this.date || undefined,
      clickOpens: !readonly,
      allowInput: false,
      onReady: (_selectedDates: Date[], _dateStr: string, instance: any) => {
        if (!readonly && instance.altInput) {
          instance.altInput.style.cursor = 'pointer';
        }
      },
      onChange: (dates: Date[]) => {
        if (dates.length > 0) {
          this.date = this.formatFlatPickrDate(dates[0]);
        }
      }
    });
  }

  destroyModalFlatpickr() {
    if (this.modalFlatpickrInstance) {
      this.modalFlatpickrInstance.destroy();
      this.modalFlatpickrInstance = null;
    }
  }

  clearDateRange(): void {
    if (this.flatpickrInstance) {
      this.currentPage = 1;
      this.loadExpenses('', '');
    }
  }

  parseDateOnly(date: any): string {
    if (typeof date === 'string') {
      return date; // Already a string
    }
    // If it's an object with year, month, day properties
    if (date.year && date.month && date.day) {
      const month = String(date.month).padStart(2, '0');
      const day = String(date.day).padStart(2, '0');
      return `${date.year}-${month}-${day}`;
    }
    return date.toString();
  }

  formatFlatPickrDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  currentLocale: string = 'en';

  fromDate: string = '';
  toDate: string = '';

  expensesList: DayExpenses[] = [];

  totalPages: number = 0;
  currentPage: number = 1;
  pageSize: number = 10;

  // Expenses data
  id = '';
  date = '';
  location = '';
  participants = '';

  // Add new user with access
  newUserWithAccess = '';
  shareIsSuccess = false;
  shareError = '';

  // Searching
  filterText = '';

  // Sorting
  sortColumn: 'date' | 'location' | 'participants' | 'totalSum' = 'date';
  sortOrder: 'asc' | 'desc' = 'desc';

  constructor(
    private expensesService: ExpensesService,
    private router: Router,
    private datePipe: DatePipe,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.loadExpenses();    
  }

  loadExpenses(fromDate = this.fromDate, toDate = this.toDate) {
    this.isLoading = true;

    this.expensesService.getAllDayExpenses(this.sortColumn, this.sortOrder,
      this.filterText, this.filterCriteria,
      fromDate, toDate,
      this.currentPage, this.pageSize).subscribe({
      next: (data) => {
        this.expensesList = data.items;
        this.totalPages = data.totalPages;
        this.fromDate = this.parseDateOnly(data.fromDate);
        this.toDate = this.parseDateOnly(data.toDate);

        // Initialize flatpickr
        if (!this.flatpickrInitialized) {
          setTimeout(() => {
            this.tryInitializeFlatpickr();

            // Set the initial dates in flatpickr
            if (this.fromDate && this.toDate) {
              this.setFlatpickrDates(this.fromDate, this.toDate);
            }
          }, 100);
        }
        else {
          this.setFlatpickrDates(this.fromDate, this.toDate);
        }

        // Re-initialize tooltips after data loads
        setTimeout(() => this.initializeTooltips(), 0);

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error day expenses list:', err);
        this.isLoading = false;
      }
    })
  }

  loadSingleExpenses(id: string) {
    this.expensesService.getDayExpenses(id).subscribe({
      next: (data) => {
        this.id = data.id
        this.date = (new Date(data.date)).toISOString().substring(0, 10)
        this.location = data.location
        this.participants = data.participants.join(', ')

        if (this.modalFlatpickrInstance) {
          this.modalFlatpickrInstance.setDate(this.date, true);
        }
      },
      error: (err) => console.error('Error day expenses:', err)
    })
  }

  modalInstance: any;
  currentModalContent: 'add' | 'edit' | 'delete' | 'share' = 'add';
  modalTitle: string = '';

  private modalTitleKeys: Record<string, string> = {
    add: 'EXPENSES.MODAL.ADD_TITLE',
    edit: 'EXPENSES.MODAL.EDIT_TITLE',
    delete: 'EXPENSES.MODAL.DELETE_TITLE',
    share: 'EXPENSES.MODAL.SHARE_TITLE'
  };

  openModal(type: 'add' | 'edit' | 'delete' | 'share', id: string) {
    this.currentModalContent = type;
    this.modalTitle = this.translate.instant(this.modalTitleKeys[type]);

    const modalElement = document.getElementById('staticBackdrop');
    if (!modalElement) return;

    if (type !== 'add' && id !== undefined)
      this.loadSingleExpenses(id);

    if (!this.modalInstance) {
      this.modalInstance = new bootstrap.Modal(modalElement, {
        backdrop: 'static',
        keyboard: false
      });
    }

    this.modalInstance.show();

    const readonly = type === 'delete' || type === 'share';
    setTimeout(() => this.initModalFlatpickr(readonly), 0);
  }

  hideModal() {
    if (this.modalInstance) {
      this.destroyModalFlatpickr();
      this.modalInstance.hide();

      this.date = '';
      this.location = '';
      this.participants = '';
    }
  }  

  // Filtering
  filterCriteria: string = 'Location';
  isLoading = false;

  get filterCriteriaKey(): string {
    const keyMap: Record<string, string> = {
      'Location': 'EXPENSES.FILTER.LOCATION',
      'Participants': 'EXPENSES.FILTER.PARTICIPANTS'
    };
    return keyMap[this.filterCriteria] || 'EXPENSES.FILTER.LOCATION';
  }

  onFilterChange(): void {
    this.loadExpenses();
  }

  changeFilterCriteria(criteria: string) {
    this.filterCriteria = criteria;
    this.loadExpenses();
  }

  formatDate(date: string | Date): string {
    return this.datePipe.transform(date, 'MMM dd, yyyy') ?? '';
  }

  // Sorting
  sortExpenses(column: 'date' | 'location' | 'participants' | 'totalSum') {
    if (this.sortColumn === column) {
      // Toggle direction if the same column is clicked again
      this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortOrder = 'asc';
    }

    this.currentPage = 1;

    this.loadExpenses();
  }

  getIconOrderClass(column: 'date' | 'location' | 'participants' | 'totalSum') {
    if (this.sortColumn !== column)
      return 'ps-1 bi bi-funnel-fill'
    else
      return this.sortOrder == 'asc' ? 'ps-1 bi-arrow-up' : 'ps-1 bi-arrow-down'
  }

  // Tooltips
  getTooltipContent(id: string) {
    let participants = this.expensesList.find(d => d.id === id)?.participants || []

    const maxDisplay = 3;
    const displayUsers = participants?.slice(0, maxDisplay);
    const moreCount = participants?.length > maxDisplay ? participants?.length - maxDisplay : 0

    let content = `<i class="bi bi-people-fill me-1"></i><span class="fw-bold">${this.translate.instant('EXPENSES.TOOLTIP.PARTICIPANTS_TITLE')}</span><br/>`;
    displayUsers?.forEach((participant) => {
      content += `<i class="bi bi-person-fill me-1"></i> ${this.trimText(participant)}<br>`;
    });

    if (moreCount > 0) {
      content += this.translate.instant('EXPENSES.TOOLTIP.AND_MORE', { count: moreCount });
    }
    content += '</div>';

    return content;
  }

  trimText(text: string, maxLength = 8): string {
    if (!text) return '';
    return text.length > maxLength
      ? text.substring(0, maxLength) + '...'
      : text;
  }

  translateBackendError(errorMessage: string): string {
    if (!errorMessage) return '';

    // Map common backend error messages to translation keys
    const errorMap: Record<string, string> = {
      'User not found': 'EXPENSES.BACKEND_ERRORS.USER_NOT_FOUND',
      'Invalid data': 'EXPENSES.BACKEND_ERRORS.INVALID_DATA',
      'Unauthorized': 'EXPENSES.BACKEND_ERRORS.UNAUTHORIZED',
      'Already shared': 'EXPENSES.BACKEND_ERRORS.ALREADY_SHARED',
      'already has access': 'EXPENSES.BACKEND_ERRORS.ALREADY_HAS_ACCESS'
    };

    // Check if we have a translation for this error
    const translationKey = errorMap[errorMessage];
    if (translationKey) {
      return this.translate.instant(translationKey);
    }

    // If no exact match, check for partial matches
    for (const [key, value] of Object.entries(errorMap)) {
      if (errorMessage.toLowerCase().includes(key.toLowerCase())) {
        return this.translate.instant(value);
      }
    }

    // Return original message if no translation found
    return errorMessage;
  }

  // Toasts
  showToast = false;
  toastMessage = '';
  eventDate = '';
  toastHeaderText = '';
  status = '';

  showBootstrapToast(message: string, header: string, status: 'success' | 'danger', duration: number = 3000) {
    this.toastMessage = message;
    this.showToast = true;
    this.eventDate = this.formatDateTime(Date());
    this.toastHeaderText = header;
    this.status = 'bg-' + status;

    setTimeout(() => {
      this.showToast = false;
    }, duration);
  }

  hideToast() {
    this.showToast = false;
  }

  formatDateTime(date: string | Date): string {
    const formatted = this.datePipe.transform(date, 'MMM dd, yyyy HH:mm:ss', undefined, this.currentLocale) ?? '';
    return formatted.charAt(0).toUpperCase() + formatted.slice(1);
  }

  // Pagination
  getPageNumbers(): number[] {
    const maxVisible = 5;
    const pages: number[] = [];
    
    if (this.totalPages <= maxVisible) {
      // Show all pages if total is small
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Show smart range around current page
      let start = Math.max(1, this.currentPage - 2);
      let end = Math.min(this.totalPages, this.currentPage + 2);
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
    }
    return pages;
  }

  changePageSize(size: number): void {
    if (size !== this.pageSize)
    {
      this.currentPage = 1;
      this.pageSize = size;
      this.loadExpenses();
    }    
  }

  goToPage(pageNumber: number = 1){
    this.currentPage = pageNumber;
    this.loadExpenses();
  }

  // Data modification
  createDayExpenses() {
    let participantsList = this.participants.split(',').map(p => p.trim())

    this.expensesService.createDayExpenses(this.date, this.location, participantsList).subscribe({
      next: () => {
        this.hideModal();
        this.loadExpenses('', '');

        this.date = '';
        this.location = '';
        this.participants = '';

        this.showBootstrapToast(
          this.translate.instant('EXPENSES.TOAST.CREATE_SUCCESS'),
          this.translate.instant('EXPENSES.TOAST.SUCCESS'), 'success');
      },
      error: error => {
        const errorMessage = error?.error?.message || error?.message || this.translate.instant('EXPENSES.TOAST.CREATE_ERROR');
        this.showBootstrapToast(
          this.translateBackendError(errorMessage),
          this.translate.instant('EXPENSES.TOAST.ERROR'), 'danger');
        console.log(error)
      }
    })
  }

  editDayExpenses() {
    let participantsList = this.participants.split(',').map(p => p.trim())

    this.expensesService.editDayExpenses(this.id, this.date, this.location, participantsList).subscribe({
      next: () => {
        this.hideModal();
        this.loadExpenses('', '');

        this.date = '';
        this.location = '';
        this.participants = '';

        this.showBootstrapToast(
          this.translate.instant('EXPENSES.TOAST.EDIT_SUCCESS'),
          this.translate.instant('EXPENSES.TOAST.SUCCESS'), 'success');
      },
      error: error =>
      {
        const errorMessage = error?.error?.message || error?.message || this.translate.instant('EXPENSES.TOAST.EDIT_ERROR');
        this.showBootstrapToast(
          this.translateBackendError(errorMessage),
          this.translate.instant('EXPENSES.TOAST.ERROR'), 'danger');
        console.log(error)
      }
    })
  }

  deleteDayExpenses() {
    this.expensesService.deleteDayExpenses(this.id).subscribe({
      next: () => {
        this.hideModal();
        this.currentPage = 1;
        this.loadExpenses('', '');

        this.date = '';
        this.location = '';
        this.participants = '';

        this.showBootstrapToast(
          this.translate.instant('EXPENSES.TOAST.DELETE_SUCCESS'),
          this.translate.instant('EXPENSES.TOAST.SUCCESS'), 'success');
      },
      error: error =>
        {
          console.log(error);
          const errorMessage = error?.error?.message || error?.message || this.translate.instant('EXPENSES.TOAST.DELETE_ERROR');
          this.showBootstrapToast(
            this.translateBackendError(errorMessage),
            this.translate.instant('EXPENSES.TOAST.ERROR'), 'danger');
        }
    })
  }

  shareDayExpenses() {
    this.expensesService.shareDayExpenses(this.id, this.newUserWithAccess).subscribe({
      next: (data) => {
        if (data.isSuccess) {
          this.hideModal();
          this.loadExpenses();

          this.date = '';
          this.location = '';
          this.participants = '';

          this.newUserWithAccess = '';
          this.shareIsSuccess = false;
          this.shareError = '';

          this.showBootstrapToast(
            this.translate.instant('EXPENSES.TOAST.SHARE_SUCCESS'),
            this.translate.instant('EXPENSES.TOAST.SUCCESS'), 'success');
        }
        else {
          this.shareIsSuccess = data.isSuccess;
          this.shareError = this.translateBackendError(data.error);
        }
      },
      error: error =>
        {
          console.log(error);
          const errorMessage = error?.error?.message || error?.message || this.translate.instant('EXPENSES.BACKEND_ERRORS.UNKNOWN_ERROR');
          this.showBootstrapToast(
            this.translateBackendError(errorMessage),
            this.translate.instant('EXPENSES.TOAST.ERROR'), 'danger');
        }
    })
  }
}

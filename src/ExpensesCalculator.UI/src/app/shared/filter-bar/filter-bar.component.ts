import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

export interface FilterOption {
  value: string;
  labelKey: string;
}

@Component({
  selector: 'app-filter-bar',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './filter-bar.component.html',
  styleUrl: './filter-bar.component.css'
})
export class FilterBarComponent implements OnChanges {
  @Input() filterOptions: FilterOption[] = [];
  @Input() selectedCriteria: string = '';
  @Input() filterText: string = '';
  @Input() placeholderKey: string = 'EXPENSES.SEARCH';

  @Output() criteriaChange = new EventEmitter<string>();
  @Output() filterTextChange = new EventEmitter<string>();

  localFilterText: string = '';

  ngOnChanges(changes: SimpleChanges): void {
    // Sync from parent on first load or when parent explicitly clears the filter
    if (changes['filterText']) {
      if (changes['filterText'].firstChange || this.filterText !== this.localFilterText) {
        this.localFilterText = this.filterText;
      }
    }
  }

  get selectedCriteriaLabel(): string {
    const option = this.filterOptions.find(opt => opt.value === this.selectedCriteria);
    if (option) {
      return option.labelKey;
    }
    // Fallback to first option if selectedCriteria doesn't match
    return this.filterOptions.length > 0 ? this.filterOptions[0].labelKey : '';
  }

  onCriteriaChange(criteria: string): void {
    this.criteriaChange.emit(criteria);
  }

  onFilterInput(): void {
    this.filterTextChange.emit(this.localFilterText);
  }
}

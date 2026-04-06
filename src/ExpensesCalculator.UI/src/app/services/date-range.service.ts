import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import flatpickr from 'flatpickr';
import { Ukrainian } from 'flatpickr/dist/l10n/uk';

export interface DateRangeOptions {
  mode?: 'single' | 'range';
  readonly?: boolean;
  defaultDate?: string;
  onChange?: (dates: Date[]) => void;
  elementId?: string;
  calendarButtonId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class DateRangeService {
  constructor(private translate: TranslateService) {}

  /**
   * Convert language code to locale
   */
  langToLocale(lang: string): string {
    return lang === 'ua' ? 'uk' : 'en';
  }

  /**
   * Get Flatpickr locale configuration based on current language
   */
  getLocaleConfig(lang: string, rangeSeparator: boolean = false): any {
    const config = lang === 'ua' ? Ukrainian : {};
    if (rangeSeparator) {
      return { ...config, rangeSeparator: ' - ' };
    }
    return config;
  }

  /**
   * Initialize a date range picker
   */
  initializeDateRangePicker(
    elementId: string,
    options: DateRangeOptions,
    settingDatesFromApi: { value: boolean }
  ): any {
    const element = document.getElementById(elementId);
    const calendarButton = options.calendarButtonId
      ? document.getElementById(options.calendarButtonId)
      : null;

    if (!element) return null;

    const instance = flatpickr(element, {
      mode: 'range',
      dateFormat: 'Y-m-d',
      altInput: true,
      altFormat: 'M d, Y',
      locale: this.getLocaleConfig(this.translate.currentLang, true),

      onReady: (selectedDates, dateStr, inst) => {
        if (inst.altInput) {
          // Add Bootstrap classes for proper styling
          inst.altInput.classList.add('form-control', 'form-control-sm', 'bg-transparent', 'text-white', 'border-primary', 'text-center');

          // Set height and remove outline/shadow to match design
          inst.altInput.style.height = '37px';
          inst.altInput.style.outline = 'none';
          inst.altInput.style.boxShadow = 'none';
          inst.altInput.style.cursor = 'pointer';
          inst.altInput.value = 'All';
        }

        if (calendarButton) {
          calendarButton.addEventListener('click', () => {
            inst.open();
          });
        }
      },

      onChange: (dates: Date[]) => {
        if (settingDatesFromApi.value) return;

        if (dates.length === 2 && options.onChange) {
          options.onChange(dates);
        }
      }
    });

    return instance;
  }

  /**
   * Initialize a single date picker (for modals)
   */
  initializeSingleDatePicker(
    elementId: string,
    options: DateRangeOptions
  ): any {
    const element = document.getElementById(elementId);
    if (!element) return null;

    // Clear the input element's value before initializing Flatpickr
    (element as HTMLInputElement).value = '';

    const instance = flatpickr(element, {
      dateFormat: 'Y-m-d',
      altInput: true,
      altFormat: 'D, M d, Y',
      locale: this.getLocaleConfig(this.translate.currentLang),
      defaultDate: options.defaultDate || undefined,
      clickOpens: !options.readonly,
      allowInput: false,

      onReady: (_selectedDates: Date[], _dateStr: string, inst: any) => {
        if (!options.readonly && inst.altInput) {
          inst.altInput.style.cursor = 'pointer';
        }
      },

      onChange: (dates: Date[]) => {
        if (dates.length > 0 && options.onChange) {
          options.onChange(dates);
        }
      }
    });

    // Explicitly clear Flatpickr if no date is set
    if (!options.defaultDate) {
      instance.clear();
    }

    return instance;
  }

  /**
   * Set dates in a Flatpickr instance
   */
  setFlatpickrDates(
    instance: any,
    fromDate: string,
    toDate: string,
    settingDatesFromApi: { value: boolean }
  ): void {
    if (!instance) return;

    try {
      settingDatesFromApi.value = true;
      const startDate = new Date(fromDate);
      const endDate = new Date(toDate);
      instance.setDate([startDate, endDate], true);

      // Manually update altInput to ensure it displays the formatted dates
      if (instance.altInput && instance.formatDate) {
        const formattedStart = instance.formatDate(startDate, instance.config.altFormat);
        const formattedEnd = instance.formatDate(endDate, instance.config.altFormat);
        const separator = instance.config.locale.rangeSeparator || ' - ';
        instance.altInput.value = `${formattedStart}${separator}${formattedEnd}`;
      }
    } catch (error) {
      console.error('Error setting flatpickr dates:', error);
    } finally {
      settingDatesFromApi.value = false;
    }
  }

  /**
   * Update locale for a Flatpickr instance
   */
  updateLocale(instance: any, lang: string, rangeSeparator: boolean = false): void {
    if (!instance) return;

    instance.set('locale', this.getLocaleConfig(lang, rangeSeparator));
  }

  /**
   * Format a Date object to YYYY-MM-DD
   */
  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  /**
   * Clear a Flatpickr date range instance and reset display
   */
  clearDateRange(instance: any): void {
    if (!instance) return;

    instance.clear();

    // Reset the altInput to show "All" after clearing
    if (instance.altInput) {
      instance.altInput.value = 'All';
    }
  }

  /**
   * Destroy a Flatpickr instance
   */
  destroy(instance: any): void {
    if (instance) {
      instance.destroy();
    }
  }
}

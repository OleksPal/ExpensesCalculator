import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

declare var bootstrap: any;

@Injectable({
  providedIn: 'root'
})
export class TooltipService {
  constructor(private translate: TranslateService) {}

  /**
   * Initialize all Bootstrap tooltips on the page
   */
  initialize(options: { html?: boolean } = {}): void {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltipTriggerList.forEach(tooltipTriggerEl => {
      new bootstrap.Tooltip(tooltipTriggerEl, {
        html: options.html !== undefined ? options.html : true
      });
    });
  }

  /**
   * Destroy all Bootstrap tooltips on the page
   */
  destroy(): void {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltipTriggerList.forEach(tooltipTriggerEl => {
      const existing = bootstrap.Tooltip.getInstance(tooltipTriggerEl);
      if (existing) existing.dispose();
    });
  }

  /**
   * Generate tooltip content for participants list
   */
  generateParticipantsTooltip(participants: string[], maxDisplay: number = 3): string {
    const displayUsers = participants?.slice(0, maxDisplay);
    const moreCount = participants?.length > maxDisplay ? participants?.length - maxDisplay : 0;

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

  /**
   * Generate tooltip content for items users list
   */
  generateUsersTooltip(users: string[], maxDisplay: number = 3): string {
    const displayUsers = users?.slice(0, maxDisplay);
    const moreCount = users?.length > maxDisplay ? users?.length - maxDisplay : 0;

    let content = `<i class="bi bi-people-fill me-1"></i><span class="fw-bold">${this.translate.instant('ITEMS.TOOLTIP.USERS_TITLE')}</span><br/>`;

    displayUsers?.forEach((user) => {
      content += `<i class="bi bi-person-fill me-1"></i> ${this.trimText(user)}<br>`;
    });

    if (moreCount > 0) {
      content += this.translate.instant('ITEMS.TOOLTIP.AND_MORE', { count: moreCount });
    }

    return content;
  }

  /**
   * Generate tooltip content for check details
   */
  generateCheckTooltip(sum: number, payer: string): string {
    return `<i class="bi bi-coin me-1"></i><span class="fw-bold">${this.translate.instant('CALCULATIONS.SUM')}:</span> ${sum.toFixed(2)}₴<br/>` +
           `<i class="bi bi-person-fill me-1"></i><span class="fw-bold">${this.translate.instant('CALCULATIONS.PAYER')}:</span> ${payer}`;
  }

  /**
   * Trim text to specified length
   */
  trimText(text: string, maxLength: number = 8): string {
    if (!text) return '';
    return text.length > maxLength
      ? text.substring(0, maxLength) + '...'
      : text;
  }
}

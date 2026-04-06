import { Directive, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';

declare var bootstrap: any;

@Directive({
  selector: '[bsTooltip]',
  standalone: true
})
export class BsTooltipDirective implements AfterViewInit, OnDestroy {
  private tooltip: any;

  constructor(private el: ElementRef) {}

  ngAfterViewInit() {
    this.tooltip = new bootstrap.Tooltip(this.el.nativeElement);
  }

  ngOnDestroy() {
    this.tooltip?.dispose();
  }
}

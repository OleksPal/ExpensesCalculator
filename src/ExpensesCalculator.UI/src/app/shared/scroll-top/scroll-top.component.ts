import { Component, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-scroll-top',
  standalone: true,
  templateUrl: './scroll-top.component.html',
  styleUrls: ['./scroll-top.component.css'],
  imports: [CommonModule]
})
export class ScrollTopComponent {
  showButton = signal(false);

  @HostListener('window:scroll', [])
  onWindowScroll() {
    this.showButton.set(window.scrollY > 200);
  }

  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}

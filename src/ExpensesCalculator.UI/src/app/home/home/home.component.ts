import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ScrollTopComponent } from '../../shared/scroll-top/scroll-top.component';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-home',
  imports: [CommonModule, ScrollTopComponent, RouterLink, TranslatePipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  activeTab: 'expenses' | 'split' | 'calculate' | 'tags' | 'recommendations' = 'expenses';
}

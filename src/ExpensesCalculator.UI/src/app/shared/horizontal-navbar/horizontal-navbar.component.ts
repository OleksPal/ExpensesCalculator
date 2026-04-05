import { Component, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { RouterLink } from "@angular/router";
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-horizontal-navbar',
  imports: [RouterLink, CommonModule, TranslatePipe],
  templateUrl: './horizontal-navbar.component.html',
  styleUrl: './horizontal-navbar.component.css'
})
export class HorizontalNavbarComponent {
  currentLang = 'en';
  private platformId = inject(PLATFORM_ID);

  constructor(private auth: AuthService, private router: Router, private translate: TranslateService) {
    this.currentLang = this.translate.currentLang || 'en';
  }

  get flagSrc(): string {
    return this.currentLang === 'en' ? '/images/en-US-flag.svg' : '/images/uk-UA-flag.svg';
  }

  get langLabel(): string {
    return this.currentLang === 'en' ? 'EN' : 'UA';
  }

  toggleLanguage() {
    this.currentLang = this.currentLang === 'en' ? 'ua' : 'en';
    this.translate.use(this.currentLang);
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('lang', this.currentLang);
    }
  }

  getUsername() {
    return this.auth.userName;
  }

  isAuthenticated() { 
    return this.auth.isAuthenticated()
  }

  logout() {
    this.auth.logout().subscribe({
      next: () => {
        this.router.navigate(['/'])
      }
    })
  }
}

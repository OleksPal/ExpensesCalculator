import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

export interface ResponseError {
  code: string;
  description: string;
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, TranslatePipe],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  userName = '';
  password = '';
  errors : ResponseError[] = [];

  isLoading = false;

  constructor(private auth: AuthService, private router: Router, private translate: TranslateService){}

  translateAuthError(errorMessage: string): string {
    if (!errorMessage) return '';

    const errorMap: Record<string, string> = {
      'Invalid email or password': 'LOGIN.ERRORS.INVALID_CREDENTIALS',
      'Invalid username or password': 'LOGIN.ERRORS.INVALID_CREDENTIALS',
      'Invalid credentials': 'LOGIN.ERRORS.INVALID_CREDENTIALS',
      'invalid': 'LOGIN.ERRORS.INVALID_CREDENTIALS',
      'required': 'LOGIN.ERRORS.REQUIRED_FIELD'
    };

    // Check for exact match first
    const translationKey = errorMap[errorMessage];
    if (translationKey) {
      return this.translate.instant(translationKey);
    }

    // Check for partial matches (case-insensitive)
    for (const [key, value] of Object.entries(errorMap)) {
      if (errorMessage.toLowerCase().includes(key.toLowerCase())) {
        return this.translate.instant(value);
      }
    }

    // Return original message if no translation found
    return errorMessage;
  }

  login() {
    this.isLoading = true;

    this.auth.login(this.userName, this.password).subscribe({
      next: () => {
        this.router.navigate(['/day-expenses'])
        this.isLoading = false;
      },
      error: error => {
        this.errors = error.error?.map((err: ResponseError) => ({
          code: err.code,
          description: this.translateAuthError(err.description)
        })) || [{ code: 'UnknownError', description: this.translate.instant('LOGIN.ERRORS.UNKNOWN_ERROR') }];
        this.isLoading = false;
      }
    })
  }
}

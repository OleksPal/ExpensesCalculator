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
  selector: 'app-register',
  imports: [FormsModule, CommonModule, TranslatePipe],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  userName = '';
  password = '';
  confirmPassword = '';
  errors : ResponseError[] = [];

  constructor(private auth: AuthService, private router: Router, private translate: TranslateService){}

  translateAuthError(errorMessage: string): string {
    if (!errorMessage) return '';

    const lowerMessage = errorMessage.toLowerCase();

    // Check specific patterns first (most specific to least specific)
    // Username errors
    if (lowerMessage.includes('user already exists') ||
        lowerMessage.includes('username is already taken') ||
        lowerMessage.includes('duplicateusername') ||
        lowerMessage.includes('already exists') ||
        lowerMessage.includes('already taken')) {
      return this.translate.instant('REGISTER.ERRORS.USERNAME_TAKEN');
    }

    if (lowerMessage.includes('invalid username')) {
      return this.translate.instant('REGISTER.ERRORS.INVALID_USERNAME');
    }

    // Password character requirements (check these before length)
    if (lowerMessage.includes('must contain at least one number') ||
        lowerMessage.includes('must contain at least one digit')) {
      return this.translate.instant('REGISTER.ERRORS.PASSWORD_REQUIRES_DIGIT');
    }

    if (lowerMessage.includes('uppercase')) {
      return this.translate.instant('REGISTER.ERRORS.PASSWORD_REQUIRES_UPPERCASE');
    }

    if (lowerMessage.includes('lowercase')) {
      return this.translate.instant('REGISTER.ERRORS.PASSWORD_REQUIRES_LOWERCASE');
    }

    if (lowerMessage.includes('special')) {
      return this.translate.instant('REGISTER.ERRORS.PASSWORD_REQUIRES_SPECIAL');
    }

    // Password length errors (check last to avoid false matches)
    if (lowerMessage.includes('must be at least') && lowerMessage.includes('characters')) {
      return this.translate.instant('REGISTER.ERRORS.PASSWORD_TOO_SHORT');
    }

    // Return original message if no translation found
    return errorMessage;
  }

  register() {
    if (this.password !== this.confirmPassword)
    {
      if (!this.errors.some(e => e.code === 'PasswordMismatch')) {
        this.errors.push({
          code: 'PasswordMismatch',
          description: this.translate.instant('REGISTER.PASSWORD_MISMATCH')
        });
      }
      return;
    }

    this.auth.register(this.userName, this.password).subscribe({
      next: () => {
        this.router.navigate(['/login'])
      },
      error: error => {
        this.errors = error.error?.map((err: ResponseError) => ({
          code: err.code,
          description: this.translateAuthError(err.description)
        })) || [{ code: 'UnknownError', description: this.translate.instant('REGISTER.ERRORS.UNKNOWN_ERROR') }];
      }
    })
  }
}

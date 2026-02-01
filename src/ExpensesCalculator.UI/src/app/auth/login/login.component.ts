import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ValidationErrors, parseValidationErrors } from '../../shared/models/validation-errors.model';

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
  fieldErrors: ValidationErrors = {};
  formValidated = false;

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

  private translateValidationError(errorMessage: string): string {
    if (!errorMessage) return '';

    const lowerMessage = errorMessage.toLowerCase();

    // Map common validation error messages to translation keys
    if (lowerMessage.includes('username') && lowerMessage.includes('required')) {
      return this.translate.instant('LOGIN.ERRORS.USERNAME_REQUIRED');
    }
    if (lowerMessage.includes('password') && lowerMessage.includes('required')) {
      return this.translate.instant('LOGIN.ERRORS.PASSWORD_REQUIRED');
    }
    if (lowerMessage.includes('required')) {
      return this.translate.instant('LOGIN.ERRORS.REQUIRED_FIELD');
    }

    // Try translateAuthError for other errors
    return this.translateAuthError(errorMessage);
  }

  private mapErrorsToFields(errors: ResponseError[]) {
    this.fieldErrors = {};
    for (const err of errors) {
      const code = err.code?.toLowerCase() || '';
      if (code.includes('credential') || code.includes('password')) {
        this.fieldErrors['password'] = err.description;
      } else if (code.includes('user') || code.includes('username')) {
        this.fieldErrors['userName'] = err.description;
      }
    }
  }

  login() {
    this.isLoading = true;
    this.fieldErrors = {};
    this.errors = [];
    this.formValidated = true;

    this.auth.login(this.userName, this.password).subscribe({
      next: () => {
        this.router.navigate(['/day-expenses'])
        this.isLoading = false;
      },
      error: error => {
        // Handle ValidationProblemDetails (from DataAnnotations)
        const validationErrors = parseValidationErrors(error);
        if (Object.keys(validationErrors).length > 0 && !validationErrors['general']) {
          // Translate validation error messages
          this.fieldErrors = {};
          for (const [field, message] of Object.entries(validationErrors)) {
            this.fieldErrors[field] = this.translateValidationError(message);
          }
          this.isLoading = false;
          return;
        }

        // Handle IdentityError[] format (from auth controller)
        this.errors = error.error?.map((err: ResponseError) => ({
          code: err.code,
          description: this.translateAuthError(err.description)
        })) || [{ code: 'UnknownError', description: this.translate.instant('LOGIN.ERRORS.UNKNOWN_ERROR') }];
        this.mapErrorsToFields(this.errors);
        this.isLoading = false;
      }
    })
  }
}

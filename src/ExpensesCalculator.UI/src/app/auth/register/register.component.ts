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
  fieldErrors: ValidationErrors = {};
  formValidated = false;

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

    // Password character requirements - translate with proper key
    if (lowerMessage.includes('must contain at least one number') ||
        lowerMessage.includes('must contain at least one digit')) {
      const key = 'REGISTER.ERRORS.PASSWORD_REQUIRES_DIGIT';
      const translated = this.translate.instant(key);
      return (translated && translated !== key && translated.trim()) ? translated : errorMessage;
    }

    if (lowerMessage.includes('uppercase')) {
      const key = 'REGISTER.ERRORS.PASSWORD_REQUIRES_UPPERCASE';
      const translated = this.translate.instant(key);
      console.log('Uppercase translation:', { key, translated, currentLang: this.translate.currentLang });
      return (translated && translated !== key && translated.trim()) ? translated : errorMessage;
    }

    if (lowerMessage.includes('lowercase')) {
      const key = 'REGISTER.ERRORS.PASSWORD_REQUIRES_LOWERCASE';
      const translated = this.translate.instant(key);
      console.log('Lowercase translation:', { key, translated, currentLang: this.translate.currentLang });
      return (translated && translated !== key && translated.trim()) ? translated : errorMessage;
    }

    if (lowerMessage.includes('special')) {
      const key = 'REGISTER.ERRORS.PASSWORD_REQUIRES_SPECIAL';
      const translated = this.translate.instant(key);
      return (translated && translated !== key && translated.trim()) ? translated : errorMessage;
    }

    // Password length - extract the number and use it in translation
    if (lowerMessage.includes('must be at least') && lowerMessage.includes('characters')) {
      // Try to extract the number from the message
      const match = errorMessage.match(/(\d+)\s+characters/i);
      if (match && match[1]) {
        const length = match[1];
        return this.translate.instant('REGISTER.ERRORS.PASSWORD_LENGTH_REQUIRED', { length });
      }
      // Fallback if we can't extract the number
      return this.translate.instant('REGISTER.ERRORS.PASSWORD_TOO_SHORT');
    }

    // Return original message if no translation found
    return errorMessage;
  }

  private translateValidationError(errorMessage: string): string {
    if (!errorMessage) return '';

    const lowerMessage = errorMessage.toLowerCase();

    // Map common validation error messages to translation keys
    if (lowerMessage.includes('username') && lowerMessage.includes('required')) {
      return this.translate.instant('REGISTER.ERRORS.USERNAME_REQUIRED');
    }
    if (lowerMessage.includes('password') && lowerMessage.includes('required')) {
      return this.translate.instant('REGISTER.ERRORS.PASSWORD_REQUIRED');
    }
    if (lowerMessage.includes('required')) {
      return this.translate.instant('REGISTER.ERRORS.REQUIRED_FIELD');
    }

    // Try translateAuthError for other errors
    return this.translateAuthError(errorMessage);
  }

  private mapErrorsToFields(errors: ResponseError[]) {
    this.fieldErrors = {};
    const passwordErrors: string[] = [];

    console.log('mapErrorsToFields called with errors:', errors);

    for (const err of errors) {
      const code = err.code?.toLowerCase() || '';
      const desc = err.description?.toLowerCase() || '';
      console.log('Processing error:', { code, desc, fullDescription: err.description });

      if (code.includes('userexists') || code.includes('username') ||
          desc.includes('username') || desc.includes('already exists')) {
        this.fieldErrors['userName'] = err.description;
      } else if (code.includes('password') || desc.includes('password') || desc.includes('пароль')) {
        console.log('Adding to passwordErrors:', err.description);
        passwordErrors.push(err.description);
      }
    }

    // Join password errors with HTML line breaks for better display
    if (passwordErrors.length > 0) {
      this.fieldErrors['password'] = passwordErrors.join('<br>');
      console.log('Final fieldErrors[password]:', this.fieldErrors['password']);
    }
  }

  register() {
    this.fieldErrors = {};
    this.errors = [];
    this.formValidated = true;

    if (this.password !== this.confirmPassword) {
      this.fieldErrors['confirmPassword'] = this.translate.instant('REGISTER.PASSWORD_MISMATCH');
      this.errors = [{
        code: 'PasswordMismatch',
        description: this.translate.instant('REGISTER.PASSWORD_MISMATCH')
      }];
      return;
    }

    this.auth.register(this.userName, this.password).subscribe({
      next: () => {
        this.router.navigate(['/login'])
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
          return;
        }

        // Handle IdentityError[] format (from auth controller)
        this.errors = error.error?.map((err: ResponseError) => ({
          code: err.code,
          description: this.translateAuthError(err.description)
        })) || [{ code: 'UnknownError', description: this.translate.instant('REGISTER.ERRORS.UNKNOWN_ERROR') }];
        this.mapErrorsToFields(this.errors);
      }
    })
  }
}

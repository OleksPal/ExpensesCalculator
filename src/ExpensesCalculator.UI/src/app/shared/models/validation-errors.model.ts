export interface ValidationErrors {
  [field: string]: string;
}

export interface ValidationProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  errors?: { [field: string]: string[] };
}

export function parseValidationErrors(error: any): ValidationErrors {
  const fieldErrors: ValidationErrors = {};

  // ASP.NET ValidationProblemDetails format
  if (error?.error?.errors) {
    const errors = error.error.errors;
    for (const field of Object.keys(errors)) {
      // Strip "$." prefix from JSON deserialization errors (e.g. "$.Date" -> "Date")
      let cleanField = field.startsWith('$.') ? field.substring(2) : field;
      // Also strip "$" alone
      if (cleanField === '$' || cleanField === '') cleanField = 'general';
      const fieldName = cleanField.charAt(0).toLowerCase() + cleanField.slice(1);
      fieldErrors[fieldName] = errors[field][0];
    }
    return fieldErrors;
  }

  // Fallback: generic error message
  if (error?.error?.message) {
    fieldErrors['general'] = error.error.message;
  }

  return fieldErrors;
}

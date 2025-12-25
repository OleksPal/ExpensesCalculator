using Microsoft.AspNetCore.Identity;

namespace ExpensesCalculator.WebAPI.Services;

public static class PasswordValidator
{
    public static IdentityError[] Validate(string password)
    {
        var errors = new List<IdentityError>();
        if (string.IsNullOrWhiteSpace(password))
            errors.Add(new IdentityError 
            { 
                Code = "EmptyPassword", 
                Description = "Password cannot be empty." 
            });

        if (password.Length < 12)
            errors.Add(new IdentityError
            { 
                Code = "ShortPassword", 
                Description = "Password must be at least 12 characters long." 
            });

        if (!password.Any(char.IsUpper))
            errors.Add(new IdentityError
            { 
                Code = "NoUppercaseLetter", 
                Description = "Password must contain at least one uppercase letter." 
            });

        if (!password.Any(char.IsLower))
            errors.Add(new IdentityError
            { 
                Code = "NoLowercaseLetter", 
                Description = "Password must contain at least one lowercase letter." 
            });

        if (!password.Any(char.IsDigit))
            errors.Add(new IdentityError 
            { 
                Code = "NoNumber", 
                Description = "Password must contain at least one number." 
            });

        return errors.ToArray();
    }
}


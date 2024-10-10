using ExpensesCalculator.Models;

namespace ExpensesCalculator.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}

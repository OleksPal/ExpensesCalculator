using ExpensesCalculator.Data;
using ExpensesCalculator.Repositories.Interfaces;

namespace ExpensesCalculator.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ExpensesContext _context;

        public UserRepository(ExpensesContext context)
        {
            _context = context;
        }

        public bool UserExists(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }
    }
}

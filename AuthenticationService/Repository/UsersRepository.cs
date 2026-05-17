using AuthenticationService.EntityModel;

using Microsoft.EntityFrameworkCore;
using Models;

namespace AuthenticationService.Repository
{

    public interface IUserRepository
    {
        Task<User> Authenticate(AuthRequest request);
        Task<Role> GetRoleForUser(int userId);
        Task<User> GetUserByEmail(string email);
    }

    public class UsersRepository : IUserRepository
    {
        private readonly UsersDbContext _context;
        public UsersRepository(UsersDbContext context)
        {
            _context = context;
        }
        public async Task<User> Authenticate(AuthRequest request)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
        }
        public async Task<Role> GetRoleForUser(int userId)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleId ==
                _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId).RoleId);
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}

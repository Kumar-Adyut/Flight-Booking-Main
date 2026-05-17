using Microsoft.EntityFrameworkCore;
using Models;


namespace AuthenticationService.EntityModel
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }

        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {

            //var n = new FaultContract(10) { FaultId = 1 };
        }
    }
}

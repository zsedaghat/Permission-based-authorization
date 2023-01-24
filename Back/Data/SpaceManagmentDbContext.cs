using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpaceManagment.Model;

namespace SpaceManagment.Data
{
    public class SpaceManagmentDbContext : IdentityDbContext<User, Role, Guid>
    {
        public SpaceManagmentDbContext(DbContextOptions options)
           : base(options)
        {

        }
    }
}

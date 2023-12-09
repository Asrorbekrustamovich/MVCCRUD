using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCCRUD.Models.Domain;

namespace MVCCRUD.Data
{
    public class Mydbcontext:IdentityDbContext<ApplicationUser>
    {
        public Mydbcontext()
        {
            
        }
        public Mydbcontext(DbContextOptions<Mydbcontext>options):base(options) 
        {
            
        }
        public DbSet<Employee>Employees { get; set; }
    }
}

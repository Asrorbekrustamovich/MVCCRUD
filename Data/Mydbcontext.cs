using Microsoft.EntityFrameworkCore;
using MVCCRUD.Models.Domain;

namespace MVCCRUD.Data
{
    public class Mydbcontext:DbContext
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

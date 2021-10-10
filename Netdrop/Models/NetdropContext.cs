using Microsoft.EntityFrameworkCore;

namespace Netdrop.Models
{
    public class NetdropContext : DbContext
    {
        public NetdropContext(DbContextOptions<NetdropContext> options) : base (options)
        {

        }

        public DbSet<Users> Users { get; set; }
    }
}

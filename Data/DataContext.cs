

namespace DotNetRPG.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().HasData(
                new Skill { Id=1,Name="Fireball", Damage = 30},
                 new Skill { Id = 2, Name = "Flenzy", Damage = 20 },
                  new Skill { Id = 3, Name = "Blizzard", Damage = 10 }
                );
        }
        public DbSet<Character> Characters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Skill> Skills { get; set; }
    }
}

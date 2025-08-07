using Microsoft.EntityFrameworkCore;

namespace ToolTemp.WPF.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
        {
        }

        public MyDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<BusDataTemp> BusDataTemp { get; set; }
        public DbSet<Style> Style { get; set; }
        public DbSet<Factory> FactoryConfigs { get; set; }
        public DbSet<Machine> machines { get; set; }
        public DbSet<DvFactoryAssembling> dvFactoryAssemblings { get; set; }
        public DbSet<ActiveType> activeTypes { get; set; }
        public DbSet<Controlcode> controlcodes { get; set; }
        public DbSet<Device> devices { get; set; }
        public DbSet<SensorData> sensorDatas { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder.UseSqlServer(@"Server=10.30.201.201;Database=PowerTempWatch;User ID=sa;Password=greenland@VN;TrustServerCertificate=True;MultipleActiveResultSets=True;"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
        }
    }
}

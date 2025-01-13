using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        public DbSet<Device> Devices { get; set; }
        public DbSet<Factory> FactoryConfigs { get; set; }
        public DbSet<Machine> machines { get; set; }
        public DbSet<DvFactoryAssembling> dvFactoryAssemblings { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder.UseSqlServer(@"Server=10.30.80.1;Database=PowerTempWatch;User ID=sa;Password=greenland@VN;TrustServerCertificate=True;MultipleActiveResultSets=True;"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BusDataTemp>(entity =>
            {
                entity.ToTable("dv_BusDataTemp");

                entity.HasKey(e => e.Id); // Khóa chính

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(50); // NVARCHAR(50)

                entity.Property(e => e.Factory)
                    .IsRequired()
                    .HasMaxLength(100); // NVARCHAR(100)

                entity.Property(e => e.Line)
                    .IsRequired()
                    .HasMaxLength(100); // NVARCHAR(100)

                entity.Property(e => e.AddressMachine)
                    .IsRequired(); // INT

                entity.Property(e => e.Port)
                    .IsRequired()
                    .HasMaxLength(50); // NVARCHAR(50)

                entity.Property(e => e.Temp)
                    .IsRequired(); // FLOAT

                entity.Property(e => e.Baudrate)
                    .IsRequired(); // INT

                

                entity.Property(e => e.UploadDate)
                    .IsRequired(); // DATETIME

                entity.Property(e => e.IsWarning)
                    .IsRequired(); // BIT

                entity.Property(e => e.Sensor_Typeid)
                    .IsRequired(); // INT

                entity.Property(e => e.Sensor_kind)
                    .HasMaxLength(63); // NVARCHAR(63)

                entity.Property(e => e.Sensor_ant)
                    .HasMaxLength(63); // NVARCHAR(63)
            });


            // Cấu hình bảng dv_style
            modelBuilder.Entity<Style>(entity =>
            {
                entity.ToTable("dv_style"); // Tên bảng

                entity.HasKey(e => e.Id); // Khóa chính

                entity.Property(e => e.NameStyle)
                    .IsRequired()
                    .HasMaxLength(50); // NVARCHAR(50)

                entity.Property(e => e.DeMax)
                    .IsRequired()
                    .HasColumnType("decimal(18,6)"); // NUMERIC(18,6)

                entity.Property(e => e.DeMin)
                    .IsRequired()
                    .HasColumnType("decimal(18,6)"); // NUMERIC(18,6)

                entity.Property(e => e.GiayMax)
                    .IsRequired()
                    .HasColumnType("decimal(18,6)"); // NUMERIC(18,6)

                entity.Property(e => e.GiayMin)
                    .IsRequired()
                    .HasColumnType("decimal(18,6)"); // NUMERIC(18,6)
                entity.Property(e => e.Devid)
                    .HasMaxLength(255); // NVARCHAR(255)

                entity.Property(e => e.StandardTemp)
                    .HasMaxLength(255); // NVARCHAR(255)

                entity.Property(e => e.CompensateVaild)
                    .HasColumnType("decimal(18,6)"); // NUMERIC(18,6), NULL được phép
            });



            // Cấu hình bảng devices
            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("devices");

                // Xác định khóa chính nếu cần
                entity.HasKey(e => e.DevId);

                entity.Property(e => e.DevId)
                    .HasColumnName("devid")
                    .HasMaxLength(20); // VARCHAR(20)

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20); // VARCHAR(20)

                entity.Property(e => e.ActiveId)
                    .IsRequired(); // INT

                entity.Property(e => e.TypeId)
                    .IsRequired(); // INT
            });

            // Cấu hình bảng dv_Factory_Configs
            modelBuilder.Entity<Factory>(entity =>
            {
                entity.ToTable("dv_Factory_Configs");

                entity.HasKey(e => e.Id); // Khóa chính

                entity.Property(e => e.FactoryCode)
                    .IsRequired()
                    .HasMaxLength(50); // NVARCHAR(50)

                entity.Property(e => e.Line)
                    .IsRequired()
                    .HasMaxLength(100); // NVARCHAR(100)

                entity.Property(e => e.Address)
                    .IsRequired(); // INT
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

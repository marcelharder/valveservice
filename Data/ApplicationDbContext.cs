namespace ValveService.Data.Entities;


    public class ApplicationDbContext : DbContext
    
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Valve_Code> ValveCodes { get; set; }
        public DbSet<Valve_Size> ValveSizes { get; set; }
       



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }


using Microsoft.EntityFrameworkCore;

namespace ECommerce.Test
{
    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    
     public TestDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);

            // إعداد بعض البيانات الأولية
            context.TestEntities.AddRange(new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Item A", IsActive = true },
            new TestEntity { Id = 2, Name = "Item B", IsActive = true },
            new TestEntity { Id = 3, Name = "Item C", IsActive = false },
            new TestEntity { Id = 4, Name = "Item D", IsActive = true }
        });
            context.SaveChanges();
            return context;
        }
    } }

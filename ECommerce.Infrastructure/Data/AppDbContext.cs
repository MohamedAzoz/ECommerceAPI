using ECommerce.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================
            // 1. USER ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<AppUser>(entity =>
            {
                // Primary Key
                entity.HasKey(u => u.Id);

                // Properties
                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(u => u.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(u => u.IsActive)
                    .HasDefaultValue(true);

                // Indexes
                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.HasIndex(u => u.PhoneNumber);

                // Relationships

                // User → Orders (One-to-Many)
                entity.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // User → Addresses (One-to-Many)
                entity.HasMany(u => u.Addresses)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // User → Reviews (One-to-Many)
                entity.HasMany(u => u.Reviews)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // User → Cart (One-to-One)
                entity.HasOne(u => u.Cart)
                    .WithOne(c => c.User)
                    .HasForeignKey<Cart>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // 2. CATEGORY ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<Category>(entity =>
            {
                // Primary Key
                entity.HasKey(c => c.Id);

                // Properties
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                entity.Property(c => c.ImageUrl)
                    .HasMaxLength(500);

                // Indexes
                entity.HasIndex(c => c.Name)
                    .IsUnique();

                entity.HasIndex(c => c.ParentCategoryId);

                // Relationships

                // Category → Products (One-to-Many)
                entity.HasMany(c => c.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Category → SubCategories (Self-referencing One-to-Many)
                entity.HasOne(c => c.ParentCategory)
                    .WithMany(c => c.SubCategories)
                    .HasForeignKey(c => c.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // 3. PRODUCT ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<Product>(entity =>
            {
                // Primary Key
                entity.HasKey(p => p.Id);

                // Properties
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Description)
                    .HasMaxLength(2000);

                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(p => p.StockQuantity)
                    .HasDefaultValue(0);

                entity.Property(p => p.ImageUrl)
                    .HasMaxLength(500);

                entity.Property(p => p.IsActive)
                    .HasDefaultValue(true);

                entity.Property(p => p.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Indexes
                entity.HasIndex(p => p.CategoryId);
                entity.HasIndex(p => p.Name);
                entity.HasIndex(p => p.Price);
                entity.HasIndex(p => p.IsActive);

                // Relationships

                // Product → Category (Many-to-One) - Already configured above
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Product → ProductImages (One-to-Many)
                entity.HasMany(p => p.Images)
                    .WithOne(pi => pi.Product)
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Product → Reviews (One-to-Many)
                entity.HasMany(p => p.Reviews)
                    .WithOne(r => r.Product)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Product → CartItems (One-to-Many)
                entity.HasMany(p => p.CartItems)
                    .WithOne(ci => ci.Product)
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Product → OrderItems (One-to-Many)
                entity.HasMany(p => p.OrderItems)
                    .WithOne(oi => oi.Product)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // 4. PRODUCT IMAGE ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<ProductImage>(entity =>
            {
                // Primary Key
                entity.HasKey(pi => pi.Id);

                // Properties
                entity.Property(pi => pi.ImageName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(pi => pi.StoredImageName)
                    .IsRequired()
                    .HasMaxLength(50); 

                entity.Property(pi => pi.ContentType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(pi => pi.IsMain)
                    .HasDefaultValue(false);

                // Indexes
                entity.HasIndex(pi => pi.ProductId);
                entity.HasIndex(pi => new { pi.ProductId, pi.IsMain });

                // Relationships
                // ProductImage → Product (Many-to-One) - Already configured above
            });

            // ============================================
            // 5. CART ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<Cart>(entity =>
            {
                // Primary Key
                entity.HasKey(c => c.Id);

                // Properties
                entity.Property(c => c.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(c => c.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Indexes
                entity.HasIndex(c => c.UserId)
                    .IsUnique();

                // Relationships

                // Cart → User (One-to-One) - Already configured above

                // Cart → CartItems (One-to-Many)
                entity.HasMany(c => c.CartItems)
                    .WithOne(ci => ci.Cart)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // 6. CART ITEM ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<CartItem>(entity =>
            {
                // Primary Key
                entity.HasKey(ci => ci.Id);

                // Properties
                entity.Property(ci => ci.Quantity)
                    .IsRequired()
                    .HasDefaultValue(1);

                // Indexes
                entity.HasIndex(ci => ci.CartId);
                entity.HasIndex(ci => ci.ProductId);

                // Unique constraint: منع تكرار نفس المنتج في نفس السلة
                entity.HasIndex(ci => new { ci.CartId, ci.ProductId })
                    .IsUnique();

                // Relationships
                // CartItem → Cart (Many-to-One) - Already configured above
                // CartItem → Product (Many-to-One) - Already configured above

                // Check constraint: الكمية يجب أن تكون أكبر من صفر
                entity.HasCheckConstraint("CK_CartItem_Quantity", "[Quantity] > 0");
            });

            // ============================================
            // 7. ORDER ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<Order>(entity =>
            {
                // Primary Key
                entity.HasKey(o => o.Id);

                // Properties
                entity.Property(o => o.TotalAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(o => o.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");

                entity.Property(o => o.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(o => o.OrderDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Indexes
                entity.HasIndex(o => o.UserId);
                entity.HasIndex(o => o.OrderDate);
                entity.HasIndex(o => o.Status);
                entity.HasIndex(o => o.ShippingAddressId);

                // Relationships

                // Order → User (Many-to-One) - Already configured above

                // Order → Address (Many-to-One)
                entity.HasOne(o => o.ShippingAddress)
                    .WithMany(a => a.Orders)
                    .HasForeignKey(o => o.ShippingAddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Order → OrderItems (One-to-Many)
                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Check constraint: المبلغ الإجمالي يجب أن يكون أكبر من صفر
                entity.HasCheckConstraint("CK_Order_TotalAmount", "[TotalAmount] > 0");
            });

            // ============================================
            // 8. ORDER ITEM ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<OrderItem>(entity =>
            {
                // Primary Key
                entity.HasKey(oi => oi.Id);

                // **تأكد من وجود هذا السطر:**
                entity.Property(oi => oi.Id)
                       .ValueGeneratedOnAdd();

                // Properties
                entity.Property(oi => oi.Quantity)
                    .IsRequired();

                entity.Property(oi => oi.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                // Indexes
                entity.HasIndex(oi => oi.OrderId);
                entity.HasIndex(oi => oi.ProductId);

                // Relationships
                // OrderItem → Order (Many-to-One) - Already configured above
                // OrderItem → Product (Many-to-One) - Already configured above

                // Check constraints
                entity.HasCheckConstraint("CK_OrderItem_Quantity", "[Quantity] > 0");
                entity.HasCheckConstraint("CK_OrderItem_Price", "[Price] > 0");
            });

            // ============================================
            // 9. ADDRESS ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<Address>(entity =>
            {
                // Primary Key
                entity.HasKey(a => a.Id);

                // Properties
                entity.Property(a => a.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.State)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.PostalCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.IsDefault)
                    .HasDefaultValue(false);

                entity.Property(a => a.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Indexes
                entity.HasIndex(a => a.UserId);
                entity.HasIndex(a => new { a.UserId, a.IsDefault });

                // Relationships
                // Address → User (Many-to-One) - Already configured above
                // Address → Orders (One-to-Many) - Already configured above
            });

            // ============================================
            // 10. REVIEW ENTITY CONFIGURATION
            // ============================================
            modelBuilder.Entity<Review>(entity =>
            {
                // Primary Key
                entity.HasKey(r => r.Id);

                // Properties
                entity.Property(r => r.Rating)
                    .IsRequired();

                entity.Property(r => r.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(r => r.Comment)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(r => r.IsVerifiedPurchase)
                    .HasDefaultValue(false);

                entity.Property(r => r.IsApproved)
                    .HasDefaultValue(true);

                entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Indexes
                entity.HasIndex(r => r.ProductId);
                entity.HasIndex(r => r.UserId);
                entity.HasIndex(r => new { r.ProductId, r.UserId });
                entity.HasIndex(r => r.Rating);
                entity.HasIndex(r => r.IsApproved);

                // Relationships
                // Review → Product (Many-to-One) - Already configured above
                // Review → User (Many-to-One) - Already configured above

                // Check constraint: التقييم من 1 إلى 5
                entity.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5");
            });

        }
    }
}
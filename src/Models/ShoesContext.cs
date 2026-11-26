using Microsoft.EntityFrameworkCore;

namespace ShoesShop.Models;

public partial class ShoesContext : DbContext
{
    public ShoesContext()
    {
    }

    public ShoesContext(DbContextOptions<ShoesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersItem> OrdersItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Production> Productions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Subcategory> Subcategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Shoes;User=export;Password=1201;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3213E83FB157E80C");

            entity.ToTable("Category$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders$__3213E83FDDBF5F65");

            entity.ToTable("Orders$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders$_Users$");
        });

        modelBuilder.Entity<OrdersItem>(entity =>
        {
            entity.ToTable("OrdersItem$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemId).HasColumnName("itemID");
            entity.Property(e => e.OrderId).HasColumnName("orderID");
            entity.Property(e => e.Slotprice).HasColumnName("slotprice");
            entity.Property(e => e.Slotquantity).HasColumnName("slotquantity");

            entity.HasOne(d => d.Item).WithMany(p => p.OrdersItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdersItem$_Warehouse$");

            entity.HasOne(d => d.Order).WithMany(p => p.OrdersItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdersItem$_Orders$");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("categoryID");
            entity.Property(e => e.Descriptions)
                .HasMaxLength(255)
                .HasColumnName("descriptions");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Material)
                .HasMaxLength(255)
                .HasColumnName("material");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductionId).HasColumnName("productionID");
            entity.Property(e => e.SubcategoryId).HasColumnName("subcategoryID");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product$_Category$");

            entity.HasOne(d => d.Production).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product$_Production$");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product$_Subcategory$");
        });

        modelBuilder.Entity<Production>(entity =>
        {
            entity.ToTable("Production$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.ToTable("Size$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Subcategory>(entity =>
        {
            entity.ToTable("Subcategory$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fathername)
                .HasMaxLength(255)
                .HasColumnName("fathername");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.RoleId).HasColumnName("Role_Id");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users$_Role$");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouse$");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SizeId).HasColumnName("sizeID");

            entity.HasOne(d => d.Product).WithMany(p => p.Warehouses)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Warehouse$_Product$");

            entity.HasOne(d => d.Size).WithMany(p => p.Warehouses)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Warehouse$_Size$");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

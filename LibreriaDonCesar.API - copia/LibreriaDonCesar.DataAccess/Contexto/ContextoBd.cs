using Microsoft.EntityFrameworkCore;
using System;
using LibreriaDonCesar.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaDonCesar.DataAccess.Contexto
{
    public class ContextoBd : DbContext
    {
        public ContextoBd(DbContextOptions<ContextoBd> options) : base(options)
        {

        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Color> Colors { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Inventory> Inventories { get; set; } 

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Purchase> Purchases { get; set; }

        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<SaleDetail> SaleDetails { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<SaleTransaction>SaleTransactions { get; set; }

        public DbSet<PurchaseTransaction> purchaseTransactions { get; set; }

        public DbSet<Attributes >Attributes { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<UnitMeasure> UnitMeasures { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<Presentation> Presentations { get; set; }






    }
}

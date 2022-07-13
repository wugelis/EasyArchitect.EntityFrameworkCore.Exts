using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyArchitect.EntityFrameworkCore.ExtsTests.Fake.Models
{
    public class HousingLoansDbContext: DbContext
    {
#pragma warning disable CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
        public HousingLoansDbContext(DbContextOptions options)
#pragma warning restore CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
            : base(options)
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Housingloansdata>()
                .HasKey(o => new  { o.customerId, o.loansId });
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customerdetail> Customerdetails { get; set; }
        public DbSet<Housingloansdata> Housingloansdatas { get; set; }
    }
}

﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.Models.DataModels;

namespace Shop.DbContext
{
    public class ShopDbContext : IdentityDbContext<IdentityUser>
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<ProductPurchaseHistory> ProductPurchaseHistory { get; set; }
    }
}

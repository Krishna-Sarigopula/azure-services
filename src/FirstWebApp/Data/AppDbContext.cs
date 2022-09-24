﻿using Microsoft.EntityFrameworkCore;

namespace FirstWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Products> Products { get; set; }
    }
}

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Mvc_systemtest.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("server=Lucky;Database=master;uid=Laxman;pwd=Lucky@0221")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Cart { get; set; }
    }
}
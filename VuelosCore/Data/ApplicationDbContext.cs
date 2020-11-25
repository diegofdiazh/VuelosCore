using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using VuelosCore.Data.Entities;

namespace VuelosCore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public virtual DbSet<Aeropuertos> Aeropuertos { get; set; }
        public virtual DbSet<ReservaVuelo> ReservaVuelos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

    }
}

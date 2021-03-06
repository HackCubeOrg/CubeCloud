﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haris.DataModel.DataModels;

namespace Haris.DataModel
{
    public class HarisDbContext : DbContext
    {
        public HarisDbContext():base(nameOrConnectionString: "HarisConnectionString")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Cube> Cubes { get; set; }

        public DbSet<OutputCube> OutputCubes { get; set; }
    }
}

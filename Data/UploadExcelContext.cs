using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UploadExcel.Models;

namespace UploadExcel.Data
{
    public class UploadExcelContext : DbContext
    {
        public UploadExcelContext (DbContextOptions<UploadExcelContext> options)
            : base(options)
        {
        }

        public DbSet<UploadExcel.Models.People> People { get; set; } = default!;
    }
}

using intermag.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace intermag.Areas.Admin.Data
{
    public class Db : DbContext
    {
        public DbSet<PagesDTO> Pages { get; set; }

        public System.Data.Entity.DbSet<intermag.Models.ViewModels.Pages.PageVM> PageVMs { get; set; }

        public DbSet<SidebarDTO> Sidebars { get; set; }

        //подключение таблицы Categories
        public DbSet<CategoryDTO> Categories { get; set; }

        public DbSet<ProductDTO> Products { get; set; }
    }
}
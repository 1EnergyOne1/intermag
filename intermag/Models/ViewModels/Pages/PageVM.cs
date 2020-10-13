using intermag.Areas.Admin.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace intermag.Models.ViewModels.Pages
{
    public class PageVM
    {
        public PageVM()
        { }
        public PageVM(PagesDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;

        }
      
        public int Id { get; set; }
        [Required] //поле обязательное для заполнения
        [StringLength(50, MinimumLength = 3)] //ограничения на максимальное и минимальное длину строки
        public string Title { get; set; }
        public string Slug { get; set; }
        [Required] //поле обязательное для заполнения
        [StringLength(50, MinimumLength = 3)] //ограничения на максимальное и минимальное длину строки
        public string Body { get; set; }
        public int Sorting { get; set; }
        [Display(Name = "Sidebar")]
        public bool HasSidebar { get; set; }
    }
}
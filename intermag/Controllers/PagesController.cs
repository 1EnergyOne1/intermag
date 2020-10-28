using intermag.Areas.Admin.Data;
using intermag.Models.Data;
using intermag.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace intermag.Controllers
{
    public class PagesController : Controller
    {        
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            // Получаем/устанавливаем краткий заголовок (Slug)
            if (page == "")
                page = "home";

            // Объявляем модель и данные (класс DTO)
            PageVM model;
            PagesDTO dto;

            // Проверяем доступна ли текущая страница
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                    return RedirectToAction("Index", new { page = ""});
            }

            // Получае контекст данных этой страницы
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            // Устанавливаем нормальный заголовок страницы (Title)
            ViewBag.PageTitle = dto.Title;

                // Проверяем есть ли боковая панель (sidebar)
            if(dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            // Заполняем модель данными
            model = new PageVM(dto);

                // Возвращаем представление вместе с моделью
                return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            // Инициализируем лист PageVM
            List<PageVM> pageVMList;

            // Получаем все страницы, кроме Home
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }

                // Возвращаем частичное представление и лист с данными
                return PartialView("_PagesMenuPartial", pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            // Объявляем модель
            SidebarVM model;

            // Инициализировать модель данными
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);

                model = new SidebarVM(dto);
            }
                      

                // Возвращаем модель в частичное представление
                return PartialView("_SidebarPartial",model);
        }
    }
}
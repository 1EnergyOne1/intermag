using intermag.Areas.Admin.Data;
using intermag.Models.Data;
//using intermag.Models.Data;
using intermag.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace intermag.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Объявляем список для представления PageVM
            List<PageVM> pageList;
            //Инициализируем список (DB)
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
                //Возвращаем список в представление
                return View(pageList);
        }

        // GET: Admin/Pages/AddPages
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // // POST: Admin/Pages/AddPages

        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            else

                using (Db db = new Db())
                {

                    // Получаем Id страницы

                    int id = model.Id;

                    //Объявление переменной для краткого описания (Slug)

                    string slug;

                    //Инициализация класса PageDTO

                    PagesDTO dto = new PagesDTO();

                    //присвоение заголовка модели

                    dto.Title = model.Title.ToUpper();

                    //проверка есть ли краткое описание, если нет, то присваиваем его

                    
                        if (string.IsNullOrWhiteSpace(model.Slug))
                        {
                            slug = model.Title.Replace(" ", "-".ToLower());
                        }
                        else
                            slug = model.Slug.Replace(" ","-".ToLower());
                    

                    //Убеждаемся, что заголовок и краткое описание - уникальны

                    if (db.Pages.Any(x => x.Title == model.Title))
                    {
                        ModelState.AddModelError("", "Этот заголовок недоступен, ска");
                        return View(model);
                    }

                    else 
                        if (db.Pages.Any(x => x.Slug == model.Slug))
                    {
                        ModelState.AddModelError("", "Этот заголовок недоступен, ска");
                        return View(model);
                    }
                    //Присваивание оставшихся значений модели

                    dto.Slug = slug;
                    dto.Body = model.Body;
                    dto.HasSidebar = model.HasSidebar;
                    dto.Sorting = 100;

                    //сохраняем модель в БД

                    db.Pages.Add(dto);
                    db.SaveChanges();
                }

            //Вывод сообщения пользователю черещ TempData

            TempData["SM"] = "Сохранено епта";

            //Переадресовываем пользователя на метод Index

            return RedirectToAction("Index");
        }

        //GET: Admin/PAges/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {

                //получаем страницу и id
                PagesDTO dto = db.Pages.Find(id);

                //проверяем доступна ли страница
                if (dto == null)
                {
                    return Content("Страница не найдена, лошара");
                }

                //если страница доступна, то инициализируем модель данными
                model = new PageVM(dto);

            }
            //возвращаем модель в проедставлении
            return View(model);
        }

        //POST: Admin/PAges/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Получаем Id страницы

                int id = model.Id;

                //Объявление переменной для краткого описания (Slug)

                string slug = "home";

                //Инициализация класса PageDTO

                PagesDTO dto = db.Pages.Find(id);

                //присвоение заголовка модели

                dto.Title = model.Title;

                //проверка есть ли краткое описание, если нет, то присваиваем его

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-".ToLower());
                    }
                    else
                        slug = model.Slug.Replace(" ", "-".ToLower());
                }

                //Убеждаемся, что заголовок и краткое описание - уникальны

                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                else

                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == slug))
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model);
                }
                //Присваивание оставшихся значений модели

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //сохраняем модель в БД

                db.SaveChanges();
            }

            //устанавливаем сообщения для пользователя в TempDate
            TempData["SM"] = "You have edited the page.";

                //возвращаем представление для пользователя (обратно)


                return RedirectToAction("EditPage");
        }

        //Get: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //Убеждаемся что страница доступна
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }




                //Присваиваем модели все поля из БД по текущей странице

                model = new PageVM(dto);
            }
                //возвращаем модель в представление

                return View(model);
        }

        //Метод удаления страниц
        //Get: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Получение страницы
                PagesDTO dto = db.Pages.Find(id);

                //Удаление страницы
                db.Pages.Remove(dto);

                //Сохранение изменений в базе
                db.SaveChanges();
            }

            //Сообщение пользователю об успешном удалении

            TempData["SM"] = "Страница удалена епта";
            //Переадресация пользоателя на страницу Index

            return RedirectToAction("Index");
        }


        //Создаем метод сортировки
        [HttpPost]
        public void ReorderPages(int [] id)
        {
            using (Db db = new Db())
            {
                // Реализуем начальный счетчик
                int count = 1;

                // Инициализируем модель данных
                PagesDTO dto;

                //Устанавливаем сортировку для каждой страницы
                foreach(var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }

            }
        }

        //Get: Admin/Pages/EditSidebar

       public ActionResult EditSidebar(SidebarDTO row)
        {
            //Объявляем модель
            SidebarVM model;

            using (Db db = new Db())
            {
                //Получаем данные из DTO
                SidebarDTO dto = db.Sidebars.Find(1);

                //заполняем модель данными
                model = new SidebarVM(dto);
            }

                return View(model);
        }

        //Post: Admin/Pages/EditSidebar
        [HttpPost]

        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {


                //Получаем данные из DTO
                SidebarDTO dto = db.Sidebars.Find(1);

                //Присваиваем данные в тело (в свойство Body
                dto.Body = model.Body;

                //Сохранение
                db.SaveChanges();
            }
            //Сообщение об удачном редактировании
            TempData["SM"] = "Удачно отредактировано епта";

                //Переадресация полльзователя на даннную страницу
                return RedirectToAction("EditSidebar");
        }
    }
}
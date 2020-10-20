using intermag.Areas.Admin.Data;
using intermag.Models.Data;
using intermag.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace intermag.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            //Объявляем модель типа List
            List<CategoryVM> categoryVMList;

            
            using (Db db = new Db())
            {
                //Инициализируем модель данными
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
                
            }
                //Возвращаем List в представление
            return View(categoryVMList);
        }

        //POST: /admin/shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Объявляем строковую переменную типа Id
            string id;

            
            using (Db db = new Db())
            {
                //Проверка категории на уникальность
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                //Инициализация модели DTO
                CategoryDTO dto = new CategoryDTO();

                //Заполение данными
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                //Сохраняем изменения
                db.Categories.Add(dto);
                db.SaveChanges();

                //Получить id чтобы вернуть в представление
                id = dto.Id.ToString();

            }
            //Возвращаем Id в представление
            return id;

        }

        //Создаем метод сортировки
        
        //GET: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                // Реализуем начальный счетчик
                int count = 1;

                // Инициализируем модель данных
                CategoryDTO dto;

                //Устанавливаем сортировку для каждой страницы
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }

            }
        }

        //Метод удаления категории страницы
        //Get: Admin/Shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                //Получение категории страницы
                CategoryDTO dto = db.Categories.Find(id);

                //Удаление страницы
                db.Categories.Remove(dto);

                //Сохранение изменений в базе
                db.SaveChanges();
            }

            //Сообщение пользователю об успешном удалении

            TempData["SM"] = "Категория удалена епта";
            //Переадресация пользоателя на страницу Index

            return RedirectToAction("Categories");
        }


        //POST: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                //Проверка имени на уникальность
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                //Получаем данные из бд (модель DTO)
                CategoryDTO dto = db.Categories.Find(id);


                //Редактируем модель  DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                // Сохраняем изменения
                db.SaveChanges();

            }
            // Возвращаем слово

            return "Ок, Бро!";
            
        }

        //Создаем метод добавления товаров
        //Get: Admin/Shop/AddProduct

        public ActionResult AddProduct ()
        {
            // Объявляем модель данных
            ProductVM model = new ProductVM();

            // Добавляем список категорий из базы в модель
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }


                // Возвращаем модель в представление

                return View(model);
        }
    }
}
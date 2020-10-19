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
    }
}
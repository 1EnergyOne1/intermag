using intermag.Areas.Admin.Data;
using intermag.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace intermag.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // Объявляем модель типа List CategoryVM
            List<CategoryVM> categoryVMList;

            // Инициализируем модель данными
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

                // Возвращаем частичное представление с моделью
                return PartialView("_CategoryMenuPartial", categoryVMList);
        }
    }
}
﻿using intermag.Areas.Admin.Data;
using intermag.Models.Data;
using intermag.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
        [HttpGet]
        public ActionResult AddProduct()
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

        //Создаем метод добавления товаров
        //Post: Admin/Shop/AddProduct

        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            // проверка модели на валидность
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "ID", "Name");
                    return View(model);
                }
            }

            // проверка имени продукта на уникальность
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "ID", "Name");
                    ModelState.AddModelError("", "Это имя продукта занято, баран");
                    return View(model);
                }
            }


            // Объявление переменной ProductID
            int id;

            // Инициализируе и сохраняем в базу модель на основе ProductDTO
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;
            }

            // Добавляем сообщение в TempData
            TempData["SM"] = "Вы успешно вс ёепта добавили";

            #region Upload Image

            // Создаем необходимые ссылки на директории 
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            // Проверка наличия директорий (если нет, то создаём)
            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Проверяем загружен ли такой файл
            if (file != null && file.ContentLength > 0)
            {
                //Получаем расширение файла
                string ext = file.ContentType.ToLower();

                // Проверка расширения файла 
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Изображение нихрена не загружено. Формат блин не тот");
                        return View(model);
                    }
                }



                // Объявляем переменную с именем изображения
                string imageName = file.FileName;

                // Сохраняем имя изображения в модель DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Назначаем пути к оригинальному и уменьшеному изображению
                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                // Сохраняем оригинальное изображение
                file.SaveAs(path);

                // Создаем и сохраняем уменьшеную копию
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }
            #endregion

            // Переадресайия пользователя

            return RedirectToAction("AddProduct");
        }

        //Создаем метод списка товаров
        //Get: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult Products (int? page, int? catId)
        {
            // Объявляем модель ProductVM типа List
            List<ProductVM> listOfProductVM;

            // Устанавливаем номер страницы
            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                // Инициализируем list и заполняем данными
                listOfProductVM = db.Products.ToArray()
                                  .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                                  .Select(x => new ProductVM(x)).ToList();

                // Заполняем категории данными
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Устанавливаем выбранную категорию
                ViewBag.SelectedCat = catId.ToString();
            
            }

            //Устанавливаем постраничную навигацию
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.onePageOfProducts = onePageOfProducts;

            // Возвращаем представление с данными
            return View(listOfProductVM);
        }


        //Создаем метод редактирования товаров
        //Get: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct (int id )
        {
            //Объявляем модель ProductVM
            ProductVM model;

            
            using (Db db = new Db())
            {
                //Получаем продукт 
                ProductDTO dto = db.Products.Find(id);

                // Проверка доступен ли продукт
                if (dto == null)
                {
                    return Content("Нету такого");
                }

                // инициализируем модель данных
                model = new ProductVM(dto);

                //создаем список категорий
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // получаем все изображения из галереи
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fn => Path.GetFileName(fn));
            }

            // вернуть модель и представление
            return View(model);
        }


        //Создаем метод редактирования товаров
        //Post: Admin/Shop/EditProduct

        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            // Получаем id продукта
            int id = model.Id;

            // Заполняем выпадающий список категориями и изображениями
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fn => Path.GetFileName(fn));

            // Проверяем модель на валидность
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            // Проверка продукта на уникальность
            using (Db db = new Db())
            {
                if(db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Есть уже такое, урод!");
                    return View(model);
                }
            }

            // Обновляем продукт в бд
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            // выводим сообщение в TempData
            
            TempData["SM"] = "Вы успешно всё епта нахрен поменяли";

            // реализовываем логику обработки изображений (потом)
            #region Image Upload

            // Проверка загружен ли файл
            if (file != null && file.ContentLength > 0)
            {

                // Получаем расширение файла
                string ext = file.ContentType.ToLower();

                // Проверить расширение файла
                if (ext != "image/jpg" &&
                   ext != "image/jpeg" &&
                   ext != "image/pjpeg" &&
                   ext != "image/gif" &&
                   ext != "image/x-png" &&
                   ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "Изображение нихрена не загружено. Формат блин не тот");
                        return View(model);
                    }
                }

                // Установка путей загрузки
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // Если файлы существуют, то надо удаляем их в директориях и удаляем сами директории
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }
                
                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                // Сохраняем новое имя изображения
                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Сохраняем оригинал изображения и его превью
                // Назначаем пути к оригинальному и уменьшеному изображению
                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

                // Сохраняем оригинальное изображение
                file.SaveAs(path);

                // Создаем и сохраняем уменьшеную копию
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);
                img.Save(path2);

            }
            #endregion

            //Переадресайия пользователя
            return RedirectToAction("EditProduct");
        }

        //Создаем метод Удаления товара
        //Post: Admin/Shop/DeleteProduct/id
        public ActionResult DeleteProduct (int id)
        {
            // Удаляем товар из базы данных
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);
                db.SaveChanges();
            }

            // Удаляем директории товара (изображения)
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            // Переадресовываем пользователя

            return RedirectToAction("Products");
        }

        //Создаем метод добавления изображения в галерею
        //Post: Admin/Shop/SaveGalleryImages/id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Перебираем все полученные файлы
            foreach (string fileName  in Request.Files)
            {
                // Инициализируем файлы
                HttpPostedFileBase file = Request.Files[fileName];

                // Проверка на отсутствие файлов
                if (file != null && file.ContentLength >0)
                {
                    // Назначаем все пути к директориям
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // Назначаем пути изображений
                    var path = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    // Сохраняем оригинальные изображения и уменьшеные копии
                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1,1);
                    img.Save(path2);
                }
            }
        }
        //Создаем метод удаления изображения из галереи
        //Post: Admin/Shop/DeleteGalleryImages/id/imageName
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            // Прописываем пути
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            // Если есть, то удаляем оригинальные изображения и уменьшеные копии
            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
                        
        }
    }
}
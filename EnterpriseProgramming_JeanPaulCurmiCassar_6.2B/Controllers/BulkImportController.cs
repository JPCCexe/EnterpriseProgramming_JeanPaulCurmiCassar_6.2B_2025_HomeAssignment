using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Factory;
using System.IO.Compression;

namespace EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Controllers
{
    public class BulkImportController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public BulkImportController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult BulkImport()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BulkImport(string jsonData,
            [FromKeyedServices("memory")] IItemsRepository memCache)
        {
            //Use sample data if empty
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                jsonData = @"[{""type"":""restaurant"",""name"":""Test Restaurant"",""ownerEmailAddress"":""test@example.com"",""phone"":""123456""}]";
            }

            //Parse JSON using factory
            var factory = new ImportItemFactory();
            var items = factory.Create(jsonData);

            //Save to memory
            memCache.Save(items);

            //Generate ZIP
            string zipFile = CreateZip(items);
            TempData["Zip"] = zipFile;

            ViewBag.Message = $"Parsed {items.Count} items";
            return View("Preview", items);
        }

        private string CreateZip(List<IItemValidating> items)
        {
            //Setup paths
            string tempFolder = Path.Combine(_env.WebRootPath, "temp", Guid.NewGuid().ToString());
            string defaultImg = Path.Combine(_env.WebRootPath, "images", "default.jpg");
            Directory.CreateDirectory(tempFolder);

            //Create folders with images
            foreach (var item in items)
            {
                string folderId = "";
                if (item is Restaurant r)
                {
                    folderId = $"restaurant-{Guid.NewGuid()}";
                }
                else if (item is MenuItem m)
                {
                    folderId = $"menuitem-{m.Id}";
                }
                string itemFolder = Path.Combine(tempFolder, folderId);
                Directory.CreateDirectory(itemFolder);
                
                if (System.IO.File.Exists(defaultImg))
                {
                    System.IO.File.Copy(defaultImg, Path.Combine(itemFolder, "default.jpg"), true);
                }
            }

            //Create the zip
            string zipName = $"items-{DateTime.Now:yyyyMMddHHmmss}.zip";
            string zipPath = Path.Combine(_env.WebRootPath, "temp", zipName);

            //Delete if exists
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(tempFolder, zipPath);

            //Cleanup
            Directory.Delete(tempFolder, true);
            
            return zipName;
        }

        [HttpGet]
        public IActionResult DownloadZip()
        {
            string zipName = TempData["Zip"] as string;
            if (zipName == null) return NotFound();

            string zipPath = Path.Combine(_env.WebRootPath, "temp", zipName);
            if (!System.IO.File.Exists(zipPath)) return NotFound();

            return File(System.IO.File.ReadAllBytes(zipPath), "application/zip", zipName);
        }

        [HttpGet]
        public IActionResult Commit()
        {
            //Show upload form for ZIP with images
            return View();
        }

        [HttpPost]
        public IActionResult Commit(IFormFile zipFile,
            [FromKeyedServices("memory")] IItemsRepository memCache,
            [FromKeyedServices("db")] IItemsRepository dbRepo)
        {
            if (zipFile == null || zipFile.Length == 0)
            {
                TempData["error"] = "Please upload a ZIP file";
                return View();
            }

            //Get items from memory
            var items = memCache.Get();
            if (items.Count == 0)
            {
                TempData["error"] = "No items in memory. Please upload JSON first.";
                return RedirectToAction("BulkImport");
            }

            //Extract ZIP and process images
            ProcessZipAndLinkImages(zipFile, items);

            //Save to database
            dbRepo.Save(items);

            //Clear memory cache
            if (memCache is DataAccess.Repositories.ItemsInMemoryRepository inMemRepo)
            {
                inMemRepo.Clear();
            }

            TempData["success"] = $"Successfully saved {items.Count} items to database!";
            return RedirectToAction("Catalog", "ItemsRestaurant");
        }

        private void ProcessZipAndLinkImages(IFormFile zipFile, List<IItemValidating> items)
        {
            //Create temp folder for extraction
            string extractPath = Path.Combine(_env.WebRootPath, "temp", Guid.NewGuid().ToString());
            Directory.CreateDirectory(extractPath);

            //Save uploaded ZIP temporarily
            string zipPath = Path.Combine(extractPath, "upload.zip");
            using (var stream = new FileStream(zipPath, FileMode.Create))
            {
                zipFile.CopyTo(stream);
            }

            //Extract ZIP
            ZipFile.ExtractToDirectory(zipPath, extractPath);

            //Process each item and link images
            foreach (var item in items)
            {
                string folderName = "";
                if (item is Restaurant r)
                {
                    folderName = $"restaurant-{r.Name.Replace(" ", "-").ToLower()}";
                }
                else if (item is MenuItem m)
                {
                    folderName = $"menuitem-{m.Id}";
                }

                string itemFolder = Path.Combine(extractPath, folderName);
                if (Directory.Exists(itemFolder))
                {
                    //Find image in folder
                    var imageFiles = Directory.GetFiles(itemFolder, "*.jpg")
                        .Concat(Directory.GetFiles(itemFolder, "*.png"))
                        .ToArray();

                    if (imageFiles.Length > 0)
                    {
                        //Copy image to wwwroot/images with unique name
                        string originalFile = imageFiles[0];
                        string uniqueName = $"{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(originalFile)}";
                        string destinationPath = Path.Combine(_env.WebRootPath, "images", uniqueName);

                        System.IO.File.Copy(originalFile, destinationPath, true);

                        //Link image to item
                        if (item is Restaurant rest)
                        {
                            rest.ImagePath = $"/images/{uniqueName}";
                        }
                        else if (item is MenuItem menu)
                        {
                            menu.ImagePath = $"/images/{uniqueName}";
                        }
                    }
                }
            }

            //Cleanup temp folder
            Directory.Delete(extractPath, true);
        }
    }
}
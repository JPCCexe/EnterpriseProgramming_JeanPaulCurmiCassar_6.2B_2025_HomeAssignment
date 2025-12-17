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
        public IActionResult BulkImport(IFormFile jsonFile,
        [FromKeyedServices("memory")] IItemsRepository memCache)
        {
            //Check if file was uploaded
            if (jsonFile == null || jsonFile.Length == 0)
            {
                ViewBag.Error = "Please upload a JSON file";
                return View();
            }

            //Reading the json from file
            string jsonData = "";
            using (var reader = new StreamReader(jsonFile.OpenReadStream()))
            {
                jsonData = reader.ReadToEnd();
            }

            //Use the factory to parse json into items
            var factory = new ImportItemFactory();
            var items = factory.Create(jsonData);

            //Saving into the memmory cache
            memCache.Save(items);

            //Generate ZIP with folder and default images
            string zipFile = CreateZip(items);
            TempData["Zip"] = zipFile;

            ViewBag.Message = $"Parsed {items.Count} items from {jsonFile.FileName}";
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
                    folderId = $"restaurant-{r.Name.Replace(" ", "-").ToLower()}";
                }
                else if (item is MenuItem m)
                {
                    folderId = $"menuitem-{m.Title.Replace(" ", "-").ToLower()}";
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
            // Create images folder if not exists
            string imagesFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            string extractPath = Path.Combine(_env.WebRootPath, "temp", Guid.NewGuid().ToString());
            Directory.CreateDirectory(extractPath);

            // Extract the uploaded zip to temp folder
            string zipPath = Path.Combine(extractPath, "upload.zip");
            using (var stream = new FileStream(zipPath, FileMode.Create))
            {
                zipFile.CopyTo(stream);
            }

            //Extract zip
            ZipFile.ExtractToDirectory(zipPath, extractPath);

            //Check if zip extracted to a nested folder
            var subFolders = Directory.GetDirectories(extractPath);
            string searchPath = extractPath;

            //If there's only one subfolder, it might be a nested zip structure
            if (subFolders.Length == 1 && !Path.GetFileName(subFolders[0]).StartsWith("restaurant-")
                                        && !Path.GetFileName(subFolders[0]).StartsWith("menuitem-"))
            {
                searchPath = subFolders[0];
            }

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
                    folderName = $"menuitem-{m.Title.Replace(" ", "-").ToLower()}";
                }

                string itemFolder = Path.Combine(searchPath, folderName);

                if (Directory.Exists(itemFolder))
                {
                    //Find image in folder and can have any name
                    var imageFiles = Directory.GetFiles(itemFolder, "*.jpg")
                        .Concat(Directory.GetFiles(itemFolder, "*.png"))
                        .Concat(Directory.GetFiles(itemFolder, "*.jpeg"))
                        .ToArray();

                    if (imageFiles.Length > 0)
                    {
                        string originalFile = imageFiles[0];
                        string uniqueName = $"{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(originalFile)}";
                        string destinationPath = Path.Combine(_env.WebRootPath, "images", uniqueName);

                        System.IO.File.Copy(originalFile, destinationPath, true);

                        //Linking the image to item
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
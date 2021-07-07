using BFNet_2021_StorageMVC.Models.Entities;
using BFNet_2021_StorageMVC.Repositories;
using BFNet_2021_StorageMVC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageMVC.Controllers
{
    public class ImageController : Controller
    {
        private ILogger<ImageController> _Logger;
        public ImageController(ILogger<ImageController> logger)
        {
            _Logger = logger;
        }
        public IActionResult Upload([FromServices] BlobStorageService storageService, [FromServices] UserRepository repo, IFormFile file)
        {
            Stream stream = file.OpenReadStream();
            byte[] bytes = new byte[file.Length];
            stream.Read(bytes);
            string path = storageService.UploadFile(file.FileName, bytes, file.ContentType);
            _Logger.LogDebug(path);
            User u = new User();
            u.PartitionKey = "user";
            u.RowKey = "1";
            u.Username = "Flavian";
            u.Password = "Test1234=";
            u.Timestamp = DateTime.Now;
            u.IconPath = path;

            repo.Create(u);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ReadAll([FromServices] BlobStorageService storageService)
        {
            return View(storageService.ReadFiles());
        }

    }
}

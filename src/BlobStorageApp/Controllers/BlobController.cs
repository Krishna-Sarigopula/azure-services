using BlobStorageApp.Models;
using BlobStorageApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorageApp.Controllers
{
    public class BlobController : Controller
    {
        StorageService StorageService;

        public BlobController(StorageService storageService)
        {
            StorageService = storageService;
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(BlobUploadModel blobUploadModel)
        {
            StorageService.UploadFile(blobUploadModel.FormFile, blobUploadModel.Name, blobUploadModel.FileName);
            return View();
        }

        [HttpGet("list")]
        public IActionResult List()
        {
            var items = StorageService.GetBlobItems();
            return View(items);
        }

        [HttpGet("Files/{name}")]
        public IActionResult Files(string name)
        {
            var items = StorageService.GetBlobItems(name);
            return View(items);
        }

        [HttpGet("FileInfo/{fileName}")]
        public IActionResult FileInfo(string fileName)
        {
            var items = StorageService.GetBlobItems(fileName);
            return View(items);
        }
    }
}

//using ECommerce.Core.DTOs.Product;
//using ECommerce.Core.Entities;
//using ECommerce.Core.Result_Pattern;
//using ECommerce.Core.Services;
//using ECommerce.Infrastructure.Data;
//using Microsoft.AspNetCore.Hosting;

//using System.IO;
//namespace ECommerce.Infrastructure.Services
//{
//    public class FileService : IFileService
//    {
//        private readonly AppDbContext context;
//        private readonly IWebHostEnvironment webHostEnvironment;

//        public FileService(AppDbContext _context, IWebHostEnvironment _webHostEnvironment)
//        {
//            context = _context;
//            webHostEnvironment = _webHostEnvironment;
//        }
//        public Result UploadFiles(UploadFileFormDto model)
//        {
//            if (model.Files.Count > 0)
//            {
//                List<ProductImage> uploadFiles = new List<ProductImage>();
//                foreach (var item in model.Files)
//                {
//                    var fakFileName = Path.GetRandomFileName();
//                    ProductImage uploadFile = new ProductImage()
//                    {
//                        ContentType = item.ContentType,
//                        ImageName = item.FileName,
//                        StoredImageName = fakFileName
//                    };
//                    var path = Path.Combine(webHostEnvironment.WebRootPath, "Files", fakFileName);

//                    FileStream fileStream = new(path, FileMode.Create);
//                    item.CopyTo(fileStream);
//                    uploadFiles.Add(uploadFile);
//                }
//                context.ProductImages.AddRange(uploadFiles);
//                context.SaveChanges();
//                return Result.Success();
//            }
//            return Result.Failure("Is Empty", 400);
//        }

//        public Result DownloadFile(string fileName)
//        {
//            var uploadfile = context.ProductImages.SingleOrDefault(x => x.StoredImageName == fileName);
//            if (uploadfile == null)
//                return Result.Failure("Not found",404);

//            var path = Path.Combine(webHostEnvironment.WebRootPath, "Files", fileName);

//            MemoryStream memory = new MemoryStream();
//            FileStream fileStream = new(path, FileMode.Open);
//            fileStream.CopyTo(memory);
//            memory.Position = 0;
//            object obj = new{
//                memory,
//                uploadfile.ContentType,
//                uploadfile.ImageName
//            };
//            return Result<object>.Success(obj);
//        }

//        public Result DeleteFile(string fileName)
//        {
//            // 1. نبحث في الداتا بيز عن الملف
//            var uploadFile = context.ProductImages.SingleOrDefault(x => x.StoredImageName == fileName);
//            if (uploadFile == null)
//                return Result.Failure("Not Found",404);

//            // 2. نحدد المسار على السيرفر
//            var path = Path.Combine(webHostEnvironment.WebRootPath, "Files", fileName);

//            // 3. لو الملف موجود على السيرفر نمسحه
//            if (System.IO.File.Exists(path))
//            {
//                System.IO.File.Delete(path);
//            }

//            // 4. نمسح الـ record من الداتا بيز
//            context.ProductImages.Remove(uploadFile);
//            context.SaveChanges();

//            // 5. بعد المسح نرجع لصفحة الملفات أو Index
//           return Result.Success();
//        }

//    }
//}

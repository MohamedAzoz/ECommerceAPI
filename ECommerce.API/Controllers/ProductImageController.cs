using ECommerce.Core.DTOs.Product;
using ECommerce.Core.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProductImageController> _logger;

        // قبول IConfiguration لو أردت قراءة max file size أو whitelist من appsettings
        public ProductImageController(AppDbContext context, IWebHostEnvironment env, ILogger<ProductImageController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // POST api/ProductImage/upload
        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadFiles([FromForm] UploadFileFormDto model, CancellationToken cancellationToken)
        //{
        //    if (model?.Files == null || model.Files.Count == 0)
        //        return BadRequest(new { message = "No files provided." });

        //    // إعداد whitelist بسيطة (يمكن جعلها من IConfiguration)
        //    var allowedContentTypes = new[] { "image/png", "image/jpeg", "image/jpg", "image/gif" };
        //    long maxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        //    var savedFiles = new List<ProductImage>();
        //    var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Files");
        //    Directory.CreateDirectory(uploadsFolder);

        //    foreach (var file in model.Files)
        //    {
        //        if (file == null || file.Length == 0)
        //            continue;

        //        if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        //        {
        //            _logger.LogWarning("Rejected file with content type {ContentType}", file.ContentType);
        //            return BadRequest(new { message = $"Unsupported file type: {file.ContentType}" });
        //        }

        //        if (file.Length > maxFileSizeBytes)
        //        {
        //            return BadRequest(new { message = $"File too large. Max is {maxFileSizeBytes} bytes." });
        //        }

        //        // امن: اجعل الاسم فقط اسم ملف
        //        var originalFileName = Path.GetFileName(file.FileName); // يمنع path traversal
        //        var extension = Path.GetExtension(originalFileName);
        //        // استخدم GUID أو timestamp لتقليل التصادمات، واحتفظ بالامتداد
        //        var storedFileName = $"{Guid.NewGuid()}{extension}";

        //        var filePath = Path.Combine(uploadsFolder, storedFileName);

        //        try
        //        {
        //            // استخدم using لضمان إغلاق الملف
        //            await using (var stream = new FileStream(filePath, FileMode.CreateNew))
        //            {
        //                await file.CopyToAsync(stream, cancellationToken);
        //            }

        //            var entity = new ProductImage
        //            {
        //                ContentType = file.ContentType,
        //                ImageName = originalFileName,
        //                StoredImageName = storedFileName,
        //                // إضافة حقول مساعدة إن وُجدت في الـ entity مثل CreatedAt أو ProductId
        //                ProductId = model.ProductId,
        //                AltText = Path.GetFileNameWithoutExtension(file.FileName)
        //            };

        //            savedFiles.Add(entity);
        //        }
        //        catch (IOException ex)
        //        {
        //            _logger.LogError(ex, "Error saving file {FileName}", originalFileName);
        //            // تنظيف أي ملفات أنشئت مسبقاً في هذه الدفعة
        //            foreach (var s in savedFiles)
        //            {
        //                var p = Path.Combine(uploadsFolder, s.StoredImageName);
        //                if (System.IO.File.Exists(p)) System.IO.File.Delete(p);
        //            }
        //            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to save files." });
        //        }
        //    }

        //    if (savedFiles.Count == 0)
        //        return BadRequest(new { message = "No valid files to save." });

        //    await _context.ProductImages.AddRangeAsync(savedFiles);
        //    await _context.SaveChangesAsync(cancellationToken);

        //    // أعد قائمة بالملفات مع الرابط للتحميل
        //    var results = savedFiles.Select(f => new
        //    {
        //        f.Id,
        //        f.ImageName,
        //        f.StoredImageName,
        //        downloadUrl = Url.Action(nameof(DownloadFile), "ProductImage", new { fileName = f.StoredImageName }, Request.Scheme)
        //    });

        //    return Created(string.Empty, results);
        //}

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles([FromForm] UploadFileFormDto model, CancellationToken cancellationToken)
        {
            // 1. التحقق الأساسي من الملفات ومعرف المنتج
            if (model?.Files == null || model.Files.Count == 0)
                return BadRequest(new { message = "No files provided." });

            // (يجب أن يتم التحقق من وجود ProductId هنا، وإلا قد يفشل لاحقاً)
            if (model.ProductId <= 0)
                return BadRequest(new { message = "ProductId is required." });

            // 2. البحث عن المنتج للتأكد من وجوده ولتحديثه
            var productToUpdate = await _context.Products
                .SingleOrDefaultAsync(p => p.Id == model.ProductId, cancellationToken);

            if (productToUpdate == null)
                return NotFound(new { message = $"Product with ID {model.ProductId} not found." });


            // إعداد whitelist بسيطة (يمكن جعلها من IConfiguration)
            var allowedContentTypes = new[] { "image/png", "image/jpeg", "image/jpg", "image/gif" };
            long maxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

            var savedFiles = new List<ProductImage>();
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Files");
            Directory.CreateDirectory(uploadsFolder);

            string mainImageUrl = null; // متغير لتخزين اسم أول ملف مرفوع (الصورة الرئيسية)

            foreach (var file in model.Files.Select((f, i) => new { File = f, Index = i }))
            {
                if (file.File == null || file.File.Length == 0)
                    continue;

                // ... (تحقق من نوع وحجم الملف)
                if (!allowedContentTypes.Contains(file.File.ContentType.ToLowerInvariant()) || file.File.Length > maxFileSizeBytes)
                {
                    // يمكنك التعامل مع الأخطاء هنا أو تخطي الملف غير الصالح
                    _logger.LogWarning("File rejected due to type or size.");
                    continue;
                }

                var originalFileName = Path.GetFileName(file.File.FileName);
                var extension = Path.GetExtension(originalFileName);
                var storedFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, storedFileName);

                try
                {
                    // حفظ الملف على السيرفر
                    await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        await file.File.CopyToAsync(stream, cancellationToken);
                    }

                    // 3. تعيين الصورة الرئيسية (IsMain) وتحديد الـ ImgUrl
                    var isMain = file.Index == 0; // أول ملف مرفوع هو الرئيسي
                    if (isMain)
                    {
                        mainImageUrl = storedFileName;
                    }

                    var entity = new ProductImage
                    {
                        ContentType = file.File.ContentType,
                        ImageName = originalFileName,
                        StoredImageName = storedFileName,
                        ProductId = model.ProductId,
                        AltText = Path.GetFileNameWithoutExtension(file.File.FileName),
                        IsMain = isMain // تعيين إذا ما كانت الصورة رئيسية
                    };

                    savedFiles.Add(entity);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Error saving file {FileName}", originalFileName);
                    // التعامل مع أخطاء الحفظ...
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to save files." });
                }
            }

            if (savedFiles.Count == 0)
                return BadRequest(new { message = "No valid files were successfully processed." });

            // 4. حفظ سجلات الصور الجديدة في ProductImages
            await _context.ProductImages.AddRangeAsync(savedFiles);

            // 5. تحديث خاصية ImgUrl في كيان المنتج
            if (mainImageUrl != null)
            {
                // حفظ فقط اسم الملف المخزن، والمسار الكامل (URL) يتم بناؤه في الفرونت إند أو عبر action DownloadFile
                productToUpdate.ImageUrl = mainImageUrl;
            }

            // 6. حفظ جميع التغييرات (صور المنتج + تحديث المنتج نفسه)
            await _context.SaveChangesAsync(cancellationToken);

            // ... (كود إرجاع النتائج)
            var results = savedFiles.Select(f => new
            {
                f.Id,
                f.ImageName,
                f.StoredImageName,
                IsMain = f.IsMain,
                downloadUrl = Url.Action(nameof(DownloadFile), "ProductImage", new { fileName = f.StoredImageName }, Request.Scheme)
            });

            return Created(string.Empty, results);
        }

        // GET api/ProductImage/download/{fileName}
        //[Authorize("Admin")]
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest();

            // امن: لا تسمح بمسارات مثل ../../
            fileName = Path.GetFileName(fileName);

            var uploadfile = await _context.ProductImages.SingleOrDefaultAsync(x => x.StoredImageName == fileName);
            if (uploadfile == null)
                return NotFound("Not found");

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Files");
            var path = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(path))
                return NotFound("File not found on server");

            var memory = new MemoryStream();
            await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, uploadfile.ContentType ?? "application/octet-stream", uploadfile.ImageName);
        }

        // DELETE api/ProductImage/{fileName}
        [Authorize("Admin")]
        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest();

            fileName = Path.GetFileName(fileName);

            var uploadFile = await _context.ProductImages.SingleOrDefaultAsync(x => x.StoredImageName == fileName);
            if (uploadFile == null)
                return NotFound("Not Found");

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Files");
            var path = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(path))
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete file from disk: {Path}", path);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to delete file from disk." });
                }
            }

            _context.ProductImages.Remove(uploadFile);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Handlers
{
    /// <summary>
    /// Global exception handler that catches all unhandled exceptions in the application.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // تحديد نوع الخطأ والاستجابة المناسبة
            var (statusCode, title, detail) = exception switch
            {
                // أخطاء التحقق من الصحة
                ValidationException validationEx => (
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    validationEx.Message
                ),

                // أخطاء عدم وجود البيانات
                KeyNotFoundException notFoundEx => (
                    StatusCodes.Status404NotFound,
                    "Resource Not Found",
                    notFoundEx.Message
                ),

                // أخطاء عدم الصلاحية
                UnauthorizedAccessException _ => (
                    StatusCodes.Status403Forbidden,
                    "Forbidden",
                    "You don't have permission to access this resource."
                ),

                // أخطاء قاعدة البيانات
                DbUpdateException dbEx => (
                    StatusCodes.Status500InternalServerError,
                    "Database Error",
                    "A database error occurred. Please try again later."
                ),

                // أخطاء Argument
                ArgumentException argEx => (
                    StatusCodes.Status400BadRequest,
                    "Invalid Argument",
                    argEx.Message
                ),

                // باقي الأخطاء
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "An error occurred",
                    "An internal server error occurred. Please try again later."
                )
            };

            // تسجيل الخطأ بشكل مفصل
            _logger.LogError(
                exception,
                "An unhandled exception occurred: {ExceptionType} - {Message} | Path: {Path} | User: {User}",
                exception.GetType().Name,
                exception.Message,
                httpContext.Request.Path,
                httpContext.User?.Identity?.Name ?? "Anonymous"
            );

            // إنشاء ProblemDetails
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };

            // إضافة معلومات إضافية في Development فقط
            var environment = httpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>();

            if (environment.IsDevelopment())
            {
                problemDetails.Extensions["exceptionType"] = exception.GetType().FullName;
                problemDetails.Extensions["exceptionMessage"] = exception.Message;

                // إضافة أول 5 أسطر من Stack Trace فقط
                if (exception.StackTrace != null)
                {
                    problemDetails.Extensions["stackTrace"] = exception.StackTrace
                        .Split('\n')
                        .Take(5)
                        .Select(line => line.Trim())
                        .ToList();
                }

                // إضافة Inner Exception إذا وجد
                if (exception.InnerException != null)
                {
                    problemDetails.Extensions["innerException"] = new
                    {
                        type = exception.InnerException.GetType().Name,
                        message = exception.InnerException.Message
                    };
                }
            }

            // ضبط Status Code والاستجابة
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // تم معالجة الخطأ بنجاح
        }
    }
}
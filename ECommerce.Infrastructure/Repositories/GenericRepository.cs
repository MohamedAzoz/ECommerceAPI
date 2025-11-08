using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace ECommerce.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext context;

        public GenericRepository(AppDbContext _context)
        {
            context = _context;
        }
        public async Task<Result> ClearAsync(Expression<Func<T, bool>> expression)
        {
            // ملاحظة: EF Core لا يحتوي على RemoveRangeAsync، لكن الاستعلام يمكن أن يكون Async
            var items = await context.Set<T>().Where(expression).ToListAsync();

            if (items.Count == 0)
            {
                return Result.Success();
            }

            context.RemoveRange(items);

            return Result.Success();
        }


        public async Task<Result<ICollection<T>>> FindAllAsync(Expression<Func<T,bool>> expression)
        {
           var items=await context.Set<T>().Where(expression).ToListAsync();

            return Result<ICollection<T>>.Success(items);
        }

        public async Task<Result<T>> AddAsync(T item)
        {
            if (item == null)
            {
                return Result<T>.Failure("Cannot add a null item.");
            }
            await context.AddAsync(item); // استخدام Async
            return Result<T>.Success(item);
        }

        public Result Delete(T item)
        {
            if (item == null)
            {
                return Result.Failure("Cannot delete a null item."); // استخدام Result غير العام
            }
            context.Remove(item);
            return Result.Success(); // استخدام Result غير العام
        }

        public async Task<Result<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            var item = await context.Set<T>().FirstOrDefaultAsync(expression);
            if (item == null)
            {
                // إرجاع فشل لأن "Find" تعني البحث عن عنصر واحد محدد ونتوقع وجوده
                return Result<T>.Failure("Item not found matching the criteria.");
            }
            return Result<T>.Success(item);
        }

        public async Task<Result<ICollection<T>>> GetAllAsync()
        {
            var items = await context.Set<T>().ToListAsync();
            // إرجاع نجاح حتى لو كانت القائمة فارغة (هذا طبيعي)
            return Result<ICollection<T>>.Success(items);
        }

        public Result<T> Update(T item)
        {
            if (item == null)
            {
                return Result<T>.Failure("Cannot update a null item.");
            }
            context.Update(item);
            // التصحيح: يجب إرجاع كائن Result<T>
            return Result<T>.Success(item);
        }

        public async Task<Result<T>> GetWithIncludeAsync(
     Expression<Func<T, bool>> expression,
     params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>();

            // تطبيق الـ Includes (التحميل المتضمن)
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // البحث عن العنصر وتطبيق الشرط
            var item = await query.FirstOrDefaultAsync(expression);

            if (item == null)
            {
                return Result<T>.Failure("Item not found matching the criteria.");
            }

            return Result<T>.Success(item);
        }

        // في الكلاس العام Repository<T>
        public async Task<Result<ICollection<T>>> FindAllWithIncludeAsync(
            Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>();

            // 1. تطبيق التحميل المتضمن (Eager Loading)
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // 2. تطبيق شرط البحث (Where Clause)
            query = query.Where(expression);

            // 3. تنفيذ الاستعلام وجلب البيانات
            var items = await query.ToListAsync();

            if (!items.Any())
            {
                // يمكنك اختيار إرجاع نجاح بقائمة فارغة بدلاً من فشل، لكن هذا يعتمد على منطقك
                // هنا سنرجع فشل إذا لم نجد أي شيء مطابق للشرط
                return Result<ICollection<T>>.Failure("No items found matching the criteria.");
            }

            return Result<ICollection<T>>.Success(items);
        }

    }
}

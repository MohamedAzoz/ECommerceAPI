namespace ECommerce.Core.Result_Pattern
{

    //public class Result
    //{
    //    public bool IsSuccess { get; }
    //    public string Error { get; }
    //    protected Result(bool isSuccess,string error) {
    //        IsSuccess = isSuccess;
    //        Error = error;
    //    }
    //    public static Result Success()=> new Result(true,null);
    //    public static Result Failure(string error) => new Result(false, error);

    //}

    //public class Result<T> : Result
    //{
    //    public T Value { get;}
    //    protected Result(bool isSuccess,T value, string error) : base(isSuccess, error)
    //    {
    //        Value = value;
    //    }
    //    public static Result<T> Success(T value)=> new Result<T>(true,value,null);
    //    public static Result<T> Failure(string error)=>new Result<T>(false,default(T) ,error);
    //}

    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public int? StatusCode { get; }
        public bool IsFailure => !IsSuccess; // خاصية مساعدة للقراءة

        // جعل الباني Protected
        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }
        protected Result(bool isSuccess, string error,int statusCode)
        {
            IsSuccess = isSuccess;
            Error = error;
            StatusCode = statusCode;
        }

        public static Result Success() => new Result(true, null);
        //public static Result Failure(string error) => new Result(false, error);
        public static Result Failure(string error, int statusCode = 400) // القيمة الافتراضية 400
        => new Result(false, error, statusCode);
    
    }

    // ------------------------------------------------------------------

    public class Result<T> : Result
    {
        public T Value { get; }

        // باني خاص بالنجاح (Success)
        protected Result(T value) : base(true, null)
        {
            Value = value;
        }

        // باني خاص بالفشل (Failure)
        protected Result(string error,int statusCode) : base(false, error, statusCode)
        {
            Value = default(T); // القيمة الافتراضية
        }

        public static Result<T> Success(T value) => new Result<T>(value);

        // يمكن لدالة الفشل أن تقبل Result غير نوعية (مثل Result.Failure())
        //public static Result<T> Failure(string error) => new Result<T>(error);
        public static Result<T> Failure(string error, int statusCode = 400)
        => new Result<T>(error, statusCode);
    
    }
}

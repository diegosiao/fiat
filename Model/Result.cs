namespace Fiat
{
    public class Result
    {
        public bool Succeded { get; protected set; }

        public bool Failed => !Succeded;

        public string Info { get; protected set; }

        public string Error { get; protected set; }

        protected Result() { }

        public static Result Success(string info = null)
        {
            return new Result() { Succeded = true, Info = info };
        }

        public static Result Failure(string error)
        {
            return new Result() { Error = error };
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; private set; }

        public static Result<T> Success(T value)
        {
            return new Result<T>() { Succeded = true, Value = value };
        }

        new public static Result<T> Failure(string error)
        {
            return new Result<T>() { Error = error };
        }
    }
}

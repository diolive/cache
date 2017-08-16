namespace DioLive.BlackMint.Logic
{
    public class Response<TResult>
    {
        public Response(ResponseStatus status)
        {
            Status = status;
        }

        public ResponseStatus Status { get; }

        public TResult Result { get; private set; }

        public static Response<TResult> Forbidden()
        {
            return new Response<TResult>(ResponseStatus.Forbidden);
        }

        public static Response<TResult> NotFound()
        {
            return new Response<TResult>(ResponseStatus.NotFound);
        }

        public static Response<TResult> Success(TResult result)
        {
            return new Response<TResult>(ResponseStatus.Success) { Result = result };
        }
    }
}
namespace Swagger2Doc.Models.DTO
{
    public interface ILogBase
    {
        public int ResponseStatusCode { get; set; }

        public Guid ActionId { get; set; }

        public string? Method { get; set; }

        public string? Url { get; set; }

        public string? Action { get; set; }

        public object? RequestHeaders { get; set; }

        public object? RequestModel { get; set; }

        public object? ResponseData { get; set; }

        public object? CreateTime { get; set; }

        public object? LastUpdate { get; set; }
    }

    public class LogBase : ILogBase
    {
        public int ResponseStatusCode { get; set; }

        public Guid ActionId { get; set; }

        public string? Method { get; set; }

        public string? Url { get; set; }

        public string? Action { get; set; }

        public object? RequestHeaders { get; set; }

        public object? RequestModel { get; set; }

        public object? ResponseData { get; set; }

        public object? CreateTime { get; set; }

        public object? LastUpdate { get; set; }
    }

    public interface IWebClientLog : ILogBase
    {
        public string? TraceIdentifier { get; set; }

        public string? IP { get; set; }

        /// <summary>
        /// Debog or Some Infomation
        /// </summary>
        public object? Content { get; set; }
    }

    public class WebClientLog : LogBase, IWebClientLog
    {
        public string? TraceIdentifier { get; set; }

        public string? IP { get; set; }

        /// <summary>
        /// Debog or Some Infomation
        /// </summary>
        public object? Content { get; set; }
    }

    public interface IApiLog
    {
        public Guid CallApiActionId { get; set; }
    }

    public class ApiLog : LogBase, IApiLog
    {
        public Guid CallApiActionId { get; set; }

        public long? DurationMilliseconds { get; set; }
    }

    public interface IExceptionLog : ILogBase
    {
        public string? ErrorCode { get; set; }

        public string? ErrorTitle { get; set; }

        public string? ErrorMessage { get; set; }

        public object? ExceptionObj { get; set; }
    }

    public class ExceptionLog : WebClientLog, IExceptionLog
    {
        public string? ErrorCode { get; set; }

        public string? ErrorTitle { get; set; }

        public string? ErrorMessage { get; set; }

        public object? ExceptionObj { get; set; }
    }

}

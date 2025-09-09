using System.Net;

namespace BudgetManager.Api.Models.Interfaces
{
    public interface IApiResponse
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }

    public interface IApiResponse<T> : IApiResponse
    {
        public T Result { get; set; }
    }

}

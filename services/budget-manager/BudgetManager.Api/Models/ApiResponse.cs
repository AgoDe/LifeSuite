using System.Net;
using BudgetManager.Api.Models.Interfaces;

namespace BudgetManager.Api.Models
{
    public class ApiResponse : IApiResponse
    {
        
        public enum ApiResponseType
        {
            Success,
            Error,
            NotFound,
            Created,
            NoContent

        }
         
        public bool IsSuccess { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; } 

        public ApiResponse(ApiResponseType type)
        {
            switch (type) 
            {
                case ApiResponseType.Success:
                    IsSuccess = true;
                    StatusCode = HttpStatusCode.OK; 
                    break;
                case ApiResponseType.Created:
                    IsSuccess = true;
                    StatusCode = HttpStatusCode.Created;
                    break;
                case ApiResponseType.Error:
                    IsSuccess = false;
                    StatusCode = HttpStatusCode.BadRequest; 
                    break;
                case ApiResponseType.NotFound:
                    IsSuccess = false;
                    StatusCode = HttpStatusCode.NotFound; 
                    break;
                case ApiResponseType.NoContent:
                    IsSuccess = true;
                    StatusCode= HttpStatusCode.NoContent;
                    break;

            }
        }

        public ApiResponse(ApiResponseType type, string message) : this(type)
        {
            Message = message;
        }

    }

    public class ApiResponse<T> : ApiResultResponse, IApiResponse<T>
    {
        public ApiResponse(T result) : base(ApiResponseType.Success)
        {
            Result = result;
        }

        public T Result { get; set; }
    }

    public class ApiResultResponse : ApiResponse
    {
        public ApiResultResponse(object result) : base(ApiResponseType.Success)
        {
            Result = result;    
        }
        
        public object Result { get; set; }
    }
    



}

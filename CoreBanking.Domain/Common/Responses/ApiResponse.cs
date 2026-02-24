using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Domain.Common.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public string Code { get; set; } = default!;
        public T? Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Code = "00",
                Data = data
            };
        }
        public static ApiResponse<T> BadRequest(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Code = "400",
                Data = default
            };

        }
        
        public static ApiResponse<T> Unauthorized(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Code = "401",
                Message = message,
                Data = default
            };
        }

        public static ApiResponse<T> Duplicate(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Code = "409",
                Message = message,
                Data = default
            };

        }

        public static ApiResponse<T> NotFound(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Code = "404",
                Message = message,
                Data = default
            };
        }

        public static ApiResponse<T> InternalServerError(string message = "An unexpected error occured")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Code = "500",
                Message = message,
                Data = default
            };
        }
    }
}

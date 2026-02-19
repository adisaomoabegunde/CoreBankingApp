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

        public static ApiResponse<T> SuccessResponse<T>(T data, string message = "Successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Code = "00",
                Data = data
            };
        }
        public static ApiResponse<T> FailureResponse(string message, string code = "99")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Code = code,
                Data = default
            };

        }
    }
}

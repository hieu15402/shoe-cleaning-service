using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TP4SCS.Library.Models.Response.General
{
    public class ApiResponse<T>
    {
        public ApiResponse(string status, string message, T? data, int statusCode = 200, Pagination? pagination = null)
        {
            Status = status;
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Pagination = pagination;
        }

        public ApiResponse(string status, int statusCode, string message)
        {
            Status = status;
            StatusCode = statusCode;
            Message = message;
        }

        public string Status { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DefaultValue(200)]
        public int StatusCode { get; set; } = 200;

        public string Message { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Pagination? Pagination { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
    }
}

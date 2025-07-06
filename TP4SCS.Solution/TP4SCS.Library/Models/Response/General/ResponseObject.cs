using System.Text.Json.Serialization;
namespace TP4SCS.Library.Models.Response.General
{
    public class ResponseObject<T>
    {
        public string message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? data { get; set; }

        public ResponseObject(string message, T? data = default)
        {
            this.message = message;
            this.data = data;
        }
    }
}

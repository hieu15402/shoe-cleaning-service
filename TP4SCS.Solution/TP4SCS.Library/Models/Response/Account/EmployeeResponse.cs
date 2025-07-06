using System.Text.Json.Serialization;
using TP4SCS.Library.Models.Response.Branch;

namespace TP4SCS.Library.Models.Response.Account
{
    public class EmployeeResponse
    {

        public EmployeeBranchResponse? Branch { get; set; } = new EmployeeBranchResponse();

        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public DateOnly Dob { get; set; }

        public string? ImageUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CreatedByOwnerId { get; set; }

        public string Status { get; set; } = null!;
    }
}

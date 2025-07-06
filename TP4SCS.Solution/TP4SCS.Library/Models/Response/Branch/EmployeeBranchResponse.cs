namespace TP4SCS.Library.Models.Response.Branch
{
    public class EmployeeBranchResponse
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}

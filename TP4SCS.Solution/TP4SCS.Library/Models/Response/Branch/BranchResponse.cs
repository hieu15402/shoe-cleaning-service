namespace TP4SCS.Library.Models.Response.Branch
{
    public class BranchResponse
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string WardCode { get; set; } = string.Empty;

        public string Ward { get; set; } = string.Empty;

        public int DistrictId { get; set; }

        public string District { get; set; } = string.Empty;

        public int ProvinceId { get; set; }

        public string Province { get; set; } = string.Empty;

        public string? EmployeeIds { get; set; }

        public int PendingAmount { get; set; }

        public int ProcessingAmount { get; set; }

        public int FinishedAmount { get; set; }

        public int CanceledAmount { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}

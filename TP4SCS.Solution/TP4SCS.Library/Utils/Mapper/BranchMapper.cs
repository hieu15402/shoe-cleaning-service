using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Branch;
using TP4SCS.Library.Models.Response.Branch;

namespace TP4SCS.Library.Utils.Mapper
{
    public class BranchMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<BusinessBranch, BranchResponse>();

            config.NewConfig<CreateBranchRequest, BusinessBranch>()
                .Map(dest => dest.EmployeeIds, opt => "")
                .Map(dest => dest.PendingAmount, opt => 0)
                .Map(dest => dest.ProcessingAmount, opt => 0)
                .Map(dest => dest.FinishedAmount, opt => 0)
                .Map(dest => dest.CanceledAmount, opt => 0)
                .Map(dest => dest.Status, opt => "ACTIVE");

            config.NewConfig<BusinessBranch, UpdateBranchRequest>();

            config.NewConfig<BusinessBranch, UpdateBranchEmployeeRequest>();

            config.NewConfig<BusinessBranch, UpdateBranchStatisticRequest>();

            config.NewConfig<BusinessBranch, UpdateBranchStatusRequest>();
        }
    }
}

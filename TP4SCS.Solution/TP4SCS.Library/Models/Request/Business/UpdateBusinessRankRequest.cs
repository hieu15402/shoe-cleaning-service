using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Business
{
    public class UpdateBusinessRankRequest
    {
        [Required]
        public int Rank { get; set; }
    }
}

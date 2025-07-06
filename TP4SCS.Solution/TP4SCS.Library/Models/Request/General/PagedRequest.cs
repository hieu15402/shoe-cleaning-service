using System.ComponentModel;
using System.Runtime.Serialization;

namespace TP4SCS.Library.Models.Request.General
{
    public class PagedRequest
    {
        public string? Keyword { get; set; } = string.Empty;

        public string? Status { get; set; } = string.Empty;

        [DefaultValue(1)]
        public int PageIndex { get; set; }

        [DefaultValue(10)]
        public int PageSize { get; set; }

        public OrderByEnum OrderBy { get; set; } = OrderByEnum.IdAsc;
    }

    public enum OrderByEnum
    {
        [EnumMember(Value = "Id Ascending")]
        IdAsc,

        [EnumMember(Value = "Id Descending")]
        IdDesc
    }
    public enum OrderByEnumV2
    {
        [EnumMember(Value = "Create time Descending")]
        CreateDes,

        [EnumMember(Value = "Create time Ascending")]
        CreateAsc
    }

    public enum OrderedOrderByEnum
    {
        [EnumMember(Value = "Id Ascending")]
        IdAsc,

        [EnumMember(Value = "Id Descending")]
        IdDesc,

        [EnumMember(Value = "Create Date Ascending")]
        CreateDateAsc,

        [EnumMember(Value = "Create Date Descending")]
        CreateDateDes
    }
}

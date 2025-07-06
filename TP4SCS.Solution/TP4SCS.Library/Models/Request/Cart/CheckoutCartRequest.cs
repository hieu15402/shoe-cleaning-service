using System.ComponentModel;

namespace TP4SCS.Library.Models.Request.Cart
{
    public class CheckoutCartRequest
    {
        public CartCheckout Cart { get; set; } = null!;

        public int AccountId { get; set; }

        public int? AddressId { get; set; }

        [DefaultValue(false)]
        public bool IsAutoReject { get; set; }
    }
}

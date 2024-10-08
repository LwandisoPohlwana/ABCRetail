using System.ComponentModel.DataAnnotations;

namespace ABCRetail.ViewModel
{
    public class ShippingDetailViewModel
    {
        public string RowKey { get; set; }
        public string UserId { get; set; } // ID of the user associated with the shipping details

        [Required(ErrorMessage = "Address Line 1 is required.")]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State/Province is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip Code is required.")]
        [Display(Name = "Zip/Postal Code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }
    }

}

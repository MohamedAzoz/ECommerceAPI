namespace ECommerce.Core.DTOs.Address
{
    public class AddressDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

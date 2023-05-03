using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models.Mobile
{
    public class AddressMobile
    {
        public string? Id { get; set; }
        public string? FullAddress { get; set; }
        public string? Country { get; set; }
        public string? StateProvince { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? House { get; set; }
        public string? PostalCode { get; set; }
        public string? Lat { get; set; }
        public string? Lon { get; set; }
        public string? Apartments { get; set; } //Квартира/Офис
        public string? Floor { get; set; } //Этаж
        public string? Entrance { get; set; } //Подъезд
        public string? Description { get; set; }

        public AddressMobile(Address originalAddress)
        {
            this.Id = originalAddress.Id;
            this.FullAddress= originalAddress.FullAddress;
            this.Country = originalAddress.Country;
            this.StateProvince = originalAddress.StateProvince;
            this.City = originalAddress.City;
            this.Street = originalAddress.Street;
            this.House = originalAddress.House;
            this.PostalCode = originalAddress.PostalCode;
            this.Lat = originalAddress.Lat;
            this.Lon = originalAddress.Lon;
            this.Apartments = originalAddress.Apartments;
            this.Floor = originalAddress.Floor;
            this.Entrance = originalAddress.Entrance;
            this.Description = originalAddress.Description;
        }
    }

}

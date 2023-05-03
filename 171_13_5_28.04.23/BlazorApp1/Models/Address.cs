using GeoTimeZone;
using MongoDB.Bson.Serialization.Attributes;
using NodaTime;
using TimeZoneConverter;

namespace BlazorApp1.Models
{
    public class AddressType
    {
        public string IdCode { get; set; }
        public string AddressTypeName { get; set; }

        public AddressType(string IdCode, string name)
        {
            this.IdCode = IdCode;
            this.AddressTypeName = name;
        }
    }

    public class Address
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        public string? FullAddress { get; set; }
        public string? Country { get; set; }
        public string? StateProvince { get; set; }
        public string? City { get; set; }
        public string? Area { get; set; } //Район
        public string? Settlement { get; set; }
        public string? Street { get; set; }
        public string? House { get; set; }
        public string? PostalCode { get; set; }
        public string? Lat { get; set; }
        public string? Lon { get; set; }
        public string? Apartments { get; set; } //Квартира/Офис
        public string? Floor { get; set; } //Этаж
        public string? Entrance { get; set; } //Подъезд
        public string? Description { get; set; }

        public bool IsAutogen { get; set; } = true;

        public void SetDaDataValues(DaData.Models.Suggestions.Results.AddressResult address)
        {
            if(address == null)
            {
                return;
            }
            if(address.Data == null)
            {
                return;
            }

            this.FullAddress = address.UnrestrictedValue;
            this.Country = address.Data.Country;
            this.StateProvince = address.Data.Region;
            this.City = address.Data.City;
            this.Area = address.Data.Area;
            this.Settlement = address.Data.Settlement;
            this.Street = address.Data.Street;
            this.House = address.Data.House;
            this.PostalCode = address.Data.PostalCode;
            this.Lat = address.Data.GeoLat;
            this.Lon = address.Data.GeoLon;

        }

        public string GetAddress() 
        {
            try
            {
                if (IsAutogen)
                {
                    return this.FullAddress;
                }
                else
                {
                    string result = string.Empty;
                    if (!string.IsNullOrEmpty(this.PostalCode))
                        result = string.Concat(result, this.PostalCode);

                    if (!string.IsNullOrEmpty(this.StateProvince))
                        result = string.Concat(result, ", ", this.StateProvince);

                    if (!string.IsNullOrEmpty(this.City))
                        result = string.Concat(result, ", г ", this.City);

                    if (!string.IsNullOrEmpty(this.Area))
                        result = string.Concat(result, ", ", this.Area);

                    if (!string.IsNullOrEmpty(this.Settlement))
                        result = string.Concat(result, ", ", this.Settlement);

                    if (!string.IsNullOrEmpty(this.Street))
                        result = string.Concat(result, ", ул ", this.Street);

                    if (!string.IsNullOrEmpty(this.House))
                        result = string.Concat(result, ", д ", this.House);

                    return result;
                }
            }
            catch 
            {
                return string.Empty;
            }
        }

        //Удалить пробелы из Адреса
        public void Trim()
        {
            if (!string.IsNullOrEmpty(this.FullAddress))
                this.FullAddress = this.FullAddress.Trim();

            if (!string.IsNullOrEmpty(this.Country))
                this.Country = this.Country.Trim();

            if (!string.IsNullOrEmpty(this.StateProvince))
                this.StateProvince = this.StateProvince.Trim();

            if (!string.IsNullOrEmpty(this.City))
                this.City = this.City.Trim();

            if (!string.IsNullOrEmpty(this.Area))
                this.Area = this.Area.Trim();

            if (!string.IsNullOrEmpty(this.Settlement))
                this.Settlement = this.Settlement.Trim();

            if (!string.IsNullOrEmpty(this.Street))
                this.Street = this.Street.Trim();

            if (!string.IsNullOrEmpty(this.House))
                this.House = this.House.Trim();

            if (!string.IsNullOrEmpty(this.PostalCode))
                this.PostalCode = this.PostalCode.Trim();

            if (!string.IsNullOrEmpty(this.Lat))
                this.Lat = this.Lat.Trim();

            if (!string.IsNullOrEmpty(this.Lon))
                this.Lon = this.Lon.Trim();

            if (!string.IsNullOrEmpty(this.Apartments))
                this.Apartments = this.Apartments.Trim();

            if (!string.IsNullOrEmpty(this.Floor))
                this.Floor = this.Floor.Trim();

            if (!string.IsNullOrEmpty(this.Entrance))
                this.Entrance = this.Entrance.Trim();

            if (!string.IsNullOrEmpty(this.Description))
                this.Description = this.Description.Trim();
        }
    }
}

using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models
{
    public class DataList : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string Name { get; set; }
        public bool IsSingleSelection { get; set; } = true;
        public List<DataValue> Values { get; set; }

        [BsonIgnore]
        public DataValue SelectedSingleValue 
        {
            get 
            {
                if (SelectedValues!= null && SelectedValues.Count()>0 && SelectedValues.ToList()[0] != null) 
                {
                    return SelectedValues.ToList()[0];
                }
                else 
                {
                    return new();
                }
            }
            set 
            {
                if (value != null)
                {
                    var selectedValues = SelectedValues.ToList();
                    selectedValues.Clear();
                    selectedValues.Add(value);
                    SelectedValues = selectedValues;
                }
            }
        }
        public IEnumerable<DataValue> SelectedValues { get; set; } = new HashSet<DataValue>();        

        public override string ToString()
        {
            if (this.Name == null)
            {
                return string.Empty;
            }
            return Name.ToString();
        }
    }

    public class DataValue 
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]

        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        //длядропконтейнера

        public int Position { get; set; }

        public string Selector { get; set; } = "myselector";

        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            if (this.Value == null)
            {
                return string.Empty;
            }
            return Value.ToString();
        }
        public override bool Equals(object? obj)
        {
            var item = obj as DataValue;
            if (item == null)
            {
                return false;
            }

            return this.Value.Equals(item.Value);
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}

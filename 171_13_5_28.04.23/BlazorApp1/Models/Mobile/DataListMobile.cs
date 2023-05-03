using System.Net.WebSockets;

namespace BlazorApp1.Models.Mobile
{
    public class DataListMobile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSingle { get; set; }
        public List<DataListValueMobile> Items { get; set; } = new();

        public DataListMobile(){}

        public DataListMobile (DataList originalDataList)
        {
            
            this.Id = originalDataList.Id;
            this.Name = originalDataList.Name;
            this.IsSingle = originalDataList.IsSingleSelection;

            bool isChecked = false;
            var allValues = originalDataList.Values;
            var selectedValues = originalDataList.SelectedValues;
            foreach (var item in allValues)
            {
                var value = new DataListValueMobile();
                value.Id = item.Id;
                value.Value = item.Value;
                
                foreach (var selectedItem in selectedValues)
                {
                    if (item.Id == selectedItem.Id)
                    {
                        isChecked = true;
                        value.IsChecked = isChecked;
                    }
                }

                this.Items.Add(value);
            }

        }

        public DataList? ToOriginalList()
        {
            try
            {
                var valuesMobile = this.Items;

                var allValues = new List<DataValue>();
                var selectedValues = new List<DataValue>();
                foreach (var item in valuesMobile)
                {
                    var originalItem = new DataValue();
                    originalItem.Id = item.Id;
                    originalItem.Value = item.Value;

                    if (item.IsChecked == true)
                    {
                        selectedValues.Add(originalItem);
                    }
                    allValues.Add(originalItem);
                }

                var originalDataList = new DataList();
                originalDataList.Id = this.Id;
                originalDataList.Name = this.Name;
                originalDataList.Values = allValues;
                originalDataList.SelectedValues = selectedValues;
                originalDataList.IsSingleSelection = this.IsSingle;

                return originalDataList;
            }
            catch (Exception ex) 
            {
                Console.WriteLine("DataList.ToOriginalList_Exception: " + ex.Message);
                return null;
            }
        }
    }

    public class DataListValueMobile
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public bool IsChecked { get; set; }
        public DataListValueMobile() { }
    }
}

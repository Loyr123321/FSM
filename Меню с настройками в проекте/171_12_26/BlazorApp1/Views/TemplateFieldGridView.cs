using BlazorApp1.Models;
using BlazorApp1.Utils;

namespace BlazorApp1.Views
{
    public class TemplateFieldGridView
    {
        //!!new 31.03.23
        public int Position { get; set; }

        public string Id { get; set; }
        public string FieldName { get; set; }
        public string FieldValueAsString { get; private set; }
        //
        public bool IsNumeric { get; private set; } = false;
        public double FieldValueAsNumeric { get; private set; }

        public string FilterValue { get; set; } = string.Empty; //!!!!!!!!! 03.04.23



        public TemplateFieldGridView(TemplateField templateField)
        {
            //!!new 31.03.23
            this.Position = templateField.Position;

            this.Id = templateField.Id;
            this.FieldName = templateField.FieldName;

            switch (templateField.Type)
            {
                case (FieldType.FTText):
                case (FieldType.FTLink):
                    string stringValue = Convert.ToString(templateField._value);
                    if (!string.IsNullOrWhiteSpace(stringValue))
                    {
                        this.FieldValueAsString = stringValue;
                    }
                    break;

                case (FieldType.FTFile):
                case (FieldType.FTPhoto):
                    if (templateField._value != null)
                    {
                        List<TFile>? tfiles = templateField._value as List<TFile>;
                        if (tfiles != null && tfiles.Count > 0)
                        {
                            this.FieldValueAsString = string.Empty;
                            foreach (var tfile in tfiles)
                            {
                                FileView file = new FileView(templateField.Id, tfile);
                                this.FieldValueAsString += file.TFile.InitialName + ";";
                            }
                        }
                    }
                    else
                    {
                        this.FieldValueAsString = string.Empty;
                    }
                    break;

                case (FieldType.FTList):
                    if ((Models.DataList)templateField._value != null && ((Models.DataList)templateField._value).SelectedValues.Count() > 0)
                    {
                        this.FieldValueAsString = string.Empty;
                        foreach (var selectedValue in ((Models.DataList)templateField._value).SelectedValues)
                        {
                            this.FieldValueAsString += selectedValue.Value + ";";
                        }
                    }
                    break;

                case (FieldType.FTRuble):
                case (FieldType.FTDouble):
                    double doubleValue = Convert.ToDouble(templateField._value);
                    if (doubleValue != 0)
                    {
                        this.FieldValueAsString = doubleValue.ToString();
                    }
                    this.FieldValueAsNumeric = doubleValue;
                    this.IsNumeric = true;

                    break;

                case (FieldType.FTLong):
                    long longValue = Convert.ToInt64(templateField._value);
                    if (longValue != 0)
                    {
                        this.FieldValueAsString = longValue.ToString();
                    }
                    this.FieldValueAsNumeric = Convert.ToDouble(longValue);
                    this.IsNumeric = true;
                    break;

                case (FieldType.FTDate):
                    if (templateField._value != null)
                    {
                        DateTime date = (DateTime)templateField._value;
                        if (date != new DateTime() && date != null && date != new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        {
                            this.FieldValueAsString = date.ToShortDateString();
                        }
                    }
                    break;
                case (FieldType.FTDateTime):
                    if (templateField._value != null)
                    {
                        DateTime datetime = (DateTime)templateField._value;
                        if (datetime != new DateTime() && datetime != null && datetime != new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        {
                            this.FieldValueAsString = datetime.ToString();
                        }
                    }
                    break;

                case (FieldType.FTYesNo):
                    this.FieldValueAsString = Convert.ToBoolean(templateField._value).ToString();
                    break;
            }

        }

        public override string ToString()
        {
            return FieldName.ToString();
        }

        //Нужен для галочек
        public override bool Equals(object? obj)
        {
            var item = obj as TemplateFieldGridView;
            if (item == null)
            {
                return false;
            }

            return this.Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return FieldName.GetHashCode();
        }
    }

    
}

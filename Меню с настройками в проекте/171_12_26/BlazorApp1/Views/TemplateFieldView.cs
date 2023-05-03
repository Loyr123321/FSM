using BlazorApp1.Models;
using BlazorApp1.Utils;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.IO.Pipes;

namespace BlazorApp1.Views
{
    public class TemplateFieldView
    {
        public TemplateField TemplateField { get; set; }
        public string? AsString { get; set; }
        public long? AsLong { get; set; }
        public double? AsDouble { get; set; }
        public bool? AsBool { get; set; }
        public DateTime? AsDateTime { get; set; }
        public TimeSpan? AsTimeSpan { get; set; }
        public Models.DataList AsDataList { get; set; }
        public List<FileView>? AsFiles { get; set; }

        public int Position { get; set; }

        public void AddFiles(IBrowserFile[] entries)
        {
            foreach(var entry in entries)
            {
                AddFile(entry);
            }
        }
        public void AddFile(IBrowserFile entry)
        {
            string newFileName = string.Concat(Guid.NewGuid().ToString(), Path.GetExtension(entry.Name));
            TFile tfile = new TFile();
            tfile.InitialName = entry.Name;
            tfile.Name = newFileName;
            tfile.Size = entry.Size;

            FileView newFile = new FileView(this.TemplateField.Id, /*newFileName*/tfile, entry);
            this.AsFiles.Add(newFile);
        }
        public void DeleteFile(string fileName)
        {
            //!! 16,11,22 Проверить правильность //FileView file = this.AsFiles.First(x => x.TFile.InitialName == fileName);
            FileView file = this.AsFiles.First(x=>x.TFile.Name == fileName);
            this.AsFiles.Remove(file);

            var files = TemplateField._value as List<TFile>;
            if (files != null) 
            {
                files.Remove(file.TFile);

                if (this.AsFiles.Count == 0)
                {
                    this.AsFiles.Clear();
                    this.TemplateField._value = null;
                }
            }
        }
        
        public TemplateFieldView(TemplateField templateField)
        {
            this.TemplateField = templateField;
            this.Position = templateField.Position;

            switch (templateField.Type)
            {
                case (FieldType.FTText):
                case (FieldType.FTLink):
                    string stringValue = Convert.ToString(templateField._value);
                    if(!string.IsNullOrWhiteSpace(stringValue))
                    {
                        this.AsString = stringValue;
                    }
                    break;

                case (FieldType.FTFile):
                case (FieldType.FTPhoto):
                    if (templateField._value != null)
                    {
                        List<TFile>? tfiles = templateField._value as List<TFile>;
                        if (tfiles != null && tfiles.Count > 0)
                        {
                            this.AsFiles = new();
                            foreach (var tfile in tfiles)
                            {
                                FileView file = new FileView(templateField.Id, tfile);
                                this.AsFiles.Add(file);
                            }
                        }
                    }
                    else
                    {
                        this.AsFiles = new();
                    }
                    
                    break;
               
                case (FieldType.FTList):
                    this.AsDataList = (Models.DataList)templateField._value;
                    break;

                case (FieldType.FTRuble):
                case (FieldType.FTDouble):
                    double doubleValue = Convert.ToDouble(templateField._value);
                    if (doubleValue != 0)
                    {
                        this.AsDouble = doubleValue;
                    }
                    break;

                case (FieldType.FTLong):
                    long longValue = Convert.ToInt64(templateField._value);
                    if (longValue != 0)
                    {
                        this.AsLong = longValue;
                    }
                    break;

                case (FieldType.FTDate):
                    if(templateField._value != null)
                    {
                        
                        DateTime date = (DateTime)templateField._value;
                        //date = date.ToLocalTime();
                        if (date != new DateTime() && date != null && date != new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        {
                            this.AsDateTime = date;
                        }
                    }
                    break;
                case (FieldType.FTDateTime):
                    if (templateField._value != null)
                    {
                        DateTime datetime = (DateTime)templateField._value;
                        //datetime = datetime.ToLocalTime();
                        if (datetime != new DateTime() && datetime != null && datetime != new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        {
                            this.AsDateTime = new DateTime(datetime.Year, datetime.Month, datetime.Day);
                            this.AsTimeSpan = new TimeSpan(datetime.Hour, datetime.Minute, 0);
                        }
                    }
                    break;
                
                case (FieldType.FTYesNo):
                    this.AsBool = Convert.ToBoolean(templateField._value); 
                    break;
            }
        }

        public TemplateField GetTemplateField()
        {
            TemplateField templateField = this.TemplateField;

            switch (templateField.Type)
            {
                case (FieldType.FTText):
                case (FieldType.FTLink):
                        templateField._value = Convert.ToString(this.AsString);
                    break;
                case (FieldType.FTFile):
                case (FieldType.FTPhoto):

                    List<FileView> files = this.AsFiles;
                    List<TFile> tfiles = new List<TFile>();
                    if (files != null && files.Count > 0)
                    {
                        foreach (FileView file in files)
                        {
                            tfiles.Add(file.TFile);
                        }
                        templateField._value = tfiles;
                    }
                    else
                    {
                        templateField._value = null;
                    }
                    
                    break;

                case (FieldType.FTList):
                    templateField._value = this.AsDataList;
                    break;

                case (FieldType.FTRuble):
                case (FieldType.FTDouble):
                        templateField._value = Convert.ToDouble(this.AsDouble);
                    break;

                case (FieldType.FTLong):
                        templateField._value = Convert.ToInt64(this.AsLong);
                    break;

                case (FieldType.FTDate):
                    if(this.AsDateTime != null)
                    {
                        templateField._value = (DateTime)this.AsDateTime;
                    }
                    else 
                    {
                        templateField._value = null;
                    }
                    break;
                case (FieldType.FTDateTime):
                    var time = this.AsTimeSpan;
                    var date = this.AsDateTime;
                    if (time != null && date != null)
                    {
                        var dt = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, time.Value.Hours, time.Value.Minutes, 0);
                        templateField._value = dt;
                    }
                    else
                    {
                        templateField._value = null;
                    }
                    break;
                case (FieldType.FTYesNo):
                    templateField._value = Convert.ToBoolean(this.AsBool);
                    break;
            }
            templateField.Position = this.Position;

            return templateField;
        }
    }
}

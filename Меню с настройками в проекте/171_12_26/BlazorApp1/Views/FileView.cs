using BlazorApp1.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.CodeAnalysis;

namespace BlazorApp1.Views
{
    public class FileView
    {
        public string OwnerId { get; set; } //ID поля
        public TFile TFile { get; set; }
        public IBrowserFile browserFile { get; set; }

        public FileView(string fieldId, TFile tfile, IBrowserFile browserFile)
        {
            this.OwnerId = fieldId;
            this.TFile = tfile;//this.fileName = fileName;
            this.browserFile = browserFile;
        }
        public FileView(string fieldId, TFile tfile)
        {
            this.OwnerId = fieldId;
            this.TFile = tfile;//this.fileName = fileName;
        }

        

    }
}

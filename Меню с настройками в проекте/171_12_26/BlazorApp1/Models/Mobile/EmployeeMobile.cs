using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models.Mobile
{
    public class EmployeeMobile
    {
        public string? Id { get; set; } = null;
        public string? FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;
        public string? MiddleName { get; set; } = null;
        public string? Phone { get; set; } = string.Empty;
        public string? Mail { get; set; } = null;
        public List<string>? Skills { get; set; } = null;
        public List<RegionMobile>? Regions { get; set; }
        public TFile? Photo { get; set; } = null;

        public bool? CurrentStatus { get; set; } = null; //текущ статус из планировщика на текущий момент. чтобы пон раб или нет

        public double Rating { get; set; }

        public EmployeeMobile(Employee originalEmployee) 
        {
            this.Id = originalEmployee.Id;
            this.FirstName = originalEmployee.FirstName;
            this.LastName = originalEmployee.LastName;
            this.MiddleName = originalEmployee.MiddleName;
            this.Phone = originalEmployee.Phone;
            this.Mail = originalEmployee.Mail;

            //Навыки
            if(originalEmployee.Skills != null && originalEmployee.Skills.Count > 0) 
            {
                List<string> skills = new();
                foreach (var skill in originalEmployee.Skills.OrderBy(x=>x.SkillName).ToList())
                {
                    skills.Add(skill.SkillName);
                }
                this.Skills = skills;
            }
            
            this.Photo = originalEmployee.Photo;

            //Регионы
            if (originalEmployee.Regions != null && originalEmployee.Regions.Count > 0)
            {
                var regions = new List<RegionMobile>();

                var regionCount = new HashSet<string>();
                foreach (var regionValue in originalEmployee.Regions)
                {
                    regionCount.Add(regionValue.RegionName);
                }

                foreach (var regionName in regionCount)
                {
                    var newRegion = new RegionMobile();
                    newRegion.RegionName = regionName;
                    newRegion.SelectedValues = new();

                    foreach (var regionValue in originalEmployee.Regions.FindAll(x => x.RegionName == regionName))
                    {
                        newRegion.SelectedValues.Add(regionValue.Value);
                    }
                    regions.Add(newRegion);
                }
                this.Regions = regions;
            }
            else { this.Regions = null; }
            //

            //Рейтинг
            this.Rating = originalEmployee.GetRating();
        }

    }
}

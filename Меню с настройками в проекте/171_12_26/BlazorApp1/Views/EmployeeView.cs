using BlazorApp1.Models;
using BlazorApp1.Services;

namespace BlazorApp1.Views
{
    public class EmployeeView
    {
        public Employee Employee { get; set; }
        public List<Skill> Skills { get; set; }

        public EmployeeView(Employee employee)
        {
            this.Employee= employee;

            //SkillService service= new SkillService();
            //service.GetSkill();
        }
    }
}

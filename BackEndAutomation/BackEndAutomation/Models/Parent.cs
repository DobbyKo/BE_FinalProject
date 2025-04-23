using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndAutomation.Models
{
    public class Parent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Student> Children { get; set; } = new List<Student>();

        public Dictionary<string, int> ViewChildGrades(int childId)
        {
            var child = Children.FirstOrDefault(c => c.Id == childId);
            return child != null ? child.Grades : new Dictionary<string, int>();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndAutomation.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Class> Classes { get; set; } = new List<Class>();

        public Class CreateClass(string className, List<Subject> subjects)
        {
            if (subjects.Count <= 3)
            {
                var newClass = new Class { Name = className, Subjects = subjects };
                Classes.Add(newClass);
                return newClass;
            }
            throw new Exception("A class can only have up to 3 subjects.");
        }

    }
}

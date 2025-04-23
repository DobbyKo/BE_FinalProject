using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndAutomation.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();

        public bool AddStudent(Student student)
        {
            if (Students.Count >= 20)
            {
                throw new InvalidOperationException("Cannot add more than 20 students to a class.");
            }

            student.Subjects.AddRange(Subjects);
            Students.Add(student);
            return true;

        }

    }
}

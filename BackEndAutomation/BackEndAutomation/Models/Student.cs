using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BackEndAutomation.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public Dictionary<string, int> Grades { get; set; } = new Dictionary<string, int>();

        public void AddGrade(string subjectName, int grade)
        {
            if (Subjects.Any(s => s.Name == subjectName))
            {
                Grades[subjectName] = grade;
            }
            else
            {
                throw new Exception("Subject not found for student.");
            }
        }

    }
}

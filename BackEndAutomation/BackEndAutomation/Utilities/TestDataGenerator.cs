using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndAutomation.Utilities
{
    public class TestDataGenerator
    {
        public static string GenerateRandomName() => $"TestName{DateTime.Now.Ticks}";

        public static string GenerateRandomClassName()
        {
            string baseClassName = "Class";
            string uniqueSuffix = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"{baseClassName}_{uniqueSuffix}";
        }
    }
}

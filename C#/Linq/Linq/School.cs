using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq
{
    class School
    {

        //public List<University> Universities { get; set; } = new List<University>();
        public class University 
        {
            public int Id {get; set; }
            public string Name {get; set; }

            public void Print()
            {
                Console.WriteLine("Universtiy{0}with id {1]", Name, Id);
            }
        }

        public class Student
        {
            public int Id{ get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }
            public int Age { get; set; }


            // foreign Key
            public int UniversityId { get; set; }

            public void Print()
            {
                Console.WriteLine("Student {0} with Id {1}, Gender {2} " +
                    "and Age {3} from University with the Id {4}", Name, Id, Gender, Age, UniversityId);
            }

            
        }
        

        }


    
}

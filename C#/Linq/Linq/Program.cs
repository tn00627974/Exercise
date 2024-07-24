using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Linq.School;



namespace Linq
{

    internal class Program
    {
        static void Main(string[] args)
        {
            // 1~10
            //int[] numbers = new int [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            //OddNumbers(numbers);
            UniversityManager um = new UniversityManager();

            //um.MaleStudents();
            //um.FemaleStudents();
            //um.SortStudentsByAge();
            um.StudentAndUniversityNameCollection();
            Console.ReadKey();


        }
    class UniversityManager 
    {
            public List<University> universities = new List<University>();
            public List<Student> students = new List<Student>();


            public UniversityManager()
            {
                universities.Add(new University { Id = 1, Name = "Yale" });
                universities.Add(new University { Id = 2, Name = "Beijing Tech" });


                students.Add(new Student { Id = 1, Name = "Carla", Gender = "female", Age = 17, UniversityId = 1 });
                students.Add(new Student { Id = 2, Name = "Toni", Gender = "male", Age = 21, UniversityId = 1 });
                students.Add(new Student { Id = 3, Name = "Frank", Gender = "male", Age = 22, UniversityId = 2 });
                students.Add(new Student { Id = 4, Name = "Leyla", Gender = "female", Age = 19, UniversityId = 2 });
                students.Add(new Student { Id = 5, Name = "James", Gender = "trans-gender", Age = 25, UniversityId = 2 });
                students.Add(new Student { Id = 6, Name = "Linda", Gender = "female", Age = 22, UniversityId = 2 });

            }

            public void MaleStudents() // 學校男生
            {

                IEnumerable<Student> maleStudents = from student in students where student.Gender == "male" select student;
                Console.WriteLine("male - Students");

                foreach (Student student in maleStudents)
                {
                    student.Print();
                }
            }

            public void FemaleStudents() // 學校女生
            {
                IEnumerable<Student> femaleStudents = from student in students where student.Gender == "female" select student;
                Console.WriteLine("Female - Students: ");

                foreach (Student student in femaleStudents)
                {
                    student.Print();
                }
            }

            public void SortStudentsByAge()
            {
                IEnumerable<Student> sortstudents = from student in students orderby student.Age select student ;
                Console.WriteLine("Sort Students by Age: ");

                foreach (Student student in sortstudents)
                {
                    student.Print() ;
                }

            }

            public void AllStudentsFromBeijingTech()
            {
                IEnumerable<Student> bjtStudents = from student in students
                                                   join university in universities on student.UniversityId equals university.Id
                                                   where university.Name == "Beijing Tech" 
                                                   select student ;
                foreach (Student student in bjtStudents)
                {
                    student.Print();
                }

            }


            public void AllStudentsFromThatUni(int Id)
            {
                IEnumerable<Student> myStudents = from student in students
                                                  join university in universities on student.UniversityId equals university.Id
                                                  where university.Id == Id
                                                  select student;

                Console.WriteLine("Students from that uni {0}", Id);
                foreach (Student student in myStudents)
                {
                    student.Print();
                }
            }

            public void StudentAndUniversityNameCollection()
            {
                var newCollection = from student in students
                                    join university in universities on student.UniversityId equals university.Id
                                    orderby student.Name descending
                                    select new { StudentName = student.Name, UniversityName = university.Name } ;

                Console.WriteLine("New Collection : ");

                foreach (var col in newCollection)
                {
                    Console.WriteLine("Student Name: {0}, University Name: {1}", col.StudentName, col.UniversityName);
                }
            }


            
            






        }
        //static void OddNumbers(int[] numbers)
        //{
        //    Console.WriteLine("Odd Numbers:");

        //    //IEnumerable<int> oddNumbers = numbers.Where(n => n % 2!= 0);

        //    IEnumerable<int> oddNumbers = from number in numbers.Where(n => n % 2!= 0) select number ;


        //    //Console.WriteLine(string.Join(", ", number));
        //    foreach (int num in oddNumbers)
        //    {
        //        Console.WriteLine(num); // 1, 3, 5, 7, 9
        //    }
        //}


    }
}

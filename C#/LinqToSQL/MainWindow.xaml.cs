using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace LinqToSQL
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        LinqToSqlDataClassesDataContext dataContext; // 資料庫連線物件Linq 
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            dataContext = new LinqToSqlDataClassesDataContext(connectionString);
            Console.WriteLine($"{dataContext}資料庫連線成功");
            //InsertUniversities();
            //InsertStudent();
            //InsertLectures();
            //InsertStudentLectureAssociations();
            //GetUniversityOfToni();
            //GetLecturesFromToni();
            //GetAllStudentsFromYale();
            //GetAllUniversitiesWithTransgenders();
            //UpdateToni();
            //DeleteJame();
            SelectAll();

            // 取得資料庫連線
            //string connectionString = ConfigurationManager.ConnectionStrings["Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=G:\\我的雲端硬碟\\Exercise\\C#\\LinqToSQL\\LinqToSQL\\LinqData.mdf;Integrated Security=True"].ConnectionString;



        }

        public void InsertUniversities()
        {
            // 清除原有資料
            //dataContext.ExecuteCommand("select * from University");
            //dataContext.ExecuteCommand("delete from University");

            //University yale = new University();
            //yale.Name = "Yale";
            //dataContext.Universities.InsertOnSubmit(yale);

            //University beijingTech = new University();
            //beijingTech.Name = "Beijing Tech";
            //dataContext.Universities.InsertOnSubmit(beijingTech);

            // 新增資料List
            List<University> Universities = new List<University>
            {
                new University { Name = "Yale" },
                new University { Name = "Beijing Tech" },
            };

            // 新增資料
            foreach (var university in Universities)
            {
                // 新增資料到資料庫
                dataContext.University.InsertOnSubmit(university);
            }

            foreach (var university in dataContext.University)
            {
                // 新增資料到資料庫
                Console.WriteLine($"學生: {university.Id}, 性別: {university.Name}");
            }


            // 送出資料
            dataContext.SubmitChanges();
            // 在DataGrid 顯示資料
            MainDataGrid.ItemsSource = dataContext.University;
        }

        public void InsertStudent()
        {
            // 新增學校
            University yale = dataContext.University.First(u => u.Name == "Yale"); // 取得Yale學校
            University beijingTech = dataContext.University.First(u => u.Name == "Beijing Tech"); // 取得Beijing Tech學校

            List<Student> students = new List<Student>();
            //{
            students.Add(new Student { Name = "Carla", Gender = "female", UniversityId = yale.Id });
            students.Add(new Student { Name = "Toni", Gender = "male", University = yale });
            students.Add(new Student { Name = "Leyle", Gender = "female", University = beijingTech });
            students.Add(new Student { Name = "Jame", Gender = "trans-gender", University = beijingTech });
            //};



            foreach (var student in students)
            {
                // 新增資料到資料庫
                dataContext.Student.InsertOnSubmit(student);
            }

            foreach (var student in dataContext.Student)
            {
                Console.WriteLine($"學生: {student.Name}, 性別: {student.Gender}, 大學ID: {student.UniversityId}");
            }

            dataContext.SubmitChanges(); // 送出資料

            MainDataGrid.ItemsSource = dataContext.Student;  // 在DataGrid 顯示資料
        }

        public void InsertLectures()
        {
            dataContext.Lecture.InsertOnSubmit(new Lecture { Name = "Math" });
            dataContext.Lecture.InsertOnSubmit(new Lecture { Name = "History" });

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Lecture;
        }

        public void InsertStudentLectureAssociations()
        {
            Student Carla = dataContext.Student.First(st => st.Name.Equals("Carla")); // First 找出第一個符合條件 Equals 避免大小寫不同
            Student Toni = dataContext.Student.First(st => st.Name.Equals("Toni"));
            Student Leyle = dataContext.Student.First(st => st.Name.Equals("Leyle"));
            Student Jame = dataContext.Student.First(st => st.Name.Equals("Jame"));

            Lecture Math = dataContext.Lecture.First(lc => lc.Name.Equals("Math"));
            Lecture History = dataContext.Lecture.First(lc => lc.Name.Equals("History"));

            dataContext.StudentLecture.InsertOnSubmit(new StudentLecture { Student = Carla, Lecture = Math });
            dataContext.StudentLecture.InsertOnSubmit(new StudentLecture { Student = Toni, Lecture = Math });

            StudentLecture slToni = new StudentLecture();
            slToni.StudentId = Toni.Id;
            slToni.LectureId = History.Id;
            dataContext.StudentLecture.InsertOnSubmit(slToni);

            dataContext.StudentLecture.InsertOnSubmit(new StudentLecture { Student = Leyle, Lecture = History });

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.StudentLecture;
        }

        public void GetUniversityOfToni()
        {
            Student Toni = dataContext.Student.First(st => st.Name.Equals("Toni"));

            University TonisUniversity = Toni.University;

            List<University> universities = new List<University>();
            universities.Add(TonisUniversity);

            MainDataGrid.ItemsSource = universities;
        }

        public void GetLecturesFromToni()
        {
            Student Toni = dataContext.Student.First(st => st.Name.Equals("Toni"));

            //var tonisLectures = Toni.StudentLecture.Select(sl => sl.Lecture);
            var tonisLectures = from sl in Toni.StudentLecture select sl.Lecture;

            MainDataGrid.ItemsSource = tonisLectures;
        }

        public void GetAllStudentsFromYale()
        {
            var studentsFromYale = from student in dataContext.Student
                                   where student.University.Name == "Yale"
                                   select student;

            MainDataGrid.ItemsSource = studentsFromYale;
        }
        public void GetAllUniversitiesWithTransgenders()
        {
            var transgernderUniversities = from student in dataContext.Student
                                           join University in dataContext.University
                                           on student.UniversityId equals University.Id
                                           where student.Gender == "trans-gender"
                                           select new
                                           {
                                               StudentName = student.Name,  // 學生姓名
                                               StudentGender = student.Gender,  // 學生性別
                                               UniversityName = University.Name  // 大學名稱
                                           };
            MainDataGrid.ItemsSource = transgernderUniversities;
        }
        public void GetAllLecturesFromBeijingTech()
        {
            var lecturesFromBeijingTech = from sl in dataContext.StudentLecture
                                          join student in dataContext.Student on sl.StudentId equals student.Id
                                          where student.University.Name == "Beijing Tech"
                                          select sl.Lecture;

            MainDataGrid.ItemsSource = lecturesFromBeijingTech;
        }

        public void UpdateToni()
        {
            Student Toni = dataContext.Student.FirstOrDefault(st => st.Name == "Toni");

            Toni.Name = "Antonio"; // 更新Toni的姓名為Antonio

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Student;
        }
        public void DeleteJame()
        {
            Student Jame = dataContext.Student.FirstOrDefault(st => st.Name == "Jame");
            dataContext.Student.DeleteOnSubmit(Jame);
            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Student;
        }
        public void SelectAll()
        {
            MainDataGrid.ItemsSource = dataContext.Student.Select(u => u);
            MainDataGrid1.ItemsSource = dataContext.University.Select(u => u);
            MainDataGrid2.ItemsSource = dataContext.Lecture.Select(u => u);
            MainDataGrid3.ItemsSource = dataContext.StudentLecture.Select(u => u);

        }

    }
}




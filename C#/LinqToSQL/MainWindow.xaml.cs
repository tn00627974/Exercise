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
            dataContext = new LinqToSqlDataClassesDataContext(); // 資料庫連線物件Linq 


            InitializeComponent();
            //InsertUniversities();
            InsertStudent();
            //InsertLectures();
            //InsertStudentLectureAssociations();

            // 取得資料庫連線
            //string connectionString = ConfigurationManager.ConnectionStrings["Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=G:\\我的雲端硬碟\\Exercise\\C#\\LinqToSQL\\LinqToSQL\\LinqData.mdf;Integrated Security=True"].ConnectionString;
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            dataContext = new LinqToSqlDataClassesDataContext(connectionString);
            Console.WriteLine("資料庫連線成功");

            
        }

        public void InsertUniversities()
        {
            // 清除原有資料
            //dataContext.ExecuteCommand("select * from University");
            dataContext.ExecuteCommand("delete from University");

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
           
                students.Add(new Student { Name = "Carla", Gender = "female", UniversityId = yale.Id });
                students.Add(new Student { Name = "Toni", Gender = "male", University = yale });
                students.Add(new Student { Name = "Leyle", Gender = "female", University = beijingTech });
                students.Add(new Student { Name = "Jame", Gender = "trans-gender", University = beijingTech });
            
            foreach (var student in students)
            {
                // 新增資料到資料庫
                dataContext.Student.InsertOnSubmit(student);
            }
            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Student; 
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
            Student Carla = dataContext.Student.First(st => st.Name.Equals("Carla"));
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



    }
}




using System.Text.RegularExpressions;

namespace Todo.Parameter
{
    public class TodoSelectParameter
    {
        public string? name { get; set; }
        public bool? enable { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? minOrder { get; set; }
        public int? maxOrder { get; set; }
        private string _order { get; set;}

        public string? order
        {
            get { return _order; }
            set
            {
                Regex regex = new Regex(@"\d+-\d+");  // 例 : 2-3
                if (regex.IsMatch(value))
                {
                    minOrder = int.Parse(value.Split('-')[0]);
                    maxOrder = int.Parse(value.Split('-')[1]);
                }
                _order = value;

            }
        }
    }
}

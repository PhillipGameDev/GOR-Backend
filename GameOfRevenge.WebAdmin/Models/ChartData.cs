namespace GameOfRevenge.WebAdmin.Models
{
    public class ChartData
    {
        public string NewUsers;
        public string Recurring;
        public int WithinOneMonth;
        public int WithinThreeMonths;
        public int WithinSixMonths;

        public int TotalNewUsers
        {
            get
            {
                var total = 0;
                var strs = NewUsers.Split(',');
                foreach (var str in strs)
                {
                    if (int.TryParse(str, out int val)) total += val;
                }
                return total;
            }
        }

        public ChartData()
        {
        }
    }
}

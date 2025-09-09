namespace BudgetManager.Data.Models.Dto.Date
{
	public class MonthDatesDto
	{
		public int MonthStartDay { get; set; }
		public int MonthEndDay { get; set; }
		public DateTime CurrentMonthStartDate { get; set; }
		public DateTime CurrentMonthEndDate { get; set; }

		public DateTime MonthStartDate { get; set; }
		public DateTime MonthEndDate { get; set; }
	}
}

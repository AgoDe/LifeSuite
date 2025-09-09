using BudgetManager.Data.Models.Dto.Date;

namespace BudgetManager.Data.Services
{
    public class DateService
    {
        public DateService()
        {
            CurrentMonthStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, MonthStartDay);

            if (DateTime.Today.Day < MonthStartDay)
            {
                CurrentMonthStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month - 1, MonthStartDay);
            }

            CurrentMonthEndDate = CurrentMonthStartDate.AddMonths(1).AddDays(-1);
            MonthEndDay = CurrentMonthEndDate.Day;
        }

        public int MonthStartDay => 6;
        public int MonthEndDay { get; set; }
        public DateTime CurrentMonthStartDate { get; set; }
        public DateTime CurrentMonthEndDate { get; set; }

        public DateTime GetMonthStartDate(int year, int month)
        {
            var date = new DateTime(year, month, MonthStartDay);

            return date;
        }

        public DateTime GetMonthEndDate(int year, int month)
        {
            var StartDate = GetMonthStartDate(year, month);

            return StartDate.AddMonths(1).AddDays(-1);
        }

        public MonthDatesDto GetMonthDates(int year, int month)
        {
            MonthDatesDto dto = new MonthDatesDto();
            dto.MonthStartDate = GetMonthStartDate(year, month);
            dto.MonthEndDate = GetMonthEndDate(year, month);
            dto.MonthStartDay = MonthStartDay;
            dto.MonthEndDay = MonthEndDay;
            dto.CurrentMonthStartDate = CurrentMonthStartDate;
            dto.CurrentMonthEndDate = CurrentMonthEndDate;

            return dto;
        }

        public CurrentMonthDatesDto GetCurrentMonthDates()
        {
            var dto = new CurrentMonthDatesDto();
            dto.MonthStartDay = MonthStartDay;
            dto.MonthEndDay = MonthEndDay;
            dto.CurrentMonthStartDate = CurrentMonthStartDate;
            dto.CurrentMonthEndDate = CurrentMonthEndDate;
            return dto;
        }

        public IEnumerable<DateTime> GetMonthsBetweenTwoDates(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddMonths(1))
            {
                yield return date;
            }
        }

        public int GetMonthsNumberBetweenTwoDates(DateTime startDate, DateTime endDate)
        {
            var result = 0;
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddMonths(1))
            {
                result++;
            }

            return result;
        }







    }
}

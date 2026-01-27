namespace SecureJournal.Data.Services
{
    // Simple “selected date” state like reference CalendarFilterService 
    public class CalendarStateService
    {
        public event Action? SelectedDateChanged;

        public DateTime SelectedDate { get; private set; } = DateTime.Now.Date;

        public void SetSelectedDate(DateTime date)
        {
            SelectedDate = date.Date;
            SelectedDateChanged?.Invoke();
        }
    }
}

namespace HospitalWeb.ViewModels.Doctors
{
    public class ScheduleViewModel
    {
        public string DoctorId { get; set; }

        public DateTime Date { get; set; }

        public IEnumerable<SlotViewModel> Slots { get; set; }
    }
}

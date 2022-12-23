namespace HospitalWeb.Mvc.ViewModels.Doctors
{
    public class SlotViewModel
    {
        public DateTime Time { get; set; }

        public bool IsFree { get; set; }

        public override string ToString()
        {
            return Time.ToString("HH:mm");
        }
    }
}

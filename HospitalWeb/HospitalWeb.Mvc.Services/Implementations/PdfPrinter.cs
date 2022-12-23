using HospitalWeb.Domain.Entities;
using HospitalWeb.Mvc.Services.Interfaces;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace HospitalWeb.Mvc.Services.Implementations
{
    public class PdfPrinter : IPdfPrinter
    {
        public void PrintAppointment(Appointment appointment, string filePath)
        {
            if (appointment == null)
            {
                throw new Exception("Cannot print an empty appointment");
            }

            var fileStream = new FileStream(filePath, FileMode.Create);
            var writer = new PdfWriter(fileStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var header = new Paragraph("HospitalWeb")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(30)
                .SetMarginTop(25)
                .SetMarginBottom(50);
            document.Add(header);

            var table = new Table(2)
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetMarginBottom(30);
            var id = new Cell()
                .SetTextAlignment(TextAlignment.LEFT)
                .SetBackgroundColor(new DeviceRgb(126, 151, 173))
                .Add(new Paragraph($"APPOINTMENT: {appointment.AppointmentId}"));
            var date = new Cell()
                .SetBackgroundColor(new DeviceRgb(126, 151, 173))
                .SetTextAlignment(TextAlignment.RIGHT)
                .Add(new Paragraph($"DATE: {appointment.AppointmentDate.ToString("MM/dd/yyyy HH:mm")}"));
            var doctorHeader = new Cell()
                .SetBold()
                .SetBorder(Border.NO_BORDER)
                .Add(new Paragraph("Doctor"));
            var patientHeader = new Cell()
                .SetBold()
                .SetBorder(Border.NO_BORDER)
                .Add(new Paragraph("Patient"));
            var doctorName = new Cell()
                .SetBorder(Border.NO_BORDER)
                .Add(new Paragraph($"Full name: {appointment.Doctor.ToString()}"));
            var patientName = new Cell()
                .SetBorder(Border.NO_BORDER)
                .Add(new Paragraph($"Full name: {appointment.Patient.ToString()}"));
            var specialty = new Cell()
                .SetBorder(Border.NO_BORDER)
                .Add(new Paragraph($"Specialty: {appointment.Doctor.Specialty.SpecialtyName}"));
            var birthDate = new Cell()
                .SetBorder(Border.NO_BORDER)
                .Add(new Paragraph($"Date of birth: {appointment.Patient.BirthDate.ToString("MM/dd/yyyy")}"));

            table.AddCell(id);
            table.AddCell(date);
            table.AddCell(doctorHeader);
            table.AddCell(patientHeader);
            table.AddCell(doctorName);
            table.AddCell(patientName);
            table.AddCell(specialty);
            table.AddCell(birthDate);
            document.Add(table);

            var details = new Paragraph("DETAILS")
                .SetFontSize(20)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER);
            var diagnosisLabel = new Paragraph("Diagnosis:")
                .SetBold();
            var diagnosis = new Paragraph(appointment.Diagnosis.DiagnosisName);
            var prescriptionLabel = new Paragraph("Prescription:")
                .SetBold();
            var prescription = new Paragraph(appointment.Prescription);

            document.Add(details);
            document.Add(diagnosisLabel);
            document.Add(diagnosis);
            document.Add(prescriptionLabel);
            document.Add(prescription);

            document.Close();
        }
    }
}

namespace ClinicManagementAPI.DTOs
{
    public class PatientListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string EstateNumber { get; set; }
    }
}

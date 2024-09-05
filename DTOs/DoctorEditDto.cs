namespace ClinicManagementAPI.DTOs
{
    public class DoctorEditDto
    {
        public string FullName { get; set; }
        public int CabinetId { get; set; }
        public int SpecializationId { get; set; }
        public int? EstateId { get; set; }
    }
}

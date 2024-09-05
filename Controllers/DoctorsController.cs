using AutoMapper;
using ClinicManagementAPI.Data;
using ClinicManagementAPI.DTOs;
using ClinicManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly ClinicContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(ClinicContext context, IMapper mapper, ILogger<DoctorsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorListDto>>> GetDoctors([FromQuery] string sortBy = "FullName", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Doctors.AsQueryable();

                query = sortBy switch
                {
                    "FullName" => query.OrderBy(d => d.FullName),
                    "SpecializationName" => query.OrderBy(d => d.Specialization.Name),
                    _ => query.OrderBy(d => d.FullName),
                };

                query = query
                    .Include(d => d.Cabinet)
                    .Include(d => d.Specialization)
                    .Include(d => d.Estate)
                    .AsNoTracking();

                var doctors = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                var doctorDtos = _mapper.Map<IEnumerable<DoctorListDto>>(doctors);

                return Ok(doctorDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting doctors.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorEditDto>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            var doctorDto = new DoctorEditDto
            {
                FullName = doctor.FullName,
                CabinetId = doctor.CabinetId,
                SpecializationId = doctor.SpecializationId,
                EstateId = doctor.EstateId
            };

            return Ok(doctorDto);
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(DoctorEditDto doctorDto)
        {
            try
            {
                if (!_context.Cabinets.Any(c => c.Id == doctorDto.CabinetId) ||
                    !_context.Specializations.Any(s => s.Id == doctorDto.SpecializationId) ||
                    (doctorDto.EstateId.HasValue && !_context.Uchastki.Any(u => u.Id == doctorDto.EstateId)))
                {
                    return BadRequest("One or more provided IDs are invalid.");
                }

                var doctor = _mapper.Map<Doctor>(doctorDto);

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving the doctor.");
                return StatusCode(500, "An error occurred while saving the data. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, DoctorEditDto doctorDto)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    return NotFound();
                }

                _mapper.Map(doctorDto, doctor);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the doctor.");
                return StatusCode(500, "An error occurred while updating the data. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    return NotFound();
                }

                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the doctor.");
                return StatusCode(500, "An error occurred while deleting the data. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }
    }
}

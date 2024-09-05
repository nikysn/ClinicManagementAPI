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
    public class PatientsController : ControllerBase
    {
        private readonly ClinicContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(ClinicContext context, IMapper mapper, ILogger<PatientsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientListDto>>> GetPatients([FromQuery] string sortBy = "LastName", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Patients
                    .Include(p => p.Estate)
                    .AsNoTracking();

                query = sortBy switch
                {
                    "LastName" => query.OrderBy(p => p.LastName),
                    "DateOfBirth" => query.OrderBy(p => p.DateOfBirth),
                    _ => query.OrderBy(p => p.LastName),
                };

                var patients = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                var patientDtos = _mapper.Map<IEnumerable<PatientListDto>>(patients);

                return Ok(patientDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting patients.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientEditDto>> GetPatient(int id)
        {
            try
            {
                var patient = await _context.Patients.FindAsync(id);

                if (patient == null)
                {
                    return NotFound();
                }

                var patientDto = _mapper.Map<PatientEditDto>(patient);

                return Ok(patientDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the patient.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(PatientEditDto patientDto)
        {
            try
            {
                var patient = _mapper.Map<Patient>(patientDto);

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving the patient.");
                return StatusCode(500, "An error occurred while saving the data. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientEditDto patientDto)
        {
            try
            {
                if (id != patientDto.Id)
                    return BadRequest();

                var patient = await _context.Patients.FindAsync(id);

                if (patient == null)
                    return NotFound();

                _mapper.Map(patientDto, patient);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the patient.");
                return StatusCode(500, "An error occurred while updating the data. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                {
                    return NotFound();
                }

                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the patient.");
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
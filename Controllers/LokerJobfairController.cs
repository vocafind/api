using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vocafind_api.DTO;
using vocafind_api.Models;

namespace vocafind_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LokerJobfairController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IMapper _mapper;

        public LokerJobfairController(TalentcerdasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/LokerJobfair/by-jobfair/{jobfairId} - Get all loker by jobfair
        [HttpGet("by-jobfair/{jobfairId}")]
        public async Task<ActionResult<IEnumerable<LokerUmumDTO>>> GetAllByJobfair(ulong jobfairId)
        {
            // Validasi apakah jobfair exists
            var jobfairExists = await _context.AcaraJobfairs
                .AnyAsync(j => j.Id == jobfairId && j.TanggalSelesaiAcara >= DateOnly.FromDateTime(DateTime.Now));

            if (!jobfairExists)
            {
                return NotFound(new { message = "Jobfair tidak ditemukan atau sudah berakhir." });
            }

            var lokerDTO = await _context.JobVacancies
                .Include(j => j.Company)
                .Include(j => j.LowonganAcaras)
                .Where(j => j.Status == "aktif")

                // FILTER: Hanya ambil yang ADA di jobfair tertentu (ada di LowonganAcaras dengan AcaraJobfairId yang sesuai)
                .Where(j => j.LowonganAcaras.Any(la => la.AcaraJobfairId == jobfairId))

                .ProjectTo<LokerUmumDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(lokerDTO);
        }

        // GET: api/LokerJobfair/{id}/detail - Alternatif tanpa query parameter
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<LokerUmumDetailDTO>> GetDetailById(string id)
        {
            var lokerQuery = _context.JobVacancies
                .Include(j => j.Company)
                .Include(j => j.JobQualifications)
                .Include(j => j.JobBenefits)
                .Include(j => j.JobAdditionalRequirements)
                .Include(j => j.JobAdditionalFacilities)
                .Include(j => j.LowonganAcaras)
                .Where(j => j.LowonganId == id && j.Status == "aktif");

            var loker = await lokerQuery
                .ProjectTo<LokerUmumDetailDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (loker == null)
                return NotFound(new { message = "Lowongan tidak ditemukan." });

            return Ok(loker);
        }

       
    }
}
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vocafind_api.DTO;
using vocafind_api.Models;
using vocafind_api.Services;

namespace vocafind_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LokerUmumController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public LokerUmumController(
            TalentcerdasContext context,
            IWebHostEnvironment env,
            IMapper mapper,
            IEmailService emailService,
            ILogger<TalentsController> logger,
            JwtService jwtService)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
            _jwtService = jwtService;
        }



        //Get all loker umum
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LokerUmumDTO>>> GetAll()
        {
            var lokerDTO = await _context.JobVacancies
                .Include(j => j.Company)
                .ProjectTo<LokerUmumDTO>(_mapper.ConfigurationProvider) // Mapping di DB, bukan di memory
                .ToListAsync();

            return Ok(lokerDTO);
        }


        // GET: api/LokerUmum/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LokerUmumDetailDTO>> GetById(string id)
        {
            var lokerQuery = _context.JobVacancies
                .Include(j => j.Company)
                .Include(j => j.JobQualifications)
                .Include(j => j.JobBenefits)
                .Include(j => j.JobAdditionalRequirements)
                .Include(j => j.JobAdditionalFacilities)
                .Where(j => j.LowonganId == id);

            var loker = await lokerQuery
                .ProjectTo<LokerUmumDetailDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (loker == null)
                return NotFound(new { message = "Lowongan tidak ditemukan." });

            return Ok(loker);
        }


    }
}

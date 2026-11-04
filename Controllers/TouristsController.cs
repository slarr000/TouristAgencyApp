using Microsoft.AspNetCore.Mvc;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Interfaces;

namespace TouristAgencyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristsController : ControllerBase
    {
        private readonly ITouristRepository _touristRepository;

        public TouristsController(ITouristRepository touristRepository)
        {
            _touristRepository = touristRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTourists()
        {
            var tourists = await _touristRepository.GetAllAsync();
            return Ok(tourists);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTourist([FromBody] TouristCreateDto touristDto)
        {
            var newTouristId = await _touristRepository.AddAsync(touristDto);
            return Created($"/api/Tourists/{newTouristId}", new { id = newTouristId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTouristById(int id)
        {
            var tourist = await _touristRepository.GetByIdAsync(id);
            if (tourist == null)
            {
                return NotFound($"Турист с ID {id} не найден.");
            }
            return Ok(tourist);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTourist(int id, [FromBody] TouristCreateDto touristDto)
        {
            var updated = await _touristRepository.UpdateAsync(id, touristDto);
            if (!updated)
            {
                return NotFound($"Турист с ID {id} для обновления не найден.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTourist(int id)
        {
            var deleted = await _touristRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Турист с ID {id} для удаления не найден.");
            }

            return NoContent();
        }

        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetTouristProfile(int id)
        {
            var profile = await _touristRepository.GetTouristProfileAsync(id);

            if (profile == null)
            {
                return NotFound($"Профиль туриста с ID {id} не найден.");
            }

            return Ok(profile);
        }
    }
}
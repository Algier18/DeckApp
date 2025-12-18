using DeckApi.Data;
using DeckApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeckApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DecksController(AppDbContext context) => _context = context;

        // GET /api/decks
        [HttpGet]
        public async Task<IActionResult> GetDecks()
        {
            var decks = await _context.Decks.AsNoTracking().ToListAsync();
            return Ok(decks);
        }

        // GET /api/decks/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDeck(int id)
        {
            var deck = await _context.Decks.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (deck == null) return NotFound("Talia nie istnieje");
            return Ok(deck);
        }

        // POST /api/decks
        [HttpPost]
        public async Task<IActionResult> CreateDeck([FromBody] CreateDeckRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Nazwa talii jest wymagana");

            var deck = new Deck { Name = request.Name, UserId = request.UserId };
            _context.Decks.Add(deck);
            await _context.SaveChangesAsync();
            return Ok(deck);
        }

        // PUT /api/decks/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> RenameDeck(int id, [FromBody] RenameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Nowa nazwa jest wymagana");

            var deck = await _context.Decks.FirstOrDefaultAsync(d => d.Id == id);
            if (deck == null) return NotFound("Talia nie istnieje");

            deck.Name = request.Name;
            await _context.SaveChangesAsync();
            return Ok(deck);
        }

        // DELETE /api/decks/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDeck(int id)
        {
            var deck = await _context.Decks.FirstOrDefaultAsync(d => d.Id == id);
            if (deck == null) return NotFound("Talia nie istnieje");

            _context.Decks.Remove(deck);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Talia {id} została usunięta" });
        }

        // POST /api/decks/import — symulacja importu (PDF/XLS parser podmienisz)
        [HttpPost("import")]
        public async Task<IActionResult> ImportDeck(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Nie przesłano pliku");

            var deck = new Deck { Name = $"Zaimportowana talia: {file.FileName}" };
            _context.Decks.Add(deck);
            await _context.SaveChangesAsync();
            return Ok(deck);
        }
    }

    public class CreateDeckRequest
    {
        public string Name { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }

    public class RenameRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}

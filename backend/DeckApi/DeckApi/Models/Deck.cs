using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeckApi.Models
{
    // BD: Mapowanie na istniejącą tabelę 'decks'
    [Table("decks")]
    public class Deck
    {
        [Key]
        [Column("Decks_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("Deck_Name")]
        public string Name { get; set; } = string.Empty;

        // Opcjonalny FK do users
        [Column("Users_Id")]
        public int? UserId { get; set; }
    }
}

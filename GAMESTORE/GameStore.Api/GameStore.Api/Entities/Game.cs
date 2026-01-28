namespace GameStore.Api.Entities
{
    public class Game
    {
        public int Id { get; set; }
        // required name
        public required string Name { get; set; }

        public int GenreId { get; set; }

        public Genre? Genre { get; set; }

        public decimal Price { get; set; }

        public DateOnly ReleaseDate { get; set; }
    }
}

using System;

namespace GameStore.Entities.Games
{
    public class ProductGenre
    {
        public int ProductId { get; set; }
        public int GenreId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Genre Genre { get; set; } = null!;
    }
}

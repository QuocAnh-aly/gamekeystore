using System;
using System.Collections.Generic;

namespace GameStore.Entities.Games
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public virtual ICollection<ProductGenre> ProductGenres { get; set; } = new HashSet<ProductGenre>();
    }
}

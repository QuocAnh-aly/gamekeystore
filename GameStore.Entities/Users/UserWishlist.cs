using System;
using GameStore.Entities.Games;

namespace GameStore.Entities.Users
{
    public class UserWishlist
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}

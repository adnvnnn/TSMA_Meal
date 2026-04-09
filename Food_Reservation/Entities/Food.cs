using Food_Reservation.Entities.Common;

namespace Food_Reservation.Entities
{
    public class Food : BaseEntity
    {
        public int OrderDay { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Restaurant { get; set; }
        public string Day { get; set; }
        public string Price { get; set; }
    }
}

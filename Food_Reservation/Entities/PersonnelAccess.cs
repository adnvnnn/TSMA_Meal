using Food_Reservation.Entities.Common;

namespace Food_Reservation.Entities;

public class PersonnelAccess : BaseEntity
{
    public bool FoodReservation { get; set; }
    public int PersonnelCode { get; set; }
}
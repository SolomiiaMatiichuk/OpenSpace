using OpenSpaceBooking.BLL.Dtos;
using OpenSpaceBooking.DAL.Entities;

namespace OpenSpaceBooking.BLL.Interfaces;

public interface IReservationService
{
    Task<ReservationDto> PayReservationAsync(int id);
    Task<ReservationCallback> CreateReservationAsync(ReservationDto reservation);
    Task<ReservationCallback> UpdateReservationAsync(ReservationDto reservation);
    Task DeleteReservationAsync(int reservationId);
    Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(string userId);
    Task<IEnumerable<ReservationDto>> GetOpenSpaceReservationsAsync(int photoZoneId);
    Task<IEnumerable<ReservationDto>> GetOpenSpaceReservationsByTitleAsync(string title, int photoZoneId);
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
    Task<ReservationDto> GetReservationAsync(int id);
    Task CancelReservationAsync(int reservationId);
}
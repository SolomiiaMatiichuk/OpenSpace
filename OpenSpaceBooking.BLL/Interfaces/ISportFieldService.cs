using OpenSpaceBooking.BLL.Dtos;
using OpenSpaceBooking.DAL.Entities;

namespace OpenSpaceBooking.BLL.Interfaces;

public interface IOpenSpaceService
{
    Task<OpenSpaceDto> GetOpenSpaceWithDetailsAsync(int photoZoneId);
    Task<List<OpenSpaceDto>> GetOpenSpacesWithDetailsAsync();
    Task<OpenSpaceCallback> CreateOpenSpace(OpenSpaceDto model);
    Task DeleteOpenSpace(int id);
    Task<OpenSpaceCallback> UpdateOpenSpace(OpenSpaceDto model);
}
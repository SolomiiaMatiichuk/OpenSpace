using System.Net;
using AutoMapper;
using OpenSpaceBooking.BLL.Dtos;
using OpenSpaceBooking.BLL.Interfaces;
using OpenSpaceBooking.DAL.Entities;
using OpenSpaceBooking.DAL.Interfaces;

namespace OpenSpaceBooking.BLL.Services;

public class OpenSpaceService : IOpenSpaceService
{
    private readonly IGenericRepository<PhotoZone> _repository;
    private readonly IGenericRepository<PhotoZoneDetail> _detailRepository;
    private readonly IMapper _mapper;

    public OpenSpaceService(IGenericRepository<PhotoZone> repository, IGenericRepository<PhotoZoneDetail> detailRepository, IMapper mapper)
    {
        _repository = repository;
        _detailRepository = detailRepository;
        _mapper = mapper;
    }

    public async Task<OpenSpaceDto> GetOpenSpaceWithDetailsAsync(int photoZoneId)
    {
        var field = await _repository.GetByIdAsync(photoZoneId);
        return _mapper.Map<PhotoZone, OpenSpaceDto>(field);
    }

    public async Task<List<OpenSpaceDto>> GetOpenSpacesWithDetailsAsync()
    {
        var fields = await _repository.GetAllAsync();
        return _mapper.Map<List<PhotoZone>, List<OpenSpaceDto>>(fields);
    }

    public async Task<OpenSpaceCallback> CreateOpenSpace(OpenSpaceDto model)
    {
        var startScheduleHours = Convert.ToDouble(model.StartProgram.Split('-')[0]);
        var startScheduleMinutes = Convert.ToDouble(model.StartProgram.Split('-')[1]);
        
        var endScheduleHours = Convert.ToDouble(model.EndProgram.Split('-')[0]);
        var endScheduleMinutes = Convert.ToDouble(model.EndProgram.Split('-')[1]);

        if (startScheduleHours is < 0 or > 24 || 
            endScheduleHours is < 0 or > 24 || 
            startScheduleMinutes is < 0 or > 59 || 
            endScheduleMinutes is < 0 or > 59)
        {
            return new OpenSpaceCallback
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = "Enter valid time"
            };
        }
        
        var field = new PhotoZone
        {
            Title = model.Title,
            PricePerHour = model.PricePerHour,
            ImageUrl = model.ImageUrl
        };
        await _repository.InsertAsync(field);

        var detail = new PhotoZoneDetail
        {
            Description = model.Description,
            Address = model.Address,
            EndProgram = model.EndProgram,
            StartProgram = model.StartProgram,
            PhotoZoneId = field.Id
        };
        await _detailRepository.InsertAsync(detail);

        return new OpenSpaceCallback
        {
            StatusCode = HttpStatusCode.OK
        };
    }

    public async Task DeleteOpenSpace(int id)
    {
        var field = await _repository.GetByIdAsync(id);
        _repository.DetachLocal(field, id);
        await _repository.DeleteAsync(field);
    }

    public async Task<OpenSpaceCallback> UpdateOpenSpace(OpenSpaceDto model)
    {
        var startScheduleHours = Convert.ToDouble(model.StartProgram.Split('-')[0]);
        var startScheduleMinutes = Convert.ToDouble(model.StartProgram.Split('-')[1]);

        var endScheduleHours = Convert.ToDouble(model.EndProgram.Split('-')[0]);
        var endScheduleMinutes = Convert.ToDouble(model.EndProgram.Split('-')[1]);

        if (startScheduleHours is < 0 or > 24 ||
            endScheduleHours is < 0 or > 24 ||
            startScheduleMinutes is < 0 or > 59 ||
            endScheduleMinutes is < 0 or > 59)
        {
            return new OpenSpaceCallback
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = "Enter valid time"
            };
        }

        var updatedField = new PhotoZone
        {
            Id = model.Id,
            Title = model.Title,
            PricePerHour = model.PricePerHour,
            ImageUrl = model.ImageUrl
        };
        await _repository.UpdateAsync(updatedField);
        
        var updatedDetail = new PhotoZoneDetail
        {
            Id = model.PhotoZoneDetailId,
            Description = model.Description,
            Address = model.Address,
            EndProgram = model.EndProgram,
            StartProgram = model.StartProgram,
            PhotoZoneId = model.Id
        };
        await _detailRepository.UpdateAsync(updatedDetail);

        return new OpenSpaceCallback
        {
            StatusCode = HttpStatusCode.OK
        };
    }
}
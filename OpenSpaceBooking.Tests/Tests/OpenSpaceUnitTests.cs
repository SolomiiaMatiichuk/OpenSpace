using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OpenSpaceBooking.BLL.Dtos;
using OpenSpaceBooking.BLL.Interfaces;
using OpenSpaceBooking.BLL.Profiles;
using OpenSpaceBooking.BLL.Services;
using OpenSpaceBooking.DAL;
using OpenSpaceBooking.DAL.Entities;
using OpenSpaceBooking.DAL.Repositories;
using Xunit;

namespace OpenSpaceBooking.Tests.Tests;

public class OpenSpaceUnitTests : IDisposable
{
    private IOpenSpaceService _openSpaceService;
    private readonly IMapper _mapper;
    private DataContext _context;

    private void InitTempInstances()
    {
        var builder = new DbContextOptionsBuilder<DataContext>();
        var dbName = Guid.NewGuid().ToString();
        builder.UseInMemoryDatabase(dbName);
        _context = new DataContext(builder.Options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        var photoZoneRepository = new GenericRepository<PhotoZone>(_context);
        var detailRepository = new GenericRepository<PhotoZoneDetail>(_context);
        _openSpaceService = new OpenSpaceService(photoZoneRepository, detailRepository, _mapper);
    }

    private async Task InitTempOpenSpace()
    {
        await _context.PhotoZones.AddAsync(new PhotoZone
        {
            Id = 1,
            ImageUrl = "Test",
            PricePerHour = 999,
            Title = "Test"
        });
        await _context.SaveChangesAsync();
        await _context.PhotoZoneDetails.AddAsync(new PhotoZoneDetail
        {
            Address = "Test",
            Description = "Test",
            EndProgram = "22-00",
            StartProgram = "11-00",
            PhotoZoneId = 1
        });
        await _context.SaveChangesAsync();
    }
    
    public OpenSpaceUnitTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperProfile());
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task UnitTest_CreateOpenSpace()
    {
        //arrange
        InitTempInstances();
        var model = new OpenSpaceDto
        {
            Id = 1,
            ImageUrl = "Test",
            Address = "Test",
            Description = "Test",
            EndProgram = "22-00",
            PricePerHour = 999,
            StartProgram = "11-00",
            Title = "Test"
        };
        
        //act
        await _openSpaceService.CreateOpenSpace(model);
        
        var result = await _context.PhotoZones.SingleOrDefaultAsync(t => t.Id == model.Id);
        
        //assert
        Assert.Equal(model.Id, result.Id);
    }


    [Fact]
    public async Task UnitTest_CreateOpenSpaceWithInvalidSchedule()
    {
        //arrange
        InitTempInstances();
        var model = new OpenSpaceDto
        {
            Id = 1,
            ImageUrl = "Test",
            Address = "Test",
            Description = "Test",
            EndProgram = "22-00",
            PricePerHour = 999,
            StartProgram = "54-00",
            Title = "Test"
        };

        //act
        await _openSpaceService.CreateOpenSpace(model);

        var result = await _context.PhotoZones.SingleOrDefaultAsync(t => t.Id == model.Id);

        //assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UnitTest_GetOpenSpace()
    {
        //arrange
        InitTempInstances();
        await InitTempOpenSpace();
        
        //act
        var result = await _openSpaceService.GetOpenSpaceWithDetailsAsync(1);
        
        //assert
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task UnitTest_UpdateOpenSpace()
    {
        //arrange
        InitTempInstances();
        await InitTempOpenSpace();
        var modelToUpdate = new OpenSpaceDto
        {
            Id = 1,
            PhotoZoneDetailId = 1,
            ImageUrl = "UpdatedTest",
            Address = "UpdatedTest",
            Description = "UpdatedTest",
            EndProgram = "22-00",
            PricePerHour = 999,
            StartProgram = "11-00",
            Title = "UpdatedTest"
        };
        
        //act
        await _openSpaceService.UpdateOpenSpace(modelToUpdate);
        
        //assert
        var result = await _context.PhotoZones.SingleOrDefaultAsync(t => t.Id == modelToUpdate.Id);
        Assert.Equal(modelToUpdate.Title, result.Title);
    }



    [Fact]
    public async Task UnitTest_UpdateOpenSpaceInvalidSchedule()
    {
        //arrange
        InitTempInstances();
        await InitTempOpenSpace();
        var modelToUpdate = new OpenSpaceDto
        {
            Id = 1,
            PhotoZoneDetailId = 1,
            ImageUrl = "UpdatedTest",
            Address = "UpdatedTest",
            Description = "UpdatedTest",
            EndProgram = "22-00",
            PricePerHour = 999,
            StartProgram = "43-00",
            Title = "UpdatedTest"
        };

        //act
        await _openSpaceService.UpdateOpenSpace(modelToUpdate);

        //assert
        var result = await _context.PhotoZones.SingleOrDefaultAsync(t => t.Id == modelToUpdate.Id);
        Assert.NotEqual(modelToUpdate.Title, result.Title);
    }

    [Fact]
    public async Task UnitTest_DeleteOpenSpace()
    {
        InitTempInstances();
        await InitTempOpenSpace();
        var dataBeforeDelete = await _context.PhotoZones.ToListAsync();
        var countBeforeDelete = dataBeforeDelete.Count;
        await _openSpaceService.DeleteOpenSpace(1);
        var dataAfterDelete = await _context.PhotoZones.ToListAsync();
        var countAfterDelete = dataAfterDelete.Count;
        Assert.Equal(countBeforeDelete - 1, countAfterDelete);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
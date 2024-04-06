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

public class ReservationUnitTests : IDisposable
{
    private IReservationService _reservationService;
    private DataContext _context;
    private readonly IMapper _mapper;

    private void InitTempInstances()
    {
        var builder = new DbContextOptionsBuilder<DataContext>();
        var dbName = Guid.NewGuid().ToString();
        builder.UseInMemoryDatabase(dbName);
        _context = new DataContext(builder.Options);
        var reservationRepository = new GenericRepository<Reservation>(_context);
        var photoZoneRepository = new GenericRepository<PhotoZone>(_context);
        _reservationService = new ReservationService(reservationRepository, _mapper, photoZoneRepository);
    }

    private async Task InitTempOpenSpace()
    {
        await _context.PhotoZones.AddAsync(new PhotoZone
        {
            Id = 1,
            Title = "Test",
            ImageUrl = "Test",
            PricePerHour = 999
        });
        await _context.SaveChangesAsync();
        await _context.PhotoZoneDetails.AddAsync(new PhotoZoneDetail
        {
            Address = "Test",
            PhotoZoneId = 1,
            Description = "Test",
            StartProgram = "10-00",
            EndProgram = "23-00"
        });
        await _context.SaveChangesAsync();
    }

    private async Task InitTempUser()
    {
        await _context.Users.AddAsync(new User
        {
            Id = "UserTestId",
            UserName = "TestUser",
            Email = "Test@email.com",
            FirstName = "TestUser",
            LastName = "TestUser",
            PhoneNumber = "Test",
            PasswordHash = "UserTestHash"
        });
        await _context.SaveChangesAsync();
    }
    
    public ReservationUnitTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperProfile());
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task UnitTest_CreateReservationPossitive()
    {
        //arrange
        InitTempInstances();
        await InitTempOpenSpace();
        await InitTempUser();

        var model = new ReservationDto
        {
            Id = 1,
            Start = new DateTime(2024, 7, 10, 12, 0, 0),
            End = new DateTime(2024, 7, 10, 18, 0, 0),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Status = "Pending",
            Title = "Test",
        };

        await _reservationService.CreateReservationAsync(model);

        //act
        var result = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == model.Id);
        
        //assert
        Assert.Equal(model.Title, result.Title);
    }

    [Fact]
    public async Task UnitTest_CreateReservationWithWrongDay()
    {
        //arrange
        InitTempInstances();
        await InitTempOpenSpace();
        await InitTempUser();

        var model = new ReservationDto
        {
            Id = 1,
            Start = new DateTime(2022, 7, 10, 12, 0, 0),
            End = new DateTime(2022, 7, 10, 18, 0, 0),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Status = "Pending",
            Title = "Test",
        };

        await _reservationService.CreateReservationAsync(model);

        //act
        var result = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == model.Id);

        //assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UnitTest_CreateReservationWithWrongHour()
    {
        //arrange
        InitTempInstances();
        await InitTempOpenSpace();
        await InitTempUser();

        var model = new ReservationDto
        {
            Id = 1,
            Start = new DateTime(2024, 7, 10, 7, 0, 0),
            End = new DateTime(2024, 7, 10, 18, 0, 0),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Status = "Pending",
            Title = "Test",
        };

        await _reservationService.CreateReservationAsync(model);

        //act
        var result = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == model.Id);

        //assert
        Assert.Null(result);
    }


    [Fact]
    public async Task UnitTest_CreateReservationThatNotAvailable()
    {
        //arrange
        InitTempInstances();
        await InitTempOpenSpace();
        await InitTempUser();

        var model = new ReservationDto
        {
            Id = 1,
            Start = new DateTime(2024, 7, 10, 15, 0, 0),
            End = new DateTime(2024, 7, 10, 18, 0, 0),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Status = "Pending",
            Title = "Test",
        };

        var model2 = new ReservationDto
        {
            Id = 2,
            Start = new DateTime(2024, 7, 10, 15, 0, 0),
            End = new DateTime(2024, 7, 10, 18, 0, 0),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Status = "Pending",
            Title = "Test",
        };

        await _reservationService.CreateReservationAsync(model);

        await _reservationService.CreateReservationAsync(model2);

        //act
        var result = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == model2.Id);

        //assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UnitTest_GetReservation()
    {
        //arrange
        InitTempInstances();
        await InitTempOpenSpace();
        await InitTempUser();
        _context.Reservations.Add(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();
        
        //act
        var result = await _reservationService.GetReservationAsync(77);
        
        //assert
        Assert.Equal("Test", result.Title);
    }
    [Fact]
    public async Task UnitTest_DeleteReservation()
    {
        InitTempInstances();
        await InitTempOpenSpace();
        await InitTempUser();
        
        _context.Reservations.Add(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();
        var dataBeforeDelete = await _context.Reservations.ToListAsync();
        var countBeforeDelete = dataBeforeDelete.Count;
        await _reservationService.DeleteReservationAsync(77);
        var dataAfterDelete = await _context.Reservations.ToListAsync();
        var countAfterDelete = dataAfterDelete.Count;
        Assert.Equal(countBeforeDelete - 1, countAfterDelete);
    }

    [Fact]
    public async Task UnitTest_UpdateReservation()
    {
        InitTempInstances();
        await InitTempUser();
        await InitTempOpenSpace();
        
        _context.Reservations.Add(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();

        var modelToUpdate = new ReservationDto
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Updated",
        };

        await _reservationService.UpdateReservationAsync(modelToUpdate);

        var result = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == modelToUpdate.Id);
        Assert.Equal(modelToUpdate.Title, result.Title);
    }


    [Fact]
    public async Task UnitTest_UpdateReservationWrongHour()
    {
        InitTempInstances();
        await InitTempUser();
        await InitTempOpenSpace();

        _context.Reservations.Add(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Test",
        });
        _context.Reservations.Add(new Reservation
        {
            Id = 55,
            Start = new DateTime(1975, 7, 10, 7, 10, 24),
            End = new DateTime(1975, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();

        var modelToUpdate = new ReservationDto
        {
            Id = 77,
            Start = new DateTime(1975, 7, 10, 7, 10, 24),
            End = new DateTime(1975, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Updated",
        };

        await _reservationService.UpdateReservationAsync(modelToUpdate);

        var result = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == modelToUpdate.Id);
        Assert.NotEqual(modelToUpdate.Title, result.Title); //not updated

    }

    [Fact]
    public async Task UnitTest_GetReservationsByTitle()
    {
        //arrange
        InitTempInstances();
        await InitTempUser();
        await InitTempOpenSpace();
        
        await _context.Reservations.AddAsync(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();
        
        //act
        var result = await _reservationService.GetOpenSpaceReservationsByTitleAsync("Test", 1);
        
        //assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.First().Title);
    }

    [Fact]
    public async Task UnitTest_GetReservationsByNullTitle()
    {
        //arrange
        InitTempInstances();
        await InitTempUser();
        await InitTempOpenSpace();

        await _context.Reservations.AddAsync(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();

        //act
        var result = await _reservationService.GetOpenSpaceReservationsByTitleAsync(null, 1);

        //assert
        Assert.Null(result);
    }


    [Fact]
    public async Task UnitTest_GetReservationsByNotAvailableTitle()
    {
        //arrange
        InitTempInstances();
        await InitTempUser();
        await InitTempOpenSpace();

        await _context.Reservations.AddAsync(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();

        //act
        var result = await _reservationService.GetOpenSpaceReservationsByTitleAsync("NotAvailableTitle", 1);

        //assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task UnitTest_CancelReservationTest()
    {
        //arrange
        InitTempInstances();
        await InitTempUser();
        await InitTempOpenSpace();
        
        await _context.Reservations.AddAsync(new Reservation
        {
            Id = 77,
            Start = new DateTime(1974, 7, 10, 7, 10, 24),
            End = new DateTime(1974, 7, 10, 10, 0, 24),
            UserId = "UserTestId",
            PhotoZoneId = 1,
            Title = "Test",
        });
        await _context.SaveChangesAsync();
        
        //act
        await _reservationService.CancelReservationAsync(77);
        
        //assert
        var result = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == 77);
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
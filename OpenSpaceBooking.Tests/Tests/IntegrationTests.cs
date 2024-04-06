using AutoMapper;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using OpenSpaceBooking.BLL.Dtos;
using OpenSpaceBooking.BLL.Interfaces;
using OpenSpaceBooking.BLL.Profiles;
using OpenSpaceBooking.BLL.Services;
using OpenSpaceBooking.DAL;
using OpenSpaceBooking.DAL.Constraints;
using OpenSpaceBooking.DAL.Entities;
using OpenSpaceBooking.DAL.Repositories;
using System;
using System.Collections.Generic;
using Xunit;
using System.Threading.Tasks;
using System.Net;

namespace OpenSpaceBooking.Tests.Tests
{
    public class IntegrationTests : IDisposable
    {

        private IAuthService _authService;
        private DataContext _context;
        public RoleManager<IdentityRole> roleManager;
        public UserManager<User> userManager;
        private IOpenSpaceService _openSpaceService;
        private IReservationService _reservationService;
        private readonly IMapper _mapper;

        private void InitTempInstances()
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            var dbName = Guid.NewGuid().ToString();
            builder.UseInMemoryDatabase(dbName);
            _context = new DataContext(builder.Options);

            // Configure Identity options
            var identityOptions = new IdentityOptions();

            // Create user store and password hasher
            var userStore = new UserStore<User>(_context);
            var passwordHasher = new PasswordHasher<User>();

            // Create validators (optional, you can pass null or an empty collection if not needed)
            var userValidators = new List<IUserValidator<User>>();
            var passwordValidators = new List<IPasswordValidator<User>>();

            // Create key normalizer and error describer
            var keyNormalizer = new UpperInvariantLookupNormalizer();
            var errors = new IdentityErrorDescriber();

            // Create services and logger
            var services = new ServiceCollection();
            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<DbContext>();


            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder
                .AddConsole()
                .AddFilter(level => level >= LogLevel.Information)
            );
            var loggerFactory = serviceCollection.BuildServiceProvider()
                .GetService<ILoggerFactory>();
            var logge1r = loggerFactory.CreateLogger<UserManager<User>>();
            var logger = loggerFactory.CreateLogger<RoleManager<IdentityRole>>();


            // Create UserManager instance
            userManager = new UserManager<User>(
                userStore,
                Options.Create(identityOptions),
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services.BuildServiceProvider(),
                logge1r
            );


            // Create RoleManager instance
            roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(_context),
                null, // No role validators provided
                keyNormalizer,
                errors,
                logger
            );



            // Initialize AuthService with UserManager and RoleManager
            _authService = new AuthService(userManager, roleManager);



            // Add initial user to the context
            var admin_user = new User
            {//"eb0e0af9-796b-4a3c-953d-1c5c32d46323" "cff43952-2670-48e3-8ae7-ee1c5713efae"
                Id = SystemRoleConstraints.AdminRoleId,
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com".ToUpper(),
                EmailConfirmed = true,
                FirstName = "admin",
                LastName = "admin",
                UserName = "admin@gmail.com",
                NormalizedUserName = "admin@gmail.com".ToUpper(),
            };

            //set user password
            var passwordHasher1 = new PasswordHasher<User>();
            admin_user.PasswordHash = passwordHasher1.HashPassword(admin_user, "Qwerty1-");
            _context.Users.Add(admin_user);

            _context.SaveChanges();

            var reservationRepository = new GenericRepository<Reservation>(_context);
            var photoZoneRepository = new GenericRepository<PhotoZone>(_context);
            _reservationService = new ReservationService(reservationRepository, _mapper, photoZoneRepository);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            //var photoZoneRepository = new GenericRepository<PhotoZone>(_context);
            var detailRepository = new GenericRepository<PhotoZoneDetail>(_context);
            _openSpaceService = new OpenSpaceService(photoZoneRepository, detailRepository, _mapper);
        }


        public IntegrationTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
             _mapper = mapperConfig.CreateMapper();
        }


        [Fact]
        public async Task IntegrationTest_CreateOpenSpaceAsAdmin()
        {

            InitTempInstances();
            var userRole = await roleManager.FindByNameAsync(SystemRoleConstraints.UserRole);
            if (userRole == null)
            {
                userRole = new IdentityRole(SystemRoleConstraints.UserRole);
                await roleManager.CreateAsync(userRole);
            }
            var adminRole = await roleManager.FindByNameAsync(SystemRoleConstraints.AdminRole);
            if (adminRole == null)
            {
                adminRole = new IdentityRole(SystemRoleConstraints.AdminRole);
                await roleManager.CreateAsync(adminRole);
            }


            var registerModel = new RegisterDto
            {
                Email = "solomia1212@gmail.com",
                Password = "Qwerty1-",
                ConfirmPassword = "Qwerty1",
                FirstName = "Name",
                LastName = "Surname",
            };

            await _authService.RegisterAsync(registerModel);

            var result = await _authService.LoginAsync("solomia1212@gmail.com", "Qwerty1-");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);


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

            var result2 = await _context.PhotoZones.SingleOrDefaultAsync(t => t.Id == model.Id);

            //assert
            Assert.Equal(model.Id, result2.Id);



        }


        [Fact]
        public async Task IntegrationTest_RegisterAsUserAndDoReservationForCreatedOpenSpace()
        {

            //arrange
            InitTempInstances();

            var userRole = await roleManager.FindByNameAsync(SystemRoleConstraints.UserRole);
            if (userRole == null)
            {
                userRole = new IdentityRole(SystemRoleConstraints.UserRole);
                await roleManager.CreateAsync(userRole);
            }

            var registerModel = new RegisterDto
            {
                Email = "solomia1212@gmail.com",
                Password = "Qwerty1-",
                ConfirmPassword = "Qwerty1",
                FirstName = "Name",
                LastName = "Surname",
            };

            var result = await _authService.RegisterAsync(registerModel);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode); // user registered successfully 

            // Retrieve the registered user
            var registeredUser = await userManager.FindByEmailAsync(registerModel.Email);

            // Assert that the user exists
            Assert.NotNull(registeredUser);

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

            var result1 = await _context.PhotoZones.SingleOrDefaultAsync(t => t.Id == model.Id);

            //assert
            Assert.Equal(model.Id, result1.Id);

            var model2 = new ReservationDto
            {
                Id = 1,
                Start = new DateTime(2024, 7, 10, 12, 0, 0),
                End = new DateTime(2024, 7, 10, 18, 0, 0),
                UserId = registeredUser.Id,
                PhotoZoneId = 1,
                Status = "Pending",
                Title = "Test",
            };

            await _reservationService.CreateReservationAsync(model2);

            //act
            var result2 = await _context.Reservations.SingleOrDefaultAsync(t => t.Id == model.Id);

            //assert
            Assert.Equal(model.Title, result2.Title);
        }



        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
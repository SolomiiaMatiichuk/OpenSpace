using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
using Xunit;

namespace OpenSpaceBooking.Tests.Tests
{
    public class AuthenticationTests : IDisposable
    {
        private IAuthService _authService;
        private DataContext _context;
        public RoleManager<IdentityRole> roleManager;
        public UserManager<User> userManager;

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
        }


        public AuthenticationTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            // _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task UnitTest_RegisterUser()
        {
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

            // Clean up database after test
            _context.Users.Remove(registeredUser);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task UnitTest_RegisterUserWrongEmail()
        {
            InitTempInstances();
            var userRole = await roleManager.FindByNameAsync(SystemRoleConstraints.UserRole);
            if (userRole == null)
            {
                userRole = new IdentityRole(SystemRoleConstraints.UserRole);
                await roleManager.CreateAsync(userRole);
            }

            var registerModel = new RegisterDto
            {
                Email = "wrong email", // Wrong email
                Password = "Qwerty1-",
                ConfirmPassword = "Qwerty1",
                FirstName = "Name",
                LastName = "Surname",
            };

            var result = await _authService.RegisterAsync(registerModel);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Retrieve the registered user
            var registeredUser = await userManager.FindByEmailAsync(registerModel.Email);

            // Assert that the user exists
            Assert.NotNull(registeredUser);

            // Clean up database after test
            _context.Users.Remove(registeredUser);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task UnitTest_RegisterAlreadyRegisteredUser()
        {
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

            await _authService.RegisterAsync(registerModel);

            var result = await _authService.RegisterAsync(registerModel); // try to register again

            Assert.Equal(HttpStatusCode.Conflict, result.StatusCode); // conflict

            // Retrieve the registered user
            var registeredUser = await userManager.FindByEmailAsync(registerModel.Email);

            // Assert that the user exists
            Assert.NotNull(registeredUser);

            // Clean up database after test
            _context.Users.Remove(registeredUser);
            await _context.SaveChangesAsync();
        }



        [Fact]
        public async Task UnitTest_LoginAdmin()
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
      

            var result = await _authService.LoginAsync("admin@gmail.com", "Qwerty1-"); 

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        }


        [Fact]
        public async Task UnitTest_LoginUser()
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

            // Retrieve the registered user
            var registeredUser = await userManager.FindByEmailAsync(registerModel.Email);

            // Assert that the user exists
            Assert.NotNull(registeredUser);

            // Clean up database after test
            _context.Users.Remove(registeredUser);
            await _context.SaveChangesAsync();

        }


        [Fact]
        public async Task UnitTest_LoginUserWrongPassword()
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


            var result = await _authService.LoginAsync("solomia1212@gmail.com", "Qwerty1--");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);

            // Retrieve the registered user
            var registeredUser = await userManager.FindByEmailAsync(registerModel.Email);

            // Assert that the user exists
            Assert.NotNull(registeredUser);

            // Clean up database after test
            _context.Users.Remove(registeredUser);
            await _context.SaveChangesAsync();

        }


        [Fact]
        public async Task UnitTest_LoginNotExistingUser()
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


            var result = await _authService.LoginAsync("solomia1212@gmail.com", "Qwerty1-");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);


        }


        [Fact]
        public async Task UnitTest_ChangePassword()
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

            var user = await userManager.FindByEmailAsync(registerModel.Email);

            var result = await _authService.LoginAsync("solomia1212@gmail.com", "Qwerty1--");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);

            //await _authService.ChangePassword("Qwerty1--", "solomia1212@gmail.com");
            var registeredUser = await userManager.FindByEmailAsync(registerModel.Email);
            var passwordHasher1 = new PasswordHasher<User>();
            registeredUser.PasswordHash = passwordHasher1.HashPassword(registeredUser, "Qwerty1--");
            await _context.SaveChangesAsync();

            result = await _authService.LoginAsync("solomia1212@gmail.com", "Qwerty1--");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode); // now with changed password - we can login

            // Retrieve the registered user
            registeredUser = await userManager.FindByEmailAsync(registerModel.Email);

            // Assert that the user exists
            Assert.NotNull(registeredUser);

            // Clean up database after test
            _context.Users.Remove(registeredUser);
            await _context.SaveChangesAsync();

        }


        [Fact]
        public async Task UnitTest_ChangePasswordWrong()
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

            var user = await userManager.FindByEmailAsync(registerModel.Email);

            var result = await _authService.LoginAsync("solomia1212@gmail.com", "1234");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);


            // Retrieve the registered user
           var  registeredUser = await userManager.FindByEmailAsync(registerModel.Email);

            // Assert that the user exists
            Assert.NotNull(registeredUser);

            // Clean up database after test
            _context.Users.Remove(registeredUser);
            await _context.SaveChangesAsync();

        }


        [Fact]
        public async Task UnitTest_EditUserName()
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

            var user = await userManager.FindByEmailAsync(registerModel.Email);
            // Assert that the user exists
            Assert.NotNull(user);
            Assert.Equal("Name", user.FirstName);


            var editModel = new UserEditDto
            {
                FirstName = "Solomiia",
                LastName = "Surname",
            };

            await _authService.EditUserAsync(editModel, user.Id);

            user = await userManager.FindByEmailAsync(registerModel.Email);
            // Assert that the user exists
            Assert.NotNull(user);
            Assert.Equal("Solomiia", user.FirstName);




            // Retrieve the registered user
            var registeredUser = await userManager.FindByEmailAsync(registerModel.Email);

            // Assert that the user exists
            Assert.NotNull(registeredUser);

            // Clean up database after test
            _context.Users.Remove(registeredUser);
            await _context.SaveChangesAsync();

        }


        [Fact]
        public async Task UnitTest_EditUserSurName()
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

            var user = await userManager.FindByEmailAsync(registerModel.Email);
            // Assert that the user exists
            Assert.NotNull(user);
            Assert.Equal("Surname", user.LastName);


            var editModel = new UserEditDto
            {
                FirstName = "Name",
                LastName = "Cat",
            };

            await _authService.EditUserAsync(editModel, user.Id);

            user = await userManager.FindByEmailAsync(registerModel.Email);
            // Assert that the user exists
            Assert.NotNull(user);
            Assert.Equal("Cat", user.LastName);




            // Retrieve the registered user
            var registeredUser = await userManager.FindByEmailAsync(registerModel.Email);

            // Assert that the user exists
            Assert.NotNull(registeredUser);

            // Clean up database after test
            _context.Users.Remove(registeredUser);
            await _context.SaveChangesAsync();

        }


        [Fact]
        public async Task UnitTest_DeleteAcount()
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

            var user = await userManager.FindByEmailAsync(registerModel.Email);
            // Assert that the user exists
            Assert.NotNull(user);


            await _authService.DeleteAccountAsync(user.Id);

            await _context.SaveChangesAsync();

            user = await userManager.FindByEmailAsync(registerModel.Email);
            // Assert that the user exists
            Assert.Null(user);

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

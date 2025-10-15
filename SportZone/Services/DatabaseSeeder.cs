using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportZone.Configuration;
using SportZone.Models;
using SportZone.Repositories;

namespace SportZone.Services
{
    /// <summary>
    /// Service voor het seeden van initiële data in de database
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger<DatabaseSeeder> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        /// <summary>
        /// Seed de database met initiële data
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                // Check if admin user already exists
                var adminUser = await _userRepository.GetByUsernameAsync("admin");
                
                if (adminUser == null)
                {
                    _logger.LogInformation("Creating default admin user...");
                    
                    // Create admin user
                    var admin = new User
                    {
                        Username = "admin",
                        Email = "admin@sportzone.com",
                        Password = _passwordHasher.HashPassword("passtimadmin"),
                        Name = "Admin User",
                        PreferredSport = null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _userRepository.CreateAsync(admin);
                    _logger.LogInformation("Default admin user created successfully with username: admin");
                }
                else
                {
                    _logger.LogInformation("Admin user already exists, skipping seed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}

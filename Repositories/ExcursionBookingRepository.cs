using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Models;
using TouristAgencyApp.Interfaces;

namespace TouristAgencyApp.Repositories
{
    public class ExcursionBookingRepository : IExcursionBookingRepository
    {
        private readonly string _connectionString;

        public ExcursionBookingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<ExcursionBooking>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Excursion Bookings\"";
            return await db.QueryAsync<ExcursionBooking>(sql);
        }

        public async Task<int> AddAsync(ExcursionBookingCreateDto booking)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                INSERT INTO ""Excursion Bookings"" (tourist_id, excursion_id) 
                VALUES (@tourist_id, @excursion_id) 
                RETURNING booking_id;";

            return await db.QuerySingleAsync<int>(sql, booking);
        }

        // ... существующие методы GetAllAsync и AddAsync ...

        public async Task<ExcursionBooking?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Excursion Bookings\" WHERE booking_id = @id";

            return await db.QueryFirstOrDefaultAsync<ExcursionBooking>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, ExcursionBookingCreateDto booking)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                UPDATE ""Excursion Bookings"" 
                SET tourist_id = @tourist_id, 
                    excursion_id = @excursion_id 
                WHERE booking_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new
            {
                id,
                booking.tourist_id,
                booking.excursion_id
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Excursion Bookings\" WHERE booking_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }
    }
}
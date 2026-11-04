using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Models;
using TouristAgencyApp.Interfaces;

namespace TouristAgencyApp.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public BookingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Bookings\"";
            return await db.QueryAsync<Booking>(sql);
        }

        public async Task<int> AddAsync(BookingCreateDto booking)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                INSERT INTO ""Bookings"" (tourist_id, hotel_id, check_in_date, check_out_date, room_number) 
                VALUES (@tourist_id, @hotel_id, @check_in_date, @check_out_date, @room_number) 
                RETURNING booking_id;";

            return await db.QuerySingleAsync<int>(sql, booking);
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Bookings\" WHERE booking_id = @id";

            return await db.QueryFirstOrDefaultAsync<Booking>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, BookingCreateDto booking)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                UPDATE ""Bookings"" 
                SET tourist_id = @tourist_id, 
                    hotel_id = @hotel_id,
                    check_in_date = @check_in_date,
                    check_out_date = @check_out_date,
                    room_number = @room_number
                WHERE booking_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new
            {
                id,
                booking.tourist_id,
                booking.hotel_id,
                booking.check_in_date,
                booking.check_out_date,
                booking.room_number
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Bookings\" WHERE booking_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }
    }
}
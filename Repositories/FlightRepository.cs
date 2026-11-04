using System.Data;
using Dapper;
using Npgsql;
using TouristAgencyApp.Models;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Interfaces;
using Microsoft.Extensions.Configuration; // Добавлен using

namespace TouristAgencyApp.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly string _connectionString;

        public FlightRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<Flight>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Flights\"";
            return await db.QueryAsync<Flight>(sql);
        }

        public async Task<int> AddAsync(FlightCreateDto flight)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            // ДОБАВЛЕНО НОВОЕ ПОЛЕ: flight_type
            string sql = @"
                INSERT INTO ""Flights"" (flight_number, flight_date, aircraft_type, flight_type) 
                VALUES (@flight_number, @flight_date, @aircraft_type, @flight_type) 
                RETURNING flight_id;";

            return await db.QuerySingleAsync<int>(sql, flight);
        }

        public async Task<Flight?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Flights\" WHERE flight_id = @id";
            return await db.QueryFirstOrDefaultAsync<Flight>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, FlightCreateDto flight)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            // ДОБАВЛЕНО НОВОЕ ПОЛЕ: flight_type
            string sql = @"
                UPDATE ""Flights"" 
                SET flight_number = @flight_number, 
                    flight_date = @flight_date, 
                    aircraft_type = @aircraft_type,
                    flight_type = @flight_type
                WHERE flight_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new
            {
                id,
                flight.flight_number,
                flight.flight_date,
                flight.aircraft_type,
                flight.flight_type
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Flights\" WHERE flight_id = @id";
            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }
    }
}
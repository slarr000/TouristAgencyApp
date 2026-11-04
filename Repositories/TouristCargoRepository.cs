using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Interfaces;
using TouristAgencyApp.Models;

namespace TouristAgencyApp.Repositories
{
    public class TouristCargoRepository : ITouristCargoRepository
    {
        private readonly string _connectionString;

        public TouristCargoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<TouristCargo>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            // Таблица с пробелом берется в двойные кавычки
            string sql = "SELECT * FROM \"Tourist Cargo\"";

            return await db.QueryAsync<TouristCargo>(sql);
        }

        public async Task<int> AddAsync(TouristCargoCreateDto cargo)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            // Обрати внимание на экранирование двойными кавычками вокруг Tourist Cargo
            string sql = @"
                INSERT INTO ""Tourist Cargo"" 
                (tourist_id, cargo_type, places_count, actual_weight, volume_weight, marking, tags, packaging_cost, insurance_cost, total_amount, flight_id) 
                VALUES 
                (@tourist_id, @cargo_type, @places_count, @actual_weight, @volume_weight, @marking, @tags, @packaging_cost, @insurance_cost, @total_amount, @flight_id) 
                RETURNING cargo_id;";

            return await db.QuerySingleAsync<int>(sql, cargo);
        }

        // ... существующие методы GetAllAsync и AddAsync ...

        public async Task<TouristCargo?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Tourist Cargo\" WHERE cargo_id = @id";

            return await db.QueryFirstOrDefaultAsync<TouristCargo>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, TouristCargoCreateDto cargo)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                UPDATE ""Tourist Cargo"" 
                SET tourist_id = @tourist_id, 
                    cargo_type = @cargo_type, 
                    places_count = @places_count, 
                    actual_weight = @actual_weight, 
                    volume_weight = @volume_weight, 
                    marking = @marking, 
                    tags = @tags, 
                    packaging_cost = @packaging_cost, 
                    insurance_cost = @insurance_cost, 
                    total_amount = @total_amount, 
                    flight_id = @flight_id 
                WHERE cargo_id = @id";

            var parameters = new
            {
                id,
                cargo.tourist_id,
                cargo.cargo_type,
                cargo.places_count,
                cargo.actual_weight,
                cargo.volume_weight,
                cargo.marking,
                cargo.tags,
                cargo.packaging_cost,
                cargo.insurance_cost,
                cargo.total_amount,
                cargo.flight_id
            };

            var rowsAffected = await db.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Tourist Cargo\" WHERE cargo_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }
    }
}
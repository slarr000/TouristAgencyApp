using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Interfaces;
using TouristAgencyApp.Models;

namespace TouristAgencyApp.Repositories
{
    public class AgencyRepository : IAgencyRepository
    {
        private readonly string _connectionString;

        public AgencyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<Agency>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = "SELECT * FROM \"Agencies\"";

            return await db.QueryAsync<Agency>(sql);
        }

        public async Task<int> AddAsync(AgencyCreateDto agency)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                INSERT INTO ""Agencies"" (agency_name, quality_rating) 
                VALUES (@agency_name, @quality_rating) 
                RETURNING agency_id;";

            return await db.QuerySingleAsync<int>(sql, agency);
        }

        public async Task<Agency?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Agencies\" WHERE agency_id = @id";

            // QueryFirstOrDefaultAsync вернет null, если запись не найдена
            return await db.QueryFirstOrDefaultAsync<Agency>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, AgencyCreateDto agency)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = @"
                UPDATE ""Agencies"" 
                SET agency_name = @agency_name, 
                    quality_rating = @quality_rating 
                WHERE agency_id = @id";

            // Объединяем ID и данные из DTO в один анонимный объект
            var rowsAffected = await db.ExecuteAsync(sql, new
            {
                id,
                agency.agency_name,
                agency.quality_rating
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Agencies\" WHERE agency_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }
    }
}
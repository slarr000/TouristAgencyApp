using System.Data;
using Dapper;
using Npgsql;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Interfaces;
using TouristAgencyApp.Models;

namespace TouristAgencyApp.Repositories
{
    public class TouristChildRepository : ITouristChildRepository
    {
        private readonly string _connectionString;

        public TouristChildRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<TouristChild>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Tourist Children\"";
            return await db.QueryAsync<TouristChild>(sql);
        }

        public async Task<int> AddAsync(TouristChildCreateDto child)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                INSERT INTO ""Tourist Children"" 
                (tourist_id, first_name, last_name, patronymic, date_of_birth, gender, birth_certificate_number, date_of_issue, issued_by) 
                VALUES 
                (@tourist_id, @first_name, @last_name, @patronymic, @date_of_birth, @gender, @birth_certificate_number, @date_of_issue, @issued_by) 
                RETURNING child_id;";

            return await db.QuerySingleAsync<int>(sql, child);
        }

        // ... существующие методы GetAllAsync и AddAsync ...

        public async Task<TouristChild?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Tourist Children\" WHERE child_id = @id";

            return await db.QueryFirstOrDefaultAsync<TouristChild>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, TouristChildCreateDto child)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                UPDATE ""Tourist Children"" 
                SET tourist_id = @tourist_id, 
                    first_name = @first_name, 
                    last_name = @last_name, 
                    patronymic = @patronymic, 
                    date_of_birth = @date_of_birth, 
                    gender = @gender, 
                    birth_certificate_number = @birth_certificate_number, 
                    date_of_issue = @date_of_issue, 
                    issued_by = @issued_by 
                WHERE child_id = @id";

            var parameters = new
            {
                id,
                child.tourist_id,
                child.first_name,
                child.last_name,
                child.patronymic,
                child.date_of_birth,
                child.gender,
                child.birth_certificate_number,
                child.date_of_issue,
                child.issued_by
            };

            var rowsAffected = await db.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Tourist Children\" WHERE child_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }
    }
}
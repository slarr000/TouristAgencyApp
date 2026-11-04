using System.Data;
using Dapper;
using Npgsql;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Interfaces;
using TouristAgencyApp.Models;

namespace TouristAgencyApp.Repositories
{
    public class PassportRepository : IPassportRepository
    {
        private readonly string _connectionString;

        public PassportRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<Passport>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = "SELECT * FROM \"Tourist's Passport Details\"";

            return await db.QueryAsync<Passport>(sql);
        }

        public async Task<int> AddAsync(PassportCreateDto passport)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            // Обрати внимание на экранирование двойными кавычками
            string sql = @"
                INSERT INTO ""Tourist's Passport Details"" 
                (tourist_id, type_passport, passport_serial, passport_number, date_of_issue, issued_by, latin_name, expiry_date, department_code, registration_address) 
                VALUES 
                (@tourist_id, @type_passport, @passport_serial, @passport_number, @date_of_issue, @issued_by, @latin_name, @expiry_date, @department_code, @registration_address) 
                RETURNING passport_id;";

            return await db.QuerySingleAsync<int>(sql, passport);
        }

        // ... существующие методы GetAllAsync и AddAsync ...

        public async Task<Passport?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Tourist's Passport Details\" WHERE passport_id = @id";

            return await db.QueryFirstOrDefaultAsync<Passport>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, PassportCreateDto passport)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                UPDATE ""Tourist's Passport Details"" 
                SET tourist_id = @tourist_id, 
                    type_passport = @type_passport, 
                    passport_serial = @passport_serial, 
                    passport_number = @passport_number, 
                    date_of_issue = @date_of_issue, 
                    issued_by = @issued_by, 
                    latin_name = @latin_name, 
                    expiry_date = @expiry_date, 
                    department_code = @department_code, 
                    registration_address = @registration_address 
                WHERE passport_id = @id";

            var parameters = new
            {
                id,
                passport.tourist_id,
                passport.type_passport,
                passport.passport_serial,
                passport.passport_number,
                passport.date_of_issue,
                passport.issued_by,
                passport.latin_name,
                passport.expiry_date,
                passport.department_code,
                passport.registration_address
            };

            var rowsAffected = await db.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Tourist's Passport Details\" WHERE passport_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }
    }
}
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TouristAgencyApp.DTOs;
using TouristAgencyApp.Models;
using TouristAgencyApp.Interfaces;
using System.Text;
namespace TouristAgencyApp.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly string _connectionString;

        public HotelRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IEnumerable<Hotel>> GetAllAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Hotels\"";
            return await db.QueryAsync<Hotel>(sql);
        }

        public async Task<int> AddAsync(HotelCreateDto hotel)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                INSERT INTO ""Hotels"" (hotel_name, stars_count, address, price_per_night) 
                VALUES (@hotel_name, @stars_count, @address, @price_per_night) 
                RETURNING hotel_id;";

            return await db.QuerySingleAsync<int>(sql, hotel);
        }

        public async Task<Hotel?> GetByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM \"Hotels\" WHERE hotel_id = @id";

            return await db.QueryFirstOrDefaultAsync<Hotel>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(int id, HotelCreateDto hotel)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            string sql = @"
                UPDATE ""Hotels"" 
                SET hotel_name = @hotel_name, 
                    stars_count = @stars_count,
                    address = @address, 
                    price_per_night = @price_per_night 
                WHERE hotel_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new
            {
                id,
                hotel.hotel_name,
                hotel.stars_count,
                hotel.address,
                hotel.price_per_night
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            string sql = "DELETE FROM \"Hotels\" WHERE hotel_id = @id";

            var rowsAffected = await db.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Hotel>> GetPagedAndFilteredAsync(HotelQueryParameters parameters)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);

            // Начинаем собирать запрос. 1=1 - удобный трюк, чтобы легко приклеивать следующие AND
            var sqlBuilder = new StringBuilder("SELECT * FROM \"Hotels\" WHERE 1=1 ");
            var dynamicParameters = new DynamicParameters();

            // 1. Поиск по названию (используем ILIKE для поиска без учета регистра в PostgreSQL)
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                sqlBuilder.Append("AND hotel_name ILIKE @SearchTerm ");
                dynamicParameters.Add("SearchTerm", $"%{parameters.SearchTerm}%");
            }

            // 2. Фильтр по минимальному количеству звезд
            if (parameters.MinStars.HasValue)
            {
                sqlBuilder.Append("AND stars_count >= @MinStars ");
                dynamicParameters.Add("MinStars", parameters.MinStars.Value);
            }

            // 3. Фильтр по максимальной цене
            if (parameters.MaxPrice.HasValue)
            {
                sqlBuilder.Append("AND price_per_night <= @MaxPrice ");
                dynamicParameters.Add("MaxPrice", parameters.MaxPrice.Value);
            }

            // 4. Пагинация (LIMIT и OFFSET)
            // OFFSET вычисляет, сколько записей нужно пропустить
            sqlBuilder.Append("ORDER BY hotel_id ");
            sqlBuilder.Append("LIMIT @Limit OFFSET @Offset");

            dynamicParameters.Add("Limit", parameters.PageSize);
            dynamicParameters.Add("Offset", (parameters.PageNumber - 1) * parameters.PageSize);

            // Выполняем собранный запрос
            return await db.QueryAsync<Hotel>(sqlBuilder.ToString(), dynamicParameters);
        }
    }
}
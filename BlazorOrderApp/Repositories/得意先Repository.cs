using Dapper;
using Microsoft.Data.SqlClient;
using BlazorOrderApp.Models;

namespace BlazorOrderApp.Repositories
{
    public interface I得意先Repository
    {
        Task<IEnumerable<得意先Model>> GetAllAsync();
        Task<IEnumerable<得意先Model>> SearchAsync(string keyword);
        Task<得意先Model?> GetByIdAsync(string? 得意先ID);
        Task AddAsync(得意先Model model);
        Task UpdateAsync(得意先Model model);
        Task DeleteAsync(int 得意先ID);
    }

    public class 得意先Repository : I得意先Repository
    {
        private readonly string _connectionString;

        public 得意先Repository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        // 全件Select
        public async Task<IEnumerable<得意先Model>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);

            var dataSql = @"
                select 得意先ID, 得意先名, 電話番号, 備考
                  from 得意先
                 order by 得意先名
            ";
            var list = await conn.QueryAsync<得意先Model>(dataSql);

            return list;
        }

        // 検索
        public async Task<IEnumerable<得意先Model>> SearchAsync(string keyword)
        {
            using var conn = new SqlConnection(_connectionString);

            var dataSql = @"
                select top 10
                        得意先ID, 得意先名, 電話番号, 備考
                  from 得意先
                where ( 得意先名 like @keyword )
                 order by 得意先名
            ";
            var list = await conn.QueryAsync<得意先Model>(dataSql, new { keyword = $"%{keyword}%" });

            return list;
        }

        public async Task<得意先Model?> GetByIdAsync(string? str得意先ID)
        {
            if (!int.TryParse(str得意先ID, out int id))
            {
                id = -1;
            }

            using var conn = new SqlConnection(_connectionString);

            var sql = @"
            select 得意先ID, 得意先名, 電話番号, 備考
              from 得意先
             where 得意先ID = @id
        ";

            var item = await conn.QueryFirstOrDefaultAsync<得意先Model>(sql, new { id });
            return item;
        }

        // Insert
        public async Task AddAsync(得意先Model model)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                var sql = @"
                    insert into 得意先 (得意先名, 電話番号, 備考)
                    values (@得意先名, @電話番号, @備考)
                ";
                await conn.ExecuteAsync(sql, model, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // Update
        public async Task UpdateAsync(得意先Model model)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                var sql = @"
                    update 得意先 set
                        得意先名 = @得意先名,
                        電話番号 = @電話番号,
                        備考 = @備考
                    where 得意先ID = @得意先ID
                ";
                await conn.ExecuteAsync(sql, model, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // Delete
        public async Task DeleteAsync(int 得意先ID)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                await conn.ExecuteAsync(@"
                    delete from 得意先 
                    where 得意先ID = @得意先ID
                ", new { 得意先ID }, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
    }
}

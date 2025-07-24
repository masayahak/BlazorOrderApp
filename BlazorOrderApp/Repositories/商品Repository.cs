using BlazorOrderApp.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BlazorOrderApp.Repositories
{
    public interface I商品Repository
    {
        Task<List<商品Model>> GetAllAsync();
        Task<List<商品Model>> SearchAsync(string keyword);
        Task<商品Model?> GetByCodeAsync(string 商品コード);
        Task AddAsync(商品Model model);
        Task UpdateAsync(商品Model model);
        Task DeleteAsync(string 商品コード);
    }

    public class 商品Repository : I商品Repository
    {
        private readonly string _connectionString;

        public 商品Repository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        // 全件Select
        public async Task<List<商品Model>> GetAllAsync()
        {
            using var conn = new SqliteConnection(_connectionString);

            var dataSql = @"
                select 商品コード, 商品名, 単価, 備考
                  from 商品
                 order by 商品コード
            ";
            var list = await conn.QueryAsync<商品Model>(dataSql);

            return list.ToList();
        }

        // 検索
        public async Task<List<商品Model>> SearchAsync(string keyword)
        {
            using var conn = new SqliteConnection(_connectionString);

            var dataSql = @"
                select
                        商品コード, 商品名, 単価, 備考
                  from 商品
                where ( 商品コード like @keyword or 商品名 like @keyword)
                 order by 商品コード
                limit 10
            ";
            var list = await conn.QueryAsync<商品Model>(dataSql, new { keyword = $"%{keyword}%" });

            return list.ToList();
        }

        // 単一 Select
        public async Task<商品Model?> GetByCodeAsync(string 商品コード)
        {
            using var conn = new SqliteConnection(_connectionString);

            var sql = @"
            select 商品コード, 商品名, 単価, 備考
              from 商品
             where 商品コード = @商品コード
        ";

            var item = await conn.QueryFirstOrDefaultAsync<商品Model>(sql, new { 商品コード });
            return item;
        }

        // Insert
        public async Task AddAsync(商品Model model)
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                var sql = @"
                    insert into 商品 (商品コード, 商品名, 単価, 備考)
                    values (@商品コード, @商品名, @単価, @備考)
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
        public async Task UpdateAsync(商品Model model)
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                var sql = @"
                    update 商品 set
                        商品名 = @商品名,
                        単価 = @単価,
                        備考 = @備考
                    where 商品コード = @商品コード
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
        public async Task DeleteAsync(string 商品コード)
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                await conn.ExecuteAsync(@"
                    delete from 商品 
                    where 商品コード = @商品コード
                ", new { 商品コード }, tran);
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

using BlazorOrderApp.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BlazorOrderApp.Repositories
{
    public interface I受注Repository
    {
        Task<IEnumerable<受注Model>> GetAllAsync();
        Task<IEnumerable<受注Model>> SearchAsync(DateTime startDate, DateTime endDate, string keyword);
        Task<受注Model?> GetByIdAsync(int? 受注ID);
        Task AddAsync(受注Model model);
        Task UpdateAsync(受注Model model);
        Task DeleteAsync(int 受注ID);
    }

    public class 受注Repository : I受注Repository
    {
        private readonly string _connectionString;

        public 受注Repository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        // 全件
        public async Task<IEnumerable<受注Model>> GetAllAsync()
        {
            using var conn = new SqliteConnection(_connectionString);

            var dataSql = @"
                select 受注ID, 受注日, 得意先ID, 得意先名, 合計金額, 備考
                  from 受注
                 order by 受注日, 得意先名, 受注ID
            ";
            var list = await conn.QueryAsync<受注Model>(dataSql);

            return list;
        }

        // 検索
        public async Task<IEnumerable<受注Model>> SearchAsync(DateTime startDate, DateTime endDate, string keyword)
        {
            using var conn = new SqliteConnection(_connectionString);

            var dataSql = @"
                 with sub as (
                    select o.受注ID, o.受注日, o.得意先ID, o.得意先名, o.合計金額, o.備考
                      from 受注 o
                     where o.受注日 between @startDate and @endDate
                )
                select *
                from sub t
                where
                      @key = ''
                   or t.得意先名 like @key
                   or exists (
                         select 1 from 受注明細 d
                          where d.受注ID = t.受注ID
                            and (d.商品コード like @key or d.商品名 like @key)
                     )
                order by t.受注日, t.得意先名, t.受注ID
           ";
            var param = new
            {
                startDate,
                endDate,
                key = "%" + keyword + "%"
            };
            var list = await conn.QueryAsync<受注Model>(dataSql, param);
            return list;
        }

        // 単一 Select
        public async Task<受注Model?> GetByIdAsync(int? 受注ID)
        {
            if (受注ID == null) return null;

            using var conn = new SqliteConnection(_connectionString);

            var sql = @"
                select o.受注ID, o.受注日, o.得意先ID, o.得意先名, o.合計金額, o.備考,
                       d.明細ID, d.受注ID, d.商品コード, d.商品名, d.単価, d.数量
                  from 受注 o
                  left join 受注明細 d on o.受注ID = d.受注ID
                 where o.受注ID = @受注ID
            ";

            var lookup = new Dictionary<int, 受注Model>();

            await conn.QueryAsync<受注Model, 受注明細Model, 受注Model>(
                sql,
                (o, d) =>
                {
                    if (!lookup.TryGetValue(o.受注ID, out var order))
                    {
                        order = o;
                        order.明細一覧 = new List<受注明細Model>();
                        lookup.Add(order.受注ID, order);
                    }
                    if (d != null && d.明細ID != 0)
                    {
                        order.明細一覧.Add(d);
                    }
                    return order;
                },
                param: new { 受注ID },
                splitOn: "明細ID"
            );

            // 最初の1件（存在しなければnull）
            return lookup.Values.FirstOrDefault();
        }


        // Insert
        public async Task AddAsync(受注Model model)
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                // 合計金額の再計算（任意）
                model.合計金額 = model.明細一覧.Sum(x => x.単価 * x.数量);

                // 受注テーブル
                var sql1 = @"
                    insert into 受注 (受注日, 得意先ID, 得意先名, 合計金額, 備考)
                    values (@受注日, @得意先ID, @得意先名, @合計金額, @備考);
                    select last_insert_rowid();
                ";
                model.受注ID = (int)(await conn.ExecuteScalarAsync<long>(sql1, model, tran));

                // 受注明細テーブル
                var sql2 = @"
                    insert into 受注明細 (受注ID, 商品コード, 商品名, 単価, 数量)
                    values (@受注ID, @商品コード, @商品名, @単価, @数量)
                ";
                foreach (var 明細 in model.明細一覧)
                {
                    明細.受注ID = model.受注ID;
                    await conn.ExecuteAsync(sql2, 明細, tran);
                }

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // Update
        public async Task UpdateAsync(受注Model model)
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                var sql1 = @"
                    update 受注 set
                        受注日 = @受注日,
                        得意先ID = @得意先ID,
                        得意先名 = @得意先名,
                        合計金額 = @合計金額,
                        備考 = @備考
                    where 受注ID = @受注ID
                ";
                await conn.ExecuteAsync(sql1, model, tran);

                // 紐づく受注明細はいったん全て削除し、新しい明細を全部INSERT
                conn.Execute("delete from 受注明細 where 受注ID = @受注ID", new { model.受注ID }, tran);
                var sql2 = @"
                    insert into 受注明細 (受注ID, 商品コード, 商品名, 単価, 数量)
                    values (@受注ID, @商品コード, @商品名, @単価, @数量)
                ";
                foreach (var 明細 in model.明細一覧)
                {
                    明細.受注ID = model.受注ID;
                    await conn.ExecuteAsync(sql2, 明細, tran);
                }

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // Delete
        public async Task DeleteAsync(int 受注ID)
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                await conn.ExecuteAsync("delete from 受注明細 where 受注ID = @受注ID", new { 受注ID }, tran);
                await conn.ExecuteAsync("delete from 受注 where 受注ID = @受注ID", new { 受注ID }, tran);

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

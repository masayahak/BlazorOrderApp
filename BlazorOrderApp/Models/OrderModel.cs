using System.ComponentModel.DataAnnotations;

namespace BlazorOrderApp.Models
{
    public static class OrderModelConst
    {
        public const int MAX_受注明細_COUNT = 6;
        public static readonly string[] 明細一覧ValidationPath = new[] { "明細一覧" };
        public static readonly string[] 商品コードValidationPath = new[] { "商品コード" };
        public static readonly string[] 商品名ValidationPath = new[] { "商品名" };
        public static readonly string[] 単価ValidationPath = new[] { "単価" };
        public static readonly string[] 数量ValidationPath = new[] { "数量" };
    }

    public class OrderModel : IValidatableObject
    {
        public int 受注ID { get; set; }
        [Required(ErrorMessage = "受注日を入力してください")]
        public DateTime 受注日 { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "得意先を選択してください")]
        public int 得意先ID { get; set; }
        public string 得意先名 { get; set; } = string.Empty;
        public decimal 合計金額 { get; set; }
        public string? 備考 { get; set; }
        public int Version { get; set; }

        public List<OrderDetailModel> 明細一覧 { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 少なくとも1行は商品コードと数量が入力されていることを確認
            var validDetails = 明細一覧.Where(m => !string.IsNullOrWhiteSpace(m.商品コード) && m.数量 > 0).ToList();
            if (validDetails.Count == 0)
            {
                yield return new ValidationResult("少なくとも1つの明細に商品コードと数量を入力してください", OrderModelConst.明細一覧ValidationPath);
            }

            // 明細ごとのチェック（入力がある場合のみ）
            foreach (var (明細, i) in 明細一覧.Select((v, idx) => (v, idx + 1)))
            {
                if (!string.IsNullOrWhiteSpace(明細.商品コード))
                {
                    if (string.IsNullOrWhiteSpace(明細.商品名))
                        yield return new ValidationResult($"明細 {i} の商品名を入力してください", new[] { $"明細一覧[{i - 1}].{OrderModelConst.商品名ValidationPath[0]}" });

                    if (明細.単価 <= 0)
                        yield return new ValidationResult($"明細 {i} の単価は0より大きくしてください", new[] { $"明細一覧[{i - 1}].{OrderModelConst.単価ValidationPath[0]}" });

                    if (明細.数量 <= 0)
                        yield return new ValidationResult($"明細 {i} の数量は1以上を入力してください", new[] { $"明細一覧[{i - 1}].{OrderModelConst.数量ValidationPath[0]}" });
                }
                else if (明細.数量 > 0 || 明細.単価 > 0 || !string.IsNullOrWhiteSpace(明細.商品名))
                {
                    yield return new ValidationResult($"明細 {i} の商品コードを入力してください", new[] { $"明細一覧[{i - 1}].{OrderModelConst.商品コードValidationPath[0]}" });
                }
            }

            // 合計金額の最終チェック（算出後に 1円以上）
            var sum = 明細一覧.Sum(x => x.単価 * x.数量);
            if (sum <= 0)
                yield return new ValidationResult("合計金額は1円以上になるようにしてください",
                    new[] { nameof(合計金額) });
        }
    }

    public class OrderDetailModel
    {
        public int 明細ID { get; set; }
        public int 受注ID { get; set; }
        public string 商品コード { get; set; } = string.Empty;
        public string 商品名 { get; set; } = string.Empty;
        public decimal 単価 { get; set; }
        public int 数量 { get; set; }

        public decimal 金額 => 単価 * 数量;
    }
}
using System.ComponentModel.DataAnnotations;

namespace BlazorOrderApp.Models
{
    public static class OrderModelConst
    {
        public const int MAX_受注明細_COUNT = 10;
    }

    public class OrderModel : IValidatableObject
    {
        public int 受注ID { get; set; }
        [Required(ErrorMessage = "受注日を入力してください")]
        public DateTime 受注日 { get; set; }
        [Required(ErrorMessage = "得意先を入力してください")]
        public int 得意先ID { get; set; }
        [Required(ErrorMessage = "得意先を入力してください")]
        public string 得意先名 { get; set; } = string.Empty;
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "合計金額は1円以上になるようにしてください")]
        public decimal 合計金額 { get; set; }
        public string? 備考 { get; set; }
        public int Version { get; set; }

        public List<OrderDetailModel> 明細一覧 { get; set; } = new();

        // Blazor では親モデルしかValidationされない。親に子供のValidationを追加
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 明細行数のチェック
            if (明細一覧.Count < 1 || 明細一覧.Count > OrderModelConst.MAX_受注明細_COUNT)
                yield return new ValidationResult($"明細行は1～{OrderModelConst.MAX_受注明細_COUNT}行で入力してください", new[] { "明細一覧" });

            // 明細ごとの既存チェック
            foreach (var (明細, i) in 明細一覧.Select((v, idx) => (v, idx + 1)))
            {
                if (string.IsNullOrWhiteSpace(明細.商品コード) || string.IsNullOrWhiteSpace(明細.商品名) || 明細.単価 <= 0)
                    yield return new ValidationResult($"明細 {i} の商品コードを入力してください", new[] { $"明細一覧[{i - 1}].商品コード" });

                if (明細.数量 == 0)
                    yield return new ValidationResult($"明細 {i} の数量を入力してください", new[] { $"明細一覧[{i - 1}].数量" });

                if (明細.数量 < 0)
                    yield return new ValidationResult($"明細 {i} の数量は1以上を入力してください", new[] { $"明細一覧[{i - 1}].数量" });
            }
        }
    }

    public class OrderDetailModel
    {
        public int 明細ID { get; set; }
        public int 受注ID { get; set; }
        [Required(ErrorMessage = "商品コードを入力してください")]
        public string 商品コード { get; set; } = string.Empty;
        [Required]
        public string 商品名 { get; set; } = string.Empty;
        [Required]
        public decimal 単価 { get; set; }
        [Required(ErrorMessage = "数量を入力してください")]
        [Range(1, int.MaxValue, ErrorMessage = "数量は1以上を入力してください")]
        public int 数量 { get; set; }

        public decimal 金額 => 単価 * 数量;
    }
}

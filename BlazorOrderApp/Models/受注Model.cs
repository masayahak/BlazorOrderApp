using System.ComponentModel.DataAnnotations;

namespace BlazorOrderApp.Models
{
    public static class 受注ModelConst
    {
        public const int MAX_受注明細_COUNT = 10;
    }

    public class 受注Model : IValidatableObject
    {
        public int 受注ID { get; set; }
        public DateTime 受注日 { get; set; }
        [Required]
        public int 得意先ID { get; set; }
        [Required]
        public string 得意先名 { get; set; } = string.Empty;
        [Required]
        public decimal 合計金額 { get; set; }
        public string? 備考 { get; set; }

        public List<受注明細Model> 明細一覧 { get; set; } = new();

        // Blazor では親モデルしかValidationされない。親に子供のValidationを追加
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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

    public class 受注明細Model
    {
        public int 明細ID { get; set; }
        public int 受注ID { get; set; }
        [Required]
        public string 商品コード { get; set; } = string.Empty;
        [Required]
        public string 商品名 { get; set; } = string.Empty;
        [Required]
        public decimal 単価 { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "数量は1以上を入力してください。")]
        public int 数量 { get; set; }

        public decimal 金額 => 単価 * 数量;
    }
}

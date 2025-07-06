using System.ComponentModel.DataAnnotations;

namespace BlazorOrderApp.Models
{
    public static class 受注ModelConst
    {
        public const int MAX_受注明細_COUNT = 10;
    }

    public class 受注Model
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
        [Range(0, int.MaxValue, ErrorMessage = "数量は0以上を入力してください。")]
        public int 数量 { get; set; }

        public decimal 金額 => 単価 * 数量;
    }
}

using System.ComponentModel.DataAnnotations;

namespace BlazorOrderApp.Models
{
    public class ProductModel
    {
        [Required(ErrorMessage = "商品コードを入力してください")]
        public string 商品コード { get; set; } = string.Empty;
        [Required(ErrorMessage = "商品名を入力してください")]
        public string 商品名 { get; set; } = string.Empty;
        [Required(ErrorMessage = "単価を入力してください")]
        [Range(1, double.MaxValue, ErrorMessage = "単価は1円以上にしてください")]
        public decimal? 単価 { get; set; }
        public string? 備考 { get; set; }
        public int Version { get; set; }
    }
}

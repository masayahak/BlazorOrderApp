namespace BlazorOrderApp.Models
{
    public class 商品Model
    {
        public string 商品コード { get; set; } = string.Empty;
        public string 商品名 { get; set; } = string.Empty;
        public decimal 単価 { get; set; }
        public string? 備考 { get; set; }
    }
}

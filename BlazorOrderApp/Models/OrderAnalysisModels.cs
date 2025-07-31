namespace BlazorOrderApp.Models
{
    public class 日別受注金額Model
    {
        public DateTime 受注日 { get; set; }
        public decimal 受注金額 { get; set; }

        // 受注日から常に取得（MM/dd 形式など）
        public string 受注日ラベル => 受注日.ToString("MM/dd");
    }

    public class 得意先別受注金額Model
    {
        public int 得意先ID { get; set; }
        public string 得意先名 { get; set; } = string.Empty;
        public decimal 受注金額 { get; set; }
    }

    public class 商品別受注金額Model
    {
        public string 商品コード { get; set; } = string.Empty;
        public string 商品名 { get; set; } = string.Empty;
        public decimal 受注金額 { get; set; }

        public string 商品コード名称 => $"{商品コード} {商品名}";
    }
}

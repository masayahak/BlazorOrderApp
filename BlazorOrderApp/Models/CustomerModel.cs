using System.ComponentModel.DataAnnotations;

namespace BlazorOrderApp.Models
{
    public class CustomerModel
    {
        public int 得意先ID { get; set; }
        [Required(ErrorMessage = "得意先名を入力してください")]
        public string 得意先名 { get; set; } = string.Empty;
        [Required(ErrorMessage = "電話番号を入力してください")]
        public string 電話番号 { get; set; } = string.Empty;
        public string? 備考 { get; set; }
        public int Version { get; set; }
    }
}

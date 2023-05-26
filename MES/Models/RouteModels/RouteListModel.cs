using System.ComponentModel;

namespace MES.Models.RouteModels
{
    public class RouteListModel
    {
        [DisplayName("Referans Kodu")]
        public int ReferenceId { get; set; } = default;

        [DisplayName("Kod")]
        public string? Code { get; set; } = string.Empty;

        [DisplayName("Açıklama")]
        public string? Description { get; set; } = string.Empty;

        [DisplayName("Kart Türü")]
        public short CardType { get; set; } = default;

        [DisplayName("Durumu")]
        public bool Status {get;set;} = default;


    }
}
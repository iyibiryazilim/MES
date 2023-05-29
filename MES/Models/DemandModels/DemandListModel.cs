using System.ComponentModel;

namespace MES.Models.DemandModels;

public class DemandListModel
{
    [DisplayName("Referans Kodu")]
    public int ReferenceId { get; set; } = default;

    [DisplayName("Fiş Numarası")]
    public string? FicheNo { get; set; } = default;

    [DisplayName("Tarihi")]
    public DateTimeOffset? Date { get; set; } = DateTime.Now;

    [DisplayName("Durumu")]
    public short Status { get; set; } = default;

    [DisplayName("Ambar Numarası")]
    public int? WarehouseNo { get; set; } = default;

    [DisplayName("Ürün Referans Kodu")]
    public int ProductReferenceId { get; set; } = default;

    [DisplayName("Ürün Kodu")]
    public string? ProductCode { get; set; } = default;

    [DisplayName("Ürün Adı")]
    public string? ProductName { get; set; } = default;

    [DisplayName("Talep Referans Kodu")]
    public int DemandLineReferenceId { get; set; } = default;

    [DisplayName("Talep Miktarı")]
    public double DemandLineAmounth { get; set; } = default;

    [DisplayName("İptal edilen miktar")]
    public double DemandLineCancelAmounth { get; set; } = default;

    [DisplayName("Yapılan Miktar")]
    public double DemandLineMeetAmounth { get; set; } = default;

    [DisplayName("Subunitset Kodu")]
    public string? SubunitsetCode { get; set; } = default;

    [DisplayName("Unitset Kodu")]
    public string? UnitsetCode { get; set; } = default;



}


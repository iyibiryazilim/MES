using System.ComponentModel;

namespace MES.Models.StopTransactionModels;

public class StopTransactionListModel
{
    [DisplayName("Referans Kodu")]
    public int ReferenceId { get; set; }

    [DisplayName("Durma Tarihi")]
    public DateTimeOffset StopDate { get; set; } = default;

    [DisplayName("Durma Saati")]
    public TimeSpan StopTime { get; set; } = default;

    [DisplayName("Başlama Tarihi")]
    public DateTimeOffset StartDate { get; set; } = default;

    [DisplayName("Başlama Saati")]
    public TimeSpan StartTime { get; set; } = default;

    [DisplayName("Durma Süresi")]
    public double StopDuration { get; set; } = default;

    [DisplayName("Operasyon Referans Kodu")]
    public int OperationReferenceId { get; set; }

    [DisplayName("Operasyon Kodu")]
    public string? OperationCode { get; set; }

    [DisplayName("Operasyon Adı")]
    public string? OperationName { get; set; }

    [DisplayName("Üretim Emri Referans Kodu")]
    public int ProductionOrderReferenceId { get; set; }

    [DisplayName("Üretim Emri Kodu")]
    public string? ProductionOrderCode { get; set; }

    [DisplayName("İş Emri Kodu")]
    public int WorkOrderReferenceId { get; set; }

    [DisplayName("İş Emri Durumu")]
    public short WorkOrderStatus { get; set; }

    [DisplayName("İş İstasyonu Adı")]
    public string? WorkstationName { get; set; } = string.Empty;

    [DisplayName("İş İstasyonu Kodu")]
    public string? WorkstationCode { get; set; } = string.Empty;

    [DisplayName("İş İstasyonu Refereans Kodu")]
    public int WorkstationReferenceId { get; set; } = default;

    [DisplayName("Durma Nedeni Adı")]
    public string? StopCauseName { get; set; } = string.Empty;

    [DisplayName("Durma Nedeni Kodu")]
    public string? StopCauseCode { get; set; } = string.Empty;

    [DisplayName("Durma Nedeni Kodu")]
    public int StopCauseReferenceId { get; set; } = default;
}


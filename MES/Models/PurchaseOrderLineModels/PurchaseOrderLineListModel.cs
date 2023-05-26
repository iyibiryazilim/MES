namespace MES.Models.PurchaseOrderLineModels
{
    public class PurchaseOrderLineListModel
    {
        public int ReferenceId { get; set; }

        public short TransectionType { get; set; } = default;

        public int? CurrentReferenceId { get; set; } = default;

        public string? CurrentCode { get; set; } = string.Empty;

        public string? CurrentName { get; set; } = string.Empty;

        public int? ProductReferenceId { get; set; } = default;

        public string? ProductCode { get; set; } = string.Empty;

        public string? ProductName { get; set; } = string.Empty;

        public string? Unitset { get; set; } = string.Empty;

        public string? SubUnitset { get; set; } = string.Empty;

        public double Quantity { get; set; } = default;

        public double ShippedQuantity { get; set; } = default;

        public double WaitingQuantity { get; set; } = default;

        public int? WarehouseReferenceId { get; set; } = default;

        public int? WarehouseNo { get; set; } = default;

        public string? WarehouseName { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public string? OrderCode { get; set; } = string.Empty;

        public DateTime? OrderDate { get; set; } = default;
    }
}
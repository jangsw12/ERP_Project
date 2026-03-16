using ERP_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Purchasing
{
    public class PurchaseOrderM : ViewModelBase
    {
        public int PurchaseOrderMId { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Remark { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public byte[]? rowversion { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;

        private byte _status;

        public byte Status
        {
            get { return _status; }
            set { 
                _status = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public string StatusText
        {
            get
            {
                return Status switch
                {
                    0 => "임시",
                    1 => "확정",
                    2 => "취소",
                    _ => "알수없음"
                };
            }
        }
    }
}
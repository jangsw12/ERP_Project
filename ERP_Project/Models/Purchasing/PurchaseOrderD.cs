using ERP_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Purchasing
{
    public class PurchaseOrderD : ViewModelBase
    {
        public int PurchaseOrderDId { get; set; }
        public int PurchaseOrderMId { get; set; }

        private int _itemId;

        public int ItemId
        {
            get { return _itemId; }
            set { 
                _itemId = value;
                OnPropertyChanged();
            }
        }

        private string _itemName;

        public string ItemName
        {
            get { return _itemName; }
            set { 
                _itemName = value;
                OnPropertyChanged();
            }
        }

        private decimal _qty;

        public decimal Qty
        {
            get { return _qty; }
            set {
                _qty = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Amount));
            }
        }

        private decimal _unitPrice;

        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set { 
                _unitPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Amount));
            }
        }

        public decimal Amount => Qty * UnitPrice;

        public string? Remark { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public string ItemCode { get; set; } = string.Empty;
    }
}
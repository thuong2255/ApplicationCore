using System.Collections.Generic;
using SystemCore.Data.Enums;
using SystemCore.Service.ViewModels.Product;
using SystemCore.Utilities.Dtos;

namespace SystemCore.Service.Interfaces
{
    public interface IBillService
    {
        void Create(BillViewModel billViewModel);

        void Update(BillViewModel billViewModel);

        PagedResult<BillViewModel> GetAllPaging(string startDate, string endDate, string keyword, int page, int pageSize);

        BillViewModel GetDetail(int billId);

        BillDetailViewModel CreateBillDetail(BillDetailViewModel billDetailViewModel);

        void UpdateStatus(int orderId, BillStatus status);

        List<BillDetailViewModel> GetBillDetails(int billId);

        List<ColorViewModel> GetColors();

        List<SizeViewModel> GetSizes();

        void Save();
    }
}
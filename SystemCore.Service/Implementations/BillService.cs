using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SystemCore.Data.EF.IRepositories;
using SystemCore.Data.Entities;
using SystemCore.Data.Enums;
using SystemCore.Infrastructure.Interfaces;
using SystemCore.Service.Interfaces;
using SystemCore.Service.ViewModels.Product;
using SystemCore.Utilities.Dtos;

namespace SystemCore.Service.Implementations
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly IBillDetailRepository _billDetailRepository;
        private readonly IColorRepository _colorRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BillService(IBillRepository billRepository,
            IBillDetailRepository billDetailRepository,
            IColorRepository colorRepository,
            IProductRepository productRepository,
            ISizeRepository sizeRepository,
            IUnitOfWork unitOfWork)
        {
            _billRepository = billRepository;
            _billDetailRepository = billDetailRepository;
            _colorRepository = colorRepository;
            _productRepository = productRepository;
            _sizeRepository = sizeRepository;
            _unitOfWork = unitOfWork;
        }


        public void Create(BillViewModel billViewModel)
        {
            var order = Mapper.Map<BillViewModel, Bill>(billViewModel);
            var orderDetails = Mapper.Map<List<BillDetailViewModel>, List<BillDetail>>(billViewModel.BillDetails);
            foreach(var detail in orderDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
            }
            order.BillDetails = orderDetails;
            _billRepository.Add(order);
        }

        public BillDetailViewModel CreateBillDetail(BillDetailViewModel billDetailViewModel)
        {
            var billDetail = Mapper.Map<BillDetailViewModel, BillDetail>(billDetailViewModel);
            _billDetailRepository.Add(billDetail);
            return billDetailViewModel;
        }

        public PagedResult<BillViewModel> GetAllPaging(string startDate, string endDate, string keyword, int page, int pageSize)
        {
            var query = _billRepository.FindAll();

            if(!string.IsNullOrEmpty(startDate))
            {
                DateTime start = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.DateCreated >= start);
            }

            if(!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.CustomerName.Contains(keyword) || x.CustomerMobile.Contains(keyword));
            }

            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<BillViewModel>()
                .ToList();

            return new PagedResult<BillViewModel>
            {
                CurrentPage = page,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public List<BillDetailViewModel> GetBillDetails(int billId)
        {
            return _billDetailRepository.FindAll(x => x.BillId == billId, c => c.Bill, c => c.Color, c => c.Size, c => c.Product)
                .ProjectTo<BillDetailViewModel>().ToList();
        }

        public List<ColorViewModel> GetColors()
        {
            return _colorRepository.FindAll().ProjectTo<ColorViewModel>().ToList();
        }

        public BillViewModel GetDetail(int billId)
        {
            var bill = _billRepository.FindById(billId);
            var billVm = Mapper.Map<Bill, BillViewModel>(bill);
            var billDetailVm = _billDetailRepository.FindAll(x => x.BillId == billId).ProjectTo<BillDetailViewModel>().ToList();
            billVm.BillDetails = billDetailVm;
            return billVm;
        }

        public List<SizeViewModel> GetSizes()
        {
            return _sizeRepository.FindAll().ProjectTo<SizeViewModel>().ToList();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(BillViewModel billViewModel)
        {
            //Mapping to order domain
            var order = Mapper.Map<BillViewModel, Bill>(billViewModel);

            //Get order Detail
            var newDetails = order.BillDetails;

            //new details added
            var addedDetails = newDetails.Where(x => x.Id == 0).ToList();

            //get updated details
            var updatedDetails = newDetails.Where(x => x.Id != 0).ToList();

            //Existed details
            var existedDetails = _billDetailRepository.FindAll(x => x.BillId == billViewModel.Id);

            //Clear db
            order.BillDetails.Clear();

            foreach (var detail in updatedDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
                _billDetailRepository.Update(detail);
            }

            foreach (var detail in addedDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
                _billDetailRepository.Add(detail);
            }

            _billDetailRepository.RemoveMulti(existedDetails.Except(updatedDetails).ToList());

            _billRepository.Update(order);
        }

        public void UpdateStatus(int orderId, BillStatus status)
        {
            var order = _billRepository.FindById(orderId);
            order.BillStatus = status;
            _billRepository.Update(order);
        }
    }
}

var BillController = function () {
    var cacheObj = {
        products: [],
        colors: [],
        sizes: [],
        paymentMethods: [],
        billStatus: []
    };

    this.init = function () {
        $.when(loadBillStatus(),
            loadPaymentMethod(),
            loadColors(),
            loadSizes(),
            loadProducts()
        ).done(function () {
            loadData(true);
        });
        registerEvents();
    };

    var registerEvents = function () {
        $('#txtFromDate, #txtToDate').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
        });

        /*Init validation*/
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'en',
            rules: {
                txtCustomerName: { required: true },
                txtCustomerAddress: { required: true },
                txtCustomerMobile: { required: true },
                txtCustomerMessage: { required: true },
                ddlBillStatus: { required: true }
            }
        });

        $('#btn-create').on('click', function () {
            resetFormMaintainance();
            $('#modal-detail').modal('show');
        });

        $("#btn-search").on('click', function () {
            loadData();
        });

        $("#ddl-show-page").on('change', function () {
            common.configs.pageSize = $(this).val();
            common.configs.pageIndex = 1;
            loadData(true);
        });

        $("#btnExport").on('click', function () {
            var that = $('#hidId').val();
            $.ajax({
                type: "POST",
                url: "/Admin/Bill/ExportExcel",
                data: { billId: that },
                success: function (response) {
                    window.location.href = response;
                }
            });
        });


        $('body').on('click', '.btn-view', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $.ajax({
                type: "GET",
                url: "/Admin/Bill/GetById",
                data: { id: that },
                success: function (response) {
                    var data = response;
                    $('#hidId').val(data.Id);
                    $('#dateCreated').val(data.DateCreated);
                    $('#txtCustomerName').val(data.CustomerName);

                    $('#txtCustomerAddress').val(data.CustomerAddress);
                    $('#txtCustomerMobile').val(data.CustomerMobile);
                    $('#txtCustomerMessage').val(data.CustomerMessage);
                    $('#ddlPaymentMethod').val(data.PaymentMethod);
                    $('#ddlCustomerId').val(data.CustomerId);
                    $('#ddlBillStatus').val(data.BillStatus);

                    var billDetails = data.BillDetails;
                    if (data.BillDetails != null && data.BillDetails.length > 0) {
                        var render = '';
                        var templateDetails = $('#template-table-bill-details').html();

                        $.each(billDetails, function (i, item) {
                            var products = getProductOptions(item.ProductId);
                            var colors = getColorOptions(item.ColorId);
                            var sizes = getSizeOptions(item.SizeId);

                            render += Mustache.render(templateDetails,
                                {
                                    Id: item.Id,
                                    Products: products,
                                    Colors: colors,
                                    Sizes: sizes,
                                    Quantity: item.Quantity
                                });
                        });
                        $('#tbl-bill-details').html(render);
                    }
                    $('#modal-detail').modal('show');

                },
                error: function (e) {
                    common.notify('Has an error in progress', 'error');
                }
            });
        });


        $('#btnAddDetail').on('click', function () {
            var template = $('#template-table-bill-details').html();
            var products = getProductOptions(null);
            var colors = getColorOptions(null);
            var sizes = getSizeOptions(null);
            var render = Mustache.render(template, {
                Id: 0,
                Products: products,
                Colors: colors,
                Sizes: sizes,
                Quantity: 0,
                Total: 0
            });
            $('#tbl-bill-details').append(render);
        });

        $('body').on('click', '.btn-delete-detail', function () {
            $(this).parent().parent().remove();
        });

        $('#btnSave').on('click', function () {
            if ($('#frmMaintainance').valid()) {
                var id = $('#hidId').val();
                var customerName = $('#txtCustomerName').val();
                var customerAddress = $('#txtCustomerAddress').val();
                var customerId = $('#ddlCustomerId').val();
                var customerMobile = $('#txtCustomerMobile').val();
                var customerMessage = $('#txtCustomerMessage').val();
                var paymentMethod = $('#ddlPaymentMethod').val();
                var billStatus = $('#ddlBillStatus').val();
                var dateCreated = $('#dateCreated').val();

                var billDetails = [];
                $.each($('#tbl-bill-details tr'), function (i, item) {
                    billDetails.push({
                        Id: $(item).data('id'),
                        ProductId: $(item).find('select.ddlProductId').first().val(),
                        Quantity: $(item).find('input.txtQuantity').first().val(),
                        ColorId: $(item).find('select.ddlColorId').first().val(),
                        SizeId: $(item).find('select.ddlSizeId').first().val(),
                        BillId: id
                    });
                });

                $.ajax({
                    type: "POST",
                    url: "/Admin/Bill/SaveEntity",
                    data: {
                        Id: id,
                        BillStatus: billStatus,
                        CustomerAddress: customerAddress,
                        CustomerId: customerId,
                        CustomerMessage: customerMessage,
                        CustomerMobile: customerMobile,
                        CustomerName: customerName,
                        PaymentMethod: paymentMethod,
                        Status: 1,
                        BillDetails: billDetails,
                        DateCreated: dateCreated
                    },
                    dataType: "json",
                    beforeSend: function () {
                        common.startLoading();
                    },
                    success: function (response) {
                        common.notify('Save order successful', 'success');
                        $('#modal-detail').modal('hide');
                        resetFormMaintainance();

                        common.stopLoading();
                        loadData(true);
                    },
                    error: function () {
                        common.notify('Has an error in progress', 'error');
                        common.stopLoading();
                    }
                });
            }
        });
    };



    var loadData = function (isPageChanged) {
        $.ajax({
            type: 'GET',
            url: '/Admin/Bill/GetAllPaging',
            data: {
                startDate: $('#txtFromDate').val(),
                endDate: $('#txtToDate').val(),
                keyword: $('#txtSearchKeyword').val(),
                page: common.configs.pageIndex,
                pageSize: common.configs.pageSize
            },
            dataType: 'json',
            success: function (response) {
                var template = $('#table-template').html();
                var render = '';
                if (response.RowCount > 0) {
                    $.each(response.Results, function (i, item) {
                        render += Mustache.render(template, {
                            CustomerName: item.CustomerName,
                            Id: item.Id,
                            PaymentMethod: getPaymentMethodName(item.PaymentMethod),
                            DateCreated: common.dateTimeFormatJson(item.DateCreated),
                            BillStatus: getBillStatusName(item.BillStatus)
                        });
                    });
                    $("#lbl-total-records").text(response.RowCount);
                    if (render != undefined) {
                        $('#tbl-content').html(render);
                    }
                    wrapPaging(response.RowCount, function () {
                        loadData();
                    }, isPageChanged);
                } else {
                    $("#lbl-total-records").text('0');
                    $('#tbl-content').html('');
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    };

    var loadPaymentMethod = function () {
        return $.ajax({
            type: 'GET',
            url: '/Admin/Bill/GetPaymentMethod',
            dataType: 'json',
            success: function (response) {
                cacheObj.paymentMethods = response;
                var render = ``;
                response.forEach(function (item) {
                    render += `<option value = "${item.Value}">${item.Name}</option>`;
                });
                $('#ddlPaymentMethod').html(render);
            }
        });
    };

    var loadBillStatus = function () {
        return $.ajax({
            type: 'GET',
            url: '/Admin/Bill/GetBillStatus',
            dataType: 'json',
            success: function (response) {
                cacheObj.billStatus = response;
                var render = ``;
                response.forEach(function (item) {
                    render += `<option value = "${item.Value}">${item.Name}</option>`;
                });
                $('#ddlBillStatus').html(render);
            }
        });
    };

    var loadProducts = function () {
        return $.ajax({
            type: 'GET',
            url: '/Admin/Product/GetAll',
            dataType: 'json',
            success: function (response) {
                cacheObj.products = response;
            },
            error: function () {
                console.log('Error LoadProduct');
            }
        });
    };

    var loadColors = function () {
        return $.ajax({
            type: 'GET',
            url: '/Admin/Bill/GetColors',
            dataType: 'json',
            success: function (response) {
                cacheObj.colors = response;
            },
            error: function () {
                console.log('Error LoadColors');
            }
        });
    };

    var loadSizes = function () {
        return $.ajax({
            type: "GET",
            url: "/Admin/Bill/GetSizes",
            dataType: "json",
            success: function (response) {
                cacheObj.sizes = response;
            },
            error: function () {
                console.log('Error loadSizes');
            }
        });
    };

    var getProductOptions = function (selectedId) {
        var render = `<select class = 'form-control ddlProductId'>`;
        cacheObj.products.forEach(function (item) {
            if (selectedId === item.Id)
                render += `<option value = '${item.Id}' selected='select'>${item.Name}</option>`;
            else
                render += `<option value = '${item.Id}'>${item.Name}</option>`;
        });
        render += `</select>`;
        return render;
    };

    var getColorOptions = function (selectedId) {
        var render = `<select class = 'form-control ddlColorId'>`;
        cacheObj.colors.forEach(function (item) {
            if (selectedId === item.Id)
                render += `<option value = '${item.Id}' selected='select'>${item.Name}</option>`;
            else
                render += `<option value = '${item.Id}'>${item.Name}</option>`;
        });
        render += `</select>`;
        return render;
    };

    var getSizeOptions = function (selectedId) {
        var render = `<select class = 'form-control ddlSizeId'>`;
        cacheObj.sizes.forEach(function (item) {
            if (selectedId === item.Id)
                render += `<option value = '${item.Id}' selected='select'>${item.Name}</option>`;
            else
                render += `<option value = '${item.Id}'>${item.Name}</option>`;
        });
        render += `</select>`;
        return render;
    };

    var getPaymentMethodName = function (paymentMethod) {
        var method = (cacheObj.paymentMethods).filter(function (item) {
            return item.Value == paymentMethod;
        });
        return method[0].Name;
    };

    var getBillStatusName = function (status) {
        var status = (cacheObj.billStatus).filter(function (item) {
            return item.Value == status;
        });
        return status[0].Name;
    };

    var resetFormMaintainance = function () {
        $('#hidId').val(0);
        $('#txtCustomerName').val('');

        $('#txtCustomerAddress').val('');
        $('#txtCustomerMobile').val('');
        $('#txtCustomerMessage').val('');
        $('#ddlPaymentMethod').val('');
        $('#ddlCustomerId').val('');
        $('#ddlBillStatus').val('');
        $('#tbl-bill-details').html('');
    };

    var wrapPaging = function (recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / common.configs.pageSize);
        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }
        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            totalPages: totalsize,
            visiblePages: 7,
            first: 'Đầu',
            prev: 'Trước',
            next: 'Tiếp',
            last: 'Cuối',
            onPageClick: function (event, p) {
                common.configs.pageIndex = p;
                setTimeout(callBack(), 200);
            }
        });
    };
};
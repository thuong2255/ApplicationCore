var productController = function () {
    this.init = function () {
        loadProductCategory();
        loadData();
        registerEvent();
    };


    function registerEvent() {
        $('#ddlShowPage').on('change', function () {
            common.configs.pageSize = $(this).val();
            common.configs.pageIndex = 1;
            loadData(true);
        });

        $('#btnSearch').on('click', function (e) {
            loadData(true);
        });
    };

    function loadProductCategory() {
        var render = '<option value = "">-- Chọn danh mục sản phẩm --</option>';
        $.ajax({
            type: 'GET',
            url: '/admin/product/getproductcategory',
            dataType: 'json',
            success: function (response) {
                response.forEach(function (item) {
                    render += `<option value = "${item.Id}">${item.Name}</option>`;
                });
                $('#listProductCategory').html(render);
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    function loadData(isPageChanged) {
        var template = $('#table-template').html();
        var render = "";
        $.ajax({
            type: 'GET',
            data: {
                productCategoryId: $('#listProductCategory').val(),
                keyword: $('#txtKeyword').val(),
                page: common.configs.pageIndex,
                pageSize: common.configs.pageSize
            },
            url: '/admin/product/GetAllPaging',
            dataType: 'json',
            success: function (response) {
                response.Results.forEach(function (item) {
                    render += Mustache.render(template, {
                        Id: item.Id,
                        Name: item.Name,
                        Image: item.Image == null ? '<img src="/Admin/images/user.png" width=25' : '<img src="' + item.Image + '" width=25 />',
                        CategoryName: item.ProductCategory.Name,
                        Price: common.formatNumber(item.Price, 0),
                        CreatedDate: common.dateTimeFormatJson(item.DateCreated),
                        Status: common.getStatus(item.Status)
                    });

                });
                if (render !== '') {
                    $('#tbl-content').html(render);
                }

                $('#lblTotalRecords').text(response.RowCount);

                wrapPaging(response.RowCount, function () { loadData() }, isPageChanged)

            },
            error: function (err) {
                console.log(err);
            }
        });
    };
    function wrapPaging(totalRow, callBack, changePageSize) {
        var totalPage = Math.ceil(totalRow / common.configs.pageSize);

        //Unbind pagination if it existed or click change pageSize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }

        $('#paginationUL').twbsPagination({
            totalPages: totalPage,
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
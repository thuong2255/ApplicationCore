var productController = function () {
    this.init = function () {
        loadProductCategory();
        loadData();
        registerEvent();
    };


    function registerEvent() {

        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'eng',
            rules: {
                txtNameM: { required: true },
                ddlCategoryIdM: { required: true },
                txtPriceM: {
                    required: true,
                    number: true
                }
            }
        });

        $('#ddlShowPage').on('change', function () {
            common.configs.pageSize = $(this).val();
            common.configs.pageIndex = 1;
            loadData(true);
        });

        $('#btnSearch').on('click', function (e) {
            loadData(true);
        });

        $('#btnCreate').off('click').on('click', function () {
            resetForm();
            initTreeDropDownCategory();
            $('#productModal').modal('show');
        });

        $('body').on('click', '.btnDelete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            common.confirm('Bạn có chắc chắn xóa?', function () {
                $.ajax({
                    type: 'POST',
                    url: '/Admin/Product/Delete',
                    data: {
                        id: id
                    },
                    beforeSend: function () {
                        common.startLoading();
                    },
                    success: function (response) {
                        common.notify('Xóa thành công', 'success');
                        loadData(true);
                        common.stopLoading();
                    },
                    error: function (err) {
                        common.stopLoading();
                        common.notify('Xóa không thành công', 'error');
                    }
                });
            });
        });

        $('body').on('click', '.btnEdit', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            $.ajax({
                type: 'GET',
                url: '/Admin/Product/GetById',
                data: {
                    id: id
                },
                beforeSend: function () {
                    common.startLoading();
                },
                success: function (response) {
                    var data = response;
                    $('#hidIdM').val(data.Id);
                    $('#txtNameM').val(data.Name);
                    initTreeDropDownCategory(data.CategoryId);

                    $('#txtDescM').val(data.Description);
                    $('#txtUnitM').val(data.Unit);

                    $('#txtPriceM').val(data.Price);
                    $('#txtOriginalPriceM').val(data.OriginalPrice);
                    $('#txtPromotionPriceM').val(data.PromotionPrice);

                    $('#txtDateCreated').val(data.DateCreated);

                    // $('#txtImageM').val(data.ThumbnailImage);

                    $('#txtTagM').val(data.Tags);
                    $('#txtMetakeywordM').val(data.SeoKeywords);
                    $('#txtMetaDescriptionM').val(data.SeoDescription);
                    $('#txtSeoPageTitleM').val(data.SeoPageTitle);
                    $('#txtSeoAliasM').val(data.SeoAlias);

                    //CKEDITOR.instances.txtContentM.setData(data.Content);
                    $('#ckStatusM').prop('checked', data.Status == 1);
                    $('#ckHotM').prop('checked', data.HotFlag);
                    $('#ckShowHomeM').prop('checked', data.HomeFlag);

                    $('#productModal').modal('show');
                    common.stopLoading();
                },
                error: function (err) {
                    common.notify('Có lỗi xảy ra', 'error');
                    common.stopLoading();
                }
            });
        });

        $('#btnSave').off('click').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();

                $.ajax({
                    type: 'POST',
                    url: '/Admin/Product/SaveEntity',
                    dataType: 'json',
                    data: {
                        Id: $('#hidIdM').val(),
                        Name: $('#txtNameM').val(),
                        CategoryId: $('#ddlCategoryIdM').combotree('getValue'),
                        Description: $('#txtDescM').val(),
                        Unit: $('#txtUnitM').val(),
                        Price: $('#txtPriceM').val(),
                        OriginalPrice: $('#txtOriginalPriceM').val(),
                        PromotionPrice: $('#txtPromotionPriceM').val(),

                        DateCreated: $('#txtDateCreated').val(),

                        //$('#txtImageM').val(''),
                        Tags: $('#txtTagM').val(),
                        SeoKeywords: $('#txtMetakeywordM').val(),
                        SeoDescription: $('#txtMetaDescriptionM').val(),
                        SeoPageTitle: $('#txtSeoPageTitleM').val(),
                        SeoAlias: $('#txtSeoAliasM').val(),
                        //CKEDITOR.instances.txtContentM.setData(''),
                        Status: $('#ckStatusM').prop('checked') == true ? 1 : 0,
                        HotFlag: $('#ckHotM').prop('checked'),
                        HomeFlag: $('#ckShowHomeM').prop('checked'),
                        Content: ''
                    },
                    beforeSend: function () {
                        common.startLoading();
                    },

                    success: function (response) {
                        common.notify('Cập nhật thành công', 'success');
                        $('#productModal').modal('hide');
                        resetForm();
                        common.stopLoading();
                        loadData(true);
                        common.stopLoading();

                    },
                    error: function (err) {
                        console.log(err);
                    }
                });
            }
        });
    };

    function loadProductCategory() {
        var render = '<option value = "">--Product Category--</option>';
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

    function initTreeDropDownCategory(selectedId) {
        $.ajax({
            type: 'GET',
            url: '/admin/productCategory/getproductcategory',
            dataType: 'json',
            success: function (response) {
                var data = [];
                response.forEach(function (item) {
                    data.push({
                        id: item.Id,
                        text: item.Name,
                        parentId: item.ParentId,
                        sortOrder: item.SortOrder
                    });
                    var dataTree = common.unflattern(data);
                    $('#ddlCategoryIdM').combotree({
                        data: dataTree
                    });
                    if (selectedId != undefined) {
                        $('#ddlCategoryIdM').combotree('setValue', selectedId);
                    }
                });
            }
        });
    };

    function resetForm() {
        $('#hidIdM').val(0);
        $('#txtNameM').val('');
        initTreeDropDownCategory('');

        $('#txtDescM').val('');
        $('#txtUnitM').val('');

        $('#txtPriceM').val('0');
        $('#txtOriginalPriceM').val('');
        $('#txtPromotionPriceM').val('');

        //$('#txtImageM').val('');

        $('#txtTagM').val('');
        $('#txtMetakeywordM').val('');
        $('#txtMetaDescriptionM').val('');
        $('#txtSeoPageTitleM').val('');
        $('#txtSeoAliasM').val('');

        //CKEDITOR.instances.txtContentM.setData('');
        $('#ckStatusM').prop('checked', true);
        $('#ckHotM').prop('checked', false);
        $('#ckShowHomeM').prop('checked', false);

    }

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
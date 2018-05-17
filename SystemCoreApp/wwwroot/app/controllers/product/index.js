var productController = function () {
    this.init = function () {
        loadProductCategory();
        loadData();
        registerEvent();
        configCkEditor();
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

                    $('#txtImage').val(data.Image);

                    $('#txtTagM').val(data.Tags);
                    $('#txtMetakeywordM').val(data.SeoKeywords);
                    $('#txtMetaDescriptionM').val(data.SeoDescription);
                    $('#txtSeoPageTitleM').val(data.SeoPageTitle);
                    $('#txtSeoAliasM').val(data.SeoAlias);

                    CKEDITOR.instances.txtContent.setData(data.Content);
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
                        Image: $('#txtImage').val(),
                        Tags: $('#txtTagM').val(),
                        SeoKeywords: $('#txtMetakeywordM').val(),
                        SeoDescription: $('#txtMetaDescriptionM').val(),
                        SeoPageTitle: $('#txtSeoPageTitleM').val(),
                        SeoAlias: $('#txtSeoAliasM').val(),
                        Content: CKEDITOR.instances.txtContent.getData(),
                        Status: $('#ckStatusM').prop('checked') == true ? 1 : 0,
                        HotFlag: $('#ckHotM').prop('checked'),
                        HomeFlag: $('#ckShowHomeM').prop('checked'),
                       
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

        $('#btnSelectImg').on('click', function () {
            $('#fileInputImage').click();
        });

        $('#fileInputImage').on('change', function () {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }
            $.ajax({
                type: "POST",
                url: "/Admin/Upload/UploadImage",
                contentType: false,
                processData: false,
                data: data,
                success: function (path) {
                    $('#txtImage').val(path);
                    common.notify('Upload image succesful!', 'success');

                },
                error: function () {
                    common.notify('There was error uploading files!', 'error');
                }
            });

        });

        $('#btn-import').on('click', function () {
            initTreeDropDownCategory();
            $('#modal-import-excel').modal('show');
        })

        $('#btn-export').on('click', function () {
            $.ajax({
                type: 'POST',
                url: '/Admin/Product/ExportExcel',
                success: function (response) {
                    window.location.href = response;
                },
                error: function (err) {
                    common.notify('Có lỗi download xảy ra', 'error');
                }
            });

        });

        $('#btnImportExcel').on('click', function () {
            var fileUpload = $('#fileInputExcel').get(0);
            var files = fileUpload.files;

            //Create FormData object
            var fileData = new FormData();
            //Looping over all files and ad it to formdata object
            for (var i = 0; i < files.length; i++) {
                fileData.append("files", files[i]);
            }
            fileData.append('categoryId', $('#ddlCategoryIdImportExcel').combotree('getValue'));
            $.ajax({
                url: '/Admin/Product/ImportExcel',
                type: 'POST',
                data: fileData,
                processData: false,  // tell jQuery not to process the data
                contentType: false,  // tell jQuery not to set contentType
                success: function (data) {
                    $('#modal-import-excel').modal('hide');
                    loadData();

                }
            });
        });

    };

    function configCkEditor() {
        CKEDITOR.replace("txtContent");
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

                    $('#ddlCategoryIdImportExcel').combotree({
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

        $('#txtImage').val('');

        $('#txtTagM').val('');
        $('#txtMetakeywordM').val('');
        $('#txtMetaDescriptionM').val('');
        $('#txtSeoPageTitleM').val('');
        $('#txtSeoAliasM').val('');

        CKEDITOR.instances.txtContent.setData('');
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
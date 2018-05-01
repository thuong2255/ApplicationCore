var roleController = function () {
    this.init = function () {
        registerEvent();
        loadData();
    };

    var registerEvent = function () {

        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'en',
            rules: {
                txtNameRole: { required: true },
                txtDescriptionRole: { required: true }
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
            $('#roleModal').modal('show');
        });

        $('#btnSave').off('click').on('click', (e) => {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();

                $.ajax({
                    type: 'POST',
                    url: '/Admin/Role/SaveEntity',
                    dataType: 'json',
                    data: {
                        Id: $('#hidId').val(),
                        Name: $('#txtNameRole').val(),
                        Description: $('#txtDescriptionRole').val(),
                    },
                    success: function (response) {
                        common.notify('Thêm thành công', 'success');
                        $('#roleModal').modal('hide');
                        resetForm();
                        loadData(true);
                    },
                    error: function (err) {
                        common.notify('Có lỗi xảy ra', 'error');
                    }
                });
            }
        });

        $('body').on('click', '.btnEdit', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            $.ajax({
                type: 'GET',
                url: '/Admin/Role/GetById',
                data: {
                    id: id
                },
                success: function (data) {
                    $('#hidId').val(data.Id);
                    $('#txtNameRole').val(data.Name);
                    $('#txtDescriptionRole').val(data.Description),
                    $('#roleModal').modal('show');

                }
            });
        });

        $('body').on('click', '.btnDelete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            common.confirm('Bạn có chắc chắn muốn xóa không?', function () {
                $.ajax({
                    type: 'POST',
                    url: '/Admin/Role/Delete',
                    data: {
                        id: id
                    },
                    success: function () {
                        common.notify('Xóa thành công', 'success');
                        loadData(true);
                    },
                    error: function () {
                        common.notify('Xóa không thành công', 'error');
                    }
                });
            });
        });
    };

    var loadData = function (isPageChanged) {
        var template = $('#table-template').html();
        var render = "";
        $.ajax({
            type: 'GET',
            data: {
                keyword: $('#txtKeyword').val(),
                page: common.configs.pageIndex,
                pageSize: common.configs.pageSize
            },
            url: '/Admin/Role/GetAllPaging',
            dataType: 'json',
            success: function (response) {
                response.Results.forEach(function (item) {
                    render += Mustache.render(template, {
                        Id: item.Id,
                        Name: item.Name,
                        Description: item.Description,
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

    var resetForm = function () {
        $('#hidId').val('');
        $('#txtNameRole').val('');
        $('#txtDescriptionRole').val('');
    };

    var wrapPaging = function (totalRow, callBack, changePageSize) {
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
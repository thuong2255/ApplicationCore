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

        $('body').on('click', '.btnGrant', function () {
            $('#hidRoleId').val($(this).data('id'));

            //$.when(loadFunctionList()).done(fillPermission($('#hidRoleId').val()));

            loadFunctionList();

            $('#modalGrantpermission').modal('show');
        });

        $('#btnSavePermission').off('click').on('click', function () {
            var listPermission = [];

            $.each($('#tblFunction tbody tr'), function (i, item) {
                listPermission.push({
                    RoleId: $('#hidRoleId').val(),
                    FunctionId: $(item).data('id'),
                    CanRead: $(item).find('.ckView').first().prop('checked'),
                    CanCreate: $(item).find('.ckAdd').first().prop('checked'),
                    CanUpdate: $(item).find('.ckEdit').first().prop('checked'),
                    CanDelete: $(item).find('.ckDelete').first().prop('checked')
                });
            });

            $.ajax({
                type: 'POST',
                url: '/Admin/Role/SavePermission',
                data: {
                    permissionVms: listPermission,
                    roleId: $('#hidRoleId').val()
                },
                success: function (response) {
                    common.notify('Lưu quyền thành công', 'success');
                    $('#modalGrantpermission').modal('hide');
                },
                error: function (err) {
                    common.notify('Có lỗi xảy ra khi lưu quyền', 'error');
                }
            });
        });


    };

    var loadFunctionList = function () {
        $.ajax({
            type: 'GET',
            url: '/Admin/Function/GetAll',
            dataType: 'json',
            success: function (response) {
                var template = $('#result-data-function').html();
                var render = '';
                response.forEach(function (item) {
                    render += Mustache.render(template, {
                        Name: item.Name,
                        treegridparent: item.ParentId != null ? "treegrid-parent-" + item.ParentId : "",
                        Id: item.Id,
                    });
                });

                $('#lst-data-function').html(render);
                $('.tree').treegrid();

                $('#ckCheckAllView').on('click', function () {
                    $('.ckView').prop('checked', $(this).prop('checked'));
                });

                $('#ckCheckAllCreate').on('click', function () {
                    $('.ckAdd').prop('checked', $(this).prop('checked'));
                });

                $('#ckCheckAllEdit').on('click', function () {
                    $('.ckEdit').prop('checked', $(this).prop('checked'));
                });

                $('#ckCheckAllDelete').on('click', function () {
                    $('.ckDelete').prop('checked', $(this).prop('checked'));
                });

                $('.ckView').on('click', function () {
                    if ($('.ckView:checked').length == response.length) {
                        $('#ckCheckAllView').prop('checked', true);
                    }
                    else {
                        $('#ckCheckAllView').prop('checked', false);
                    }
                });

                $('.ckAdd').on('click', function () {
                    if ($('.ckAdd:checked').length == response.length) {
                        $('#ckCheckAllCreate').prop('checked', true);
                    }
                    else {
                        $('#ckCheckAllCreate').prop('checked', false);
                    }
                });

                $('.ckEdit').on('click', function () {
                    if ($('.ckEdit:checked').length == response.length) {
                        $('#ckCheckAllEdit').prop('checked', true);
                    }
                    else {
                        $('#ckCheckAllEdit').prop('checked', false);
                    }
                });

                $('.ckDelete').on('click', function () {
                    if ($('.ckDelete:checked').length == response.length) {
                        $('#ckCheckAllDelete').prop('checked', true);
                    }
                    else {
                        $('#ckCheckAllDelete').prop('checked', false);
                    }
                });

                fillPermission($('#hidRoleId').val());

            },
            error: function (err) {
                console.log(err);
            }
        });
    };

    var fillPermission = function (roleId) {
        $.ajax({
            type: 'GET',
            url: '/Admin/Role/GetListPermission',
            data: {
                roleId: roleId,
            },
            dataType: 'json',
            success: function (response) {
                var listPermission = response;
                $.each($('#tblFunction tbody tr'), function (i, item) {
                    $.each(listPermission, function (j, jitem) {
                        if (jitem.FunctionId === $(item).data('id')) {
                            $(item).find('.ckView').first().prop('checked', jitem.CanRead);
                            $(item).find('.ckAdd').first().prop('checked', jitem.CanCreate);
                            $(item).find('.ckEdit').first().prop('checked', jitem.CanUpdate);
                            $(item).find('.ckDelete').first().prop('checked', jitem.CanDelete);
                        }
                    });
                });

                if ($('.ckView:checked').length == $('#tblFunction tbody tr .ckView').length) {
                    $('#ckCheckAllView').prop('checked', true);
                } else {
                    $('#ckCheckAllView').prop('checked', false);
                }
                if ($('.ckAdd:checked').length == $('#tblFunction tbody tr .ckAdd').length) {
                    $('#ckCheckAllCreate').prop('checked', true);
                } else {
                    $('#ckCheckAllCreate').prop('checked', false);
                }
                if ($('.ckEdit:checked').length == $('#tblFunction tbody tr .ckEdit').length) {
                    $('#ckCheckAllEdit').prop('checked', true);
                } else {
                    $('#ckCheckAllEdit').prop('checked', false);
                }
                if ($('.ckDelete:checked').length == $('#tblFunction tbody tr .ckDelete').length) {
                    $('#ckCheckAllDelete').prop('checked', true);
                } else {
                    $('#ckCheckAllDelete').prop('checked', false);
                }
            },
            error: function (err) {

            }
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
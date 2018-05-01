var userController = function () {
    this.init = function () {
        registerEvent();
    };

    var registerEvent = function () {

        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'en',
            rules: {
                txtFullName: { required: true },
                txtUserName: { required: true },
                txtPassword: {
                    required: true,
                    minlength: 6
                },
                txtConfirmPassword: {
                    equalTo: "#txtPassword"
                },
                txtEmail: {
                    required: true,
                    email: true
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


        loadData();

        $('#btnCreate').off('click').on('click', () => {
            resetForm();
            initRoleList();
            $('#userModal').modal('show');
        });

        $('#btnSave').off('click').on('click', (e) => {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var roles = [];

                $('#checkboxes input:checked').each(function () {
                    roles.push($(this).attr('value'));
                });

                $.ajax({
                    type: 'POST',
                    url: '/Admin/User/SaveEntity',
                    dataType: 'json',
                    data: {
                        Id: $('#hidId').val(),
                        FullName: $('#txtFullName').val(),
                        UserName: $('#txtUserName').val(),
                        Password: $('#txtPassword').val(),
                        Email: $('#txtEmail').val(),
                        PhoneNumber: $('#txtPhoneNumber').val(),
                        Avatar: $('#txtImage').val(),
                        Status: $('#ckStatus').prop('checked') == true ? 1 : 0,
                        Roles: roles
                    },
                    success: function (response) {
                        common.notify('Thêm thành công', 'success');
                        $('#userModal').modal('hide');
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
                url: '/Admin/User/GetById',
                data: {
                    id: id
                },
                success: function (data) {
                    disableFieldEdit(true);
                    $('#hidId').val(data.Id);
                    $('#txtFullName').val(data.FullName);
                    $('#txtUserName').val(data.UserName);
                    $('#txtEmail').val(data.Email);
                    $('#txtPhoneNumber').val(data.PhoneNumber);
                    $('#txtImage').val(data.Avatar);
                    $('#ckStatus').prop('checked', data.Status === 1)

                    initRoleList(data.Roles);
                    $('#userModal').modal('show');

                }
            });
        });

        $('body').on('click', '.btnDelete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            common.confirm('Bạn có chắc chắn muốn xóa không?', function () {
                $.ajax({
                    type: 'POST',
                    url: '/Admin/User/Delete',
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
            url: '/Admin/User/GetAllPaging',
            dataType: 'json',
            success: function (response) {
                response.Results.forEach(function (item) {
                    render += Mustache.render(template, {
                        Id: item.Id,
                        UserName: item.UserName,
                        FullName: item.FullName,
                        Avatar: item.Avatar == null ? '<img src="/Admin/images/user.png" width="25"/>' : `<img src="${Image.Avatar}" width= "25"/>`,
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

    var initRoleList = function (selectedRoles) {
        $.ajax({
            type: 'GET',
            url: '/Admin/Role/GetAll',
            dataType: 'json',
            async: false,
            success: function (data) {
                var template = $('#role-template').html();
                var render = '';
                data.forEach(item => {
                    var checked = '';
                    if (selectedRoles !== undefined && selectedRoles.indexOf(item.Name) !== -1)
                        checked = 'checked';
                    render += Mustache.render(template, {
                        Name: item.Name,
                        Description: item.Description,
                        Checked: checked
                    });
                });
                $('#listRole').html(render);
            },
            error: function (err) {
                console.log(err);
            }
        });
    };



    var disableFieldEdit = function (disabled) {
        $('#txtUserName').prop('disabled', disabled);
        $('#txtPassword').prop('disabled', disabled);
        $('#txtConfirmPassword').prop('disabled', disabled);
    };

    var resetForm = function () {
        disableFieldEdit(false);
        $('#hidId').val('');
        initRoleList();
        $('#txtFullName').val('');
        $('#txtUserName').val('');
        $('#txtPassword').val('');
        $('#txtConfirmPassword').val('');
        $('input[name="ckRoles"]').removeAttr('checked');
        $('#txtEmail').val('');
        $('#txtPhoneNumber').val('');
        $('#txtImage').val('');
        $('#ckStatus').prop('checked', true)
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
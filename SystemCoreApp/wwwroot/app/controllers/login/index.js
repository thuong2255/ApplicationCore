var loginController = function () {
    this.init = function () {
        registerEvent();
    };

    var registerEvent = function () {
        $('#btnLogin').on('click', function (e) {
            e.preventDefault();
            var user = $('#txtUserName').val();
            var pass = $('#txtPassword').val();

            if (validateForm(user, pass)) {
                login(user, pass);
            }
        });
    };

    var login = function (user, pass) {
        $.ajax({
            type: 'POST',
            data: {
                UserName: user,
                Password: pass
            },
            dataType: 'json',
            url: '/admin/login/authen',
            success: function (res) {
                if (res.success) {
                    window.location.href = '/Admin/Home/Index';
                }
                else {
                    common.notify('Đăng nhập không đúng', 'error');
                }
            }
        });
    };

    var validateForm = function (user, pass) {
        if (user === '') {
            alert('Tên đăng nhập không được bỏ trống');
            return false;
        }

        if (pass === '') {
            alert('Mật khẩu đăng nhập không được bỏ trống');
            return false;
        }
        return true;
    };
};
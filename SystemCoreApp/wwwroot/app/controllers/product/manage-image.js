var ManageImage = function () {


    this.init = function () {
        registerEvents();
    };

    function registerEvents() {
        $('body').on('click', '.btn-images', function (e) {
            e.preventDefault();

            var productId = $(this).data('id');
            $('#hidId').val(productId);
            loadImages();
            $('#modal-image-manage').modal('show');
        });

        $('#fileImage').on('change', function () {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }

            $.ajax({
                type: 'POST',
                url: "/Admin/Upload/UploadImage",
                contentType: false,
                processData: false,
                data: data,
                success: function (path) {
                    $('#image-list').append(`<div class = "col-md-3"><img width = "100" data-path = "${path}" src = "${path}"/><br/><a href = "#" class = "btn-delete-image">Xóa</a></div>`);
                    common.notify('Upload ảnh thành công', 'success');
                },
                error: function (err) {
                    common.notify('Lỗi Upfile', 'error');
                }
            });
        });

        $('body').on('click', '#btnCancel', function () {
            $('#modal-image-manage').modal('hide');
            clearFileInput();
        });

        $('body').on('click','.btn-delete-image', function () {
            $(this).parent().remove();
        });

        $("#btnSaveImages").on('click', function () {
            var listImage = [];
            $.each($('#image-list').find('img'), function (i, item) {
                listImage.push($(this).data('path'));
            });
            $.ajax({
                type: 'POST',
                url: '/Admin/Product/SaveImage',
                data: {
                    productId: $('#hidId').val(),
                    images: listImage
                },
                success: function (response) {
                    $('#modal-image-manage').modal('hide');
                    clearFileInput();
                }
            });
        });
    };

    function loadImages() {
        $.ajax({
            type: 'GET',
            url: '/Admin/Product/GetImages',
            data: { productId: $('#hidId').val() },
            success: function (response) {
                var render = ``;
                response.forEach(function (item) {
                    render += `<div class = "col-md-3"><img width = "100" data-path = "${item.Path}" src = "${item.Path}"/><br/><a href = "#" class = "btn-delete-image">Xóa</a></div>`;
                });
                $('#image-list').html(render);
            }

        });
    }

    function clearFileInput() {
        $('#fileImage').val(null);
        $('#image-list').html('');
    }
};
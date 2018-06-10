var ProductQuantity = function () {
    var cacheObj = {
        colors: [],
        sizes: []
    };
    this.init = function () {
        loadColors();
        loadSizes();
        registerEvents();
    };

    var registerEvents = function () {
        $('body').on('click', '.btn-quantity', function (e) {
            e.preventDefault();
            var productId = $(this).data('id');

            $('#hidId').val(productId);

            loadQuantities();

            $('#modal-quantity-management').modal('show');
        });

        $('#btn-add-quantity').on('click', function () {
            var template = $('#template-table-quantity').html();
            var render = Mustache.render(template, {
                Id: 0,
                Colors: getColorOptions(null),
                Sizes: getSizeOptions(null),
                Quantity: 0
            });

            $('#table-quantity-content').append(render);
        });

        $('body').on('click', '.btn-delete-quantity', function (e) {
            e.preventDefault();
            $(this).parent().parent().remove();
        });

        $('#btnSaveQuantity').on('click', function () {
            var listQuantities = [];
            $.each($('#table-quantity-content').find('tr'), function (i, item) {
                listQuantities.push({
                    Id: $(item).data('id'),
                    ProductId: $('#hidId').val(),
                    SizeId: $(item).find('select.ddlColorId').first().val(),
                    ColorId: $(item).find('select.ddlSizeId').first().val(),
                    Quantity: $(item).find('input.txtQuantity').first().val()
                });
            });

            $.ajax({
                type: 'POST',
                url: '/Admin/Product/SaveQuantity',
                data: {
                    productId: $('#hidId').val(),
                    quantities: listQuantities
                },
                success: function (response) {
                    $('#modal-quantity-management').modal('hide');
                    $('#table-quantity-content').html('');
                }
            });
        });
    };

    var loadQuantities = function () {
        $.ajax({
            type: 'GET',
            url: '/Admin/Product/GetQuantities',
            data: {
                productId: $('#hidId').val()
            },
            success: function (response) {
                var template = $('#template-table-quantity').html();
                var render = '';
                response.forEach(function (item) {
                    render += Mustache.render(template, {
                        Id: item.Id,
                        Colors: getColorOptions(item.ColorId),
                        Sizes: getSizeOptions(item.SizeId),
                        Quantity: item.Quantity
                    });
                });

                $('#table-quantity-content').html(render);

            }

        });
    };

    var loadSizes = function () {
        $.ajax({
            type: 'GET',
            url: '/Admin/Bill/GetSizes',
            dataType: 'json',
            success: function (response) {
                cacheObj.sizes = response;
                console.log(response);
            }
        });
    };

    var loadColors = function () {
        $.ajax({
            type: 'GET',
            url: '/Admin/Bill/GetColors',
            dataType: 'json',
            success: function (response) {
                cacheObj.colors = response;
            }
        });
    };

    var getColorOptions = function (selectedId) {
        var colors = `<select class = "form-control ddlColorId">`;
        cacheObj.colors.forEach(function (item) {
            if (selectedId == item.Id)
                colors += `<option value = "${item.Id}" selected = "select">${item.Name}</option>`;
            else
                colors += `<option value = "${item.Id}">${item.Name}</option>`;
        });
        colors += `</select>`;
        return colors;
    };

    var getSizeOptions = function (selectedId) {
        var sizes = `<select class = "form-control ddlSizeId">`;
        cacheObj.sizes.forEach(function (item) {
            if (selectedId == item.Id)
                sizes += `<option value = "${item.Id}" selected = "select">${item.Name}</option>`;
            else
                sizes += `<option value = "${item.Id}">${item.Name}</option>`;
        });
        sizes += `</select>`;
        return sizes;
    };
};
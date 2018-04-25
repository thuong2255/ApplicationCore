var productCategory = function() {
    this.init = function () {
        loadData();
    };

    var loadData = function () {
        var data = [];

        $.ajax({
            type: 'GET',
            url: '/admin/productcategory/getproductcategory',
            dataType: 'json',
            success: function (response) {
                response.forEach(function (item) {
                    data.push({
                        id: item.Id,
                        text: item.Name,
                        parentId: item.ParentId,
                        sortOrder: item.SortOrder
                    });
                });

                var dataTree = common.unflattern(data);
                $("#tree").tree({
                    data: dataTree,
                    dnd: true
                });
            },
            error: function (err) {
                console.log(err);
            }
        });
    };
};
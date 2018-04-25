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

                dataTree.sort(function (a, b) {
                    return a.sortOrder - b.sortOrder;
                });

                $("#tree").tree({
                    data: dataTree,
                    dnd: true,
                    onDrop: function (target, source, point) {
                        var targetNode = $(this).tree('getNode', target);

                        console.log(targetNode.children);

                        if (point === 'append') {
                            var children = [];
                            $.each(targetNode.children, function (i, item) {
                                children.push({
                                    key: item.id,
                                    value: i
                                });
                            });

                            //Update to database
                            $.ajax({
                                type: 'POST',
                                url: '/Admin/ProductCategory/UpdateParentId',
                                data: {
                                    sourceId: source.id,
                                    targetId: targetNode.id,
                                    items: children
                                },
                                success: function (result) {
                                    loadData();
                                },
                                error: function (err) {
                                    console.log(err);
                                }
                            });
                        }
                        else if (point === 'top' || point === 'bottom') {
                            $.ajax({
                                type: 'POST',
                                url: '/Admin/ProductCategory/ReOrder',
                                data: {
                                    sourceId: source.id,
                                    targetId: targetNode.id,
                                },
                                success: function (result) {
                                    loadData();
                                },
                                error: function (err) {
                                    console.log(err);
                                }
                            });
                        }
                    }
                });
            },
            error: function (err) {
                console.log(err);
            }
        });
    };
};
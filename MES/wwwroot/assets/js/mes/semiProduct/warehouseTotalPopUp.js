
"use strict";

// Class definition
var WarehouseTotalShowModalPageInit = function () {

    var table;
    var datatable;
    var referenceId;

    var initDatatable = function () {
        var postUrl = '/SemiProduct/GetWarehouseJsonResult?productReferenceId=' + referenceId;
        console.log(postUrl)


        datatable = $(table).DataTable({

            responsive: true,
            autoWidth: false,
            searchDelay: 500,
            destroy: true,
            info: false,
            order: [],
            pageLength: 10,
            ajax: {
                url: postUrl,
                type: 'POST',
            },
            columns: [

                { data: 'warehouse.name' },
                { data: 'onhand' },
                { data: 'onhand' },
                { data: 'onhand' },
                { data: 'onhand' },
            ],
            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    render: function (data, type, full, meta) {

                        console.log(full)
                        var output;

                        output = `<td>` + full.warehouse.name + `</td>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 1,
                    render: function (data, type, full, meta) {

                        var output;



                        output = `<td>` + full.onhand + `</td>`
                        return output;

                    },

                },

                {

                    orderable: true,
                    targets: 2,
                    className: 'text-end pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<td>` + full.onhand + `</td>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 3,
                    className: 'text-end pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<td>` + full.onhand + `</td>`
                        return output;

                    },

                },
            ]

        });



        // Re-init functions on datatable re-draws

        datatable.on('draw', function () {

            KTMenu.createInstances();

        });

    }

    // Private functions

    var loadModalPage = function () {
        $('#mes_semiProduct_warehouseTotal').on('shown.bs.modal', function () {
            console.log("Ambar Toplamları tablosu")
            initDatatable();

        });
    };

    var bindEventHandlers = function () {
        $(document).on('click', 'a#SemiProductWarehouseTotalList', function () {
            referenceId = $(this).data('reference-id');
        });
    };

    // Public methods
    return {
        init: function () {
            table = document.querySelector('#mes_warehouse_total_table');

            if (!table) {
                console.log("Ambar Toplamları tablosu bulunamadı")
                return;
            }
            bindEventHandlers();
            loadModalPage();


        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    WarehouseTotalShowModalPageInit.init();
});
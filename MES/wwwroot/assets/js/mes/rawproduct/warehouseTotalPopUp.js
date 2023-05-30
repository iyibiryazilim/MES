"use strict";

// Class definition
var ShowModalWarehouseTotalPageInit = function () {

    var table;
    var datatable;
    var referenceId;

    var initDatatable = function () {

        var postUrl = 'RawProduct/GetWarehouseTotalJsonResult?productReferenceId=' + referenceId;
        console.log(postUrl);

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
                type: 'POST'
            },
            columns: [
                { data: 'referenceId' },
                { data: 'lastTransactionDate' },
                { data: 'warehouseName' },
                { data: 'stockQuantity' },

            ],
            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<div class="form-check form-check-sm form-check-custom form-check-solid">
									<input class="form-check-input" type="checkbox" value="1" />
								</div>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 1,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var formattedDate = new Date(full.lastTransactionDate);
                        var d = formattedDate.getDate();
                        var m = formattedDate.getMonth();
                        m += 1;
                        var y = formattedDate.getFullYear();

                        var output;
                        output = `<span class="fw-bold">` + d.toString().padStart(2, '0') + '.' + m.toString().padStart(2, '0') + '.' + y + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 2,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<span class="fw-bold">` + full.warehouseName + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 3,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<span class="fw-bold">` + full.stockQuantity + `</span>`

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
        $('#mes_modal_warehouse_total').on('shown.bs.modal', function () {

            initDatatable();
        });
    };
    var bindEventHandlers = function () {
        $(document).on('click', 'a#RawProductWarehouseTotalList', function () {
            referenceId = $(this).data('reference-id');
            console.log("warehouseTotal " + referenceId);
        });
    };

    // Public methods
    return {
        init: function () {
            table = document.querySelector('#mes_warehouse_total_table');

            if (!table) {
                return;
            }
            bindEventHandlers();
            loadModalPage();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    ShowModalWarehouseTotalPageInit.init();
});
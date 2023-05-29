"use strict";

// Class definition
var ShowModalPurchaseOrderPageInit = function () {

    var table;
    var datatable;
    var referenceId;

    var initDatatable = function () {

        var postUrl = 'EndProduct/GetPurchaseOrderLineJsonResult?productReferenceId=' + referenceId;
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
                { data: 'orderDate' },
                { data: 'orderCode' },
                { data: 'currentCode' },
                { data: 'warehouseNo' },
                { data: 'quantity' },
                { data: 'shippedQuantity' },
                { data: 'waitingQuantity' },
                { data: 'description' },

            ],

            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    render: function (data, type, full, meta) {

                        var output;

                        output = `<div class="form-check form-check-sm form-check-custom form-check-solid">
                            <input class="form-check-input" type="checkbox" value="`+ full.referenceId + `" />
                        </div>`
                        return output;

                    },

                },
                {
                    orderable: true,
                    targets: 1,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var formattedDate = new Date(full.orderDate);
                        var d = formattedDate.getDate();
                        var m = formattedDate.getMonth();
                        m += 1;
                        var y = formattedDate.getFullYear();

                        var output;

                        output = ` <div class="fw-bold fs-6">` + d.toString().padStart(2, '0') + '.' + m.toString().padStart(2, '0') + '.' + y + `</div>`

                        return output;

                    },
                },
                {

                    orderable: true,
                    targets: 2,
                    classname: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = ` <div class="d-flex flex-column">
                            <a href="#" class="text-gray-800 text-hover-primary mb-1">` + full.orderCode + `</a>
                        </div>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 3,
                    className: 'd-flex align-items-center',
                    render: function (data, type, full, meta) {

                        var output;

                        output = `<!--begin:: Avatar -->
															<div class="symbol symbol-circle symbol-50px overflow-hidden me-3">
																<a href="../../demo1/dist/apps/user-management/users/view.html">
																	<div class="symbol-label">
																		<img src="assets/media/avatars/300-6.jpg" alt="Emma Smith" class="w-100" />
																	</div>
																</a>
															</div>
															<!--end::Avatar-->
															<!--begin::User details-->
															<div class="d-flex flex-column">
																<a href="#" class="text-gray-800 text-hover-primary mb-1">` + full.currentCode + `</a>
																<span>`+ full.currentName + `</span>
															</div>
															<!--begin::User details-->`
                        return output;

                    },

                },

                {

                    orderable: true,
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        var value = "";
                        if (full.warehouseNo != null) {
                            value = full.warehouseNo
                        }
                        output = `<span class="fw-bold ms-3">` + value + `</span>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 5,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = ` <div class="badge badge-light-primary fs-6">` + full.quantity + `</div>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 6,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = ` <div class="badge badge-light-success fs-6">` + full.shippedQuantity + `</div>`


                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 7,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        if (full.warehouse != null) {
                            var value = full.warehouse.name
                        }
                        var output;
                        output = ` <div class="badge badge-light-danger fs-6">` + full.waitingQuantity.toFixed(1) + `</div>`


                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 8,
                    render: function (data, type, full, meta) {

                        console.log()
                        var output;

                        output = `<span class="fw-bold ms-3">` + full.description + `</span>`
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
        $('#mes_modal_purchase_order').on('shown.bs.modal', function () {

            console.log("purchase order" + referenceId)

            initDatatable();
        });
    };

    var bindEventHandlers = function () {
        $(document).on('click', 'a#EndProductPurchaseOrderList', function () {
            referenceId = $(this).data('reference-id');
            console.log(referenceId);
        });
    };

    // Public methods
    return {
        init: function () {
            table = document.querySelector('#mes_purchase_order_table');

            if (!table) {
                console.log("Bekleyen Satıl Alma Table'ı bulunamadı'")
                return;
            }
            bindEventHandlers();
            loadModalPage();

        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    ShowModalPurchaseOrderPageInit.init();
});
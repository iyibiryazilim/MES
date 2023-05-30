"use strict";

// Class definition
var EndProductPurchaseOrderLineList = function () {
    // Shared variables
    var table;
    var datatable;

    // Private functions
    var initDatatable = function () {
        var referenceId = $('#ProductId').val()
        var postUrl = '../../EndProduct/GetPurchaseOrderLineJsonResult?productReferenceId=' + referenceId;

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
																		<img src="~/assets/media/avatars/300-6.jpg" alt="Emma Smith" class="w-100" />
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

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[wms-salesorder-table-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Handle status filter dropdown
    var handleStatusFilter = () => {
        const filterStatus = document.querySelector('[data-kt-ecommerce-product-filter="status"]');
        $(filterStatus).on('change', e => {
            let value = e.target.value;
            if (value === 'all') {
                value = '';
            }
            datatable.column(1).search(value).draw();
        });
    }

    // Delete cateogry
    var handleDeleteRows = () => {
        // Select all delete buttons
        const deleteButtons = table.querySelectorAll('[data-kt-ecommerce-product-filter="delete_row"]');

        deleteButtons.forEach(d => {
            // Delete button on click
            d.addEventListener('click', function (e) {
                e.preventDefault();

                // Select parent row
                const parent = e.target.closest('tr');

                // Get category name
                const productName = parent.querySelector('[data-kt-ecommerce-product-filter="product_name"]').innerText;

                // SweetAlert2 pop up --- official docs reference: https://sweetalert2.github.io/
                Swal.fire({
                    text: "Are you sure you want to delete " + productName + "?",
                    icon: "warning",
                    showCancelButton: true,
                    buttonsStyling: false,
                    confirmButtonText: "Yes, delete!",
                    cancelButtonText: "No, cancel",
                    customClass: {
                        confirmButton: "btn fw-bold btn-danger",
                        cancelButton: "btn fw-bold btn-active-light-primary"
                    }
                }).then(function (result) {
                    if (result.value) {
                        Swal.fire({
                            text: "You have deleted " + productName + "!.",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Ok, got it!",
                            customClass: {
                                confirmButton: "btn fw-bold btn-primary",
                            }
                        }).then(function () {
                            // Remove current row
                            datatable.row($(parent)).remove().draw();
                        });
                    } else if (result.dismiss === 'cancel') {
                        Swal.fire({
                            text: productName + " was not deleted.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Ok, got it!",
                            customClass: {
                                confirmButton: "btn fw-bold btn-primary",
                            }
                        });
                    }
                });
            })
        });
    }


    // Public methods
    return {
        init: function () {
            table = document.querySelector('#mes_endproduct_purchase_order_line_table');

            if (!table) {
                return;
            }

            initDatatable();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    EndProductPurchaseOrderLineList.init();
});
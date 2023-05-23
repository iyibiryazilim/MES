
"use strict";


// Class definition

var SalesOrderLineList = function () {

    // Shared variables
    var table;
    var datatable;


    // Private functions

    var initDatatable = function () {

        var postUrl = '/SalesOrderLine/GetJsonResult';

        datatable = $(table).DataTable({
            responsive: true,
            autoWidth: false,
            searchDelay: 500,
            destroy: true,
            info: false,
            order: [],
            pageLength: 25,
            ajax: {
                url: postUrl,
                type: 'POST'
            },
            columns: [

                { data: 'referenceId' },
                { data: 'orderDate' },
                { data: 'orderCode' },
                { data: 'currentCode' },
                { data: 'productName' },
                { data: 'warehouseName' },
                { data: 'quantity' },
                { data: 'shippedQuantity' },
                { data: 'waitingQuantity' },
                { data: 'description' },
                { data: 'referenceId' },


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
                    classname: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var formattedDate = new Date(full.orderDate);
                        var d = formattedDate.getDate();
                        var m = formattedDate.getMonth();
                        m += 1;
                        var y = formattedDate.getFullYear();

                        var output;

                        output = ` <a href="#" class="badge badge-light-success fs-5 fw-bold my-2">` + d.toString().padStart(2, '0') + '.' + m.toString().padStart(2, '0') + '.' + y + `</a>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 2,
                    classname: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<a href="#" class="fs-5 text-gray-800 text-hover-primary fw-bold">` + full.orderCode + `</a>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 3,
                    classname: 'd-flex align-items-center',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="d-flex align-items-center mb-7">
														<!--begin::Symbol-->
														<div class="symbol symbol-50px me-5">
															<span class="symbol-label bg-light-success">
																<i class="ki-duotone ki-abstract-26 fs-2x text-success">
																	<span class="path1"></span>
																	<span class="path2"></span>
																</i>
															</span>
														</div>
														<!--end::Symbol-->
														<!--begin::Text-->
														<div class="d-flex flex-column">
															<a href="#" class="text-dark text-hover-primary fs-6 fw-bold">`+ full.currentName + `</a>
															<span class="text-muted fw-bold">`+ full.currentCode + `</span>
														</div>
														<!--end::Text-->
													</div>
													<!--end::Item-->`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 4,
                    className: 'd-flex align-items-center',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="d-flex align-items-center mb-7">
														<!--begin::Symbol-->
														<div class="symbol symbol-50px me-5">
															<span class="symbol-label bg-light-success">
																<i class="ki-duotone ki-abstract-26 fs-2x text-success">
																	<span class="path1"></span>
																	<span class="path2"></span>
																</i>
															</span>
														</div>
														<!--end::Symbol-->
														<!--begin::Text-->
														<div class="d-flex flex-column">
															<a href="#" class="text-dark text-hover-primary fs-6 fw-bold">`+ full.productName + `</a>
															<span class="text-muted fw-bold">`+ full.productCode + `</span>
														</div>
														<!--end::Text-->
													</div>
													<!--end::Item-->`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 5,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        var value = "";
                        if (full.warehouseNo != null) {
                            value = full.warehouseNo
                        }
                        output = `<div class="text-gray-800 fw-bold d-block fs-4">` + value + `</div>`
                        return output;


                    },

                },
                {

                    orderable: true,
                    targets: 6,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="text-gray-800 d-block fs-4">` + full.quantity.toFixed(1) + ` ` + full.subUnitset + `</div>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 7,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="text-gray-800 d-block fs-4">` + full.shippedQuantity.toFixed(1) + ` ` + full.subUnitset + `</div>
                        
                        `
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 8,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="text-gray-800 d-block fs-4">` + full.waitingQuantity.toFixed(1) + ` ` + full.subUnitset + `</div>
                        
                        `
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 9,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        console.log()
                        var output;

                        output = `<div class="text-gray-800 fw-bold d-block fs-4">` + full.description + `</div>`
                        return output;

                    },

                },
                {

                    orderable: false,
                    targets: 10,
                    className: 'text-end',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<a href="#" class="btn btn-sm btn-light btn-active-light-primary btn-flex btn-center" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
							İşlemler
							<i class="ki-duotone ki-down fs-5 ms-1"></i>
						</a>
						<!--begin::Menu-->
						<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-semibold fs-7 w-125px py-4" data-kt-menu="true">
							<!--begin::Menu item-->
							<div class="menu-item px-3">
								<a href="../../demo46/dist/apps/ecommerce/catalog/add-category.html" class="menu-link px-3">Düzenle</a>
							</div>
							<!--end::Menu item-->
							<!--begin::Menu item-->
							<div class="menu-item px-3">
								<a href="#" class="menu-link px-3" data-kt-ecommerce-category-filter="delete_row">Sil</a>
							</div>
							<!--end::Menu item-->
						</div>
						<!--end::Menu-->`

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

        const filterSearch = document.querySelector('[mes-sales-order-line-table-filter="search"]');

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
            table = document.querySelector('#mes_salesOrderLine_table');

            if (!table) {

                return;

            }

            initDatatable();
            handleSearchDatatable();
            // handleStatusFilter();
            //handleDeleteRows();
        }

    };

}();



// On document ready

KTUtil.onDOMContentLoaded(function () {
    SalesOrderLineList.init();
});
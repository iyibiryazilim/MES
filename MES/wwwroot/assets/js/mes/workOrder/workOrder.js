
"use strict";


// Class definition

var WorkOrderList = function () {

    // Shared variables
    var table;
    var datatable;


    // Private functions

    var initDatatable = function () {

        var postUrl = '/WorkOrder/GetJsonResult';

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
                { data: 'referenceId' },
                { data: 'status' },
                { data: 'operationBeginDate' },
                { data: 'operationDueDate' },
                { data: 'realizationRate' },              
                { data: 'referenceId' },

            ],
            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    render: function (data, type, full, meta) {

                        var output;

                        output = `<div class="form-check form-check-sm form-check-custom form-check-solid">
                            <input class="form-check-input" type="checkbox" value="`+ full.referenceId +`" />
                        </div>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 1,
                    render: function (data, type, full, meta) {

                        var output;

                        output = `<div class="d-flex">
							<!--begin::Thumbnail-->
							<a href="../../demo46/dist/apps/ecommerce/catalog/edit-category.html" class="symbol symbol-50px">
								<span class="symbol-label" style="background-image:url(assets/media//stock/ecommerce/68.gif);"></span>
							</a>
							<!--end::Thumbnail-->
							<div class="d-flex align-items-center">
								<!--begin::Title-->
								<a href="../../demo46/dist/apps/ecommerce/catalog/edit-category.html" class="text-gray-800 text-hover-primary fs-5 fw-bold mb-1" data-kt-ecommerce-category-filter="category_name">`+ full.referenceId + `</a>								
							</div>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 2,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        switch (full.status) {
                            case 0:
                                output = `<div class="badge badge-light fw-bold">` + "Statü " + full.status + `</div>`
                                return output;
                                break;
                            case 1:
                                output = `<div class="badge badge-light fw-bold">` + "Statü "+full.status + `</div>`
                                return output;
                                break;
                            case 2:
                                output = `<div class="badge badge-light fw-bold">` + "Statü " + full.status + `</div>`
                                return output;
                                break;
                            case 3:
                                output = `<div class="badge badge-light fw-bold">` + "Statü " + full.status + `</div>`
                                return output;
                                break;
                            case 4:
                                output = `<div class="badge badge-light fw-bold">` + "Statü " +full.status + `</div>`
                                return output;
                                break;
                            default:
                        }
                        

                    },

                },
                {

                    orderable: true,
                    targets: 3,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="text-gray-800 fw-bold d-block fs-4">` + full.operationBeginDate + `</div>`
                        return output;


                    },

                },
                {

                    orderable: true,
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="text-gray-800 fw-bold d-block fs-4">` + full.operationDueDate + `</div>`
                        return output;


                    },

                },
                {

                    orderable: true,
                    targets: 5,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        if (full.realizationRate <= 33 && full.realizationRate >= 0) {
                            output = `<div class="text-muted fs-7 fw-bold">` + "%" + full.realizationRate + `</div>
                                    <div class="h-8px mx-3 w-100 bg-light-danger rounded">
									<div class="bg-danger rounded h-8px" role="progressbar" style="width: `+ full.realizationRate + `%;" aria-valuenow="50" aria-valuemin="0" aria-valuemax="100"></div>
								  </div>`
                            return output;
                        } else if (full.realizationRate > 33 && full.realizationRate <= 66) {
                            output = `<div class="text-muted fs-7 fw-bold">` + "%" + full.realizationRate + `</div>
                                    <div class="h-8px mx-3 w-100 bg-light-warning rounded">
									<div class="bg-warning rounded h-8px" role="progressbar" style="width: `+ full.realizationRate + `%;" aria-valuenow="50" aria-valuemin="0" aria-valuemax="100"></div>
								  </div>`
                            return output;

                        }
                        else {
                            output = `<div class="text-muted fs-7 fw-bold">` + "%" + full.realizationRate + `</div>
                                    <div class="h-8px mx-3 w-100 bg-light-primary rounded">
									<div class="bg-primary rounded h-8px" role="progressbar" style="width: `+ full.realizationRate + `%;" aria-valuenow="50" aria-valuemin="0" aria-valuemax="100"></div>
								  </div>`
                            return output;
                        }

                        
                        

                    },

                },
                
                {

                    orderable: false,
                    targets: 6,
                    className: 'text-end',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<a href="#" class="btn btn-sm btn-light btn-active-light-primary btn-flex btn-center" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">İşlemler<i class="ki-duotone ki-down fs-5 ms-1"></i>
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

        const filterSearch = document.querySelector('[mes-workorder-filter="search"]');

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
            table = document.querySelector('#mes_workOrder_table');

            if (!table) {

                return;

            }

            initDatatable();
            handleSearchDatatable();
            handleStatusFilter();
            //handleDeleteRows();
        }

    };

}();



// On document ready

KTUtil.onDOMContentLoaded(function () {
    WorkOrderList.init();
});
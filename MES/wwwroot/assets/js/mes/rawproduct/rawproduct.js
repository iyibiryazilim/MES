﻿
"use strict";


// Class definition

var RawProductList = function () {

    // Shared variables
    var table;
    var datatable;


    // Private functions

    var initDatatable = function () {

        var postUrl = '/RawProduct/GetRawProductJsonResult';
        var detailUrl = `/RawProduct/Detail/`


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
                { data: 'code' },
                { data: 'stockQuantity' },
                { data: 'producerCode' },
                { data: 'speCode' },
                { data: 'inputQuantity' },
                { data: 'outputQuantity' },
                { data: 'lastTransactionDate' },
                { data: 'revolutionSpeed' },
                { data: 'referenceId' },

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
                    className: 'd-flex align-items-center',
                    render: function (data, type, full, meta) {
                        var value = full.name;
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
																<a href="EndProduct/Detail/?referenceId=`+ full.referenceId + `" class="text-gray-800 text-hover-primary mb-1">` + full.code + `</a>
																<span>`+ full.name + `</span>
															</div>
															<!--begin::User details-->`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 2,
                    className: 'text-end pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<span class="fw-bold ms-3">` + full.stockQuantity + `</span> <span class="fw-bold">` + full.unitset.code + `</span> `

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 3,
                    className: 'text-end pe-0',
                    render: function (data, type, full, meta) {
                        var output;

                        output = `<span class="fw-bold ms-3">` + full.producerCode + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 4,
                    className: 'text-end pe-0',
                    render: function (data, type, full, meta) {
                        var output;

                        output = `<span class="fw-bold ms-3">` + full.speCode + `</span>`

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 5,
                    className: 'text-end pe-0',
                    render: function (data, type, full, meta) {
                        var output;

                        output = ` <div class="badge badge-light-success">` + full.inputQuantity + `</div>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 6,
                    className: 'text-end pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = ` <div class="badge badge-light-danger">` + full.outputQuantity + `</div>`

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 7,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        var formattedDate = new Date(full.lastTransactionDate);
                        var d = formattedDate.getDate();
                        var m = formattedDate.getMonth();
                        m += 1;
                        var y = formattedDate.getFullYear();

                        var output;

                        output = ` <div class="badge badge-light-primary">` + d.toString().padStart(2, '0') + '.' + m.toString().padStart(2, '0') + '.' + y + `</div>`

                        return output;

                    },
                },

                {
                    orderable: true,
                    targets: 8,
                    className: 'text-center pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = ` <div class="h-5px mx-3 w-100 bg-light mb-3">
                    <div class="bg-success rounded h-5px" role="progressbar" style="width: 50%;" aria-valuenow="50" aria-valuemin="0" aria-valuemax="100"></div>
                </div>`

                        return output;
                    },
                },

                {
                    orderable: true,
                    targets: 9,
                    className: 'text-end pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<a href="#" class="btn btn-sm btn-light btn-active-light-primary btn-flex btn-center" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
							İşlemler
							<i class="ki-duotone ki-down fs-5 ms-1"></i>
						</a>
						<!--begin::Menu-->
						<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-semibold fs-8 w-150px py-2" data-kt-menu="true">
							<!--begin::Menu item-->
							<div class="menu-item px-3">
								<a href="RawProduct/Detail/?referenceId=`+ full.referenceId + `" class="menu-link px-3">Özet</a>
							</div>
							<!--end::Menu item-->
                            <div class="menu-item px-3">
								<a href="#" id="RawProductInputTransactionList" class="menu-link px-3" data-bs-toggle="modal" data-reference-id = `+ full.referenceId + ` data-bs-target="#mes_rawProduct_inputTransaction">Giriş Hareketleri</a>
							</div>
							<!--end::Menu item-->
                            <!--begin::Menu item-->
							<div class="menu-item px-3">
								<a href="#" id="RawProductOutputTransactionList" class="menu-link px-3" data-bs-toggle="modal" data-reference-id = `+ full.referenceId + ` data-bs-target="#mes_rawProduct_outputTransaction">Çıkış Hareketleri</a>
							</div>
							<!--end::Menu item--><!--begin::Menu item-->
							<div class="menu-item px-3">
								<a href="#" id="RawProductWarehouseTotalList" class="menu-link px-3" data-bs-toggle="modal" data-reference-id = `+ full.referenceId + ` data-bs-target="#mes_rawProduct_warehouseTotal" >Ambar Toplamları</a>
							</div>
							<!--end::Menu item--><!--begin::Menu item-->
							<div class="menu-item px-3">
								<a href="#" id="RawProductSaleOrderList" class="menu-link px-3" data-bs-toggle="modal" data-reference-id = `+ full.referenceId + ` data-bs-target="#mes_rawProduct_salesOrder" >Satış Siparişleri</a>
							</div>
							<!--end::Menu item--><!--begin::Menu item-->
							<div class="menu-item px-3">
								<a href="#" id="RawProductPurchaseOrderList" class="menu-link px-3" data-bs-toggle="modal" data-reference-id = `+ full.referenceId + ` data-bs-target="#mes_rawProduct_purchaseOrder">Satınalma Siparişleri</a>
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

        const filterSearch = document.querySelector('[data-kt-ecommerce-category-filter="search"]');

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
            table = document.querySelector('#mes_rawProduct_table');

            if (!table) {
                console.log("table bulunamadı")
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
    RawProductList.init();
});
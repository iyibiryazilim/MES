﻿
"use strict";




var SemiProductOutputStockList = function () {

    // Shared variables
    var table;
    var datatable;


    // Private functions

    var initDatatable = function () {
        var productId = $('#ProductId').val()
        var postUrl = '/SemiProduct/GetOutputJsonResult?productReferenceId=' + productId;
        //console.log(postUrl)


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

                { data: 'productTransaction.transactionDate' },
                { data: 'subUnitset.unitset.name' },
                { data: 'quentity' },
                { data: 'warehouse.name' },
                { data: 'description' },
                { data: 'referenceId' },
            ],
            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;

                        output = `<td>` + full.productTransaction.transactionDate + `</td>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 1,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;

                        output = `<td>` + full.subUnitset.name + `</td>`
                        return output;

                    },

                },

                {

                    orderable: true,
                    targets: 2,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<td>` + full.quentity + `</td>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 3,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<td>` + full.warehouse.name + `</td>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<td>` + full.description + `</td>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 5,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<a href="#" class="btn btn-sm btn-light btn-active-light-primary">View</a>`
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

        const filterStatus = document.querySelector('[data-kt-ecommerce-product-filter="status"');

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
            table = document.querySelector('#mes_semiProductStockOutput_table');

            if (!table) {

                return;

            }

            initDatatable();
            //handleSearchDatatable();
            //handleStatusFilter();
            //handleDeleteRows();
        }

    };

}();

// On document ready

KTUtil.onDOMContentLoaded(function () {
    SemiProductOutputStockList.init();
});
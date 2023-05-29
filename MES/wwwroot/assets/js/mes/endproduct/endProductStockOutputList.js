"use strict";




// Class definition
var MESProductInputProductTransactionList = function () {
    // Shared variables
    var table;
    var datatable;

    // Private functions
    var initDatatable = function () {

        var referenceId;

        $(table).on('click', 'a#EndProductList', function (event) {
            referenceId = $(this).data('id');
            //console.log('Giriş Hareketleri asadsadsa için tıklandı. referenceId:', referenceId);
        });

        var postUrl = '../../EndProduct/GetInputJsonResult?productReferenceId=' + referenceId;

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
                { data: 'transactionDate' },
                { data: 'transactionCode' },
                { data: 'transactionType' },
                { data: 'quantity' },
                { data: 'unitsetCode' },
                { data: 'warehouseName' },
                { data: 'description' },

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
                        var formattedDate = new Date(full.transactionDate);
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
                        output = `<span class="fw-bold">` + full.transactionCode + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 3,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<span class="fw-bold">` + full.transactionType + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        output = `<span class="fw-bold">` + full.quantity + `</span>`

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
                        if (full.unitsetCode != null)
                            value = full.unitsetCode;

                        output = `<span class="fw-bold">` + value + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 6,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        var value = "";
                        if (full.warehouseName != null) {
                            value = full.warehouseName;
                        }
                        output = `<span class="fw-bold">` + value + `</span>`

                        return output;
                    },
                },
                {
                    orderable: true,
                    targets: 7,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        var output;
                        var value = "";
                        if (full.description != null) {
                            value = full.description;
                        }
                        output = `<span class="fw-bold">` + value + `</span>`

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
        const filterSearch = document.querySelector('[wms-input-product-transaction-table-filter="search"]');
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
            table = document.querySelector('#mes_output_transaction_table');

            if (!table) {
                return;
            }

            initDatatable();
            handleSearchDatatable();
            //handleStatusFilter();
            //handleDeleteRows();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    MESProductInputProductTransactionList.init();
});

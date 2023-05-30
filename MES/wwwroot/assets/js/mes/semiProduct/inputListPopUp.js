"use strict";

// Class definition
var InputShowModalPageInit = function () {

    var table;
    var datatable;
    var referenceId;

    var initDatatable = function () {

        var postUrl = 'SemiProduct/GetInputJsonResult?productReferenceId=' + referenceId;
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
    // Private functions

    var loadModalPage = function () {
        $('#mes_modal_input_transaction').on('shown.bs.modal', function () {
            console.log("Giriş Tablosu Açıldı")
            initDatatable();

        });
    };

    var bindEventHandlers = function () {
        $(document).on('click', 'a#SemiProductInputTransactionList', function () {
            referenceId = $(this).data('reference-id');
            console.log("aaaaa " + referenceId);
        });
    };

    // Public methods
    return {
        init: function () {
            table = document.querySelector('#mes_input_transaction_table');
            if (!table) {
                console.log("Girş hareketleri tablosu bulunamadı")
                return;
            }
            bindEventHandlers();
            loadModalPage();


        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    InputShowModalPageInit.init();
});
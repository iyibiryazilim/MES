
"use strict";

// Class definition
var PurchaseOrderShowModalPageInit = function () {

    var table;
    var datatable;
    var referenceId;

    var initDatatable = function () {

        var postUrl = '/PurchaseOrderLine/GetJsonResult';

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

                { data: 'product' },
                { data: 'quantity' },
                { data: 'description' },
                { data: 'warehouse' },
                { data: 'referenceId' },


            ],

            columnDefs: [
                {
                    orderable: true,
                    targets: 0,
                    render: function (data, type, full, meta) {

                        var output;

                        output = `<div class="form-check form-check-sm form-check-custom form-check-solid">
                            <input class="form-check-input" type="checkbox" value="`+ data + `" />
                        </div>`
                        return output;

                    },

                },

                {

                    orderable: true,
                    targets: 1,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="text-gray-800 fw-bold d-block fs-4">` + full.product.name + `</div>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 2,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="text-gray-800 fw-bold d-block fs-4">` + full.quantity + `</div>`
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 3,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {

                        var output;
                        output = `<div class="text-gray-800 fw-bold d-block fs-4">` + full.description + `</div>
                        
                        `
                        return output;

                    },

                },
                {

                    orderable: true,
                    targets: 4,
                    className: 'text-start pe-0',
                    render: function (data, type, full, meta) {
                        console.log(full)
                        if (full.warehouse != null) {
                            var value = full.warehouse.name
                        }
                        var output;
                        output = `<div class="text-gray-800 fw-bold d-block fs-4">` + value + `</div>
                        
                        `
                        return output;

                    },

                },
                {

                    orderable: false,
                    targets: 5,
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
    // Private functions

    var loadModalPage = function () {
        $('#mes_rawProduct_purchaseOrder').on('shown.bs.modal', function () {
            console.log("Alış Siparişleri Açıldı")
            initDatatable();

        });
    };

    var bindEventHandlers = function () {
        $(document).on('click', 'a#RawProductPurchaseOrderList', function () {
            referenceId = $(this).data('reference-id');
        });
    };

    // Public methods
    return {
        init: function () {
            table = document.querySelector('#mes_purchase_order_table');

            if (!table) {
                console.log("Alış Siparişler tablosu bulunamadı")
                return;
            }
            bindEventHandlers();
            loadModalPage();


        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    PurchaseOrderShowModalPageInit.init();
});
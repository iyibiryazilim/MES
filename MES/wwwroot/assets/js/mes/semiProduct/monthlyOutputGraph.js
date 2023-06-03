
var MESMonthlyOutput = function () {
    var referenceId = $('#ProductId').val()

    var e = {
        self: null, rendered: !1
    }, t = function (e) {
        var t = document.getElementById("MESMonthlyOutput");
        if (t == null)
            console.log("MESMonthlyOutput bulunamadı")

        $.post('../../SemiProduct/GetMonthlyOutputJsonResult?productReferenceId=' + referenceId, function (result) {


            var lastItem = result.data[result.data.length - 1];
            var monthlyOutputKeys = lastItem.monthlyOutputValues;
            var keys = Object.keys(monthlyOutputKeys);
            var monthlyOutputValues = lastItem.monthlyOutputValues;
            var values = Object.values(monthlyOutputValues);

            console.log("Son Elemanın Anahtarları:", keys);
            console.log("Son Elemanın Değerleri:", values);


            if (t) {
                var a = parseInt(KTUtil.css(t, "height")),
                    l = KTUtil.getCssVariableValue("--bs-gray-500"),
                    r = KTUtil.getCssVariableValue("--bs-border-dashed-color"),
                    o = KTUtil.getCssVariableValue("--bs-danger")
                i = {
                    series: [{ name: "Toplam", data: values }],
                    chart: {
                        fontFamily: "inherit", type: "area", height: a,
                        toolbar: { show: !1 }
                    },
                    legend: { show: !1 },
                    dataLabels: { enabled: !1 },
                    fill: {
                        type: "gradient",
                        gradient: { shadeIntensity: 1, opacityFrom: .4, opacityTo: 0, stops: [0, 80, 100] }
                    },
                    stroke: {
                        curve: "smooth", show: !0, width: 3, colors: [o]
                    },
                    xaxis: {
                        categories: keys,
                        axisBorder: {
                            show: !1
                        },
                        offsetX: 20,
                        axisTicks: { show: !1 },
                        tickAmount: 3,
                        labels: {
                            rotate: 0, rotateAlways: !1,
                            style: { colors: l, fontSize: "12px" }
                        },
                        crosshairs: {
                            position: "front", stroke: { color: o, width: 1, dashArray: 3 }
                        },
                        tooltip: {
                            enabled: !0, formatter: void 0, offsetY: 0, style: { fontSize: "12px" }
                        }
                    },
                    yaxis: {
                        labels: {
                            style: {
                                colors: l, fontSize: "12px"
                            },
                            formatter: function (e) {
                                return e
                            }
                        }
                    },
                    states: {
                        normal: {
                            filter: { type: "none", value: 0 }
                        },
                        hover: {
                            filter: { type: "none", value: 0 }
                        },
                        active: {
                            allowMultipleDataPointsSelection: !1, filter: {
                                type: "none", value: 0
                            }
                        }
                    },
                    tooltip: {
                        style: { fontSize: "12px" }, y: { formatter: function (e) { return e } }
                    },
                    colors: [o], grid: {
                        borderColor: r, strokeDashArray: 4, yaxis: { lines: { show: !0 } }
                    }, markers: {
                        strokeColor: o, strokeWidth: 3
                    }
                };
                e.self = new ApexCharts(t, i),
                    setTimeout((function () {
                        e.self.render(), e.rendered = !0
                    }), 200)
            }
        });
    };
    return {
        init: function () {
            t(e), KTThemeMode.on("kt.thememode.change",
                (function () {
                    e.rendered && e.self.destroy(),
                        t(e)
                }))
        }
    }
}();
"undefined" != typeof module && (module.exports = WMSMonthlyInput),
    KTUtil.onDOMContentLoaded((function () {
        MESMonthlyOutput.init()
    }));

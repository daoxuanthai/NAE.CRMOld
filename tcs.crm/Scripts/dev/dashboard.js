var from7day = "";
var to7day = "";
$(function () {
    from7day = $(".dashboard").data("from");
    to7day = $(".dashboard").data("to");
    statusChart();
    countryChart();
    dailyChart();
    contractStatusChart();
    contractCountryChart();
    contractDailyChart();
});

function statusChart() {
    $.ajax({
        url: '/Report/StatusSummary',
        data: { },
        success: function (response) {
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                buildStatusChart("status7day", data.Total, data.Contracted, data.MaybeContract, data.Potential, data.NotPotential, data.ContinueCare, data.NotCaring);
            }
        },
        cache: false
    });
}

function contractStatusChart() {
    $.ajax({
        url: '/Report/ContractStatusSummary',
        data: {},
        success: function (response) {
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                buildContractStatusChart("contractstatus7day", data.Total, data.New, data.Process, data.VisaFail, data.Complete, data.Liquidated);
            }
        },
        cache: false
    });
}

function countryChart() {
    $.ajax({
        url: '/Report/CountrySummary',
        data: {},
        success: function (response) {
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                var options = {
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false,
                        type: 'pie',
                        style: {
                            fontFamily: 'Helvetica'
                        }
                    },
                    title: {
                        text: 'Thống kê HSKH theo quốc gia'
                    },
                    subtitle: {
                        text: 'Từ <b> ' + from7day + '</b> đến <b> ' + to7day + '</b> - Tổng hồ sơ: <b>' + formatNumeric(data.Total) + '</b>',
                        x: -10
                    },
                    tooltip: {
                        formatter: function () {
                            return '<b>' + this.point.name + '</b><br/>' + formatNumeric(this.y) + ' hồ sơ';
                        }
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: false
                            },
                            showInLegend: true
                        }
                    },
                    series: []
                };

                var objdata = [];
                for (var i = 0; i < data.ListData.length; i++) {
                    var obj = data.ListData[i];
                    objdata[objdata.length] = { name: obj.Name, y: obj.Count };
                }
                options.series[0] = { name: "Thống kê", data: objdata};
                $('#country7day').highcharts(options);
            }
        },
        cache: false
    });
}

function contractCountryChart() {
    $.ajax({
        url: '/Report/ContractCountrySummary',
        data: {},
        success: function (response) {
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                var options = {
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false,
                        type: 'pie',
                        style: {
                            fontFamily: 'Helvetica'
                        }
                    },
                    title: {
                        text: 'Thống kê hợp đồng theo quốc gia'
                    },
                    subtitle: {
                        text: 'Từ <b> ' + from7day + '</b> đến <b> ' + to7day + '</b> - Tổng hồ sơ: <b>' + formatNumeric(data.Total) + '</b>',
                        x: -10
                    },
                    tooltip: {
                        formatter: function () {
                            return '<b>' + this.point.name + '</b><br/>' + formatNumeric(this.y) + ' hồ sơ';
                        }
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: false
                            },
                            showInLegend: true
                        }
                    },
                    series: []
                };

                var objdata = [];
                for (var i = 0; i < data.ListData.length; i++) {
                    var obj = data.ListData[i];
                    objdata[objdata.length] = { name: obj.Name, y: obj.Count };
                }
                options.series[0] = { name: "Thống kê", data: objdata };
                $('#contractcountry7day').highcharts(options);
            }
        },
        cache: false
    });
}

function dailyChart() {
    $.ajax({
        url: '/Report/CustomerDaily',
        data: {},
        success: function (response) {
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                var options = {
                    chart: {
                        renderTo: 'container',
                        type: 'line',
                        style: {
                            fontFamily: 'Helvetica'
                        }
                    },
                    title: {
                        text: 'Thống kê HSKH theo ngày'
                    },
                    subtitle: {
                        text: '',
                        x: -10
                    },
                    xAxis: {
                        categories: []
                    },
                    yAxis: {
                        title: {
                            text: 'Số hồ sơ'
                        }
                    },
                    tooltip: {
                        formatter: function () {
                            return '<b>' + this.series.name + '</b><br/>' +
                                this.x + ': ' + formatNumeric(this.y) + ' hồ sơ';
                        }
                    },
                    series: []
                };

                options.xAxis.categories = data.Categories.split(',');
                options.subtitle.text = 'Từ <b> ' + from7day + '</b> đến <b> ' + to7day + '</b> - Tổng số hồ sơ: <b>' + formatNumeric(data.Total) + '</b> - Tổng số hợp đồng: <b>' + formatNumeric(data.TotalContract) + '</b>';

                for (var i = 0; i < data.Series.Items.length; i++) {
                    var obj = data.Series.Items[i];
                    var objdata = [];
                    obj.Data.split(",").forEach(function (e) { objdata.push(parseInt(e, 10)) });
                    options.series[i] = { name: obj.Name, data: objdata };
                }
                $('#daily7day').highcharts(options);
            }
        },
        cache: false
    });
}

function contractDailyChart() {
    $.ajax({
        url: '/Report/ContractDaily',
        data: {},
        success: function (response) {
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                var options = {
                    chart: {
                        renderTo: 'container',
                        type: 'line',
                        style: {
                            fontFamily: 'Helvetica'
                        }
                    },
                    title: {
                        text: 'Thống kê hợp đồng theo ngày'
                    },
                    subtitle: {
                        text: '',
                        x: -10
                    },
                    xAxis: {
                        categories: []
                    },
                    yAxis: {
                        title: {
                            text: 'Số hợp đồng'
                        }
                    },
                    tooltip: {
                        formatter: function () {
                            return '<b>' + this.series.name + '</b><br/>' +
                                this.x + ': ' + formatNumeric(this.y) + ' hợp đồng';
                        }
                    },
                    series: []
                };

                options.xAxis.categories = data.Categories.split(',');
                options.subtitle.text = 'Từ <b> ' + from7day + '</b> đến <b> ' + to7day + '</b> - Tổng số hợp đồng: <b>' + formatNumeric(data.Total) + '</b>';

                for (var i = 0; i < data.Series.Items.length; i++) {
                    var obj = data.Series.Items[i];
                    var objdata = [];
                    obj.Data.split(",").forEach(function (e) { objdata.push(parseInt(e, 10)) });
                    options.series[i] = { name: obj.Name, data: objdata };
                }
                $('#contractdaily7day').highcharts(options);
            }
        },
        cache: false
    });
}

function buildStatusChart(name, total, totalContract, maybeContract, potential, notPotential, continueCare, notCare) {
    $('#' + name).highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie',
            style: {
                fontFamily: 'Helvetica'
            }
        },
        title: {
            text: "Thống kê HSKH theo trạng thái"
        },
        subtitle: {
            text: 'Từ <b> ' + from7day + '</b> đến <b> ' + to7day + '</b> - Tổng hồ sơ: <b>' + formatNumeric(total) + '</b>',
            x: -10
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.point.name + '</b><br/>' + formatNumeric(this.y) + ' hồ sơ';
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Thống kê',
            data: [
                { name: 'Đã ký hợp đồng', y: totalContract },
                { name: 'Có thể ký hợp đồng', y: maybeContract },
                { name: 'KH tiềm năng', y: potential },
                { name: 'KH không tiềm năng', y: notPotential },
                { name: 'Chưa chăm sóc', y: notCare },
                { name: 'Tiếp tục chăm sóc', y: continueCare }
            ]
        }]
    });
}

function buildContractStatusChart(name, total, newcontract, process, visaFail, complete, liquidated) {
    $('#' + name).highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie',
            style: {
                fontFamily: 'Helvetica'
            }
        },
        title: {
            text: "Thống kê hợp đồng theo trạng thái"
        },
        subtitle: {
            text: 'Từ <b> ' + from7day + '</b> đến <b> ' + to7day + '</b> - Tổng hồ sơ: <b>' + formatNumeric(total) + '</b>',
            x: -10
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.point.name + '</b><br/>' + formatNumeric(this.y) + ' hồ sơ';
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Thống kê',
            data: [
                { name: 'Mới ký hợp đồng', y: newcontract },
                { name: 'Đang xử lý', y: process },
                { name: 'Rớt visa', y: visaFail },
                { name: 'Hoàn tất', y: complete },
                { name: 'Thanh lý', y: liquidated }
            ]
        }]
    });
}

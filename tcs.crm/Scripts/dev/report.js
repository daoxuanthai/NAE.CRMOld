var chart1; // globally available
var objSelect;
$(function () {
    if ($(".multipleselect").length > 0) {
        objSelect = $(".multipleselect").multipleSelect();
    }

    Highcharts.setOptions({
        colors: ["#f45b5b", "#8085e9", "#8d4654", "#7798BF", "#aaeeee", "#ff0066", "#eeaaee",
		"#55BF3B", "#DF5353", "#7798BF", "#aaeeee"]
    });

    $("#btnViewReport").click();

});

function excelCustomerMonthly() {
    var fromDate = '01/' + $('#FromMonth').val() + '/' + $('#FromYear').val();
    var toDate = '01/' + $('#ToMonth').val() + '/' + $('#ToYear').val();
    var reportType = 'rptCustomerMonthly';
    var country = objSelect.multipleSelect('getSelects').toString();
    window.open('/CMSReport/ExportExcel/?fromDate=' + fromDate + '&to=' + toDate + '&reportType=' + reportType + '&country=' + country);
    return false;
}

function excelCustomerDaily()
{
    var fromDate = $('#FromDate').val();
    var toDate = $('#ToDate').val();
    var reportType = 'rptCustomerDaily';
    var country = objSelect.multipleSelect('getSelects').toString();
    window.open('/CMSReport/ExportExcel/?fromDate=' + fromDate + '&to=' + toDate + '&reportType=' + reportType + '&country=' + country);
    return false;
}

formatNumeric = function (val) { if (val != '') { var a = parseInt(val); val = a.toString(); } val += ''; x = val.split('.'); x1 = x[0]; x2 = x.length > 1 ? ',' + x[1] : ''; var rgx = /(\d+)(\d{3})/; while (rgx.test(x1)) { x1 = x1.replace(rgx, '$1' + ',' + '$2'); } return x1 + x2; }

function viewCustomerDailyReport() {
    var fromDate = $('#FromDate').val();
    var toDate = $('#ToDate').val();
    var reportType = 'rptCustomerDaily';
    var country = objSelect.multipleSelect('getSelects').toString();
    $.ajax({
        url: '/CMSReport/ViewReport',
        data: { 'from': fromDate, 'to': toDate, 'type': reportType, 'country': country },
        success: function (data) {
            var options = {
                chart: {
                    renderTo: 'container',
                    type: 'line'
                },
                title: {
                    text: 'Thống kê hồ sơ khách hàng theo ngày'
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

            var jsonContent = $.parseJSON("[" + data.Chart + "]");

            options.xAxis.categories = jsonContent[0].data.categories.split(',');
            options.subtitle.text = 'Từ <b>' + $('#FromDate').val() + '</b> đến <b>' + $('#ToDate').val() + '</b>' + '<br>Tổng số hồ sơ: <b>' + formatNumeric(data.Total) + '</b> - Tổng số hợp đồng: <b>' + formatNumeric(data.TotalContract) + '</b>';

            for (var i = 0; i < jsonContent[0].data.series.items.length; i++) {
                var obj = jsonContent[0].data.series.items[i];
                var objdata = [];
                obj.data.split(",").forEach(function (e) { objdata.push(parseInt(e, 10)) });
                options.series[i] = { name: obj.name, data: objdata };
            }
            chart1 = new Highcharts.Chart(options);
            getSummaryDailyReport();
            //buildSummaryChart(data.Total, data.TotalContract, data.TotalMaybeContract, data.TotalPotential, data.TotalNotPotential, data.TotalContinueCare, data.TotalNotCare);
        },
        cache: false
    });
    if ($("#listCustomer").length > 0) {
        viewCustomerDataDailyReport();
    }
}

function viewCustomerMonthlyReport() {
    var fromDate = '01/' + $('#FromMonth').val() + '/' + $('#FromYear').val();
    var toDate = '01/' + $('#ToMonth').val() + '/' + $('#ToYear').val();
    var reportType = 'rptCustomerMonthly';
    var country = objSelect.multipleSelect('getSelects').toString();
    $.ajax({
        url: '/CMSReport/ViewReport',
        data: { 'from': fromDate, 'to': toDate, 'type': reportType, 'country': country },
        success: function (data) {
            var options = {
                chart: {
                    renderTo: 'container',
                    type: 'line'
                },
                title: {
                    text: 'Thống kê hồ sơ khách hàng theo tháng'
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

            var jsonContent = $.parseJSON("[" + data.Chart + "]");

            options.xAxis.categories = jsonContent[0].data.categories.split(',');
            options.subtitle.text = 'Từ <b>' + $('#FromMonth').val() + '/' + $('#FromYear').val() + '</b> đến <b>' + $('#ToMonth').val() + '/' + $('#ToYear').val() + '</b>' + '<br>Tổng số hồ sơ: <b>' + formatNumeric(data.Total) + '</b> - Tổng số hợp đồng: <b>' + formatNumeric(data.TotalContract) + '</b>';

            for (var i = 0; i < jsonContent[0].data.series.items.length; i++) {
                var obj = jsonContent[0].data.series.items[i];
                var objdata = [];
                obj.data.split(",").forEach(function (e) { objdata.push(parseInt(e, 10)) });
                options.series[i] = { name: obj.name, data: objdata };
            }
            chart1 = new Highcharts.Chart(options);
            getSummaryMonthlyReport();
            //buildSummaryChart(data.Total, data.TotalContract, data.TotalMaybeContract, data.TotalPotential, data.TotalNotPotential, data.TotalContinueCare, data.TotalNotCare);
        },
        cache: false
    });
}

function viewCustomerDataByCountryReport() {
    var fromDate = '01/' + $('#FromMonth').val() + '/' + $('#FromYear').val();
    var toDate = '01/' + $('#ToMonth').val() + '/' + $('#ToYear').val();
    var reportType = 'rptCustomerDataByCountry';
    var country = $("#ddlCountry").val();
    $.ajax({
        url: '/CMSReport/ViewReport',
        data: { 'from': fromDate, 'to': toDate, 'type': reportType, 'country': country },
        success: function (data) {
            var options = {
                chart: {
                    renderTo: 'container',
                    type: 'line'
                },
                title: {
                    text: 'Thống kê hồ sơ khách hàng theo quốc gia'
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

            var jsonContent = $.parseJSON("[" + data.Chart + "]");

            options.xAxis.categories = jsonContent[0].data.categories.split(',');
            options.subtitle.text = 'Từ <b>' + $('#FromMonth').val() + '/' + $('#FromYear').val() + '</b> đến <b>' + $('#ToMonth').val() + '/' + $('#ToYear').val() + '</b>' + '<br>Tổng số hồ sơ: <b>' + formatNumeric(data.Total) + '</b> - Tổng số hợp đồng: <b>' + formatNumeric(data.TotalContract) + '</b>';

            for (var i = 0; i < jsonContent[0].data.series.items.length; i++) {
                var obj = jsonContent[0].data.series.items[i];
                var objdata = [];
                obj.data.split(",").forEach(function (e) { objdata.push(parseInt(e, 10)) });
                options.series[i] = { name: obj.name, data: objdata };
            }
            chart1 = new Highcharts.Chart(options);
            getSummaryMonthlyByCountryReport();
        },
        cache: false
    });
}

function viewVisaMonthlyReport() {
    var fromDate = '01/' + $('#FromMonth').val() + '/' + $('#FromYear').val();
    var toDate = '01/' + $('#ToMonth').val() + '/' + $('#ToYear').val();
    var reportType = 'rptVisaMonthly';
    var country = objSelect.multipleSelect('getSelects').toString();
    $.ajax({
        url: '/CMSReport/ViewReport',
        data: { 'from': fromDate, 'to': toDate, 'type': reportType, 'country': country },
        success: function (data) {
            var options = {
                chart: {
                    renderTo: 'container',
                    type: 'line'
                },
                title: {
                    text: 'Thống kê visa theo tháng'
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
                        text: 'Số visa'
                    }
                },
                tooltip: {
                    formatter: function () {
                        return '<b>' + this.series.name + '</b><br/>' +
                        this.x + ': ' + formatNumeric(this.y) + ' visa';
                    }
                },
                series: []
            };

            var jsonContent = $.parseJSON("[" + data.Chart + "]");

            options.xAxis.categories = jsonContent[0].data.categories.split(',');
            options.subtitle.text = 'Từ <b>' + $('#FromMonth').val() + '/' + $('#FromYear').val() + '</b> đến <b>' + $('#ToMonth').val() + '/' + $('#ToYear').val() + '</b>' + '<br>Tổng số visa: <b>' + formatNumeric(data.Total) + '</b>';

            for (var i = 0; i < jsonContent[0].data.series.items.length; i++) {
                var obj = jsonContent[0].data.series.items[i];
                var objdata = [];
                obj.data.split(",").forEach(function (e) { objdata.push(parseInt(e, 10)) });
                options.series[i] = { name: obj.name, data: objdata };
            }
            chart1 = new Highcharts.Chart(options);
            getSummaryVisaMonthlyReport(data.Total, data.TotalUc, data.TotalAnh, data.TotalThuySy, data.TotalPhilippin, data.TotalCanada, data.TotalNewZealand, data.TotalPhap, data.TotalTayBanNha, data.TotalDaiLoan, data.TotalMy, data.TotalSingapore, data.TotalKhac);
        },
        cache: false
    });
}

// Radialize the colors
Highcharts.getOptions().colors = Highcharts.map(Highcharts.getOptions().colors, function (color) {
    return {
        radialGradient: {
            cx: 0.5,
            cy: 0.3,
            r: 0.7
        },
        stops: [
            [0, color],
            [1, Highcharts.Color(color).brighten(-0.3).get('rgb')] // darken
        ]
    };
});

function getSummaryDailyReport() {
    var fromDate = $('#FromDate').val();
    var toDate = $('#ToDate').val();
    var reportType = 'rptSummaryDailyReport';
    var country = objSelect.multipleSelect('getSelects').toString();
    $.ajax({
        url: '/CMSReport/ViewReport',
        data: { 'from': fromDate, 'to': toDate, 'type': reportType, 'country': country },
        success: function (data) {
            buildSummaryChart(data.Total, data.TotalContract, data.TotalMaybeContract, data.TotalPotential, data.TotalNotPotential, data.TotalContinueCare, data.TotalNotCare);
            // cập nhật tổng số hồ sơ và tổng số hợp đồng

        },
        cache: false
    });
}

function getSummaryMonthlyReport() {
    var fromDate = '01/' + $('#FromMonth').val() + '/' + $('#FromYear').val();
    var toDate = '01/' + $('#ToMonth').val() + '/' + $('#ToYear').val();
    var reportType = 'rptSummaryMonthlyReport';
    var country = objSelect.multipleSelect('getSelects').toString();
    $.ajax({
        url: '/CMSReport/ViewReport',
        data: { 'from': fromDate, 'to': toDate, 'type': reportType, 'country': country },
        success: function (data) {
            buildSummaryChart(data.Total, data.TotalContract, data.TotalMaybeContract, data.TotalPotential, data.TotalNotPotential, data.TotalContinueCare, data.TotalNotCare);
        },
        cache: false
    });
}

function getSummaryVisaMonthlyReport(total, totalUc, totalAnh, totalThuySy, totalPhilippin, totalCanada, totalNewZealand, totalPhap, totalTayBanNha, totalDaiLoan, totalMy, totalSingapore, totalKhac) {
    buildVisaSummaryChart(total, totalUc, totalAnh, totalThuySy, totalPhilippin, totalCanada, totalNewZealand, totalPhap, totalTayBanNha, totalDaiLoan, totalMy, totalSingapore, totalKhac);
}

function getSummaryMonthlyByCountryReport() {
    var fromDate = '01/' + $('#FromMonth').val() + '/' + $('#FromYear').val();
    var toDate = '01/' + $('#ToMonth').val() + '/' + $('#ToYear').val();
    var reportType = 'rptSummaryMonthlyByCountryReport';
    var country = $("#ddlCountry").val();
    $.ajax({
        url: '/CMSReport/ViewReport',
        data: { 'from': fromDate, 'to': toDate, 'type': reportType, 'country': country },
        success: function (data) {
            buildSummaryChart(data.Total, data.TotalContract, data.TotalMaybeContract, data.TotalPotential, data.TotalNotPotential, data.TotalContinueCare, data.TotalNotCare);
        },
        cache: false
    });
}

function buildSummaryChart(total, totalContract, maybeContract, potential, notPotential, continueCare, notCare) {
    $('#summary').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: 'Tổng hồ sơ: <b>' + formatNumeric(total) + '</b>'
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

function buildVisaSummaryChart(total, totalUc, totalAnh, totalThuySy, totalPhilippin, totalCanada, totalNewZealand, totalPhap, totalTayBanNha, totalDaiLoan, totalMy, totalSingapore, totalKhac) {
    $('#summary').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: 'Tổng visa: <b>' + formatNumeric(total) + '</b>'
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.point.name + '</b><br/>' + formatNumeric(this.y) + ' visa';
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
                { name: 'Úc', y: totalUc },
                { name: 'Anh', y: totalAnh },
                { name: 'Thụy sỹ', y: totalThuySy },
                { name: 'Philippin', y: totalPhilippin },
                { name: 'Canada', y: totalCanada },
                { name: 'New zealand', y: totalNewZealand },
                { name: 'Pháp', y: totalPhap },
                { name: 'Tây ban nha', y: totalTayBanNha },
                { name: 'Đài loan', y: totalDaiLoan },
                { name: 'Mỹ', y: totalMy },
                { name: 'Singapore', y: totalSingapore },
                { name: 'Khác', y: totalKhac }
            ]
        }]
    });
}

function viewCustomerDataDailyReport() {
    var fromDate = $('#FromDate').val();
    var toDate = $('#ToDate').val();
    var reportType = 'rptCustomerDataDaily';
    var country = objSelect.multipleSelect('getSelects').toString();
    $.ajax({
        url: '/CMSReport/ViewReport',
        data: { 'from': fromDate, 'to': toDate, 'type': reportType, 'country': country },
        success: function (data) {
            $("#listCustomer").html(data.Data);
        },
        cache: false
    });
}
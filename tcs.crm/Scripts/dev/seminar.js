var seminarId = 0;
$(function () {
    $(".customername, .updatelink").click(function () {
        var id = parseInt($(this).data("id"));
        loadSeminar(id);
    });
});

function submitSeminar() {
    if (loading)
        return false;
    var frm = "frmSeminar";
    showLoading();
    hideAllError(frm);

    $.post("/Seminar/Detail", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function initTabEvent() {
    $("#tabs ul li a").click(function () {
        var tab = $(this).data("tab");
        if ($("#" + tab).is(":hidden")) {
            $("#tabs ul li").removeClass("active");
            $(this).parent().addClass("active");
            $(".tabs-ct").removeClass("active");
            $("#" + tab).addClass("active");
            var ps = new PerfectScrollbar("#" + tab);
            $.fancybox.update();
            $("#ProvinceId").select2({ width: "100%" });
        }
    });
    initSpecialInput();
}

function initInsertUpdate(name) {
    $("#frm" + name + " #Id").val("0");
    $("#frm" + name).trigger("reset");
    $("#frm" + name + " .divaction").hide();
    $("#IU" + name).show();
    $("#dt_SeminarPlacelist").hide();
    return false;
}

function finishInsertUpdate(name) {
    $("#IU" + name).hide();
    $("#frm" + name + " .divaction").show();
    $("#dt_SeminarPlacelist").show();
    return false;
}

function deleteSeminar() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbseminar").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin hội thảo đang chọn?")) {
            showLoading();
            $.post("/Seminar/DeleteSeminar", { "ids": iddelete },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#Seminar_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}

function loadSeminar(id) {
    $.get("/Seminar/Detail", { "id": id },
        function (response) {
            hideLoading();
            seminarId = id;
            var data = JSON.parse(response.Msg);
            if (response.Code === 200) {
                var obj = $(data.Html);
                $.fancybox(obj,
                    {
                        'showCloseButton': false,
                        'autoScale': false,
                        'modal': true,
                        'margin': 0,
                        'padding': 0,
                        'closeBtn': false,
                        'scrolling': 'no',
                        'helpers': {
                            'overlay': { 'closeClick': false }
                        }
                    });
                initTabEvent();
                $("#btnSaveSeminar").click(function () {
                    var frm = $(this).data("frm");
                    submitSeminar(frm);
                    return false;
                });
            }
            else {
                alert(data.Message);
            }
        });
    return false;
}

function editSeminarPlace(id) {
    if (loading) return false;

    showLoading();
    $.get("/Seminar/SeminarPlace", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmSeminarPlace #Id").val(data.Id);
                $("#frmSeminarPlace #Place").val(data.Place);
                $("#frmSeminarPlace #Address").val(data.Address);
                
                $("#frmSeminarPlace #ProvinceId").val(formatNumeric(data.ProvinceId));
                $("#frmSeminarPlace #ProvinceId").select2({ width: "100%" });
                $("#frmSeminarPlace #SeminarDateString").val(data.SeminarDateString);
                $("#frmSeminarPlace #Status").val(data.Status);
                $("#frmSeminarPlace #Note").val(data.Note);
                $("#frmSeminarPlace .divaction").hide();
                $("#dt_SeminarPlacelist").hide();
                $("#IUSeminarPlace").show();
            }
        });
    return false;
}

function submitSeminarPlace(frm) {
    if (loading)
        return false;

    showLoading();
    hideAllError(frm);
    $.post("/Seminar/SeminarPlace", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_SeminarPlacelist").replaceWith(data.Html);
                }
                $(".divaction").show();
                $("#IUSeminarPlace").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function deleteSeminarPlace() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbSeminarPlace").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin địa điểm tổ chức hội thảo đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteSeminarPlace", { "ids": iddelete, "cid": customerId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#seminarplace_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}
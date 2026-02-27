var seminarId = 0;
$(function () {
    $("#SeminarPlaceId").select2();
    $(".customername, .updatelink").click(function () {
        var seminarId = $("#SeminarId").val();
        var id = parseInt($(this).data("id"));
        var cusstomerid = parseInt($(this).data("cid"));
        //loadSeminarRegister(id, seminarId);
        window.location = "/Customer?view=" + cusstomerid + "&tab=tabs-seminar";
    });
    if ($("#btnexcel").length > 0) {
        $("#btnexcel").click(function () {
            var seminar = $("#SeminarId").val();
            var place = $("#SeminarPlaceId").val();
            var title = $("#TitleId").val();
            window.open('/SeminarRegister/ExportExcel/?id=' + seminar + '&place=' + place + '&title=' + title);
        });
    }
});

function submitSeminarRegister() {
    if (loading)
        return false;
    var frm = "frmSeminarRegister";
    showLoading();
    hideAllError(frm);

    $.post("/SeminarRegister/Detail", $("#" + frm).serialize(),
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

function loadSeminarRegister(id, seminarId) {
    $.get("/SeminarRegister/Detail", { "id": id, "seminarId": seminarId },
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
                $("#btnSaveSeminarRegister").click(function () {
                    var frm = $(this).data("frm");
                    submitSeminarRegister(frm);
                    return false;
                });
            }
            else {
                alert(data.Message);
            }
        });
    return false;
}

function deleteSeminarRegister(id, name) {
    var msg = "Bạn chắc chắn muốn xóa thông tin đăng ký hội thảo được chọn?";
    if (name !== undefined && name !== "")
    {
        msg = "Bạn chắc chắn muốn xóa thông tin đăng ký hội thảo của khách hàng " + name + "?";
    }
    if (confirm(msg)) {
        $.post("/SeminarRegister/Delete", { "ids": id},
            function (response) {
                hideLoading();
                if (response.Code === 200) {
                    $("#Register_" + id).remove();
                }
            });
    }
    return false;
}

function deleteRegister() {
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
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin đăng ký hội thảo đang chọn?")) {
            showLoading();
            $.post("/SeminarRegister/Delete", { "ids": iddelete },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#Register_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }

    return false;
}
$(function () {
    $(".customername, .updatelink").click(function () {
        var id = parseInt($(this).data("id"));
        insertUpdate(id);
    });
});

function insertUpdate(id) {
    if (loading) return false;
    showLoading();
    $.get("/CompanyOffice/Detail", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code == 200) {
                var data = JSON.parse(response.Msg);
                var obj = $(data.Html);
                $(obj).find(".divbtn .btn-info").click(function () {
                    return submit("frmCompanyOffice");
                });
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
                initSpecialInput();
            }
        });
    return false;
}

function submit(frm) {
    if (loading)
        return false;
    showLoading();
    hideAllError(frm);
    $.post("/CompanyOffice/Detail", $("#" + frm).serialize(),
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

function deleteOffice() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbOffice").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin vị trí đang chọn?")) {
            showLoading();
            $.post("/CompanyOffice/Delete", { "ids": iddelete, companyId: companyId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#Office_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }

    return false;
}
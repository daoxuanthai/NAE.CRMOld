$(function () {
    $(".customername, .updatelink").click(function () {
        var id = parseInt($(this).data("id"));
        insertUpdate(id);
    });
});

function lockCompanyTitle(id, cid) {
    var lock = $("#lock_" + id).is(":checked");
    $.post("/CompanyTitle/LockCompanyTitle", { "id": id, "cId": cid, "isLock": lock },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                alert(data.Message);
            } else {
                var data = JSON.parse(response.Msg);
                alert(data.Message);
            }
        });
    return false;
}

function insertUpdate(id) {
    if (loading) return false;
    showLoading();
    $.get("/CompanyTitle/Detail", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code == 200) {
                var data = JSON.parse(response.Msg);
                var obj = $(data.Html);
                $(obj).find(".divbtn .btn-info").click(function () {
                    return submit("frmCompanyTitle");
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
                $('#frmCompanyTitle #UserId').select2({ width: '100%' });
            }
        });
    return false;
}

function submit(frm) {
    if (loading)
        return false;
    showLoading();
    hideAllError(frm);
    $.post("/CompanyTitle/Detail", $("#" + frm).serialize(),
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

function deleteTitle() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbTitle").each(function (index) {
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
            $.post("/CompanyTitle/Delete", { "ids": iddelete, companyId: companyId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#Title_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }

    return false;
}
$(function () {
    $(".customername, .updatelink").click(function () {
        var id = parseInt($(this).data("id"));
        insertUpdate(id);
    });
});

function insertUpdate(id) {
    if (loading) return false;
    showLoading();
    $.get("/ProfileType/Detail", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code == 200) {
                var data = JSON.parse(response.Msg);
                var obj = $(data.Html);
                $(obj).find(".divbtn .btn-info").click(function () {
                    return submit("frmProfileType");
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
                $('#frmProfileType #Country').select2({ width: '100%' });
            }
        });
    return false;
}

function submit(frm) {
    if (loading)
        return false;
    showLoading();
    hideAllError(frm);
    $.post("/ProfileType/Detail", $("#" + frm).serialize(),
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

function deleteProfileType() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbProfileType").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin loại hồ sơ đang chọn?")) {
            showLoading();
            $.post("/ProfileType/Delete", { "ids": iddelete, companyId: companyId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#ProfileType_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }

    return false;
}
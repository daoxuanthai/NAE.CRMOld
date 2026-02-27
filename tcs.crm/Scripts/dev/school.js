$(function () {
    $(".customername, .updatelink").click(function () {
        var id = parseInt($(this).data("id"));
        insertUpdate(id);
    });
});

function insertUpdate(id) {
    if (loading) return false;
    showLoading();
    $.get("/School/Detail", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code == 200) {
                var data = JSON.parse(response.Msg);
                var obj = $(data.Html);
                $(obj).find(".divbtn .btn-info").click(function () {
                    return submit("frmSchool");
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
                $('#frmSchool #Country').select2({ width: '100%' });
            }
        });
    return false;
}

function submit(frm) {
    if (loading)
        return false;
    showLoading();
    hideAllError(frm);
    $.post("/School/Detail", $("#" + frm).serialize(),
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

function deleteSchool() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbSchool").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin trường đang chọn?")) {
            showLoading();
            $.post("/School/Delete", { "ids": iddelete, companyId: companyId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#School_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }

    return false;
}
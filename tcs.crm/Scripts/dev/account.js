$(function () {
    $("#AvatarFile").change(function () {
        var fileName = $('#AvatarFile')[0].files[0].name;
        if (fileName === "" || fileName === undefined) {
            $("#inputfilename").hide();
            $("#inputfilename").html("");
        } else {
            $("#inputfilename").show();
            $("#inputfilename").html(fileName);
        }
    });
    $("#updateForm").submit(function () {
        if ($("#FullName").val() === "") {
            $(".frmerror").html("Vui lòng nhập họ tên");
            $("#FullName").focus();
            return false;
        }
        if ($("#Phone").val() === "") {
            $(".frmerror").html("Vui lòng nhập số điện thoại");
            $("#Phone").focus();
            return false;
        }
        if ($("#Email").val() === "") {
            $(".frmerror").html("Vui lòng nhập địa chỉ email");
            $("#Email").focus();
            return false;
        }
        return true;
    });
});

var isChange = false;
$(function () {
    $(".customername, .updatelink").click(function () {
        var id = parseInt($(this).data("id"));
        viewUserInfo(id);
    });
});

function viewUserInfo(id) {
    if (loading) return false;
    showLoading();
    $.get("/Account/Detail", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code !== 200) {
                var data = JSON.parse(response.Msg);
                alert(data.Message);
            }
            else {
                isChange = false;
                var data = JSON.parse(response.Msg);
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
                initSpecialInput();
            }
        });
    return false;
}

function submitUser(frm) {
    if (loading)
        return false;
    showLoading();
    hideAllError(frm);
    $.post("/Account/Detail", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                alert(data.Message);
                isChange = true;
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function deleteAccount() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbUser").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả tài khoản đang chọn?")) {
            showLoading();
            $.post("/Account/Delete", { "ids": iddelete },
                function (response) {
                    if (response.Code === 200) {
                        $('#searchForm').submit();
                    }
                    hideLoading();
                });
        }
    }

    return false;
}

function resetPassword(userName) {
    if (confirm("Bạn có muốn reset mật khẩu của tài khoản đang chọn về mật khẩu mặc định?")) {
        $.post("/Account/ResetPassword", { "userName": userName },
            function (response) {
                hideLoading();
                if (response.Code === 200) {
                    alert("Reset mật khẩu thành công");
                } else {
                    alert("Reset mật khẩu thất bại vui lòng thử lại sau");
                }
            });
    }

    return false;
}

function lockUser(id) {
    isChange = false;
    var lock = $("#lockuser_" + id).is(":checked");
    $.post("/Account/LockUser", { "id": id, "isLock": lock },
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

function closePopup() {
    $.fancybox.close();
    if (isChange) {
        $('#searchForm').submit();
    }
    return false;
}
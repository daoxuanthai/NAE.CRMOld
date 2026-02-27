$(function () {
    $(".provincelist").change(function () {
        var id = $(this).data("id");
        var office = $(this).val();
        setCompanyProvince(id, office);
    });
});

function setCompanyProvince(id, oid) {
    $.post("/CompanyProvince/UpdateProvince", { "id": id, "oId": oid },
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

function provincePermission() {
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
        if (confirm("Bạn chắc chắn muốn phân quyền tất cả tỉnh thành đang chọn?")) {
            showLoading();
            var officeId = $("#ddlPermission").val();
            $.post("/CompanyProvince/ProvincePermission", { "ids": iddelete, "officeId": officeId },
                function (response) {
                    if (response.Code === 200) {
                        alert("Cập nhật thông tin thành công");
                    }
                    hideLoading();
                });
        }
    }
}
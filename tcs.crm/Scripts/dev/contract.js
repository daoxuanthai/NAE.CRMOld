var contractId = 0;
$(function () {
    $(".updatelink").click(function () {
        var id = parseInt($(this).data("cid"));
        loadContract(id);
    });
});

function submitContract() {
    if (loading)
        return false;
    var frm = "frmContract";
    showLoading();
    hideAllError(frm);

    $.post("/Contract/ContractInfo", $("#" + frm).serialize(),
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
            if (tab === "tabs-profile") {
                var cid = parseInt($(this).data("id"));
                loadContractDocument(cid);
            }
        }
    });
    initSpecialInput();
}

function uploadFile(id) {
    var formData = new FormData();
    var totalFiles = document.getElementById("AttachFile").files.length;
    if (totalFiles <= 0) {
        alert("Vui lòng chọn file cần upload");
        return false;
    }
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("AttachFile").files[i];
        formData.append("AttachFile", file);
    }

    var xhr = new XMLHttpRequest();
    xhr.file = file; // not necessary if you create scopes like this
    xhr.addEventListener('progress', function (e) {
        var done = e.position || e.loaded, total = e.totalSize || e.total;
        console.log('xhr progress: ' + (Math.floor(done / total * 1000) / 10) + '%');
    }, false);
    if (xhr.upload) {
        xhr.upload.onprogress = function (e) {
            var done = e.position || e.loaded, total = e.totalSize || e.total;
            console.log('xhr.upload progress: ' + done + ' / ' + total + ' = ' + (Math.floor(done / total * 1000) / 10) + '%');
        };
    }
    xhr.onreadystatechange = function (e) {
        if (4 === this.readyState) {
            var response = JSON.parse(this.responseText);
            var data = JSON.parse(response.Msg);
            if (response.Code === 200) {
                $("#filelink").removeClass("hide");
                $("#filelink a").attr("href", data.Message).html(file.name);
            } else {
                alert(data.Message);
            }
        }
    };
    xhr.open('post', '/Contract/UploadFile', true);
    xhr.setRequestHeader("X-File-Name", file.name);
    xhr.setRequestHeader("X-File-Size", file.size);
    xhr.setRequestHeader("X-Contract-Id", id);
    xhr.setRequestHeader("Content-Type", "multipart/form-data");
    xhr.send(formData);
    return false;
}

function loadContractDocument(id) {
    showLoading();
    $.get("/Contract/LoadContractDocument", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#documentinfo").html(data.Html);
                $.fancybox.update();
            }
            else {
                $("#documentinfo").html("");
            }
        });
    return false;
}

function initInsertUpdate(name) {
    $("#frm" + name + " #Id").val("0");
    $("#frm" + name).trigger("reset");
    $("#frm" + name + " .divaction").hide();
    $("#dt_commissionlist").hide();
    $("#IU" + name).show();
    return false;
}

function finishInsertUpdate(name) {
    $("#IU" + name).hide();
    $("#frm" + name + " .divaction").show();
    $("#dt_commissionlist").show();
    return false;
}

function loadContract(id) {
    $.get("/Contract/Detail", { "customerId": id },
        function (response) {
            hideLoading();
            contractId = id;
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
                if ($("#frmContract #EmployeeId").is('select')) {
                    $("#frmContract #EmployeeId").select2({ width: "100%" });
                }
                if ($("#frmContract #SchoolId").is('select')) {
                    $("#frmContract #SchoolId").select2({ width: "100%" });
                }
                if ($("#frmContract #ScholarshipId").is('select')) {
                    $("#frmContract #ScholarshipId").select2({ width: "100%" });
                }
                
                $("#btnSaveContract").click(function () {
                    var frm = $(this).data("frm");
                    submitContract(frm);
                    return false;
                });
                $("#btnSaveCommission").click(function () {
                    var frm = $(this).data("frm");
                    submitCommissionInfo(frm);
                    return false;
                });

                $(document.body).on("change", "#SchoolId", function () {
                    loadScholarship(this.value);
                });
            }
            else {
                alert(data.Message);
            }
        });
    return false;
}

function loadScholarship(id) {
    if (loading) return false;
    showLoading();
    $.get("/Scholarship/LoadScholarship", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code !== 200) {
                alert("Có lỗi xảy ra vui lòng thử lại sau");
            }
            else {
                var data = JSON.parse(response.Msg);
                $("#ScholarshipId").select2('destroy');
                $("#ScholarshipId").replaceWith(data.Html);
                $("#ScholarshipId").select2({ width: "100%" });
            }
        });
    return false;
}

function editCommission(id) {
    if (loading) return false;

    showLoading();
    $.get("/Contract/CommissionInfo", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmCommissionInfo #Id").val(data.Id);
                $("#frmCommissionInfo #Name").val(data.Name);
                $("#frmCommissionInfo #Commission").val(formatNumeric(data.Commission));
                $("#frmCommissionInfo #CommissionDateString").val(data.CommissionDateString);
                $("#frmCommissionInfo #IsCollect").val(data.IsCollect);
                $("#frmCommissionInfo #Note").val(data.Note);
                $("#frmCommissionInfo .divaction").hide();
                $("#dt_commissionlist").hide();
                $("#IUCommissionInfo").show();
            }
        });
    return false;
}

function submitCommissionInfo(frm) {
    if (loading)
        return false;

    showLoading();
    hideAllError(frm);
    $.post("/Contract/CommissionInfo", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_commissionlist").replaceWith(data.Html);
                }
                $(".divaction").show();
                $("#IUCommissionInfo").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function deleteCommissionInfo() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbcommission").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin commission đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteCommission", { "ids": iddelete, "cid": customerId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#parent_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}

function deleteContract() {
    var iddelete = "";
    $(".cbdelete.cbcontract").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin hợp đôngf đang chọn?")) {
            showLoading();
            $.post("/Contract/DeleteContract", { "ids": iddelete },
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

function deleteContractById(id, customerId) {
    if (confirm("Bạn chắc chắn muốn xóa thông tin hợp đồng này?")) {
        showLoading();
        $.post("/Contract/DeleteContractById", { "id": id, "customerId": customerId },
            function (response) {
                if (response.Code === 200) {
                    $("#contract_" + id).remove();
                }
                hideLoading();
            });
    }
}

function checkCollect(id) {
    var collect = $("#iscollect_" + id).is(":checked");
    $.post("/Contract/CheckCollect", { "id": id, "collect": collect },
        function (response) {
            hideLoading();
            var data = JSON.parse(response.Msg);
            if (response.Code !== 200) {
                alert(data.Message);
            }
        });
    return false;
}

function completeDocument(id) {
    var done = $("#isdone_" + id).is(":checked");
    $.post("/Contract/CompleteDocument", { "id": id, "done": done },
        function (response) {
            hideLoading();
            var data = JSON.parse(response.Msg);
            if (response.Code !== 200) {
                alert(data.Message);
            }
            else {
                if (done) {
                    $("#isdone_" + id).parent().addClass("done");
                }
                else {
                    $("#isdone_" + id).parent().removeClass("done");
                }
            }
        });
    return false;
}

function skipDocument(id) {
    var skip = $("#isskip_" + id).is(":checked");
    $.post("/Contract/SkipDocument", { "id": id, "skip": skip },
        function (response) {
            hideLoading();
            var data = JSON.parse(response.Msg);
            if (response.Code !== 200) {
                alert(data.Message);
            }
            else {
                if (skip) {
                    $("#isskip_" + id).parent().addClass("skip");
                }
                else {
                    $("#isskip_" + id).parent().removeClass("skip");
                }
            }
        });
    return false;
}
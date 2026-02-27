var customerId = 0;
var isChange = false;
var placeId = 0;
$(function () {
    $(".customername, .updatelink").click(function() {
        var id = parseInt($(this).data("id"));
        viewCustomerInfo(id);
    });
    $("#Sort").change(function () {
        $("#searchForm").submit();
    });
    if ($("#btnexcel").length > 0) {
        $("#btnexcel").click(function () {
            var status = $("#Status").val();
            var country = $("#Country").val();
            var from = $("#FromDate").val();
            var to = $("#ToDate").val();
            var employee = $("#Employee").val();
            window.open('/Customer/ExportExcel/?status=' + status + '&country=' + country + '&from=' + from + '&to=' + to + '&employee=' + employee);
        });
    }
    var view = getParameterByName("view");
    var tab = getParameterByName("tab");
    if (view !== undefined && view !== null) {
        viewCustomerInfo(view, "tabs-info");
    }
    $(document.body).on("change", "#SeminarId", function () {
        loadSeminarPlace(this.value, customerId);
    });
});

function submitComment() {
    if (loading)
        return false;

    var content = $("#CommentContent").val();
    if (content === "") {
        alert("Vui lòng nhập nội dung bình luận");
        $("#CommentContent").focus();
        return false;
    }
    var id = $("#CustomerCareId").val();
    showLoading();
    $.post("/Customer/InsertComment", { "id": id, "cid": customerId, "content": content },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                loadComment(customerId);
                closeComment();
            } else {
                alert("Có lỗi xảy ra vui lòng thử lại sau");
            }
        });
}

function closeComment() {
    $("#IUComment").hide();
    $("#frmCustomerCare .divaction").show();
    return false;
}

function loadComment(id) {
    if (loading)
        return false;

    $.post("/Customer/LoadComment", { "id": id },
        function (response) {
            hideLoading();
            if (response !== "") {
                var data = JSON.parse(response);
                $(".cmt").remove();
                jQuery.each(data, function (i, val) {
                    $("#customercare_" + val.ObjectId + " .comment").after("<div class='cmt'>" + val.Message + "<a title='Click để xóa bình luận' onclick='deleteComment(" + val.Id + ")'>Xóa</a></div>");
                });
            }
        });
}

function deleteComment(id) {
    if (loading)
        return false;
    if (confirm("Bạn chắc chắn muốn xóa bình luận này?")) {
        $.post("/Customer/DeleteComment", { "id": id },
            function (response) {
                hideLoading();
                if (response.Code === 200) {
                    $(".cmt").remove();
                    loadComment(customerId);
                }
            });
    }
}

function submitCustomer(frm) {
    if (loading)
        return false;
    showLoading();
    hideAllError(frm);
    $.post("/Customer/Detail", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                var cusId = data.Id;
                if (cusId > 0) {
                    viewCustomerInfo(cusId);
                }
                else {
                    alert(data.Message);
                }
                
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
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
            var cid = parseInt($(this).data("id"));
            if (tab === "tabs-contract") {
                loadContract(cid);
            }
            if (tab === "tabs-document") {
                loadContractDocument(cid);
            }
            $.fancybox.update();
        }
    });
    if ($('#ProvinceId').length) { $('#ProvinceId').select2({ width: '100%' }); }
    if ($('#CountryId').length) { $('#CountryId').select2({ width: '100%' }); }
    if ($('#EmployeeId').length && $('#EmployeeId').is('select')) { $('#EmployeeId').select2({ width: '100%' });}
    if ($('#AgencyId').length && $('#AgencyId').is('select')) { $('#AgencyId').select2({ width: '100%' }); }
    if ($('#SeminarId').length) { $('#SeminarId').select2({ width: '100%' }); }
    $(".comment").click(function () {
        var id = $(this).data("id");
        $("#IUComment").show();
        $("#frmCustomerCare .divaction").hide();
        $("#CustomerCareId").val(id);
    });
    initSpecialInput();
}

function initInsertUpdate(name) {
    $("#frm" + name + " #Id").val("0");
    $("#frm" + name).trigger("reset");
    $("#frm" + name + " .divaction").hide();
    $("#frm" + name + " .listdata").hide();
    $("#IU" + name).show();
    if (name === "CustomerGuarantee") {
        loadRelatives(customerId);
    }
    return false;
}

function finishInsertUpdate(name) {
    $("#IU" + name).hide();
    $("#frm" + name + " .divaction").show();
    $("#frm" + name + " .listdata").show();
    return false;
}

function viewCustomerInfo(id, tab) {
    if (loading) return false;

    showLoading();
    $.get("/Customer/Detail", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code !== 200) {
                var tmp = JSON.parse(response.Msg);
                if (tmp.Error.length > 0 && tmp.Error[0].key === "common") {
                    alert(tmp.Error[0].msg);
                }
                else {
                    alert(tmp.Message);
                    if (response.Code === 401)
                        window.location = "/Account/Login";
                }
            }
            else {
                customerId = id;
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
                initTabEvent();
                loadComment(id);
                if (tab !== undefined && tab !== "") {
                    $("#tabs ul li").removeClass("active");
                    $("#tabs ul li a[data-tab='" + tab + "']").parent().addClass("active");
                    $(".tabs-ct").removeClass("active");
                    $("#" + tab).addClass("active");
                }
            }
        });
    return false;
}

function deleteCustomer() {
    var iddelete = "";
    $(".cbdelete.cbcustomer").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin khách hàng đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteCustomer", { "ids": iddelete },
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

function submitParent() {
    if (loading)
        return false;
    var frm = "frmCustomerParent";
    showLoading();
    hideAllError(frm);
    $.post("/Customer/CustomerParent", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_parentlist").replaceWith(data.Html);
                }
                $(".divaction").show();
                $("#IUCustomerParent").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function editParent(id) {
    // load thông tin phụ huynh lên để chỉnh sửa
    $.post("/Customer/ParentInfo", {"id" : id},
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmCustomerParent #Id").val(data.Id);
                $("#frmCustomerParent #Name").val(data.Name);
                $("#frmCustomerParent #Email").val(data.Email);
                $("#frmCustomerParent #Phone").val(data.Phone);
                $("#frmCustomerParent #BirthdayString").data();
                $("#frmCustomerParent #Desire").val(data.Desire);
                $("#frmCustomerParent #JobName").val(data.JobName);
                $("#frmCustomerParent #PositionName").val(data.PositionName);
                $("#frmCustomerParent #CompanyName").val(data.CompanyName);
                $("#frmCustomerParent #Income").val(data.Income);
                $("#frmCustomerParent #OtherIncome").val(data.OtherIncome);
                $("#frmCustomerParent .divaction").hide();
                $("#frmCustomerParent .listdata").hide();
                $("#IUCustomerParent").show();
            }
        });
    return false;
}

function deleteParent() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbparent").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteParent", { "ids": iddelete, "cid": customerId },
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

function submitCustomerCare() {
    if (loading)
        return false;
    var frm = "frmCustomerCare";
    showLoading();
    hideAllError(frm);
    $.post("/Customer/CustomerCare", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_customerCarelist").replaceWith(data.Html);
                }
                $("#" + frm + " .divaction").show();
                $("#IUCustomerCare").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function editCustomerCare(id) {
    $.post("/Customer/CustomerCareInfo", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmCustomerCare #Id").val(data.Id);
                $("#frmCustomerCare #Advisory").val(data.Advisory);
                $("#frmCustomerCare #IsAlarm").prop("checked", data.IsAlarm);
                $("#frmCustomerCare #AlarmTimeString").val(data.AlarmTimeString);
                $("#frmCustomerCare .divaction").hide();
                $("#frmCustomerCare .listdata").hide();
                $("#IUCustomerCare").show();
            }
        });
    return false;
}

function deleteCustomerCare() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbcare").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteCustomerCare", { "ids": iddelete, "cid": customerId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#customercare_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}

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

function loadContract(id) {
    showLoading();
    $.get("/Contract/ContractInfo", { "customerId": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#contractinfo").html(data.Html);
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
                $(document.body).on("change", "#SchoolId", function () {
                    loadScholarship(this.value);
                });
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

function editStudyHistory(id) {
    $.post("/Customer/StudyHistoryInfo", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmStudyHistory #Id").val(data.Id);
                $("#frmStudyHistory #School").val(data.School);
                $("#frmStudyHistory #Major").val(data.Major);
                $("#frmStudyHistory #Score").val(data.Score);
                $("#frmStudyHistory #Class").val(data.Class);
                $("#frmStudyHistory #Note").val(data.Note);
                $("#frmStudyHistory #GraduateDateString").val(data.GraduateDateString);
                $("#frmStudyHistory .divaction").hide();
                $("#frmStudyHistory .listdata").hide();
                $("#IUStudyHistory").show();
            }
        });
    return false;
}

function submitStudyHistory() {
    if (loading)
        return false;
    var frm = "frmStudyHistory";
    showLoading();
    hideAllError(frm);
    $.post("/Customer/StudyHistory", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_studyhistorylist").replaceWith(data.Html);
                }
                $("#" + frm + " .divaction").show();
                $("#IUStudyHistory").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function deleteStudyHistory() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbhistory").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteStudyHistory", { "ids": iddelete, "cid": customerId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#history_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}

function editStudyAbroad(id) {
    $.post("/Customer/StudyAbroadInfo", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmStudyAbroad #Id").val(data.Id);
                $("#frmStudyAbroad #School").val(data.School);
                $("#frmStudyAbroad #Major").val(data.Major);
                $("#frmStudyAbroad #CountryID").val(data.CountryId);
                $("#frmStudyAbroad #Note").val(data.Note);
                $("#frmStudyAbroad #Year").val(data.Year);
                $("#frmStudyAbroad #Level").val(data.Level);
                $("#frmStudyAbroad .divaction").hide();
                $("#frmStudyAbroad .listdata").hide();
                $("#IUStudyAbroad").show();
            }
        });
    return false;
}

function submitStudyAbroad() {
    if (loading)
        return false;
    var frm = "frmStudyAbroad";
    showLoading();
    hideAllError(frm);
    $.post("/Customer/StudyAbroad", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_StudyAbroadList").replaceWith(data.Html);
                }
                $("#" + frm + " .divaction").show();
                $("#IUStudyAbroad").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function deleteStudyAbroad() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbabroad").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteStudyAbroad", { "ids": iddelete, "cid": customerId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#abroad_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}

function editLanguage(id) {
    $.post("/Customer/LanguageInfo", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmLanguage #Id").val(data.Id);
                $("#frmLanguage #Language").val(data.Language);
                $("#frmLanguage #Score").val(data.Score);
                $("#frmLanguage #RetestDateString").val(data.RetestDateString);
                $("#frmLanguage #Note").val(data.Note);
                $("#frmLanguage .divaction").hide();
                $("#frmLanguage .listdata").hide();
                $("#IULanguage").show();
            }
        });
    return false;
}

function submitLanguage() {
    if (loading)
        return false;
    var frm = "frmLanguage";
    showLoading();
    hideAllError(frm);
    $.post("/Customer/Language", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_LanguageList").replaceWith(data.Html);
                }
                $("#" + frm + " .divaction").show();
                $("#IULanguage").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function deleteLanguage() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cblanguage").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteLanguage", { "ids": iddelete, "cid": customerId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#Language_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}

function addPermission() {
    isChange = false;
    var cId = parseInt($("#ddlPermission").val());
    if (cId > 0) {
        var iddelete = "";
        var idArr = [];
        $(".cbdelete.cbcustomer").each(function (index) {
            var check = $(this).prop("checked");
            var id = $(this).data("id");
            if (check) {
                iddelete += id + ",";
                idArr[idArr.length] = id;
            }
        });
        if (iddelete !== "") {
            if (confirm("Bạn chắc chắn muốn phân quyền tất cả thông tin khách hàng đang chọn?")) {
                showLoading();
                $.post("/Customer/AddPermission", { "ids": iddelete, "uId": cId },
                    function (response) {
                        var data = JSON.parse(response.Msg);
                        alert(data.Message);
                        if (response.Code === 200) {
                            $("#searchForm").submit();
                        }

                        hideLoading();
                    });
            }
        }
    }
}

function deleteCustomerById(id) {
    if (confirm("Bạn chắc chắn muốn xóa thông tin khách hàng này?")) {
        showLoading();
        $.post("/Customer/DeleteCustomer", { "ids": id },
            function (response) {
                if (response.Code === 200) {
                    $('#searchForm').submit();
                }
                hideLoading();
            });
    }
    return false;
}

function loadContractDocument(id) {
    showLoading();
    $.get("/Contract/LoadContractDocumentByCustomer", { "id": id },
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

function seminarRegister(id) {
    if (loading) return false;

    customerId = id;
    showLoading();
    $.get("/Customer/SeminarRegister", { "id": customerId },
        function (response) {
            hideLoading();
            if (response.Code !== 200) {
                alert("Có lỗi xảy ra vui lòng thử lại sau");
            }
            else {
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
                $('#SeminarId').select2();
            }
        });
    return false;
}

//function submitSeminarRegister() {
//    var frm = "frmSeminarRegister";
//    showLoading();
//    hideAllError(frm);
//    $.post("/Customer/SeminarRegister", $("#" + frm).serialize(),
//        function (response) {
//            hideLoading();
//            if (response.Code === 200) {
//                var data = JSON.parse(response.Msg);
//                alert(data.Message);
//            } else {
//                showError(frm, $.parseJSON(response.Msg));
//            }
//        });
//    return false;
//}

//function loadSeminarPlace(id, customerId) {
//    if (loading) return false;

//    showLoading();
//    $.get("/Customer/LoadSeminarPlace", { "id": id },
//        function (response) {
//            hideLoading();
//            if (response.Code !== 200) {
//                alert("Có lỗi xảy ra vui lòng thử lại sau");
//            }
//            else {
//                var data = JSON.parse(response.Msg);
//                $('#SeminarPlaceId').replaceWith(data.Html);
//                //loadSeminarRegister(id, customerId);
//            }
//        });
//    return false;
//}

// load thông tin đăng ký hội thảo của KH khi thay đổi hội thảo
//function loadSeminarRegister(id, customerId) {
//    if (loading) return false;

//    showLoading();
//    $.get("/Customer/LoadSeminarRegister", { "id": id, "customerId": customerId },
//        function (response) {
//            hideLoading();
//            if (response.Code === 200) {
//                var data = JSON.parse(response.Msg);
//                $("#frmSeminarRegister #SeminarPlaceId").val(data.SeminarPlaceId);
//                $("#frmSeminarRegister #CustomerNote").val(data.CustomerNote);
//                $("#frmSeminarRegister #Note").val(data.Note);
//            }
//            else {
//                $("#frmSeminarRegister #SeminarPlaceId").val("");
//                $("#frmSeminarRegister #CustomerNote").val("");
//                $("#frmSeminarRegister #Note").val("");
//            }
//        });
//    return false;
//}

function submitRelatives() {
    if (loading)
        return false;
    var frm = "frmCustomerRelatives";
    showLoading();
    hideAllError(frm);
    $.post("/Customer/CustomerRelatives", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_relativeslist").replaceWith(data.Html);
                }
                $(".divaction").show();
                $("#IUCustomerRelatives").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function editRelatives(id) {
    // load thông tin phụ huynh lên để chỉnh sửa
    $.post("/Customer/RelativesInfo", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmCustomerRelatives #Id").val(data.Id);
                $("#frmCustomerRelatives #Name").val(data.Name);
                $("#frmCustomerRelatives #Relationship").val(data.Relationship);
                $("#frmCustomerRelatives #CountryName").val(data.Phone);
                $("#frmCustomerRelatives #JobName").val(data.JobName);
                $("#frmCustomerRelatives #CompanyName").val(data.CompanyName);
                $("#frmCustomerRelatives #Income").val(data.Income);
                $("#frmCustomerRelatives #Address").val(data.Address);
                $("#frmCustomerRelatives .divaction").hide();
                $("#frmCustomerRelatives .listdata").hide();
                $("#IUCustomerRelatives").show();
            }
        });
    return false;
}

function deleteRelatives() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbrelatives").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteRelatives", { "ids": iddelete, "cid": customerId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#relatives_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}

function submitGuarantee() {
    if (loading)
        return false;
    var frm = "frmCustomerGuarantee";
    showLoading();
    hideAllError(frm);
    $.post("/Customer/CustomerGuarantee", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_guaranteelist").replaceWith(data.Html);
                }
                $(".divaction").show();
                $("#IUCustomerGuarantee").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function editGuarantee(id) {
    // load thông tin phụ huynh lên để chỉnh sửa
    $.post("/Customer/GuaranteeInfo", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmCustomerGuarantee #Id").val(data.Id);
                $("#frmCustomerGuarantee #RelativesId").val(data.RelativesId);
                $("#frmCustomerGuarantee #Person").val(data.Person);
                $("#frmCustomerGuarantee #GuaranteeName").val(data.GuaranteeName);
                $("#frmCustomerGuarantee #GuaranteeYear").val(data.GuaranteeYear);
                $("#frmCustomerGuarantee .divaction").hide();
                $("#frmCustomerGuarantee .listdata").hide();
                $("#IUCustomerGuarantee").show();
            }
        });
    return false;
}

function deleteGuarantee() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbguarantee").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin đang chọn?")) {
            showLoading();
            $.post("/Customer/DeleteGuarantee", { "ids": iddelete, "cid": customerId },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#guarantee_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}

function loadRelatives(customerId) {
    if (loading) return false;

    $.get("/Customer/LoadRelatives", { "id": customerId },
        function (response) {
            if (response.Code !== 200) {
                alert("Có lỗi xảy ra vui lòng thử lại sau");
            }
            else {
                var data = JSON.parse(response.Msg);
                $('#RelativesId').replaceWith(data.Html);
            }
        });
    return false;
}

function editSeminarRegister(id) {
    if (loading) return false;

    $.post("/Customer/SeminarRegisterInfo", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                $("#frmSeminarRegister #Id").val(data.Id);
                $("#frmSeminarRegister #SeminarId").val(data.SeminarId).trigger('change');
                placeId = data.SeminarPlaceId;
                $("#frmSeminarRegister #SeminarPlaceId").val(data.SeminarPlaceId);
                $("#frmSeminarRegister #TicketId").val(data.TicketId);
                $("#frmSeminarRegister #School1").val(data.School1);
                $("#frmSeminarRegister #School1Time").val(data.School1Time);
                $("#frmSeminarRegister #School2").val(data.School2);
                $("#frmSeminarRegister #School2Time").val(data.School2Time);
                $("#frmSeminarRegister #School3").val(data.School3);
                $("#frmSeminarRegister #School3Time").val(data.School3Time);
                $("#frmSeminarRegister #CustomerNote").val(data.CustomerNote);
                $("#frmSeminarRegister #Note").val(data.Note);
                $("#frmSeminarRegister .divaction").hide();
                $("#frmSeminarRegister .listdata").hide();
                $("#IUSeminarRegister").show();
            }
        });
    return false;
}

function submitSeminarRegister() {
    var frm = "frmSeminarRegister";
    showLoading();
    hideAllError(frm);
    $.post("/Customer/SeminarRegister", $("#" + frm).serialize(),
        function (response) {
            hideLoading();
            if (response.Code === 200) {
                var data = JSON.parse(response.Msg);
                if (data.Html !== undefined) {
                    $("#dt_SeminarRegisterList").replaceWith(data.Html);
                }
                $(".divaction").show();
                $("#IUSeminarRegister").hide();
                alert(data.Message);
            } else {
                showError(frm, $.parseJSON(response.Msg));
            }
        });
    return false;
}

function loadSeminarPlace(id, customerId) {
    if (loading) return false;

    showLoading();
    $.get("/Customer/LoadSeminarPlace", { "id": id },
        function (response) {
            hideLoading();
            if (response.Code !== 200) {
                alert("Có lỗi xảy ra vui lòng thử lại sau");
            }
            else {
                var data = JSON.parse(response.Msg);
                $("#SeminarPlaceId").replaceWith(data.Html);
                $("#SeminarPlaceId").val(placeId);
            }
        });
    return false;
}

function deleteSeminarRegister() {
    var iddelete = "";
    var idArr = [];
    $(".cbdelete.cbseminarregister").each(function (index) {
        var check = $(this).prop("checked");
        var id = $(this).data("id");
        if (check) {
            iddelete += id + ",";
            idArr[idArr.length] = id;
        }
    });
    if (iddelete !== "") {
        if (confirm("Bạn chắc chắn muốn xóa tất cả thông tin đang chọn?")) {
            showLoading();
            $.post("/SeminarRegister/Delete", { "ids": iddelete },
                function (response) {
                    if (response.Code === 200) {
                        for (var i = 0; i < idArr.length; i++) {
                            $("#seminarregister_" + idArr[i]).remove();
                        }
                    }
                    hideLoading();
                });
        }
    }
    return false;
}
var loading = false;
$(function () {
    $.ajaxPrefilter(function (opts, orgOpts) {
        var qr = [{ k: 'clearcache', v: window.uQuery('clearcache') }];
        for (var i = 0; i < qr.length; i++) {
            var k = qr[i].k;
            var v = qr[i].v;
            if (v != null && v !== '') {
                switch (typeof orgOpts.data) {
                    case 'undefined':
                        opts.data = k + '=' + v;
                        break;
                    case 'string':
                        opts.data = orgOpts.data + '&' + k + '=' + v;
                        break;
                    default:
                        var o = {};
                        o[k] = v;
                        opts.data = $.param($.extend(orgOpts.data, o));
                        break;
                }
            }
        }
    });

    $(".datepicker").datepicker({ format: 'dd/mm/yyyy' });
    $("#searchForm #Keyword").keypress(function (e) {
        if (e.which == 13) {
            $("#searchForm").submit();
        }
    });
    $('.select2').select2();
});

function setCompany(id) {
    $.post("/Account/SetCompany", { "id": id },
        function (data) {
            loading = false;
            if (data.Code === 200) {
                window.location.reload();
            }
        });
    return false;
}

function updateNotify(id) {
    if (loading || id === "")
        return false;

    loading = true;
    $.post("/Home/UpdateNotify", { "id": id },
        function (data) {
            loading = false;
            if (data.Code === 0) {
                $("#notifyicon").unbind("click");
                $("#notifyicon").removeAttr("onclick");
            }
        });
}

function doSearch() {
    $('#hdfCurrentPage').val(0);
    $('#searchForm').submit();
    return false;
}

function ChangePage(page) {
    $('#hdfCurrentPage').val(page);
    $('#searchForm').submit();
    return false;
}

function CheckAll(obj, val) {
    var type = $(obj).data("type");
    $(".cbdelete." + type).each(function (index) {
        $(this).prop("checked", val);
    });
}

function ToogleCheck() {
    var ischecked = true;
    var type = $(this).data("type");
    $(".cbdelete." + type).each(function (index) {
        var check = $(this).attr("checked") != undefined;
        if (!check) {
            ischecked = false;
        }
    });
    $(".cball[data-type='" + type + "']").prop('checked', ischecked);
}

function GetAllFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });
    return indexed_array;
}

jQuery.fn.ForceNumericOnly =
    function () {
        return this.each(function () {
            $(this).keydown(function (e) {
                if (e.ctrlKey || e.altKey) { // if shift, ctrl or alt keys held down
                    e.preventDefault();         // Prevent character input
                }
                else {
                    var key = e.charCode || e.keyCode || 0;
                    return (
                        key == 8 ||                     // backspace
                        key == 9 ||                     // tab
                        key == 46 ||                    // delete        
                        key == 16 ||                    // shift   
                        (key >= 35 && key <= 40) ||     // arrow keys/home/end
                        (key >= 48 && key <= 57) ||
                        (key >= 96 && key <= 105));// ||      // number on keypad
                    //  (key >= 9 && key <= 16);           //Shift + Tab 
                }
            });
        });
    };

function showLoading(hideOverlay) {
    loading = true;
    $("#loading").show();
    if (hideOverlay !== undefined && hideOverlay) {
        $("#loading .overlay").hide();
    } else {
        $("#loading .overlay").show();
    }
}

function hideLoading() {
    loading = false;
    $("#loading").hide();
}

function showError(id, obj) {
    for (var i = 0; i < obj.Error.length; i++) {
        if (i === 0) {
            $("#" + id + " #" + obj.Error[i].key).focus();
        }
        if (obj.Error[i].key == "common") {
            alert(obj.Error[i].msg);
        }
        else {
            $("#" + id + " #" + obj.Error[i].key).next().attr("title", obj.Error[i].msg).show();
        }
    }
}

function hideAllError(id) {
    $("#" + id + " .ierror").attr("title", "").hide();
}

function initSpecialInput() {
    $(".date").mask("00/00/0000", { placeholder: "__/__/____" });
    $(".phone").mask("0000-000-0000", { placeholder: "____-___-____" });
    $(".numeric").ForceNumericOnly();
    $(".money").mask("000.000.000.000.000", { reverse: true });
    return false;
}

function formatNumeric(val) {
    if (val != '') {
        var a = parseInt(val); val = a.toString();
    }
    val += '';
    var x = val.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    }
    return x1 + x2;
}

function getParameterByName(name) {
    var url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}
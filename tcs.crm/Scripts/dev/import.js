
function saveImportData() {
    if (loading)
        return false;

    loading = true;
    $.post("/Customer/SaveImportData", {},
        function (response) {
            loading = false;
            var data = JSON.parse(response.Msg);
            if (response.Code === 200) {
                alert("Import dữ liệu thành công");
            }else {
                alert(data.Msg);
            }
        });
}
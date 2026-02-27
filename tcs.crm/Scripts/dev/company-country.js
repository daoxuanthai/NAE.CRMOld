$(function () {
    
});

function updateCountry(id) {
    var visible = $("#country_" + id).is(":checked");
    $.post("/CompanyCountry/UpdateCountry", { "id": id, "visible": visible },
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
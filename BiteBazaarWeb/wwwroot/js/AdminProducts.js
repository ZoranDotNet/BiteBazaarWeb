
function searchProducts() {
    var search = $('#searchString').val();

    $.ajax({
        url: '/Admin/Products/Search',
        type: 'GET',
        data: { searchstring: search },
        success: function (result) {
            $('#searchContainer').html(result);

            $('#searchString').val('');

        }

    });
}


$(document).ready(function () {
    $('#searchButtonAdmin').click(function () {
        searchProducts();
    });

    $('#searchString').keypress(function (e) {
        if (e.which == 13) { 
            searchProducts();
        }
    });

});

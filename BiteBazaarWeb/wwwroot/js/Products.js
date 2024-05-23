
function filterProducts() {


    var filter = $('#filter').val();
    var filterTitle = $('#filter option:selected').text();


    $.ajax({
        url: '/Customer/Home/FilterProducts',
        type: 'GET',
        data: { filter: filter },
        success: function (result) {

            $('#productsContainer').html(result);

            if (filter != 0) {
                $('#showFilter').html(filterTitle)
                $('#headline').removeClass('visually-hidden')
                $('#headline2').addClass('visually-hidden')
            }

            $('#filter').val('');
        }

    });


}
function searchProducts() {
    var search = $('#searchString').val();

    $.ajax({
        url: '/Customer/Home/SearchProducts',
        type: 'GET',
        data: { searchstring: search },
        success: function (result) {
            $('#productsContainer').html(result);

            if (search != "") {
                $('#showFilter').html(search)
                $('#headline2').removeClass('visually-hidden')
                $('#headline').addClass('visually-hidden')
            }

            $('#searchString').val('');

        }

    });
}

$(document).ready(function () {
    $('#searchButton').click(function () {
        searchProducts();
    });

});



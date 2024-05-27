
function filterProducts() {


    var filter = $('#filter').val();
    var filterTitle = $('#filter option:selected').text();


    $.ajax({
        url: '/Customer/Home/FilterProducts',
        type: 'GET',
        data: { filter: filter },
        success: function (result) {

            $('#productsContainer').html(result);
            console.log(filter);
            if (filter != 0) {
                getCategoryInfo(filter);
            }

            $('#filter').val('');
        }

    });


}
function getCategoryInfo(categoryId) {
    console.log(categoryId);
    $.ajax({
        url: '/Customer/Home/GetCategoryInfo',
        type: 'GET',
        data: { categoryId: categoryId },
        success: function (result) {
            var categoryHtml = `
                <div>
                    <h3>${result.title}</h3>
                    <p>${result.description}</p>
                </div>`;
            $('#showFilter').html(categoryHtml);
            console.log(result);
            $('#headline').removeClass('visually-hidden');
            $('#headline2').addClass('visually-hidden');
        },
        
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



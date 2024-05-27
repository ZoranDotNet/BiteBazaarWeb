
function searchOrders() {
    var fromDate = $('#fromDate').val()
    var toDate = $('#toDate').val()
    console.log('inne i function')
    $.ajax({
        url: '/Admin/Dashboard/Showresults',
        type: 'GET',
        data: { fromDate: fromDate, toDate: toDate },
        success: function (result) {
            var rounded = Math.floor(result.total);
            var totalFormatted = new Intl.NumberFormat('sv-SE', { style: 'currency', currency: 'SEK' }).format(rounded);
            var salesSearch = `
    <h5>Antal köp: ${result.count} st</h5>
    <h5>Totalt ${totalFormatted} </h5>
    `;
            $('#showStatistics').html(salesSearch);
        },
        error: function (xhr, status, error) {
            console.error('error:', status, error);
            
        }

    });
}


$(document).ready(function () {
    $('#sendButton').click(function () {
        searchOrders();
    });


});

function lowProducts() {
    $.ajax({
        url: '/Admin/Dashboard/ShowLowInventory',
        type: 'GET',
        success: function (result) {
            console.log('success')
            var productsHtml = `<table class="table table-dark">
                                      <tr>
                                            <th>ID</th>
                                          <th>Produkt</th>
                                          <th>Lagersaldo</th>
                                          
                                      </tr>`;
            result.lowCount.forEach(function (product) {
                productsHtml += `<tr>
                                      <td>${product.productId}</td>
                                      <td>${product.title}</td>
                                      <td>${product.quantity} st</td>
                                    </tr>`;
            });
            productsHtml += `</table>`;
            $('#showStatistics').html(productsHtml);
        },
        error: function (xhr, status, error) {
            console.error('error:', status, error);
            console.log('XHR:', xhr);
        }

    });
}

$(document).ready(function () {
    $('#productButton').click(function () {
        lowProducts();
    });


});


function mostSoldProducts() {
    var fromDate = $('#fromDate').val()
    var toDate = $('#toDate').val()
    $.ajax({
        url: '/Admin/Dashboard/TopProducts',
        type: 'GET',
        data: { fromDate: fromDate, toDate: toDate },
        success: function (result) {
            console.log('result', result)
            var top = `<table class="table table-dark">
        <tr>
            <th>ID</th>
            <th>Produkt</th>
            <th>Antal</th>
            <th>Pris</>

        </tr>`;
            result.forEach(function (item) {
                top += `<tr>
                   <td>${item.id}</td>
                   <td>${item.title}</td>
                   <td>${item.count} st</td>
                   <td>${item.price} kr</td>
                 </tr>`;
            });
            top += `</table>`;
            $('#showStatistics').html(top);
        },
        error: function (xhr, status, error) {
            console.error('error:', status, error);

        }

    });
}


$(document).ready(function () {
    $('#topSellButton').click(function () {
        mostSoldProducts();
    });


});


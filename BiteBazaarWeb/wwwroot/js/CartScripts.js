$(document).ready(function () {
    $(document).ready(function () {
        $('#openModal').click(function () {
            $.ajax({
                url: '/Customer/Cart/Index',
                type: 'GET',
                success: function (data) {
                    $('#cartDiv').html(data)
                    $('#myModal').modal('show')
                }
            })
        })
    })
})




$(document).on('click', '#plusbutton', function () {
    console.log('inne i function plus')
    var cartId = $(this).data('id')
    $.ajax({
        url: '/Customer/Cart/Plus',
        type: 'POST',
        data: { id: cartId },
        success: function (data) {
            $('#cartDiv').html(data)

        },
        error: function (xhr, status, error) {
            console.error('Error', error)
        }
    })
})




$(document).ready(function () {
    $(document).on('click', '#minusbutton', function () {
        console.log('inne i function minus')
        var cartId = $(this).data('id')
        $.ajax({
            url: '/Customer/Cart/Minus',
            type: 'POST',
            data: { id: cartId },
            success: function (data) {
                $('#cartDiv').html(data)

            },
            error: function (xhr, status, error) {
                console.error('Error', error)
            }
        })
    })
})



$(document).ready(function () {
    $(document).on('click', '#deletebutton', function () {
        console.log('inne i function delete')
        var cartId = $(this).data('id')
        $.ajax({
            url: '/Customer/Cart/Delete',
            type: 'POST',
            data: { id: cartId },
            success: function (data) {
                $('#cartDiv').html(data)

            },
            error: function (xhr, status, error) {
                console.error('Error', error)
            }
        });
    })
})

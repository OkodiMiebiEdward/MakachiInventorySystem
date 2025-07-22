toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": true,
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};
async function getDataFromModal(whatClicked) {
    let barcodenumber = jQuery("#barcodenumber").val();
    if (whatClicked == 0) {
        $("#compmodal").modal('hide');
    }
    else {
        try {
            const response = await fetch(`?handler=FetchProductFromBarcodenumber&barcodenumber=${barcodenumber}`, { method: 'GET' });
            const json = await response.json();
            if (json != "") {
                var data = JSON.parse(json);
                if (data && data.html) {
                    $('#compmodal').empty();
                    $('#compmodal').append(data.html);
                    $("#compmodal").modal('show');
                    $('#discount').val(data.discount);
                }
            }
        }
        catch (e) {
            toastr.error("An error occurred, please try again.");
            $("#compmodal").modal('hide');
        }
    }
}
;
$('#cart').on('click', async function () {
    let barcode = jQuery('#barcodenumber').val();
    let price = jQuery('#price').val();
    let quantity = parseInt(jQuery('#quantity').val());
    let discount = parseInt(jQuery('#discount').val());
    if (barcode === "") {
        toastr.error("Barcodenumber is required.");
        return;
    }
    else if (price === "") {
        toastr.error("Price is required.");
        return;
    }
    else if (quantity === 0) {
        toastr.error("Quantity cannot be 0.");
        return;
    }
    try {
        const response = await fetch(`?handler=InitiateSale&discount=${discount}&priceSold=${price}&quantity=${quantity}&barcode=${barcode}`, { method: 'GET' });
        const json = await response.json();
        if (json != "") {
            var data = JSON.parse(json);
            if (data) {
                $('.actionTable').prop('hidden', false);
                $('#saleTable tbody').append(data);
                updateSerialNumbersAndTotal();
            }
        }
    }
    catch (e) {
    }
});
$(document).on('click', '.remove-sale-row', async function () {
    const $row = $(this).closest('tr');
    const barcode = $row.find('td').eq(1).text();
    const quantity = $row.find('td').eq(2).text();
    const price = $row.find('td').eq(3).text().replace(/,/g, '');
    const discount = $row.find('td').eq(4).text();
    try {
        const response = await fetch(`?handler=RemoveSale&discount=${discount}&priceSold=${price}&quantity=${quantity}&barcode=${barcode}`, { method: 'GET' });
        const json = await response.json();
        if (json != "") {
            var data = JSON.parse(json);
            if (data && data.Result && data.Result.Status === "Success") {
                $row.remove();
                updateSerialNumbersAndTotal();
            }
        }
    }
    catch (e) {
        toastr.error("An error occurred while removing the sale.");
    }
});
jQuery(() => {
    const status = $('#status').val();
    const description = $('#description').val();
    if (status === "Failed") {
        toastr.error(description);
    }
});
function updateSerialNumbersAndTotal() {
    let total = 0;
    const $tbody = $('#saleTable tbody');
    $tbody.find('tr').each(function (index) {
        $(this).find('td.sn').text(index + 1);
        const priceText = $(this).find('td').eq(5).text().replace(/,/g, '');
        const finalPrice = parseFloat(priceText) || 0;
        total += finalPrice;
    });
    $('#totalFinalPrice').text(total.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
    if ($tbody.find('tr').length === 0) {
        $('.actionTable').prop('hidden', true);
        $tbody.empty();
    }
}
//# sourceMappingURL=createSales.js.map
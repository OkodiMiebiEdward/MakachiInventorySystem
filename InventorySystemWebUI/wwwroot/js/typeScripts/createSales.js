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
        if (barcodenumber !== '') {
            try {
                const response = await fetch(`?handler=FetchProductFromBarcodenumber&barcodenumber=${barcodenumber}`, { method: 'GET' });
                const json = await response.json();
                if (json != "") {
                    var data = JSON.parse(json);
                    if (data && data.Html) {
                        $('#compmodal').empty();
                        $('#compmodal').append(data.Html);
                        $("#compmodal").modal('show');
                        $('#discount').val(data.Discount);
                        $('#price').val(data.Price);
                    }
                }
            }
            catch (e) {
                toastr.error("An error occurred, please try again.");
                $("#compmodal").modal('hide');
            }
        }
        else {
            $('#discount').val('');
            $('#price').val('');
            $('#quantity').val(0);
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
    else if (quantity <= 0) {
        toastr.error("Quantity cannot be 0 or less than 0.");
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
    else if (status === "ServerError") {
        toastr.error(description);
        setTimeout(() => {
            window.location.href = "/Module/Stock/ProductsStock";
        }, 5000);
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
function getDataFromTable() {
    const rows = document.querySelectorAll("#saleTable tbody tr");
    const subData = [];
    let id = 0;
    const getNumber = (cell) => {
        if (!cell)
            return 0;
        const value = cell.textContent?.replace(/,/g, '').trim() || "";
        const num = Number(value);
        return isNaN(num) ? 0 : num;
    };
    rows.forEach((row, index) => {
        const cells = row.querySelectorAll("td");
        if (index === 0) {
            id = 1;
        }
        const rowData = {
            Barcodenumber: cells[1]?.textContent?.trim() || "",
            Quantity: getNumber(cells[2]),
            PriceSold: getNumber(cells[3]),
            Discount: getNumber(cells[4]),
            FinalPrice: getNumber(cells[5]),
        };
        subData.push(rowData);
    });
    return {
        Id: id,
        SubData: subData
    };
}
$('#checkOut').on('click', async function () {
    let data = getDataFromTable();
    const uri = `${window.location.origin}/api/Checkout/PrintReceipt`;
    const response = await fetch(uri, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    const contentType = response.headers.get("Content-Type");
    if (contentType) {
        if (contentType.includes("application/octet-stream")
            || contentType.includes("application/pdf")
            || contentType.includes("application/vnd")) {
            const blob = await response.blob();
            let exportFormat = 'pdf';
            let url = (exportFormat == 'pdf') ?
                window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' })) :
                (exportFormat == 'excel') ?
                    window.URL.createObjectURL(new Blob([blob], { type: 'application/vnd.ms-excel' })) :
                    window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));
            window.location.href = "/SuccessCheckout";
            window.open(url, '_blank');
        }
        else if (contentType.includes("application/json")) {
            const data = await response.json();
            if (data.status.toLocaleLowerCase() != 'success') { }
            else {
            }
        }
        else if (contentType.includes("text/")) {
            const text = await response.text();
            { }
        }
    }
    else {
        { }
    }
    return;
});
//# sourceMappingURL=createSales.js.map
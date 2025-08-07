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

async function getDataFromModal(whatClicked: number) {
    let barcodenumber = jQuery("#barcodenumber").val();

    //Closing the Modal
    if (whatClicked == 0) {
        $("#compmodal").modal('hide');
    }
    else {
        try {
            const response = await fetch(`?handler=FetchProductFromBarcodenumber&barcodenumber=${barcodenumber}`, { method: 'GET' });
            const json = await response.json();
            if (json != "") {
                var data = JSON.parse(json)
                if (data && data.Html) {
                    $('#compmodal').empty();
                    $('#compmodal').append(data.Html);
                    $("#compmodal").modal('show');
                    $('#discount').val(data.Discount); // Set the discount value
                    $('#price').val(data.Price);
                }
            }
        }
        catch (e) {
            toastr.error("An error occurred, please try again.");
            $("#compmodal").modal('hide');
        }
    }
};

$('#cart').on('click',async function () {
    let barcode = jQuery('#barcodenumber').val() as string;
    let price = jQuery('#price').val() as string;
    let quantity = parseInt(jQuery('#quantity').val() as string);
    let discount = parseInt(jQuery('#discount').val() as string);

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
            var data = JSON.parse(json)
            if (data) {
                $('.actionTable').prop('hidden', false);
                $('#saleTable tbody').append(data); // Append to tbody
                updateSerialNumbersAndTotal();
            }
        }
    } catch (e) {

    }
});

$(document).on('click', '.remove-sale-row', async function () {
    // Get the row
    const $row = $(this).closest('tr');
    // Get column values
    const id = $row.find('td').eq(1).text();
    const barcode = $row.find('td').eq(2).text();
    const quantity = $row.find('td').eq(3).text();
    const price = $row.find('td').eq(4).text().replace(/,/g, ''); // Remove commas
    const discount = $row.find('td').eq(5).text();

    try {
        const response = await fetch(`?handler=RemoveSale&discount=${discount}&priceSold=${price}&quantity=${quantity}&barcode=${barcode}&id=${id}`, { method: 'GET' });
        const json = await response.json();
        if (json != "") {
            var data = JSON.parse(json)
            // Note: Your backend returns { Result = ResponseModel }
            if (data && data.Result && data.Result.Status === "Success") {
                // Remove the row from the DOM
                $row.remove();
                updateSerialNumbersAndTotal();
            }
        }
    } catch (e) {
        toastr.error("An error occurred while removing the sale.");
    }
});

jQuery(() => {
    const status = $('#status').val() as string;
    const description = $('#description').val() as string;

    if (status === "Failed") {
        toastr.error(description);
    }
});

function updateSerialNumbersAndTotal() {
    let total = 0;
    const $tbody = $('#saleTable tbody');
    $tbody.find('tr').each(function (index) {
        // Set S/N
        $(this).find('td.sn').text(index + 1);

        // Remove commas before parsing the final price
        const priceText = $(this).find('td').eq(6).text().replace(/,/g, '');
        const finalPrice = parseFloat(priceText) || 0;
        total += finalPrice;
    });
    $('#totalFinalPrice').text(total.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }));

    // Hide and empty the table if no rows remain
    if ($tbody.find('tr').length === 0) {
        $('.actionTable').prop('hidden', true);
        $tbody.empty();
    }
}

interface innerTableData {
    barcodenumber: string,
    quantity: number,
    priceSold: number,
    discount: number,
    finalPrice:number
}

interface TableData {
    id: number,
    subData:innerTableData[]
}

function getDataFromTable(): TableData {
    //Get all the items from the tables and store them in a type
    const rows = document.querySelectorAll<HTMLTableRowElement>("#salesTable tbody tr");
    const subData: innerTableData[] = [];
    let id = 0;
    rows.forEach((row, index) => {
        const cells = row.querySelectorAll<HTMLTableCellElement>("td");

        if (index === 0) {
            // assuming all rows share the same id, pick from the first row
            id = Number(cells[1].textContent?.trim());
        }
        const rowData: innerTableData = {
            barcodenumber: cells[2].textContent?.trim() || "",
            quantity: Number(cells[3].textContent?.trim()),
            priceSold: Number(cells[4].textContent?.trim()),
            discount: Number(cells[5].textContent?.trim()),
            finalPrice: Number(cells[6].textContent?.trim()),
        };
        subData.push(rowData);
    });

    return {
        id,
        subData
    };
}

$('#checkOut').on('click', async function () {

});

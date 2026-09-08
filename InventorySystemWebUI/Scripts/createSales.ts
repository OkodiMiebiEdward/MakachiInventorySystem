// Add this once in a shared .d.ts file (e.g. Scripts/globals.d.ts), not here:
// declare var bootstrap: any;

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
    const compModalEl = document.getElementById("compmodal")!;
    const compModal = bootstrap.Modal.getOrCreateInstance(compModalEl);

    //Closing the Modal
    if (whatClicked == 0) {
        compModal.hide();
    }
    else {
        if (barcodenumber !== '') {
            try {
                const response = await fetch(`?handler=FetchProductFromBarcodenumber&barcodenumber=${barcodenumber}`, { method: 'GET' });
                const json = await response.json();
                if (json != "") {
                    var data = JSON.parse(json)
                    if (data && data.Html) {
                        $('#compmodal').empty();
                        $('#compmodal').append(data.Html);
                        compModal.show();
                        $('#discount').val(data.Discount); // Set the discount value
                        $('#price').val(data.Price);
                    }
                }
            }
            catch (e) {
                toastr.error("An error occurred, please try again.");
                compModal.hide();
            }
        }
        else {
            $('#discount').val('');
            $('#price').val('');
            $('#quantity').val(0);
        }
    }
};

$('#cart').on('click', async function () {
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
    else if (quantity <= 0) {
        toastr.error("Quantity cannot be 0 or less than 0.");
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
    //const id = $row.find('td').eq(1).text();
    const barcode = $row.find('td').eq(1).text();
    const quantity = $row.find('td').eq(2).text();
    const price = $row.find('td').eq(3).text().replace(/,/g, ''); // Remove commas
    const discount = $row.find('td').eq(4).text();

    try {
        const response = await fetch(`?handler=RemoveSale&discount=${discount}&priceSold=${price}&quantity=${quantity}&barcode=${barcode}`, { method: 'GET' });
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

    else if (status === "ServerError") {
        toastr.error(description);
        setTimeout(() => {
            window.location.href = "/Module/Stock/ProductsStock";
        }, 5000)
    }
});

function updateSerialNumbersAndTotal() {
    let total = 0;
    const $tbody = $('#saleTable tbody');
    $tbody.find('tr').each(function (index) {
        // Set S/N
        $(this).find('td.sn').text(index + 1);

        // Remove commas before parsing the final price
        const priceText = $(this).find('td').eq(5).text().replace(/,/g, '');
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
    Barcodenumber: string,
    Quantity: number,
    PriceSold: number,
    Discount: number,
    FinalPrice: number
}

interface TableData {
    Id: number,
    SubData: innerTableData[]
}

function getDataFromTable(): TableData {
    const rows = document.querySelectorAll<HTMLTableRowElement>("#saleTable tbody tr");
    const subData: innerTableData[] = [];
    let id: number = 0;

    // Helper to safely parse numbers from table cells
    const getNumber = (cell: HTMLTableCellElement | undefined) => {
        if (!cell) return 0;
        const value = cell.textContent?.replace(/,/g, '').trim() || "";
        const num = Number(value);
        return isNaN(num) ? 0 : num;
    };

    rows.forEach((row, index) => {
        const cells = row.querySelectorAll<HTMLTableCellElement>("td");

        if (index === 0) {
            id = 1;
        }

        const rowData: innerTableData = {
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
    let data: TableData = getDataFromTable();
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

            let url = window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));

            // Try to open the PDF in a new tab first (user gesture might be required).
            const newWindow = window.open(url, '_blank');
            if (!newWindow) {
                // Popup blocked — fallback to forcing a download
                const a = document.createElement('a');
                a.href = url;
                a.download = 'Receipt.pdf';
                document.body.appendChild(a);
                a.click();
                a.remove();
            }

            // Navigate to success page after a short delay so the open/download can start
            setTimeout(() => {
                window.location.href = "/SuccessCheckout";
            }, 300);
        }
        else if (contentType.includes("application/json")) {
            const data = await response.json();
            if (data.status.toLocaleLowerCase() != 'success') { }
            else { }
        } else if (contentType.includes("text/")) {
            const text = await response.text();
            { }
        }
    }
    return;
}
);
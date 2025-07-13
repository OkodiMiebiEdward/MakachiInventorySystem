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
                if (data && data.html) {
                    $('#compmodal').empty();
                    $('#compmodal').append(data.html);
                    $("#compmodal").modal('show');
                    $('#discount').val(data.discount); // Set the discount value
                }
            }
        }
        catch (e) {
            toastr.error("An error occurred, please try again.");
            $("#compmodal").modal('hide');
        }
    }
};

async function addToCart() {
    const divElement = document.getElementById("actionTable");
    if (divElement) {
        divElement.style.display = "block";
    }

    let barcode = jQuery('#barcodenumber').val() as string;
    let price = jQuery('#price').val() as string;
    let quantity = parseInt(jQuery('#quantity').val() as string);
    let discount = parseInt(jQuery('#discount').val() as string);

    try {
        const response = await fetch(`?handler=InitiateSale&discount=${discount}&priceSold=${price}&quantity=${quantity}&barcode=${barcode}`, { method: 'GET' });
        const json = await response.json();
        const tableBody = document.querySelector("#saleTable tbody");
        const newRow = document.createElement("tr");

        // Create cells and add data
        [
            json.quantity,
            json.priceSold,
        ].forEach(value => {
            const cell = document.createElement("td");
            cell.textContent = value;
            newRow.appendChild(cell);
        });

        // Add the row to the table
        tableBody?.appendChild(newRow);
    } catch (e) {

    }
}
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

jQuery(() => {
    const status = $('#status').val() as string;
    const description = $('#description').val() as string;

    if (status === "Success") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Module/Product/ProductList";
        }, 5000)
    }

    else if (status === "Deleted") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Module/Product/ProductList";
        }, 5000)
    }

    else if (status === "Failed") {
        toastr.error(description);
    }
});


$('#hiddenSave').on('click', () => {
    const productName = $('#productname').val() as string;
    const description = $('#productDescription').val() as string;
    const category = $('#selectedCategory').val() as string;
    const sku = $('#sku').val() as string;
    const barcodenumber = $('#barcodenumber').val() as string;
    const price = $('#price').val() as string;

    if (productName === "") {
        toastr.error("Please enter product name to proceed.", "Validation Error");
        return;
    }

    if (description === "") {
        toastr.error("Please enter description to proceed.", "Validation Error");
        return;
    }

    if (category === "") {
        toastr.error("Please select category to proceed.", "Validation Error");
        return;
    }

    if (sku === "") {
        toastr.error("Please generate the SKU code to proceed.", "Validation Error");
        return;
    }

    if (barcodenumber === "") {
        toastr.error("Please generate the bar code number to proceed.", "Validation Error");
        return;
    }

    if (price === "") {
        toastr.error("Please enter price to proceed.", "Validation Error");
        return;
    }

    $('#saveBtn').click();
});

function generate(option: number) {
    let productName = $('#productname').val() as string;
    let productDescription = $('#productDescription').val() as string;
    let category = $('#selectedCategory').val() as string;
    let sku = $('#sku').val() as string;
    let barcodenumber = $('#barcodenumber').val() as string;
    let price = $('#price').val() as string;

    if (option === 0) {
        // SKU generation (as before)
        let categoryPart = (category || "GEN").replace(/\s/g, "").toUpperCase();
        categoryPart = categoryPart.length > 3 ? categoryPart.substring(0, 3) : categoryPart.padEnd(3, 'X');

        let namePart = (productName || "PROD").replace(/\s/g, "").toUpperCase();
        namePart = namePart.length > 3 ? namePart.substring(0, 3) : namePart.padEnd(3, 'X');

        let randomPart = Math.floor(1000 + Math.random() * 9000).toString();

        let generatedSku = `${categoryPart}-${namePart}-${randomPart}`;
        $('#sku').val(generatedSku);
    } else if (option === 1) {
        // Barcode number generation (EAN-13 style)
        let prefix = "200";
        // Try to get category id and product id from data attributes if available, else use 0
        let categoryId = ($('#selectedCategory').find(':selected').data('id') || 0).toString().padStart(3, '0');
        let productId = ($('#productid').val() || 0).toString().padStart(4, '0');
        let randomPart = Math.floor(Math.random() * 1000).toString().padStart(3, '0');

        let partial = `${prefix}${categoryId}${productId}${randomPart}`; // 12 digits

        // EAN-13 check digit calculation
        let sum = 0;
        for (let i = 0; i < partial.length; i++) {
            let digit = parseInt(partial.charAt(i), 10);
            sum += (i % 2 === 0) ? digit : digit * 3;
        }
        let checkDigit = (10 - (sum % 10)) % 10;

        let barcode = partial + checkDigit.toString();
        $('#barcodenumber').val(barcode);
    }
}

function addTableRow(): void {
    const tableBody = document.getElementById('variantsTableBody') as HTMLTableSectionElement | null;
    if (!tableBody) {
        console.error('Table body with id "variantsTableBody" not found.');
        return;
    }

    const rowCount: number = tableBody.rows.length;
    const newRow: HTMLTableRowElement = document.createElement('tr');

    newRow.innerHTML = `
        <td class="${rowCount}">${rowCount + 1}</td>
        <td style="padding:10px; border:1px solid lightgrey;">
            <input type="text" class="form-control ${rowCount}" name="Product.Variants[${rowCount}].Size" />
        </td>
        <td style="padding:10px; border:1px solid lightgrey;">
            <input type="text" class="form-control code ${rowCount}" name="Product.Variants[${rowCount}].Color" />
        </td>
    `;
    tableBody.appendChild(newRow);
}
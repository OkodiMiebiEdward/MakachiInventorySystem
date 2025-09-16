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
            window.location.href = "/Module/Stock/StockList";
        }, 5000)
    }

    else if (status === "Deleted") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Module/Stock/StockList";
        }, 5000)
    }

    else if (status === "Failed") {
        toastr.error(description);
    }

    else if (status === "ServerError") {
        toastr.error(description);
        setTimeout(() => {
            window.location.href = "/Module/Stock/ProductsStock";
        }, 5000)
    }
});

$('#hiddenSave').on('click', () => {
    const productName = $('#itemName').val() as string;
    const quantityInStock:number =  parseInt($('#quantityInStock').val() as string);
    const category = $('#selectedCategory').val() as string;
    const costPerUnit = parseFloat($('#costPerUnit').val() as string);
    const sellingUnitPrice = parseFloat($('#sellingUnitPrice').val() as string);
    const discount = parseFloat($('#discount').val() as string);

    if (productName === "") {
        toastr.error("Please enter product name to proceed.",
            "Validation Error");
        return;
    }

    if (quantityInStock <= 0) {
        toastr.error("The quantity in stock cannot be 0 or less than 0.",
            "Validation Error");
        return;
    }

    if (category === "") {
        toastr.error("Please select category to proceed.",
            "Validation Error");
        return;
    }

    if (costPerUnit <= 0) {
        toastr.error("Enter valid cost price, Cost cannot be 0 or less than 0.",
            "Validation Error");
        return;
    }

    if (sellingUnitPrice <= 0) {
        toastr.error("Enter valid selling price, Selling price cannot be 0 or less than 0.",
            "Validation Error");
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
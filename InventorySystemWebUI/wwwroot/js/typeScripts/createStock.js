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
    const status = $('#status').val();
    const description = $('#description').val();
    if (status === "Success") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Module/Stock/StockList";
        }, 5000);
    }
    else if (status === "Deleted") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Module/Stock/StockList";
        }, 5000);
    }
    else if (status === "Failed") {
        toastr.error(description);
    }
    else if (status === "ServerError") {
        toastr.error(description);
        setTimeout(() => {
            window.location.href = "/Module/Stock/ProductsStock";
        }, 5000);
    }
});
$('#hiddenSave').on('click', () => {
    const productName = $('#itemName').val();
    const quantityInStock = parseInt($('#quantityInStock').val());
    const category = $('#selectedCategory').val();
    const costPerUnit = parseFloat($('#costPerUnit').val());
    const sellingUnitPrice = parseFloat($('#sellingUnitPrice').val());
    const discount = parseFloat($('#discount').val());
    if (productName === "") {
        toastr.error("Please enter product name to proceed.", "Validation Error");
        return;
    }
    if (quantityInStock <= 0) {
        toastr.error("The quantity in stock cannot be 0 or less than 0.", "Validation Error");
        return;
    }
    if (category === "") {
        toastr.error("Please select category to proceed.", "Validation Error");
        return;
    }
    if (costPerUnit <= 0) {
        toastr.error("Enter valid cost price, Cost cannot be 0 or less than 0.", "Validation Error");
        return;
    }
    if (sellingUnitPrice <= 0) {
        toastr.error("Enter valid selling price, Selling price cannot be 0 or less than 0.", "Validation Error");
        return;
    }
    $('#saveBtn').click();
});
function generate(option) {
    let productName = $('#productname').val();
    let productDescription = $('#productDescription').val();
    let category = $('#selectedCategory').val();
    let sku = $('#sku').val();
    let barcodenumber = $('#barcodenumber').val();
    let price = $('#price').val();
    if (option === 0) {
        let categoryPart = (category || "GEN").replace(/\s/g, "").toUpperCase();
        categoryPart = categoryPart.length > 3 ? categoryPart.substring(0, 3) : categoryPart.padEnd(3, 'X');
        let namePart = (productName || "PROD").replace(/\s/g, "").toUpperCase();
        namePart = namePart.length > 3 ? namePart.substring(0, 3) : namePart.padEnd(3, 'X');
        let randomPart = Math.floor(1000 + Math.random() * 9000).toString();
        let generatedSku = `${categoryPart}-${namePart}-${randomPart}`;
        $('#sku').val(generatedSku);
    }
    else if (option === 1) {
        let prefix = "200";
        let categoryId = ($('#selectedCategory').find(':selected').data('id') || 0).toString().padStart(3, '0');
        let productId = ($('#productid').val() || 0).toString().padStart(4, '0');
        let randomPart = Math.floor(Math.random() * 1000).toString().padStart(3, '0');
        let partial = `${prefix}${categoryId}${productId}${randomPart}`;
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
//# sourceMappingURL=createStock.js.map
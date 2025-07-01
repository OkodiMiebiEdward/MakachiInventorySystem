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
    if (quantityInStock === 0) {
        toastr.error("The quantity in stock cannot be 0.", "Validation Error");
        return;
    }
    if (category === "") {
        toastr.error("Please select category to proceed.", "Validation Error");
        return;
    }
    if (costPerUnit === 0) {
        toastr.error("Enter valid cost price, Cost cannot be 0.", "Validation Error");
        return;
    }
    if (sellingUnitPrice === 0) {
        toastr.error("Enter valid selling price, Selling price cannot be 0.", "Validation Error");
        return;
    }
    $('#saveBtn').click();
});
//# sourceMappingURL=createStock.js.map
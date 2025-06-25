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
            window.location.href = "/Module/Product/ProductList";
        }, 5000);
    }
    else if (status === "Deleted") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Module/Product/ProductList";
        }, 5000);
    }
    else if (status === "Failed") {
        toastr.error(description);
    }
});
$('#hiddenSave').on('click', () => {
    const productName = $('#productname').val();
    const description = $('#productDescription').val();
    const category = $('#selectedCategory').val();
    const sku = $('#sku').val();
    const barcodenumber = $('#barcodenumber').val();
    const price = $('#price').val();
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
function addTableRow() {
    const tableBody = document.getElementById('variantsTableBody');
    if (!tableBody) {
        console.error('Table body with id "variantsTableBody" not found.');
        return;
    }
    const rowCount = tableBody.rows.length;
    const newRow = document.createElement('tr');
    newRow.innerHTML = `
        <td class="${rowCount}">${rowCount + 1}</td>
        <td style="padding:10px; border:1px solid lightgrey;">
            <input type="text" class="form-control ${rowCount}" name="Product.Variants[${rowCount}].Size" />
        </td>
        <td style="padding:10px; border:1px solid lightgrey;">
            <input type="text" class="form-control code ${rowCount}" name="Product.Variants[${rowCount}].Color" />
        </td>
        <td style="padding:10px; border:1px solid lightgrey;">
            <input type="text" class="form-control descr ${rowCount}" name="Product.Variants[${rowCount}].Price" value="0.00"/>
        </td>
    `;
    tableBody.appendChild(newRow);
}
//# sourceMappingURL=createProduct.js.map
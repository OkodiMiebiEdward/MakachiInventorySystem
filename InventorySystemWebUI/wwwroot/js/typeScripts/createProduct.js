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
    $('#variantsTableBody').on('click', '.remove-btn', function () {
        const $row = $(this).closest('tr');
        const $tableBody = $('#variantsTableBody');
        const rowCount = $tableBody.find('tr').length;
        if (rowCount <= 1) {
            toastr.info("At least a single row should be displayed.");
            return;
        }
        const rowIndex = $row.index();
        $row.remove();
        reindexTableRows($tableBody[0]);
    });
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
    else if (status === "ServerError") {
        toastr.error(description);
        setTimeout(() => {
            window.location.href = "/Module/Product/CreateProduct";
        }, 5000);
    }
});
$('#hiddenSave').on('click', () => {
    const productName = $('#productname').val();
    const description = $('#productDescription').val();
    const category = $('#selectedCategory').val();
    const sku = $('#sku').val();
    if (productName === "") {
        toastr.error("Please enter product name to proceed.", "Validation Error");
        return;
    }
    if (description === "") {
        toastr.error("Please enter description to proceed.", "Validation Error");
        return;
    }
    if (sku === "") {
        toastr.error("Please generate the SKU code to proceed.", "Validation Error");
        return;
    }
    if (category === "") {
        toastr.error("Please select category to proceed.", "Validation Error");
        return;
    }
    $('#saveBtn').click();
});
function randomFetch(option) {
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
function attachRemoveButtonListener(removeButton, row) {
    removeButton.addEventListener('click', () => removeTableRow(row));
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
        <td style="padding:10px; border:1px solid lightgrey; text-align:center";>
            <button class="btn btn-danger remove-btn">Remove</button>
        </td>
    `;
    const removeButton = newRow.querySelector('.remove-btn');
    if (removeButton) {
        attachRemoveButtonListener(removeButton, newRow);
    }
    tableBody.appendChild(newRow);
}
function removeTableRow(row) {
    const tableBody = document.getElementById('variantsTableBody');
    if (!tableBody)
        return;
    const rows = tableBody.querySelectorAll('tr');
    if (rows.length <= 1) {
        toastr.info("At least a single row should be displayed.");
        return;
    }
    row.remove();
    reindexTableRows(tableBody);
}
function reindexTableRows(tableBody) {
    const rows = tableBody.rows;
    for (let i = 0; i < rows.length; i++) {
        const row = rows[i];
        const numberCell = row.cells[0];
        numberCell.textContent = (i + 1).toString();
        numberCell.className = `${i}`;
        const sizeInput = row.querySelector('input[name^="Product.Variants"][name$=".Size"]');
        if (sizeInput) {
            sizeInput.name = `Product.Variants[${i}].Size`;
            sizeInput.className = `form-control ${i}`;
        }
        const colorInput = row.querySelector('input[name^="Product.Variants"][name$=".Color"]');
        if (colorInput) {
            colorInput.name = `Product.Variants[${i}].Color`;
            colorInput.className = `form-control code ${i}`;
        }
        const removeButton = row.querySelector('.remove-btn');
        if (removeButton) {
            removeButton.removeEventListener('click', () => removeTableRow(row));
            removeButton.addEventListener('click', () => removeTableRow(row));
        }
    }
}
//# sourceMappingURL=createProduct.js.map
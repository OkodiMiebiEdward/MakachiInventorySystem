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
        // Find the row containing the clicked button
        const $row = $(this).closest('tr');
        const $tableBody = $('#variantsTableBody');
        const rowCount = $tableBody.find('tr').length;

        // Prevent removing the last row
        if (rowCount <= 1) {
            toastr.info("At least a single row should be displayed.");
            return;
        }

        // Get the index of the row (0-based)
        const rowIndex = $row.index();

        // Optional: Show the index for debugging
        // toastr.info(`Removing row at index: ${rowIndex}`);

        // Remove the row
        $row.remove();

        // Optionally, reindex the remaining rows if needed
        reindexTableRows($tableBody[0] as HTMLTableSectionElement);
    });


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

    else if (status === "ServerError") {
        toastr.error(description);
        setTimeout(() => {
            window.location.href = "/Module/Product/CreateProduct";
        }, 5000)
    }
});


$('#hiddenSave').on('click', () => {
    const productName = $('#productname').val() as string;
    const description = $('#productDescription').val() as string;
    const category = $('#selectedCategory').val() as string;
    const sku = $('#sku').val() as string;

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

function randomFetch(option: number) {
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

function attachRemoveButtonListener(removeButton: HTMLButtonElement, row: HTMLTableRowElement): void {
    removeButton.addEventListener('click', () => removeTableRow(row));
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
        <td style="padding:10px; border:1px solid lightgrey; text-align:center";>
            <button class="btn btn-danger remove-btn">Remove</button>
        </td>
    `;

    // Attach event listener to the remove button
    const removeButton = newRow.querySelector('.remove-btn') as HTMLButtonElement;
    if (removeButton) {
        attachRemoveButtonListener(removeButton, newRow);
    }

    tableBody.appendChild(newRow);
}

function removeTableRow(row: HTMLTableRowElement): void {
    const tableBody = document.getElementById('variantsTableBody') as HTMLTableSectionElement | null;
    if (!tableBody) return;

    // Check how many rows are currently in the table body
    const rows = tableBody.querySelectorAll('tr');
    if (rows.length <= 1) {
        toastr.info("At least a single row should be displayed.");
        return;
    }

    row.remove();
    reindexTableRows(tableBody);
}

function reindexTableRows(tableBody: HTMLTableSectionElement): void {
    const rows = tableBody.rows;

    for (let i = 0; i < rows.length; i++) {
        const row = rows[i];

        // Update row number
        const numberCell = row.cells[0];
        numberCell.textContent = (i + 1).toString();
        numberCell.className = `${i}`;

        // Update Size input
        const sizeInput = row.querySelector('input[name^="Product.Variants"][name$=".Size"]') as HTMLInputElement;
        if (sizeInput) {
            sizeInput.name = `Product.Variants[${i}].Size`;
            sizeInput.className = `form-control ${i}`;
        }

        // Update Color input
        const colorInput = row.querySelector('input[name^="Product.Variants"][name$=".Color"]') as HTMLInputElement;
        if (colorInput) {
            colorInput.name = `Product.Variants[${i}].Color`;
            colorInput.className = `form-control code ${i}`;
        }

        // Update Remove button class if needed (optional)
        const removeButton = row.querySelector('.remove-btn') as HTMLButtonElement;
        if (removeButton) {
            removeButton.removeEventListener('click', () => removeTableRow(row)); // Remove old listener
            removeButton.addEventListener('click', () => removeTableRow(row));    // Add new one
        }
    }
}

//Implement the remove button action

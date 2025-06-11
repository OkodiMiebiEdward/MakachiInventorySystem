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
            window.location.href = "/Module/Product/CategoryList";
        }, 5000);
    }
    else if (status === "Deleted") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Module/Product/CategoryList";
        }, 5000);
    }
    else if (status === "Failed") {
        toastr.error(description);
    }
});
$('#hiddenSave').on('click', () => {
    const categoryName = $('#categoryname').val();
    const description = $('#categorydescription').val();
    if (categoryName === "") {
        toastr.error("Please enter category name to proceed.", "Validation Error");
        return;
    }
    if (description === "") {
        toastr.error("Please enter description to proceed.", "Validation Error");
        return;
    }
    $('#saveBtn').click();
});
//# sourceMappingURL=createCategory.js.map
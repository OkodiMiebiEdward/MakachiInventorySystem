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
            window.location.href = "/Module/UserManagement/RoleList";
        }, 5000);
    }
    else if (status === "Deleted") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Module/UserManagement/RoleList";
        }, 5000);
    }
    else if (status === "Failed") {
        toastr.error(description);
    }
    else if (status === "ServerError") {
        toastr.error(description);
        setTimeout(() => {
            window.location.href = "/Module/UserManagement/CreateRole";
        }, 5000);
    }
});
$('#hiddenSave').on('click', () => {
    const rolename = $('#rolename').val();
    const description = $('#textDescription').val();
    if (rolename === "") {
        toastr.error("Please enter rolename to proceed.", "Validation Error");
        return;
    }
    if (description === "") {
        toastr.error("Please enter description to proceed.", "Validation Error");
        return;
    }
    $('#saveBtn').click();
});
//# sourceMappingURL=createRole.js.map
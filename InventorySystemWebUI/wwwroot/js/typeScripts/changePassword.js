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
            window.location.href = "/Index";
        }, 5000);
    }
    else if (status === "Error") {
        toastr.error(description);
    }
    else if (status === "ServerError") {
        toastr.error(description);
    }
});
$('#hiddenSave').on('click', () => {
    const username = $('#username').val();
    const currentpassword = $('#currentpassword').val();
    const newpassword = $('#newpassword').val();
    if (username === "") {
        toastr.error("Please enter your username to proceed.", "Validation Error");
        return;
    }
    if (currentpassword === "") {
        toastr.error("Current password is required", "Validation Error");
        return;
    }
    if (newpassword === "") {
        toastr.error("New password is required", "Validation Error");
        return;
    }
    $("#saveBtn").click();
});
//# sourceMappingURL=changePassword.js.map
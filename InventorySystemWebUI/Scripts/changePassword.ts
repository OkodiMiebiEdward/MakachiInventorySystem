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

jQuery((): void => {
    const status = $('#status').val() as string;
    const description = $('#description').val() as string;

    if (status === "Success") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Login";
        }, 5000)
    }
    else if (status === "Error") {
        toastr.error(description);
    }
    else if (status === "ServerError") {
        toastr.error(description);
    }
});

$('#hiddenSave').on('click', () => {
    const username = $('#username').val() as string;
    const currentpassword = $('#currentpassword').val() as string;
    const newpassword = $('#newpassword').val() as string;

    if (username === "") {
        toastr.error("Please enter your username to proceed.", "Validation Error");
        return;
    }

    if (currentpassword === "") {
        toastr.error("Current password is required", "Validation Error")
        return;
    }

    if (newpassword === "") {
        toastr.error("New password is required", "Validation Error")
        return;
    }
    $("#saveBtn").click();
});
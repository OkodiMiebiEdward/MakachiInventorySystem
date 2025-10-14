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
            console.log("Redirecting now");
        window.location.href = `${window.location.origin}/Home`;
        }, 5000)
    }
    else if (status === "Error") {
        toastr.error(description);
    }
    else if (status === "ServerError") {
       toastr.error(description);
    }
});

$('#check').on('click', () => {
    const username = $('#username').val() as string;
    const password = $('#password').val() as string;

    if (username === "") {
        toastr.error("Please enter your username to proceed.",
            "Validation Error");
        return;
    }

    if (password === "") {
        toastr.error("password is required", "Validation Error")
        return;
    }
    $("#post").click();
});
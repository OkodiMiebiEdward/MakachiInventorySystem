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
            window.location.href = "/Home";
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
    //const email = $('#email').val() as string;
    const password = $('#password').val() as string;

    if (username === "") {
        toastr.error("Please enter your username to proceed.", "Validation Error");
        return;
    }

    //#region EmailValidation
    // Email validation regex
    //const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    //if (email === "") {
    //    toastr.error("Please enter your email address.", "Validation Error");
    //    return;
    //}

    //if (!emailRegex.test(email)) {
    //    toastr.error("Please enter a valid email address.", "Validation Error");
    //    return;
    //}
    //#endregion

    if (password === "") {
        toastr.error("password is required", "Validation Error")
        return;
    }
    $("#post").click();
});
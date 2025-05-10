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
            window.location.href = "/Login";
        }, 5000);
    }
    else if (status === "Failed") {
        toastr.error(description);
    }
});
$('#check').on('click', () => {
    const username = $('#username').val();
    const email = $('#email').val();
    const password = $('#password').val();
    const confirmpassword = $('#confirmpassword').val();
    const phone = $('#phonenumber').val();
    if (username === "") {
        toastr.error("Please enter your username to proceed.", "Validation Error");
        return;
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (email === "") {
        toastr.error("Please enter your email address.", "Validation Error");
        return;
    }
    if (!emailRegex.test(email)) {
        toastr.error("Please enter a valid email address.", "Validation Error");
        return;
    }
    if (phone === "") {
        toastr.error("phone number is required", "Validation Error");
        return;
    }
    if (password === "") {
        toastr.error("password is required", "Validation Error");
        return;
    }
    if (confirmpassword === "") {
        toastr.error("Please confirm your password", "Validation Error");
        return;
    }
    $("#post").click();
});
//# sourceMappingURL=index.js.map
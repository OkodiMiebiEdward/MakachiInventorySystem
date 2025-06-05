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
    const status = $('#status').val() as string;
    const description = $('#description').val() as string;

    if (status === "Success") {
        toastr.success(description);
        setTimeout(() => {
            window.location.href = "/Module/UserManagement/AssignRoleList";
        }, 5000)
    }
    else if (status === "Failed") {
        toastr.error(description);
    }
    else if (status === "Role Exist") {
        toastr.error(description);
    }
});

$('#hiddenSave').on('click', () => {
    const rolename = $('#selectedUser').val() as string;
    const description = $('#selectedRole').val() as string;

    if (rolename === "") {
        toastr.error("Please select a user.", "Validation Error");
        return;
    }

    if (description === "") {
        toastr.error("Please select a role to proceed.", "Validation Error");
        return;
    }

    $('#saveBtn').click();
});
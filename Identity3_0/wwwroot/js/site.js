// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$("#EmailRegistration").on(addEventListener("focusout"), function () {
    var data = $("#EmailRegistration").val();

    $.ajax({
        url: "/Validation/VerifyEmail",
        method: "POST",
        data: { email: data },
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val
        }
    }).done(function (result) {
        if (result !== true) {
            $("#EmailRegistrationVali").val = result;
        }
    })
});
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$("#EmailRegistration").on("focusout", function () {
    var data = $("#EmailRegistration").val()

    $.ajax({
        url: "/Validation/VerifyEmail",
        method: "POST",
        data: { email: data },
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    }).done(function (result) {
        if (result !== true) {
            $("#EmailRegistrationVali").html(result)
            $("#registerSubmitBtn").prop("disabled", true)
        } else {
            $("#EmailRegistrationVali").html("")
            $("#registerSubmitBtn").prop("disabled", false)
        }
    }).catch(function (error) {
        throw new Error(error);
    })
});

if ($(".updatedText").val() !== null) {
    setTimeout(() => $(".updatedText").fadeOut(), 3000)
}
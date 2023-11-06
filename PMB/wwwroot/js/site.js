// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    var texterr = $(".lblpesan").html();
    var textsuc = $(".lblpesanberhasil").html();
    if (texterr != "" && texterr != undefined) {
        toastr.options.timeOut = 2000;
        toastr.options.positionClass = "toast-top-center";
        toastr.error(texterr);
    }

    if (textsuc != "" && textsuc != undefined) {
        toastr.options.timeOut = 2000;
        toastr.options.positionClass = "toast-top-center";
        toastr.success(textsuc);
    }
});

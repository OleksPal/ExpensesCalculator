$(document).ready(function () {
    $("*", document.body).on("click", function () {
        event.stopPropagation();
        var domElement = $(this).get(0);
        if (domElement.innerHTML === "▷") {
            domElement.innerHTML = "◢";
        }
        else if (domElement.innerHTML === "◢") {
            domElement.innerHTML = "▷";
        }
    });
}); 
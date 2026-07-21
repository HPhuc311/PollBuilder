// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.querySelectorAll(".btnDelete").forEach(btn => {

    btn.addEventListener("click", function () {

        let id = this.dataset.id;

        Swal.fire({

            title: "Delete Poll?",

            text: "This action cannot be undone.",

            icon: "warning",

            showCancelButton: true,

            confirmButtonText: "Delete",

            cancelButtonText: "Cancel",

            confirmButtonColor: "#dc3545"

        }).then((result) => {

            if (result.isConfirmed) {

                document
                    .getElementById("deleteForm-" + id)
                    .submit();

            }

        });

    });

});
$("#fileUploadDoc").change(function (evt) {
    const selectedFile = this.files[0];
    console.log('selected file:', selectedFile)
    if (selectedFile) {
        $("#FileNameDocText").val(selectedFile.name);
    }
});

$("#SaveDocFile").click(function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

    const documentHeaderId = $("#DocumentHeaderId").val();
    const fileDescription = $("#FileDescription").val();

    if (!documentHeaderId || !fileDescription) {
        toastr.error("Favor especificar la descripción del archivo.");
        return false;
    }

    var formData = new FormData();
    var fileInput = $("input[type='file']")[0].files[0];

    if (fileInput) {
        formData.append("file", fileInput);
        formData.append("documentHeaderId", documentHeaderId);
        formData.append("description", fileDescription);

        $.ajax({
            url: "/AnexoReferencia/UploadFile",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.result === "200") {
                    toastr.success("El archivo fue guardado satisfactoriamente!");
                    $("#FileDescription").val('');
                    $("#FileNameDocText").val('');
                    $("input[type='file']").val('');

                    getUploadedFiles();
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (xhr, status, error) {
                console.error(error);
                toastr.error("Error tratando de guardar el archivo: " + error);
            }
        });
    } else {
        toastr.error("No ha seleccionado ningun archivo.");
    }
});

function deleteFile(id, fileName) {

    if (confirm(`Esta seguro de eliminar el archivo: ${fileName}?`)) {

        $.ajax({
            "url": `/AnexoReferencia/DeleteFile?id=${id}`,
            "type": "POST",
            "success": async function (response) {
                if (response.result === "200") {
                    toastr.success(`El archivo: ${fileName} fue eliminado con exito!`);
                    getUploadedFiles();
                } else {
                    toastr.error(response.message);
                }
            }
        });
    }
}

function getUploadedFiles() {
    const documentHeaderId = $("#DocumentHeaderId").val()
    $.ajax({
        "url": `/AnexoReferencia/GetUploadedFiles?documentHeaderId=${documentHeaderId}`,
        "type": "POST",
        "success": async function (response) {
            if (response.result === "200") {
                let docs = [];
                for (const doc of response.message) {
                    docs.push(`<a href="/AnexosReferencias/${doc.Url}" class="text-custom">${doc.Name}</a>
                                   <span class='fa fa-trash text-custom h6' style='cursor: pointer;' onclick="deleteFile(${doc.Id}, '${doc.Name}')" title="Eliminar"> </span> <br/>`);
                }

                $("#uploaded-files").html(`<span class="mt-2"> <strong>Archivos adjuntos:</strong> </span><br/>`);
                $("#uploaded-files").append(docs);

                console.log('docs:', docs )
                if (!docs.length)
                    $("#uploaded-files").html("");
            }
        }
    });
}
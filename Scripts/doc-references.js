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
    const filePath = $("#FileNameDocText").val();
    const referenceType = $("#ReferenceType").val();

    var formData = new FormData();
    var fileInput = $("input[type='file']")[0].files[0];

    if (!documentHeaderId || !fileDescription) {
        toastr.error("Favor especificar la descripción/título del anexo.");
        return false;
    }

    if (referenceType === "Archivo" && !fileInput) {
        toastr.error("No ha seleccionado ningun archivo.");
        return false;
    }

    if (referenceType === "Link" && !filePath) {
        toastr.error("No ha especificado ningun hipervinculo.");
        return false;
    }

    formData.append("referenceType", $("#ReferenceType").val());
    formData.append("file", fileInput);
    formData.append("documentHeaderId", documentHeaderId);
    formData.append("description", fileDescription);
    formData.append("filePath", filePath);

    $.ajax({
        url: "/AnexoReferencia/UploadFile",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.result === "200") {
                toastr.success("El anexo fue guardado satisfactoriamente!");
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
            toastr.error("Error tratando de guardar el anexo: " + error);
        }
    });
});

function deleteFile(id, fileName) {

    if (confirm(`Esta seguro de eliminar el anexo: ${fileName}?`)) {

        $.ajax({
            "url": `/AnexoReferencia/DeleteFile?id=${id}`,
            "type": "POST",
            "success": async function (response) {
                if (response.result === "200") {
                    toastr.success(`El anexo: ${fileName} fue eliminado con exito!`);
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
                    const url = doc.ReferenceType === "Link" ? doc.Url : `/AnexosReferencias/${doc.Url}`;

                    docs.push(`<span class='fa fa-trash text-custom h6 mr-1' style='cursor: pointer;' onclick="deleteFile(${doc.Id}, '${doc.Name}')" title="Eliminar"> </span>
                                <a href="${url}" target="_blank" class="text-custom">${doc.Name}</a> <br/>`);
                }

                $("#uploaded-files").html(`<span class="mt-1 mb-1"> <strong>Anexos agregados:</strong> </span><br/>`);
                $("#uploaded-files").append(docs);

                console.log('docs:', docs )
                if (!docs.length)
                    $("#uploaded-files").html("");
            }
        }
    });
}
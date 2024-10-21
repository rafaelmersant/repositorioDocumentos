var proceduresCount = 1;

$('.btn-new-procedure').click(function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

    if (!$('.save-procedure-btn')[0]) {
        const newRowHtml = '<tr>' +
            `<td class="field-sortindex-procedure-new"><input type="text" maxlength="3" value="${proceduresCount}" class="form-control form-control-sm new-sortindex-procedure"></td>` +
            '<td class="field-responsible-procedure-new"><input type="text" class="form-control form-control-sm col-12 new-responsible-procedure"/></td>' +
            '<td class="field-description-procedure-new"><div id="description-procedure" class="col-12 new-description-procedure" placeholder="Escribir descripci&oacute;n"></div></td>' +
            '<td class="text-center">' +
            '<button class="btn btn-sm btn-success save-procedure-btn">Guardar</button>' +
            '<button class="btn btn-sm btn-danger cancel-procedure-btn">Cancelar</button>' +
            '</td>' +
            '</tr>';

        $('#procedureTable tbody').prepend(newRowHtml);
      
        tinymce.init({
            selector: '#description-procedure',
            menubar: false,
            plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount linkchecker',
            toolbar: 'undo redo | fontfamily fontsize | bold italic underline strikethrough | table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
        });
    }
});

$(document).on('click', '.save-procedure-btn', async function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

    var sortindex = $(this).closest('tr').find('.new-sortindex-procedure').val();
    var responsible = $(this).closest('tr').find('.new-responsible-procedure').val();
    var description = await getProcedureBody();

    const documentHeaderId = $("#DocumentHeaderId").val();

    if (!sortindex || !responsible || !description) {
        toastr.error("Debe completar todos los campos.");
        return false;
    }
   
    const parameters = { documentHeaderId, sortindex, responsible, description };

    $.ajax({
        "url": `/Procedimiento/AddProcedure`,
        "type": "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(parameters),
        "success": async function (response) {
            if (response.result === "200") {
                toastr.success(`El registro fue agregado!`);
                $('.field-sortindex-procedure-new').text(sortindex);
                $('.field-responsible-procedure-new').text(responsible);
                $('.field-description-procedure-new').text(description);

                getProcedure();
            } else {
                toastr.error(response.message);
            }
        }
    });
});

$(document).on('click', '.cancel-procedure-btn', function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

    $(this).closest('tr').remove();
});

async function getProcedure() {
    $("#procedure-tbody").empty();
    const documentHeaderId = $("#DocumentHeaderId").val();

    if (processingProcedures === true) return false;
    processingProcedures = true;

    $.ajax({
        url: `/Procedimiento/GetProcedure?documentHeaderId=${documentHeaderId}`,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    }).done(function (data) {
        console.log('DATA GetProcedure:', data)

        if (data.result === "200") {
            proceduresCount = 1;

            for (const item of data.message) {
                let description = item.Description;//.replaceAll('text-indent:', '');

                const itemRow = '<tr>' +
                    `<td class="field-sortindex-procedure text-center">${item.SortIndex}</td>` +
                    `<td class="field-responsible-procedure">${item.Responsible}</td>` +
                    `<td class="field-description-procedure">${description}<input class="field-description-procedure-raw" type='hidden' value='${description.replaceAll('text-indent:', '')}'></td>` +
                    '<td class="text-center">' +
                    `<input type="hidden" class="field-id-procedure" value="${item.Id}">` +
                    '<a class="btn btn-sm btn-success btn-edit-procedure edit-button-width" href="javascript:void(0)" title="Editar">Editar</a> ' +
                    ' <a class="btn btn-sm btn-danger btn-remove-procedure" href="javascript:void(0)" title="Eliminar">Eliminar</a>' +
                    '</td>' +
                    '</tr>';

                $('#procedureTable tbody').append(itemRow);

                proceduresCount += 1;
            }
            $('.field-description-procedure p').each(function () {
                const textIndentValue = parseFloat($(this).css('text-indent'));

                if (textIndentValue < 0) {
                    $(this).css('text-indent', '-8.0pt');
                }
            });
                       
            $('#procedureTable').on('click', '.btn-edit-procedure', async function (evt) {
                evt.preventDefault();
                evt.stopPropagation();

                var editButton = $(this);

                if (editButton.prop('title') === "Editar") {
                    editButton.prop('title', 'Guardar');
                    editButton.html("Guardar");
                    editButton.closest('tr').find('.btn-remove-procedure').html("Cancelar");
                    editButton.closest('tr').find('.btn-remove-procedure').prop("title", "Cancelar");

                    const sortindex = editButton.closest('tr').find('.field-sortindex-procedure').text();
                    const responsible = editButton.closest('tr').find('.field-responsible-procedure').text();
                    const descriptionRaw = editButton.closest('tr').find('.field-description-procedure-raw').val();

                    sortindexProcedureEditing = sortindex;
                    responsibleProcedureEditing = responsible;
                    descriptionProcedureEditing = descriptionRaw;
                    
                    editButton.closest('tr').find('.field-sortindex-procedure').html(`<input type="text" maxlength="3"  class="form-control form-control-sm edit-sortindex-procedure" value="${sortindex}">`);
                    editButton.closest('tr').find('.field-responsible-procedure').html(`<input type="text" value="${responsible}" class="form-control form-control-sm col-12 edit-responsible-procedure"/>`);
                    editButton.closest('tr').find('.field-description-procedure').html(`<div id="description-procedure" class="col-12 edit-description-procedure">${descriptionRaw}</div><input class="field-description-procedure-raw" type='hidden' value='${descriptionRaw}'>`);
            
                    tinymce.init({
                        selector: '#description-procedure',
                        menubar: false,
                        plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount linkchecker',
                        toolbar: 'undo redo | fontfamily fontsize | bold italic underline strikethrough | table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
                    });
                } else {
                    const id = editButton.closest('tr').find('.field-id-procedure').val();
                    const sortindex = editButton.closest('tr').find('.edit-sortindex-procedure').val();
                    const responsible = editButton.closest('tr').find('.edit-responsible-procedure').val();
                    const description = await getProcedureBody();

                    if (!sortindex || !responsible || !description) {
                        toastr.error("Debe completar todos los campos.");
                        return false;
                    }

                    const parameters = { id, sortindex, responsible, description };

                    $.ajax({
                        "url": `/Procedimiento/UpdateProcedure`,
                        "type": "POST",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: JSON.stringify(parameters),
                        "success": function (response) {
                            if (response.result === "200") {
                                toastr.success(`El registro #${id} fue actualizado!`);
                                editButton.prop('title', 'Editar');
                                editButton.closest('tr').find('.field-sortindex-procedure').text(sortindex);
                                editButton.closest('tr').find('.field-responsible-procedure').text(responsible);
                                editButton.closest('tr').find('.field-description-procedure').text(description);

                                editButton.html("Editar");
                                editButton.closest('tr').find('.btn-remove-procedure').html("Eliminar");
                                editButton.closest('tr').find('.btn-remove-procedure').prop("title", "Eliminar");

                                getProcedure();
                            } else {
                                toastr.error(response.message);
                            }
                        }
                    });
                }
            });

            $('#procedureTable').on('click', '.btn-remove-procedure', function (evt) {
                evt.preventDefault();
                evt.stopPropagation();

                var removeButton = $(this);

                if (removeButton.html() === "Cancelar") {
                    removeButton.prop('title', 'Eliminar');
                    removeButton.html("Eliminar");

                    const __descriptionRaw = `<input class="field-description-procedure-raw" type='hidden' value='${descriptionProcedureEditing}'>`;

                    removeButton.closest('tr').find('.field-sortindex-procedure').text(sortindexProcedureEditing);
                    removeButton.closest('tr').find('.field-responsible-procedure').text(responsibleProcedureEditing);
                    removeButton.closest('tr').find('.field-description-procedure').html(`${descriptionProcedureEditing}${__descriptionRaw}`);
          
                    removeButton.closest('tr').find('.btn-edit-procedure').html("Editar");
                    removeButton.closest('tr').find('.btn-edit-procedure').prop("title", "Editar");
                } else {
                    const id = removeButton.closest('tr').find('.field-id-procedure').val();
                    const sortindex = removeButton.closest('tr').find('.field-sortindex-procedure').text();

                    if (confirm(`Confirma que desea eliminar el registro #${sortindex}`)) {
                        $.ajax({
                            "url": `/Procedimiento/DeleteProcedure?id=${id}`,
                            "type": "POST",
                            "success": function (response) {
                                if (response.result === "200") {
                                    toastr.success(`El registro #${sortindex} fue eliminado!`);
                                    getProcedure();
                                } else {
                                    toastr.error(response.message);
                                }
                            }
                        });
                    }
                }
            });
        } else {
            toastr.error(data.message);
        }

        processingProcedures = false;
    });
}

function descriptionProcedureTextarea() {
    var $textarea = $('#description-procedure');

    function resizeTextarea($textarea) {
        if ($textarea && $textarea[0]) {
            $textarea.css('height', 'auto');
            $textarea.css('height', $textarea[0].scrollHeight + 'px');
        }
    }

    resizeTextarea($textarea);

    $textarea.on('input', function () {
        resizeTextarea($(this));
    });
}

//Procedure BODY
async function getProcedureBody() {
    const currentEditor = $("#CurrentEditor").val();
    let value = "";

    if (currentEditor === "Tiny")
        value = tinymce.get("description-procedure").getContent();
    else
        value = quill_descriptionProcedure.root.innerHTML;

    return value ? value : "";
}

async function setProcedureBody(value) {
    const currentEditor = $("#CurrentEditor").val();

    if (currentEditor === "Tiny")
        tinymce.get("description-procedure").setContent(value);
    else
        quill_descriptionProcedure.root.innerHTML = value;
}
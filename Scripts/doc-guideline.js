var guidelinesCount = 1;

$('.btn-new-guideline').click(function () {
    if (!$('.save-guideline-btn')[0]) {
        const newRowHtml = '<tr>' +
            `<td class="field-sortindex-guideline-new"><input type="text" maxlength="3" value="${guidelinesCount}" class="form-control form-control-sm new-sortindex-guideline"></td>` +
            '<td class="field-description-guideline-new"><div id="description-guideline" class="col-12 new-description-guideline" placeholder="Escribir descripci&oacute;n"></div></td>' +
            '<td class="text-center">' +
            '<button class="btn btn-sm btn-success save-guideline-btn">Guardar</button>' +
            '<button class="btn btn-sm btn-danger cancel-guideline-btn">Cancelar</button>' +
            '</td>' +
            '</tr>';

        $('#guidelineTable tbody').prepend(newRowHtml);

        tinymce.init({
            selector: '#description-guideline',
            menubar: false,
            plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount linkchecker',
            toolbar: 'undo redo | fontfamily fontsize | bold italic underline strikethrough | table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
        });
    }
});

$(document).on('click', '.save-guideline-btn', async function () {
    var sortindex = $(this).closest('tr').find('.new-sortindex-guideline').val();
    //var description = $(this).closest('tr').find('.new-description-guideline').val();
    var description = await getGuidelineBody();

    const documentHeaderId = $("#DocumentHeaderId").val();

    if (!sortindex || !description) {
        toastr.error("Debe completar todos los campos.");
        return false;
    }

    const parameters = { documentHeaderId, sortindex, description };

    $.ajax({
        "url": `/Directriz/AddGuideline`,
        "type": "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(parameters),
        "success": async function (response) {
            if (response.result === "200") {
                toastr.success(`El registro fue agregado!`);
                $('.field-sortindex-guideline-new').text(sortindex);
                $('.field-description-guideline-new').text(description);

                getGuideline();
            } else {
                toastr.error(response.message);
            }
        }
    });
});

$(document).on('click', '.cancel-guideline-btn', function () {
    $(this).closest('tr').remove();
});

async function getGuideline() {
    $("#guideline-tbody").empty();
    const documentHeaderId = $("#DocumentHeaderId").val();

    if (processingGuidelines === true) return false;
    processingGuidelines = true;

    $.ajax({
        url: `/Directriz/GetGuideline?documentHeaderId=${documentHeaderId}`,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    }).done(async function (data) {
        console.log('DATA GetGuideline:', data)

        if (data.result === "200") {
            guidelinesCount = 1;

            for (const item of data.message) {
                const itemRow = '<tr>' +
                    `<td class="field-sortindex-guideline text-center">${item.SortIndex}</td>` +
                    `<td class="field-description-guideline">${item.Description}<input class="field-description-guideline-raw" type='hidden' value='${item.Description}'></td>` +
                    '<td class="text-center">' +
                    `<input type="hidden" class="field-id-guideline" value="${item.Id}">` +
                    '<a class="btn btn-sm btn-success btn-edit-guideline edit-button-width" href="javascript:void(0)" title="Editar">Editar</a> ' +
                    ' <a class="btn btn-sm btn-danger btn-remove-guideline" href="javascript:void(0)" title="Eliminar">Eliminar</a>' +
                    '</td>' +
                    '</tr>';

                $('#guidelineTable tbody').append(itemRow);

                guidelinesCount += 1;
            }

            $('#guidelineTable').on('click', '.btn-edit-guideline', async function () {
                var editButton = $(this);
                if (editButton.prop('title') === "Editar") {
                    editButton.prop('title', 'Guardar');
                    editButton.html("Guardar");
                    editButton.closest('tr').find('.btn-remove-guideline').html("Cancelar");
                    editButton.closest('tr').find('.btn-remove-guideline').prop("title", "Cancelar");

                    const sortindex = editButton.closest('tr').find('.field-sortindex-guideline').text();
                    const descriptionRaw = editButton.closest('tr').find('.field-description-guideline-raw').val();

                    sortindexGuidelineEditing = sortindex;
                    descriptionGuidelineEditing = descriptionRaw;

                    editButton.closest('tr').find('.field-sortindex-guideline').html(`<input type="text" maxlength="3"  class="form-control form-control-sm edit-sortindex-guideline" value="${sortindex}">`);
                    editButton.closest('tr').find('.field-description-guideline').html(`<div id="description-guideline" class="col-12 edit-description-guideline">${descriptionRaw}</div><input class="field-description-guideline-raw" type='hidden' value='${descriptionRaw}'>`);

                    tinymce.init({
                        selector: '#description-guideline',
                        menubar: false,
                        plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount linkchecker',
                        toolbar: 'undo redo | fontfamily fontsize | bold italic underline strikethrough | table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
                    });

                } else {
                    const id = editButton.closest('tr').find('.field-id-guideline').val();
                    const sortindex = editButton.closest('tr').find('.edit-sortindex-guideline').val();
                    const description = await getGuidelineBody();

                    if (!sortindex || !description) {
                        toastr.error("Debe completar todos los campos.");
                        return false;
                    }

                    const parameters = { id, sortindex, description };

                    $.ajax({
                        "url": `/Directriz/UpdateGuideline`,
                        "type": "POST",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: JSON.stringify(parameters),
                        "success": async function (response) {
                            if (response.result === "200") {
                                toastr.success(`El registro #${id} fue actualizado!`);
                                editButton.prop('title', 'Editar');
                                editButton.closest('tr').find('.field-sortindex-guideline').text(sortindex);
                                editButton.closest('tr').find('.field-description-guideline').text(description);

                                editButton.html("Editar");
                                editButton.closest('tr').find('.btn-remove-guideline').html("Eliminar");
                                editButton.closest('tr').find('.btn-remove-guideline').prop("title", "Eliminar");

                                getGuideline();
                            } else {
                                toastr.error(response.message);
                            }
                        }
                    });
                }
            });

            $('#guidelineTable').on('click', '.btn-remove-guideline', function () {
                var removeButton = $(this);

                if (removeButton.html() === "Cancelar") {
                    removeButton.prop('title', 'Eliminar');
                    removeButton.html("Eliminar");

                    const __descriptionRaw = `<input class="field-description-procedure-raw" type='hidden' value='${descriptionGuidelineEditing}'>`;

                    removeButton.closest('tr').find('.field-sortindex-guideline').text(sortindexGuidelineEditing);
                    removeButton.closest('tr').find('.field-description-guideline').html(`${descriptionGuidelineEditing}${__descriptionRaw}`);

                    removeButton.closest('tr').find('.btn-edit-guideline').html("Editar");
                    removeButton.closest('tr').find('.btn-edit-guideline').prop("title", "Editar");
                } else {
                    const id = removeButton.closest('tr').find('.field-id-guideline').val();
                    const sortindex = removeButton.closest('tr').find('.field-sortindex-guideline').text();

                    if (confirm(`Confirma que desea eliminar el registro #${sortindex}`)) {
                        $.ajax({
                            "url": `/Directriz/DeleteGuideline?id=${id}`,
                            "type": "POST",
                            "success": function (response) {
                                if (response.result === "200") {
                                    toastr.success(`El registro #${sortindex} fue eliminado!`);
                                    getGuideline();
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

        processingGuidelines = false;
    });
}

function descriptionGuidelineTextarea() {
    var $textarea = $('#description-guideline');

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

//Guideline BODY
async function getGuidelineBody() {
    const currentEditor = $("#CurrentEditor").val();
    let value = "";

    if (currentEditor === "Tiny")
        value = tinymce.get("description-guideline").getContent();
    else
        value = quill_descriptionGuideline.root.innerHTML;

    return value ? value : "";
}

async function setGuidelineBody(value) {
    const currentEditor = $("#CurrentEditor").val();

    if (currentEditor === "Tiny")
        tinymce.get("description-guideline").setContent(value);
    else
        quill_descriptionGuideline.root.innerHTML = value;
}
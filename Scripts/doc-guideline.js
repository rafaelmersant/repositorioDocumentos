var guidelinesCount = 1;

$('.btn-new-guideline').click(function () {
    if (!$('.save-guideline-btn')[0]) {
        const newRowHtml = '<tr>' +
            `<td class="field-sortindex-guideline-new"><input type="text" maxlength="3" value="${guidelinesCount}" class="form-control form-control-sm new-sortindex-guideline"></td>` +
            '<td class="field-description-guideline-new"><textarea id="description-guideline" class="auto-resizing-textarea form-control form-control-sm col-12 new-description-guideline" placeholder="Escribir descripci&oacute;n"></textarea></td>' +
            '<td class="text-center">' +
            '<button class="btn btn-sm btn-success save-guideline-btn">Guardar</button>' +
            '<button class="btn btn-sm btn-danger cancel-guideline-btn">Cancelar</button>' +
            '</td>' +
            '</tr>';

        $('#guidelineTable tbody').prepend(newRowHtml);

        descriptionGuidelineTextarea();
    }
});

$(document).on('click', '.save-guideline-btn', function () {
    var sortindex = $(this).closest('tr').find('.new-sortindex-guideline').val();
    var description = $(this).closest('tr').find('.new-description-guideline').val();
    const documentHeaderId = $("#DocumentHeaderId").val();

    if (!sortindex || !description) {
        toastr.error("Debe completar todos los campos.");
        return false;
    }

    $.ajax({
        "url": `/Directriz/AddGuideline?documentHeaderId=${documentHeaderId}&sortindex=${sortindex}&description=${description}`,
        "type": "POST",
        "success": function (response) {
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

function getGuideline() {
    $("#guideline-tbody").empty();
    const documentHeaderId = $("#DocumentHeaderId").val();

    $.ajax({
        url: `/Directriz/GetGuideline?documentHeaderId=${documentHeaderId}`,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    }).done(function (data) {
        console.log('DATA GetGuideline:', data)

        if (data.result === "200") {
            guidelinesCount = 1;

            for (const item of data.message) {
                const itemRow = '<tr>' +
                    `<td class="field-sortindex-guideline text-center">${item.SortIndex}</td>` +
                    `<td class="field-description-guideline">${item.Description}</td>` +
                    '<td class="text-center">' +
                    `<input type="hidden" class="field-id-guideline" value="${item.Id}">` +
                    '<a class="btn btn-sm btn-success btn-edit-guideline edit-button-width" href="javascript:void(0)" title="Editar">Editar</a> ' +
                    ' <a class="btn btn-sm btn-danger btn-remove-guideline" href="javascript:void(0)" title="Eliminar">Eliminar</a>' +
                    '</td>' +
                    '</tr>';

                $('#guidelineTable tbody').append(itemRow);

                guidelinesCount += 1;
            }

            $('.btn-edit-guideline').click(function () {
                var editButton = $(this);

                if (editButton.prop('title') === "Editar") {
                    editButton.prop('title', 'Guardar');
                    editButton.html("Guardar");
                    editButton.closest('tr').find('.btn-remove-guideline').html("Cancelar");
                    editButton.closest('tr').find('.btn-remove-guideline').prop("title", "Cancelar");

                    const sortindex = editButton.closest('tr').find('.field-sortindex-guideline').text();
                    const description = editButton.closest('tr').find('.field-description-guideline').text();

                    sortindexGuidelineEditing = sortindex;
                    descriptionGuidelineEditing = description;

                    editButton.closest('tr').find('.field-sortindex-guideline').html(`<input type="text" maxlength="3"  class="form-control form-control-sm edit-sortindex-guideline" value="${sortindex}">`);
                    editButton.closest('tr').find('.field-description-guideline').html(`<textarea id="description-guideline" class="form-control form-control-sm col-12 auto-resizing-textarea edit-description-guideline">${description}</textarea>`);

                    descriptionGuidelineTextarea();

                } else {
                    const id = editButton.closest('tr').find('.field-id-guideline').val();
                    const sortindex = editButton.closest('tr').find('.edit-sortindex-guideline').val();
                    const description = editButton.closest('tr').find('.edit-description-guideline').val();

                    if (!sortindex || !description) {
                        toastr.error("Debe completar todos los campos.");
                        return false;
                    }

                    $.ajax({
                        "url": `/Directriz/UpdateGuideline?id=${id}&sortindex=${sortindex}&description=${description}`,
                        "type": "POST",
                        "success": function (response) {
                            if (response.result === "200") {
                                toastr.success(`El registro #${id} fue actualizado!`);
                                editButton.prop('title', 'Editar');
                                editButton.closest('tr').find('.field-sortindex-guideline').text(sortindex);
                                editButton.closest('tr').find('.field-description-guideline').text(description);

                                editButton.html("Editar");
                                editButton.closest('tr').find('.btn-remove-guideline').html("Eliminar");
                                editButton.closest('tr').find('.btn-remove-guideline').prop("title", "Eliminar");
                            } else {
                                toastr.error(response.message);
                            }
                        }
                    });
                }
            });

            $('.btn-remove-guideline').click(function () {
                var removeButton = $(this);

                if (removeButton.html() === "Cancelar") {
                    removeButton.prop('title', 'Eliminar');
                    removeButton.html("Eliminar");

                    removeButton.closest('tr').find('.field-sortindex-guideline').text(sortindexGuidelineEditing);
                    removeButton.closest('tr').find('.field-description-guideline').text(descriptionGuidelineEditing);

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
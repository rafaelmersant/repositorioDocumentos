$('.btn-new-glossary').click(function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

    if (!$('.save-glossary-btn')[0]) {
        const newRowHtml = '<tr>' +
            '<td class="field-word-glossary-new"><input type="text" maxlength="50" min="3" class="form-control form-control-sm new-word-glossary"></td>' +
            '<td class="field-description-glossary-new"><textarea id="description-glossary" maxLength="800" class="auto-resizing-textarea form-control form-control-sm col-12 new-description-glossary" placeholder="Escribir descripci&oacute;n"></textarea></td>' +
            '<td class="text-center">' +
            '<button class="btn btn-sm btn-success save-glossary-btn">Guardar</button>' +
            '<button class="btn btn-sm btn-danger cancel-glossary-btn">Cancelar</button>' +
            '</td>' +
            '</tr>';

        $('#glossaryTable tbody').prepend(newRowHtml);

        descriptionGlossaryTextarea();
    }
});

$(document).on('click', '.save-glossary-btn', function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

    var word = $(this).closest('tr').find('.new-word-glossary').val();
    var description = $(this).closest('tr').find('.new-description-glossary').val();
    const documentHeaderId = $("#DocumentHeaderId").val();

    if (!word || !description) {
        toastr.error("Debe completar todos los campos.");
        return false;
    }

    $.ajax({
        "url": `/Glosario/AddGlossary?documentHeaderId=${documentHeaderId}&word=${word}&description=${description}`,
        "type": "POST",
        "success": function (response) {
            if (response.result === "200") {
                toastr.success(`El registro fue agregado!`);
                $('.field-word-glossary-new').text(word);
                $('.field-description-glossary-new').text(description);

                getGlossary();

            } else {
                toastr.error(response.message);
            }
        }
    });
});

$(document).on('click', '.cancel-glossary-btn', function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

    $(this).closest('tr').remove();
});

function getGlossary() {
    $("#glossary-tbody").empty();
    const documentHeaderId = $("#DocumentHeaderId").val();

    $.ajax({
        url: `/Glosario/GetGlossary?documentHeaderId=${documentHeaderId}`,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    }).done(function (data) {
        console.log('DATA GetGlossary:', data)

        if (data.result === "200") {
            for (const item of data.message) {
                const itemRow = '<tr>' +
                    `<td class="field-word-glossary">${item.Word}</td>` +
                    `<td class="field-description-glossary">${item.Description}</td>` +
                    '<td class="text-center">' +
                    `<input type="hidden" class="field-id-glossary" value="${item.Id}">` +
                    '<a class="btn btn-sm btn-success btn-edit-glossary edit-button-width" href="javascript:void(0)" title="Editar">Editar</a> ' +
                    ' <a class="btn btn-sm btn-danger btn-remove-glossary" href="javascript:void(0)" title="Eliminar">Eliminar</a>' +
                    '</td>' +
                    '</tr>';

                $('#glossaryTable tbody').append(itemRow);
            }

            $('#glossaryTable').on('click', '.btn-edit-glossary', function (evt) {
                evt.preventDefault();
                evt.stopPropagation();

                var editButton = $(this);

                if (editButton.prop('title') === "Editar") {
                    editButton.prop('title', 'Guardar');
                    editButton.html("Guardar");
                    editButton.closest('tr').find('.btn-remove-glossary').html("Cancelar");
                    editButton.closest('tr').find('.btn-remove-glossary').prop("title", "Cancelar");

                    const word = editButton.closest('tr').find('.field-word-glossary').text();
                    const description = editButton.closest('tr').find('.field-description-glossary').text();

                    wordGlossaryEditing = word;
                    descriptionGlossaryEditing = description;

                    editButton.closest('tr').find('.field-word-glossary').html(`<input type="text" maxlength="50" min="3" class="form-control form-control-sm edit-word-glossary" value="${word}">`);
                    editButton.closest('tr').find('.field-description-glossary').html(`<textarea id="description-glossary" maxLength="800" class="form-control form-control-sm col-12 auto-resizing-textarea edit-description-glossary">${description}</textarea>`);

                    descriptionGlossaryTextarea();

                } else {
                    const id = editButton.closest('tr').find('.field-id-glossary').val();
                    const word = editButton.closest('tr').find('.edit-word-glossary').val();
                    const description = editButton.closest('tr').find('.edit-description-glossary').val();

                    if (!word || !description) {
                        toastr.error("Debe completar todos los campos.");
                        return false;
                    }

                    $.ajax({
                        "url": `/Glosario/UpdateGlossary?id=${id}&word=${word}&description=${description}`,
                        "type": "POST",
                        "success": function (response) {
                            if (response.result === "200") {
                                toastr.success(`El registro #${id} fue actualizado!`);
                                editButton.prop('title', 'Editar');
                                editButton.closest('tr').find('.field-word-glossary').text(word);
                                editButton.closest('tr').find('.field-description-glossary').text(description);

                                editButton.html("Editar");
                                editButton.closest('tr').find('.btn-remove-glossary').html("Eliminar");
                                editButton.closest('tr').find('.btn-remove-glossary').prop("title", "Eliminar");
                            } else {
                                toastr.error(response.message);
                            }
                        }
                    });
                }
            });

            $('#glossaryTable').on('click', '.btn-remove-glossary', function (evt) {
                evt.preventDefault();
                evt.stopPropagation();

                var removeButton = $(this);

                if (removeButton.html() === "Cancelar") {
                    removeButton.prop('title', 'Eliminar');
                    removeButton.html("Eliminar");

                    removeButton.closest('tr').find('.field-word-glossary').text(wordGlossaryEditing);
                    removeButton.closest('tr').find('.field-description-glossary').text(descriptionGlossaryEditing);

                    removeButton.closest('tr').find('.btn-edit-glossary').html("Editar");
                    removeButton.closest('tr').find('.btn-edit-glossary').prop("title", "Editar");
                } else {
                    const id = removeButton.closest('tr').find('.field-id-glossary').val();
                    const word = removeButton.closest('tr').find('.field-word-glossary').text();

                    if (confirm(`Confirma que desea eliminar la palabra: ${word}`)) {
                        $.ajax({
                            "url": `/Glosario/DeleteGlossary?id=${id}`,
                            "type": "POST",
                            "success": function (response) {
                                if (response.result === "200") {
                                    toastr.success(`La palabra ${word} fue eliminada!`);
                                    getGlossary();
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

function descriptionGlossaryTextarea() {
    var $textarea = $('#description-glossary');

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
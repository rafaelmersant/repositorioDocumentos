$('.btn-new-change').click(function () {
    if (!$('.save-change-btn')[0]) {
        const newRowHtml = '<tr>' +
            '<td class="field-date-change-new"><input type="text" name="changeDate" value="" autocomplete="off" class="form-control form-control-sm col-12 new-changeDate-change datepicker"/></td>' +
            `<td class="field-revision-change-new"><input type="text" maxlength="3" value="" autocomplete="off" class="form-control form-control-sm new-revision-change"></td>` +
            `<td class="field-pagesAffected-change-new"><input type="text" maxlength="3" value="" autocomplete="off" class="form-control form-control-sm new-pagesAffected-change"></td>` +
            '<td class="field-originator-change-new"><input type="text" class="form-control form-control-sm col-12 new-originator-change"/></td>' +
            '<td class="field-natureChange-change-new"><input type="text" class="form-control form-control-sm col-12 new-natureChange-change"/></td>' +
            '<td class="text-center">' +
            '<button class="btn btn-sm btn-success save-change-btn">Guardar</button>' +
            '<button class="btn btn-sm btn-danger cancel-change-btn">Cancelar</button>' +
            '</td>' +
            '</tr>';

        $('#changeTable tbody').prepend(newRowHtml);

        $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', });
    }
});

$(document).on('click', '.save-change-btn', function () {
    var changeDate = $(this).closest('tr').find('.new-changeDate-change').val();
    var revision = $(this).closest('tr').find('.new-revision-change').val();
    var pagesAffected = $(this).closest('tr').find('.new-pagesAffected-change').val();
    var originator = $(this).closest('tr').find('.new-originator-change').val();
    var natureChange = $(this).closest('tr').find('.new-natureChange-change').val();

    const documentHeaderId = $("#DocumentHeaderId").val();

    if (!changeDate || !revision || !pagesAffected || !originator || !natureChange) {
        toastr.error("Debe completar todos los campos.");
        return false;
    }

    const changeDateFormatted = GetEnglishDate(changeDate);

    $.ajax({
        "url": `/NotificacionCambios/AddChange?documentHeaderId=${documentHeaderId}&changeDate=${changeDateFormatted}` +
                `&revision=${revision}&pagesAffected=${pagesAffected}&originator=${originator}&natureChange=${natureChange}`,
        "type": "POST",
        "success": function (response) {
            if (response.result === "200") {
                toastr.success(`El registro fue agregado!`);
                $('.field-changeDate-change-new').text(changeDate);
                $('.field-revision-change-new').text(revision);
                $('.field-pagesAffected-change-new').text(pagesAffected);
                $('.field-originator-change-new').text(originator);
                $('.field-natureChange-change-new').text(natureChange);

                getChanges();
            } else {
                toastr.error(response.message);
            }
        }
    });
});

$(document).on('click', '.cancel-change-btn', function () {
    $(this).closest('tr').remove();
});

function getChanges() {
    $("#change-tbody").empty();
    const documentHeaderId = $("#DocumentHeaderId").val();

    $.ajax({
        url: `/NotificacionCambios/GetChanges?documentHeaderId=${documentHeaderId}`,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    }).done(function (data) {
        console.log('DATA GetChanges:', data)

        if (data.result === "200") {
            for (const item of data.message) {
                const itemRow = '<tr>' +
                    `<td class="field-changeDate-change text-center">${item.Date}</td>` +
                    `<td class="field-revision-change">${item.Revision}</td>` +
                    `<td class="field-pagesAffected-change">${item.PagesAffected}</td>` +
                    `<td class="field-originator-change">${item.Originator}</td>` +
                    `<td class="field-natureChange-change">${item.NatureChange}</td>` +
                    '<td class="text-center">' +
                    `<input type="hidden" class="field-id-change" value="${item.Id}">` +
                    '<a class="btn btn-sm btn-success btn-edit-change edit-button-width" href="javascript:void(0)" title="Editar">Editar</a> ' +
                    ' <a class="btn btn-sm btn-danger btn-remove-change" href="javascript:void(0)" title="Eliminar">Eliminar</a>' +
                    '</td>' +
                    '</tr>';

                $('#changeTable tbody').append(itemRow);
            }

            $(".datepicker").datepicker({dateFormat: 'dd/mm/yy',});

            $('#changeTable').on('click', '.btn-edit-change', function () {
                var editButton = $(this);

                if (editButton.prop('title') === "Editar") {
                    editButton.prop('title', 'Guardar');
                    editButton.html("Guardar");
                    editButton.closest('tr').find('.btn-remove-change').html("Cancelar");
                    editButton.closest('tr').find('.btn-remove-change').prop("title", "Cancelar");

                    const changeDate = editButton.closest('tr').find('.field-changeDate-change').text();
                    const revision = editButton.closest('tr').find('.field-revision-change').text();
                    const pagesAffected = editButton.closest('tr').find('.field-pagesAffected-change').text();
                    const originator = editButton.closest('tr').find('.field-originator-change').text();
                    const natureChange = editButton.closest('tr').find('.field-natureChange-change').text();

                    changeDateChangeEditing = changeDate;
                    revisionChangeEditing = revision;
                    pagesAffectedChangeEditing = pagesAffected;
                    originatorChangeEditing = originator;
                    natureChangeChangeEditing = natureChange;

                    editButton.closest('tr').find('.field-changeDate-change').html(`<input type="text" value="${changeDate}" class="form-control form-control-sm edit-changeDate-change datepicker" autocomplete="off">`);
                    editButton.closest('tr').find('.field-revision-change').html(`<input type="text" maxlength="3" value="${revision}" class="form-control form-control-sm col-12 edit-revision-change" autocomplete="off"/>`);
                    editButton.closest('tr').find('.field-pagesAffected-change').html(`<input type="text" maxlength="3" value="${pagesAffected}" class="form-control form-control-sm edit-pagesAffected-change" autocomplete="off">`);
                    editButton.closest('tr').find('.field-originator-change').html(`<input type="text" value="${originator}" class="form-control form-control-sm edit-originator-change">`);
                    editButton.closest('tr').find('.field-natureChange-change').html(`<input type="text" value="${natureChange}" class="form-control form-control-sm edit-natureChange-change">`);
                } else {
                    const id = editButton.closest('tr').find('.field-id-change').val();
                    const changeDate = editButton.closest('tr').find('.edit-changeDate-change').val();
                    const revision = editButton.closest('tr').find('.edit-revision-change').val();
                    const pagesAffected = editButton.closest('tr').find('.edit-pagesAffected-change').val();
                    const originator = editButton.closest('tr').find('.edit-originator-change').val();
                    const natureChange = editButton.closest('tr').find('.edit-natureChange-change').val();

                    if (!changeDate || !revision || !pagesAffected || !originator || !natureChange) {
                        toastr.error("Debe completar todos los campos.");
                        return false;
                    }

                    const changeDateFormatted = GetEnglishDate(changeDate);

                    $.ajax({
                        "url": `/NotificacionCambios/UpdateChange?id=${id}&changeDate=${changeDateFormatted}&revision=${revision}&pagesAffected=${pagesAffected}` +
                            `&originator=${originator}&natureChange=${natureChange}`,
                        "type": "POST",
                        "success": function (response) {
                            if (response.result === "200") {
                                toastr.success(`El registro #${id} fue actualizado!`);
                                editButton.prop('title', 'Editar');
                                editButton.closest('tr').find('.field-changeDate-change').text(changeDate);
                                editButton.closest('tr').find('.field-revision-change').text(revision);
                                editButton.closest('tr').find('.field-pagesAffected-change').text(pagesAffected);
                                editButton.closest('tr').find('.field-originator-change').text(originator);
                                editButton.closest('tr').find('.field-natureChange-change').text(natureChange);

                                editButton.html("Editar");
                                editButton.closest('tr').find('.btn-remove-change').html("Eliminar");
                                editButton.closest('tr').find('.btn-remove-change').prop("title", "Eliminar");
                            } else {
                                toastr.error(response.message);
                            }
                        }
                    });
                }
            });

            $('#changeTable').on('click', '.btn-remove-change', function () {
                var removeButton = $(this);

                if (removeButton.html() === "Cancelar") {
                    removeButton.prop('title', 'Eliminar');
                    removeButton.html("Eliminar");

                    removeButton.closest('tr').find('.field-changeDate-change').text(changeDateChangeEditing);
                    removeButton.closest('tr').find('.field-revision-change').text(revisionChangeEditing);
                    removeButton.closest('tr').find('.field-pagesAffected-change').text(pagesAffectedChangeEditing);
                    removeButton.closest('tr').find('.field-originator-change').text(originatorChangeEditing);
                    removeButton.closest('tr').find('.field-natureChange-change').text(natureChangeChangeEditing);

                    removeButton.closest('tr').find('.btn-edit-change').html("Editar");
                    removeButton.closest('tr').find('.btn-edit-change').prop("title", "Editar");
                } else {
                    const id = removeButton.closest('tr').find('.field-id-change').val();
                    const originator = removeButton.closest('tr').find('.field-originator-change').text();
                    const changeDate = removeButton.closest('tr').find('.field-changeDate-change').text();

                    if (confirm(`Confirma que desea eliminar el registro #${id} Originador del cambio: ${originator} de fecha ${changeDate}`)) {
                        $.ajax({
                            "url": `/NotificacionCambios/DeleteChange?id=${id}`,
                            "type": "POST",
                            "success": function (response) {
                                if (response.result === "200") {
                                    toastr.success(`El registro #${id} fue eliminado!`);
                                    getChanges();
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

function GetEnglishDate(date) {
    if (date && date !== null) {
        const dateArray = date.split("/");
        return `${dateArray[1]}-${dateArray[0]}-${dateArray[2]}`;
    }

    return "";
}
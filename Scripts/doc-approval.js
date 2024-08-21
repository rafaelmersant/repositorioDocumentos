$('.btn-new-approval').click(function () {
    //Employees
    let employees = [];

    for (const employee of _employees) {
        employees.push(`<option value="${employee.Value}">${employee.Text.trim()}</option>`)
    }

    if (!$('.save-approval-btn')[0]) {
        const newRowHtml = '<tr>' +
            `<td class="field-producedBy-new"><select class="form-control form-control-sm new-producedBy" id="producedBy"> ${employees} </select></td>` +
            `<td class="field-managerArea-new"><select class="form-control form-control-sm new-managerArea" id="managerArea"> ${employees} </select></td>` +
            `<td class="field-directorArea-new"><select class="form-control form-control-sm new-directorArea" id="directorArea"> ${employees} </select></td>` +
            '<td class="text-center">' +
            '<button class="btn btn-sm btn-success save-approval-btn">Guardar</button>' +
            '<button class="btn btn-sm btn-danger cancel-approval-btn">Cancelar</button>' +
            '</td>' +
            '</tr>';

        $('#approvalTable tbody').prepend(newRowHtml);
    }
});

$(document).on('click', '.save-approval-btn', async function () {
    var producedBy = $(this).closest('tr').find('.new-producedBy').val();
    var managerArea = $(this).closest('tr').find('.new-managerArea').val();
    var directorArea = $(this).closest('tr').find('.new-directorArea').val();

    var producedByName = $(this).closest('tr').find('.new-producedBy :selected').text().trim();
    var managerAreaName = $(this).closest('tr').find('.new-managerArea :selected').text().trim();
    var directorAreaName = $(this).closest('tr').find('.new-directorArea :selected').text().trim();

    const documentHeaderId = $("#DocumentHeaderId").val();

    if (!producedBy || !managerArea || !directorArea) {
        toastr.error("Debe completar todos los campos.");
        return false;
    }

    const parameters = { documentHeaderId, producedBy, producedByName, managerArea, managerAreaName, directorArea, directorAreaName };

    $.ajax({
        "url": `/Aprobacion/AddApproval`,
        "type": "POST",
        "data": parameters,
        "success": async function (response) {
            if (response.result === "200") {
                toastr.success(`El registro fue agregado!`);
                $('.field-producedBy-new').text(producedByName);
                $('.field-managerArea-new').text(managerAreaName);
                $('.field-directorArea-new').text(directorAreaName);
              
                await getApprovals();
            } else {
                toastr.error(response.message);
            }
        }
    });
});

$(document).on('click', '.cancel-approval-btn', function () {
    $(this).closest('tr').remove();
});

async function getApprovals() {
    $("#approval-tbody").empty();
    const documentHeaderId = $("#DocumentHeaderId").val();

    $.ajax({
        url: `/Aprobacion/GetApprovals?documentHeaderId=${documentHeaderId}`,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    }).done(function (data) {
        console.log('DATA GetApprovals:', data)

        if (data.result === "200") {
            for (const item of data.message) {
                const itemRow = '<tr>' +
                    `<td class="field-producedBy">${item.ProducedByName}</td>` +
                    `<td class="field-managerArea">${item.ManagerAreaName}</td>` +
                    `<td class="field-directorArea">${item.DirectorAreaName}</td>` +
                    '<td class="text-center">' +
                    `<input type="hidden" class="field-id" value="${item.Id}">` +
                    '<a class="btn btn-sm btn-success btn-edit-approval edit-button-width" href="javascript:void(0)" title="Editar">Editar</a> ' +
                    ' <a class="btn btn-sm btn-danger btn-remove-approval" href="javascript:void(0)" title="Eliminar">Eliminar</a>' +
                    '</td>' +
                    '</tr>';

                $('#approvalTable tbody').append(itemRow);
            }

            $('.btn-edit-approval').click(function () {
                var editButton = $(this);

                if (editButton.prop('title') === "Editar") {
                    editButton.prop('title', 'Guardar');
                    editButton.html("Guardar");
                    editButton.closest('tr').find('.btn-remove-approval').html("Cancelar");
                    editButton.closest('tr').find('.btn-remove-approval').prop("title", "Cancelar");

                    const producedBy = editButton.closest('tr').find('.field-producedBy').text();
                    const managerArea = editButton.closest('tr').find('.field-managerArea').text();
                    const directorArea = editButton.closest('tr').find('.field-directorArea').text();
                 
                    producedByApprovalEditing = producedBy;
                    managerAreaApprovalEditing = managerArea;
                    directorAreaApprovalEditing = directorArea;

                    //Employees
                    let __producedBy = [];
                    let __managerArea = [];
                    let __directorArea = [];

                    console.log('EDITING: ', producedBy, ",", managerArea, ",", directorArea)

                    for (const employee of _employees) {
                        __producedBy.push(`<option value="${employee.Value}" ${producedBy.trim() === employee.Text.trim() ? "selected" : ""}>${employee.Text}</option>`)
                        __managerArea.push(`<option value="${employee.Value}" ${managerArea.trim() === employee.Text.trim() ? "selected" : ""}>${employee.Text}</option>`)
                        __directorArea.push(`<option value="${employee.Value}" ${directorArea.trim() === employee.Text.trim() ? "selected" : ""}>${employee.Text}</option>`)
                    }

                    editButton.closest('tr').find('.field-producedBy').html(`<select class="form-control form-control-sm edit-producedBy" id="producedBy"> ${__producedBy} </select>`);
                    editButton.closest('tr').find('.field-managerArea').html(`<select class="form-control form-control-sm edit-managerArea" id="managerArea"> ${__managerArea} </select>`);
                    editButton.closest('tr').find('.field-directorArea').html(`<select class="form-control form-control-sm edit-directorArea" id="directorArea"> ${__directorArea} </select>`);
                } else {
                    const id = editButton.closest('tr').find('.field-id').val();
                    const producedBy = editButton.closest('tr').find('.edit-producedBy').val();
                    const managerArea = editButton.closest('tr').find('.edit-managerArea').val();
                    const directorArea = editButton.closest('tr').find('.edit-directorArea').val();

                    var producedByName = $(this).closest('tr').find('.edit-producedBy :selected').text();
                    var managerAreaName = $(this).closest('tr').find('.edit-managerArea :selected').text();
                    var directorAreaName = $(this).closest('tr').find('.edit-directorArea :selected').text();
                   
                    if (!producedBy || !managerArea || !directorArea ) {
                        toastr.error("Debe completar todos los campos.");
                        return false;
                    }

                    const documentHeaderId = $("#DocumentHeaderId").val();

                    const parameters = { id, documentHeaderId, producedBy, producedByName, managerArea, managerAreaName, directorArea, directorAreaName };

                    $.ajax({
                        "url": `/Aprobacion/UpdateApproval`,
                        "type": "POST",
                        "data": parameters,
                        "success": function (response) {
                            if (response.result === "200") {
                                toastr.success(`El registro #${id} fue actualizado!`);
                                editButton.prop('title', 'Editar');
                                editButton.closest('tr').find('.field-producedBy').text(producedByName);
                                editButton.closest('tr').find('.field-managerArea').text(managerAreaName);
                                editButton.closest('tr').find('.field-directorArea').text(directorAreaName);
                                
                                editButton.html("Editar");
                                editButton.closest('tr').find('.btn-remove-approval').html("Eliminar");
                                editButton.closest('tr').find('.btn-remove-approval').prop("title", "Eliminar");
                            } else {
                                toastr.error(response.message);
                            }
                        }
                    });
                }
            });

            $('.btn-remove-approval').click(function () {
                var removeButton = $(this);

                if (removeButton.html() === "Cancelar") {
                    removeButton.prop('title', 'Eliminar');
                    removeButton.html("Eliminar");

                    removeButton.closest('tr').find('.field-producedBy').text(producedByApprovalEditing);
                    removeButton.closest('tr').find('.field-managerArea').text(managerAreaApprovalEditing);
                    removeButton.closest('tr').find('.field-directorArea').text(directorAreaApprovalEditing);
                    
                    removeButton.closest('tr').find('.btn-edit-approval').html("Editar");
                    removeButton.closest('tr').find('.btn-edit-approval').prop("title", "Editar");
                } else {
                    const id = removeButton.closest('tr').find('.field-id').val();
                    const producedBy = removeButton.closest('tr').find('.field-producedBy').text();
                    const managerArea = removeButton.closest('tr').find('.field-managerArea').text();
                    const directorArea = removeButton.closest('tr').find('.field-directorArea').text();

                    if (confirm(`Confirma que desea eliminar el registro #${id} producido por: ${producedBy}`)) {
                        $.ajax({
                            "url": `/Aprobacion/DeleteApproval?id=${id}`,
                            "type": "POST",
                            "success": async function (response) {
                                if (response.result === "200") {
                                    toastr.success(`El registro #${id} fue eliminado!`);
                                    await getApprovals();
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
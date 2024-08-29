$('.btn-new-permission').click(function () {
    //Users
    let users = [];

    for (const user of _users) {
        users.push(`<option value="${user.Value}">${user.Text.trim()}</option>`)
    }

    if (!$('.save-permission-btn')[0]) {
        const newRowHtml = '<tr>' +
            `<td colspan="2" class="field-userId-new"><select class="form-control form-control-sm new-userId" id="userId"> ${users} </select></td>` +
            '<td class="text-center">' +
            '<button class="btn btn-sm btn-success save-permission-btn">Guardar</button>' +
            '<button class="btn btn-sm btn-danger cancel-permission-btn">Cancelar</button>' +
            '</td>' +
            '</tr>';

        $('#permissionTable tbody').prepend(newRowHtml);
    }
});

$(document).on('click', '.save-permission-btn', async function () {
    var userId = $(this).closest('tr').find('.new-userId').val();
    
    const documentHeaderId = $("#DocumentHeaderId").val();

    if (!userId) {
        toastr.error("Debe seleccionar un usuario.");
        return false;
    }

    const parameters = { documentHeaderId, userId };

    $.ajax({
        "url": `/Permiso/AddPermission`,
        "type": "POST",
        "data": parameters,
        "success": async function (response) {
            if (response.result === "200") {
                toastr.success(`El registro fue agregado!`);
               
                await getPermissions();
            } else {
                toastr.error(response.message);
            }
        }
    });
});

$(document).on('click', '.cancel-permission-btn', function () {
    $(this).closest('tr').remove();
});

async function getPermissions() {
    $("#permission-tbody").empty();
    const documentHeaderId = $("#DocumentHeaderId").val();

    $.ajax({
        url: `/Permiso/GetPermissions?documentHeaderId=${documentHeaderId}`,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    }).done(function (data) {
        console.log('DATA GetPermissions:', data)

        if (data.result === "200") {
            for (const item of data.message) {
                const itemRow = '<tr>' +
                    `<td class="field-user">${item.UserName}</td>` +
                    `<td class="field-createdDate">${item.CreatedDate}</td>` +
                    '<td class="text-center">' +
                    `<input type="hidden" class="field-id" value="${item.Id}">` +
                    /*'<a class="btn btn-sm btn-success btn-edit-approval edit-button-width" href="javascript:void(0)" title="Editar">Editar</a> ' +*/
                    ' <a class="btn btn-sm btn-danger btn-remove-permission" href="javascript:void(0)" title="Eliminar">Eliminar</a>' +
                    '</td>' +
                    '</tr>';

                $('#permissionTable tbody').append(itemRow);
            }

            $('.btn-remove-permission').click(function () {
                var removeButton = $(this);

                const id = removeButton.closest('tr').find('.field-id').val();
                const user = removeButton.closest('tr').find('.field-user').text();
          
                if (confirm(`Confirma que desea eliminar el permiso para el usuario: ${user}`)) {
                    $.ajax({
                        "url": `/Permiso/DeletePermission?id=${id}`,
                        "type": "POST",
                        "success": async function (response) {
                            if (response.result === "200") {
                                toastr.success(`El permiso para el usuario ${user} fue eliminado!`);
                                await getPermissions();
                            } else {
                                toastr.error(response.message);
                            }
                        }
                    });
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
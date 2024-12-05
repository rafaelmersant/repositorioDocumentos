var guidelinesCount = 1;

var toolbarOptions = [
    ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
    ['image'],
    [{ 'header': 1 }, { 'header': 2 }],               // custom button values
    [{ 'list': 'ordered' }, { 'list': 'bullet' }, { 'list': 'check' }],
    [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
    [{ 'indent': '-1' }, { 'indent': '+1' }],          // outdent/indent
    [{ 'direction': 'rtl' }],                         // text direction

    [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
    [{ 'header': [1, 2, 3, 4, 5, 6, false] }],

    [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
    [{ 'font': [] }],
    [{ 'align': [] }],

    ['clean'],
];

$('.btn-new-guideline').click(function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

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

        const currentEditor = localStorage.getItem("currentEditor");
        if (currentEditor === "Tiny") {
            tinymce.init({
                selector: '#description-guideline',
                menubar: false,
                plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount linkchecker',
                toolbar: 'undo redo | fontfamily fontsize | bold italic underline strikethrough | table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
            });

        } else {
            quill_descriptionGuideline = new Quill('#description-guideline', {
                modules: {
                    toolbar: toolbarOptions
                },
                theme: 'snow'
            });
        }
    }
});

$(document).on('click', '.save-guideline-btn', async function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

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

$(document).on('click', '.cancel-guideline-btn', function (evt) {
    evt.preventDefault();
    evt.stopPropagation();

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
                let description = item.Description.replaceAll('text-indent:', '');

                let itemRow = '<tr>' +
                    `<td class="field-sortindex-guideline text-center">${item.SortIndex}</td>` +
                    `<td class="field-description-guideline">${description}<input class="field-description-guideline-raw" type='hidden' value='${description.replaceAll('text-indent:', '')}'></td>` +
                    '<td class="text-center">' +
                    `<input type="hidden" class="field-id-guideline" value="${item.Id}">`;

                if ("Consulta" === "Consulta") {
                    itemRow += '<a class="btn btn-sm btn-success btn-edit-guideline edit-button-width" href="javascript:void(0)" title="Editar">Editar</a> ' +
                        ' <a class="btn btn-sm btn-danger btn-remove-guideline" href="javascript:void(0)" title="Eliminar">Eliminar</a>';
                }

                itemRow += '</td>' +
                    '</tr>';

                $('#guidelineTable tbody').append(itemRow);

                guidelinesCount += 1;
            }

            $('.field-description-guideline p').each(function () {
                const textIndentValue = parseFloat($(this).css('text-indent'));

                if (textIndentValue < 0) {
                    $(this).css('text-indent', '-8.0pt');
                }
            });
                      
            $('#guidelineTable').on('click', '.btn-edit-guideline', async function (evt) {
                evt.preventDefault();
                evt.stopPropagation();

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
                    console.log(`Editing Guideline: ${sortindex} :: ${descriptionRaw}`)

                    const currentEditor = localStorage.getItem("currentEditor");
                    if (currentEditor === "Tiny") {
                        tinymce.init({
                            selector: '#description-guideline',
                            menubar: false,
                            plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount linkchecker',
                            toolbar: 'undo redo | fontfamily fontsize | bold italic underline strikethrough | table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
                        });

                    } else {
                        quill_descriptionGuideline = new Quill('#description-guideline', {
                            modules: {
                                toolbar: toolbarOptions
                            },
                            theme: 'snow'
                        });
                    }
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

            $('#guidelineTable').on('click', '.btn-remove-guideline', function (evt) {
                evt.preventDefault();
                evt.stopPropagation();

                var removeButton = $(this);

                if (removeButton.html() === "Cancelar") {
                    removeButton.prop('title', 'Eliminar');
                    removeButton.html("Eliminar");

                    const __descriptionRaw = `<input class="field-description-guideline-raw" type='hidden' value='${descriptionGuidelineEditing}'>`;

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
    const currentEditor = localStorage.getItem("currentEditor");
    let value = "";

    if (currentEditor === "Tiny")
        value = tinymce.get("description-guideline").getContent();
    else
        value = quill_descriptionGuideline.root.innerHTML;

    return value ? value : "";
}

async function setGuidelineBody(value) {
    const currentEditor = localStorage.getItem("currentEditor");

    if (currentEditor === "Tiny")
        tinymce.get("description-guideline").setContent(value);
    else
        quill_descriptionGuideline.root.innerHTML = value;
}
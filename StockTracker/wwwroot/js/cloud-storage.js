$(() => {
	addFormEvents();
})

function addFormEvents() {
	const form = $(`#${FORM_ID}`)
	submitBtnEvent(form);
	tokenInputEvent();
	exportDataBtnEvent(form);
	importDataBtnEvent(form);
}

function submitBtnEvent(form) {
	const modalExport = $(`#${EXPORT_MODAL_ID}`);

	$(document).on('submit', `#${FORM_ID}`, function (e) {
		e.preventDefault();

		if (!this.checkValidity()) {
			this.classList.add('was-validated');
			showValidationMessages(this);
			return;
		}

		setFormState(true);

		$.post({
			url: `${areaPath()}/${CHECK_DATA_DIFFERENCE_URL}`,
			data: form.serialize(),
			success: response => {
				if (!response.result) {
					showErrorAlert(response)
					return
				}

				if (response.content == 'true') {
					modalExport.modal('show');
					return;
				}

				saveConnectionString(form);
			},
			error: response => {
				showErrorAlert(response);
				setFormState(false);
			}
		})
	})
}

function exportDataBtnEvent(form) {
	const query = `#${EXPORT_MODAL_ID} #${BTN_EXPORT_ID}`
	$(document).on('click', query, () => saveConnectionString(form, false));
}

function importDataBtnEvent(form) {
	const query = `#${EXPORT_MODAL_ID} #${BTN_IMPORT_ID}`
	$(document).on('click', query, () => saveConnectionString(form, true));
}

function tokenInputEvent() {
	const btnSubmit = $(`#${BTN_SUBMIT_ID}`);
	$(document).one('click', `#${FORM_ID} #${TOKEN_INPUT_ID}`, e => {
		$(e.currentTarget).val('');
		btnSubmit.removeAttr('disabled');
	});
}

function saveConnectionString(form, overwriteLocalData = null) {
	$.post({
		url: `${areaPath()}/${SAVE_STRING_URL}`,
		data: getSaveDataForm(form, overwriteLocalData),
		success: response => {
			if (!response.result) {
				showErrorAlert(response)
				return
			}
			location.reload();
		},
		error: response => showErrorAlert(response),
		complete: () => setFormState(false)
	})
}

function getSaveDataForm(form, overwriteLocalData) {
	const data = serializeObject(form);
	data[OVERWRITE_DATA_PROP] = overwriteLocalData;
	return data;
}

function setFormState(awaitingResponse) {
	const btnSubmit = $(`#${BTN_SUBMIT_ID}`);
	btnSubmit.prop('disabled', awaitingResponse);
}

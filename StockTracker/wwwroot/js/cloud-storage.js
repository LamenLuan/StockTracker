$(() => {
	addFormEvents();
})

function addFormEvents() {
	submitBtnEvent();
	tokenInputEvent();
}

function submitBtnEvent() {
	const modalExport = $(`#${EXPORT_MODAL_ID}`);

	$(document).on('submit', `#${FORM_ID}`, function (e) {
		e.preventDefault();
		const form = $(this);

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

function saveConnectionString(form) {
	$.post({
		url: `${areaPath()}/${SAVE_STRING_URL}`,
		data: form.serialize(),
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

function setFormState(awaitingResponse) {
	const btnSubmit = $(`#${BTN_SUBMIT_ID}`);
	btnSubmit.prop('disabled', awaitingResponse);
}

function tokenInputEvent() {
	const btnSubmit = $(`#${BTN_SUBMIT_ID}`);
	$(document).one('click', `#${FORM_ID} #${TOKEN_INPUT_ID}`, e => {
		$(e.currentTarget).val('');
		btnSubmit.removeAttr('disabled');
	});
}
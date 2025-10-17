$(() => {
	addFormEvents();
})

function addFormEvents() {
	submitBtnEvent();
	tokenInputEvent();
}

function submitBtnEvent() {
	$(document).on('submit', `#${FORM_ID}`, function (e) {
		e.preventDefault();

		if (!this.checkValidity()) {
			this.classList.add('was-validated');
			showValidationMessages(this);
			return;
		}

		setFormState(true);

		$.post({
			url: `${areaPath()}/${SET_NOTIFIER_URL}`,
			data: $(this).serialize(),
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
	})
}

function setFormState(awaitingResponse) {
	const btnSubmit = $(`#${BTN_SUBMIT_ID}`);
	const helpText = $(`#${FORM_ID} #${HELP_TEXT_ID}`);
	btnSubmit.prop('disabled', awaitingResponse);
	helpText.toggleClass('d-none', !awaitingResponse);
}

function tokenInputEvent() {
	const btnSubmit = $(`#${BTN_SUBMIT_ID}`);
	$(document).one('click', `#${FORM_ID} #${TOKEN_INPUT_ID}`, e => {
		$(e.currentTarget).val('');
		btnSubmit.removeAttr('disabled');
	});
}
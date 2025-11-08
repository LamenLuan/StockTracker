$(() => {
	addFormEvents();
})

function addFormEvents() {
	const form = $(`#${FORM_ID}`);
	submitBtnEvent(form);
}

function submitBtnEvent(form) {
  const btnSubmit = $(`#${BTN_SUBMIT_ID}`);

	$(document).on('submit', `#${FORM_ID}`, function (e) {
		e.preventDefault();

    if (btnSubmit.prop('disabled')) return;

		if (!this.checkValidity()) {
			this.classList.add('was-validated');
			showValidationMessages(this);
			return;
		}

		btnSubmit.prop('disabled', true);

		$.post({
			url: `${areaPath()}/${SAVE_SETTINGS_URL}`,
			data: form.serialize(),
			success: response => {
				if (!response.result) {
					showErrorAlert(response)
					return
				}

				location.reload();
			},
			error: response => showErrorAlert(response),
			complete: () => btnSubmit.removeAttr('disabled')
		})
	})
}

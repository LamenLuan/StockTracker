$(document).ready(function () {
	const form = $(`#${API_KEY_FORM_ID}`)
	const input = form.find(`#${API_KEY_INPUT_ID}`)

	input.prop('disabled', true)
	$.get({
		url: `Home/${GET_BRAPI_KEY_URL}`,
		success: function (response) {
			if (response) {
				form.data('validKeyInserted', true)
				input.val(response)
				unlockCards()
			}
		},
		complete: () => input.removeAttr('disabled')
	})
})

function unlockCards() {

}

function showValidationMessages(form) {
	const inputs = $(form).find("input,select,textarea")

	inputs.each(function () {
		const feedback = $(this).parent().find(".invalid-feedback:first")
		if (this.validity.valueMissing)
			feedback.text("Fill this input")
		else if (this.validity.tooShort) {
			const limite = $(this).attr("minlength")
			feedback.text("Must contain at least " + limite + " characters")
		}
		else if (this.validity.tooLong) {
			const limite = $(this).attr("maxlength")
			feedback.text("Must contain " + limite + " characters at most")
		}
		else if (this.validity.rangeOverflow) {
			const limite = $(this).attr("max")
			feedback.text(`Insert values under ${limite}`)
		}
		else if (this.validity.rangeUnderflow) {
			const limite = $(this).attr("min")
			feedback.text(`Insert values above ${limite}`)
		}
	})
}

function validateKey(form) {
	const input = $(form).find(`#${API_KEY_INPUT_ID}`)
	input.prop('disabled', true)

	$.post({
		url: `Home/${CHECK_BRAPI_KEY_VALID}`,
		data: getDataToCheckBrapiKeyValid(input),
		success: function (response) {
			$(form).data('validKeyInserted', true)
		},
		error: function (response) {
			if (response.status == 400 || response.status == 401) {
				form.classList.remove('was-validated')
				$(form).find(".invalid-feedback:first").text("This is key invalid")
				input.removeClass('is-valid').addClass('is-invalid')
			}
		},
		complete: () => input.removeAttr('disabled')
	})
}

function getDataToCheckBrapiKeyValid(input) {
	const data = {}
	data[`${BRAPI_KEY_PROP}`] = input.val()
	return data
}

$(`#${API_KEY_FORM_ID}`).on('submit', function (e) {
	e.preventDefault()

	if ($(this).data('validKeyInserted')) {
		const input = $(this).find(`#${API_KEY_INPUT_ID}`)
		input.val('')
		$(this).removeData('validKeyInserted')
		return
	}

	if (!this.checkValidity()) {
		this.classList.add('was-validated')
		showValidationMessages(this)
	}
	else validateKey(this)
})
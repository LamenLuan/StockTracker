$(() => {

	brapiKeyInputEvent()
	addCardButtonEvent()

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

function addCardButtonEvent() {
	const cards = $(`#${CARDS_ID}`)
	const apiKeyform = $(`#${API_KEY_FORM_ID}`)
	const apiKeyInput = apiKeyform.find(`#${API_KEY_INPUT_ID}`)

	$(document).on('click', `.${ADD_CARD_ID}`, e => {

		if (!apiKeyform.data('validKeyInserted')) {
			apiKeyform.removeClass('was-validated')
			apiKeyform.find(".invalid-feedback:first").text("This is key invalid")
			apiKeyInput.removeClass('is-valid').addClass('is-invalid')
			return
		}

		$.get({
			url: `Home/${CREATE_CARD_URL}`,
			success: function (response) {
				if (response.result == false) {
					return
				}
				$(e.currentTarget).closest('.stock-card').addClass('d-none')
				const card = $(response)
				cards.append(card)
				configCardSelect(card)
			}
		})
	})
}

function configCardSelect(card) {
	card.find(`#${STOCK_INPUT_ID}`).select2({
		ajax: {
			url: `Home/${FIND_STOCK_URL}`,
			data: a => {
				const data = {}
				data[SEARCH_TERM_PROP] = a.term
				return data
			},
			processResults: response => {
				return {
					results: response.stocks.map((text, id) => ({ id, text }))
				}
			}
		}
	})
}

function brapiKeyInputEvent() {
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
}


function unlockCards() {

}

function validateKey(form) {
	const input = $(form).find(`#${API_KEY_INPUT_ID}`)
	input.prop('disabled', true)

	$.post({
		url: `Home/${CHECK_BRAPI_KEY_URl}`,
		data: getDataToCheckBrapiKeyValid(input),
		success: function (response) {
			if (!response.result) {
				showErrorAlert(response)
				return
			}
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
$(() => {

	brapiKeyInputEvent()
	addCardFormEvents()

	const form = $(`#${API_KEY_FORM_ID}`)
	const input = form.find(`#${API_KEY_INPUT_ID}`)

	input.prop('disabled', true)
	$.get({
		url: `Home/${GET_BRAPI_KEY_URL}`,
		success: function (response) {
			if (response) {
				form.data('validKeyInserted', true)
				input.val(response)
			}
		},
		complete: () => input.removeAttr('disabled')
	})
})

//#region Brapi key

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

//#endregion

//#region Add card form

function addCardFormEvents() {
	addCardButtonEvent()
	stockNameSelectEvent()
	priceInputEvent()
	operationBtnsEvent()
	cardButtonEvent()
	percentageInputEvent()
}

function addCardButtonEvent() {
	const cards = $(`#${CARDS_ID}`)
	const apiKeyform = $(`#${API_KEY_FORM_ID}`)
	const apiKeyInput = apiKeyform.find(`#${API_KEY_INPUT_ID}`)

	$(document).on('click', `#${ADD_CARD_ID}`, e => {

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
				$(e.currentTarget).closest('.stock-card').remove()
				const card = $(response)
				cards.prepend(card)
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
					results: response.stocks.map(text => ({ id: text, text }))
				}
			}
		}
	})
}

function stockNameSelectEvent() {
	$(document).on('select2:select', `#${STOCK_INPUT_ID}`, function () {
		$(`#${PRICE_INPUT_ID}`).trigger("focus")
	})
}

function operationBtnsEvent() {
	$(document).on('click', `.${OPERATION_INPUT_CLASS}`, updatePctgResult)
}

function priceInputEvent() {
	$(document).on('change', `#${PRICE_INPUT_ID}`, updatePctgResult)
}

function percentageInputEvent() {
	$(document).on('change', `#${PERCENTAGE_INPUT_ID}`, updatePctgResult)
}

function updatePctgResult() {
	const form = $(`#${FORM_ID}`)
	const pctgInput = form.find(`#${PERCENTAGE_INPUT_ID}`)
	const pctg = parseFloat(pctgInput.val())
	const priceInput = form.find(`#${PRICE_INPUT_ID}`)
	const price = parseFloat(priceInput.val())
	const pctgResultInput = form.find(`#${PERCENTAGE_RESULT_INPUT_ID}`)

	if (Number.isNaN(pctg) || Number.isNaN(price)) {
		pctgResultInput.val('')
		return
	}
	else {
		const buying = JSON.parse(form.find(`.${OPERATION_INPUT_CLASS}:checked`).val())
		const result = buying
			? price * (1 - (pctg / 100))
			: price * (1 + (pctg / 100))
		pctgResultInput.val(result.toFixed(2))
	}
}

function cardButtonEvent() {
	$(document).on('click', `.${CARD_BTN_CLASS}`, function () {
		const form = $(`#${FORM_ID}`)
		const btn = $(this)
		const btns = $(`.${CARD_BTN_CLASS}`)

		if (!form[0].checkValidity()) {
			form.addClass('was-validated')
			return
		}

		btns.prop('disabled', true)

		$.post({
			url: `Home/${CREATE_STOCK_TRACK_URL}`,
			data: serializeObject(form),
			success: (response) => {
				if (!response.result) {
					showErrorAlert(response)
					return
				}
				location.reload()
			},
			error: (response) => showErrorAlert(response),
			complete: () => btns.removeAttr('disabled')
		})
	})
}

//#endregion
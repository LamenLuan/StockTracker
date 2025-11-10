$(() => {

	apiKeyEvents();
	addCardFormEvents();
	viewCardRemoveBtnEvent();

	const form = $(`#${API_KEY_FORM_ID}`)
	const input = form.find(`input[name=${API_KEY_INPUT_NAME}]:first`)

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

function apiKeyEvents() {
	apiInputEvent();
	brapiKeyFormEvent();
}

function apiInputEvent() {
	$(document).one('click', `#${API_KEY_FORM_ID} input[name=${API_KEY_INPUT_NAME}]`, e => {
		$(e.currentTarget).val('');
	});
}

function brapiKeyFormEvent() {
	$(`#${API_KEY_FORM_ID}`).on('submit', function (e) {
		e.preventDefault()

		if (!this.checkValidity()) {
			this.classList.add('was-validated')
			showValidationMessages(this)
		}
		else validateKey(this)
	})
}

function validateKey(form) {
	const input = $(form).find(`input[name=${API_KEY_INPUT_NAME}]:first`)
	input.prop('disabled', true)

	$.post({
		url: `Home/${WRITE_BRAPI_KEY_URL}`,
		data: getDataToCheckBrapiKeyValid(input),
		success: function (response) {
			if (!response.result) {
				showErrorAlert(response)
				return
			}
			location.reload();
		},
		error: function (response) {
			if (response.status == 400 || response.status == 401) {
				form.classList.remove('was-validated')
				$(form).find(".invalid-feedback:first").text("This key is invalid")
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

//#region Add card

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

	$(document).on('click', `#${ADD_CARD_ID}`, e => {
		if (validApiKeyNotInserted()) return

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

//#region View card

function viewCardRemoveBtnEvent() {
	$(document).on('click', `.${CARD_REMOVE_CLASS}`, (e) => {
		if (validApiKeyNotInserted()) return

		const btn = $(e.currentTarget)
		const form = btn.closest('form')
		$.post({
			url: `Home/${REMOVE_STOCK_TRACK_URL}`,
			data: serializeObject(form, true),
			success: (response) => {
				if (!response.result) {
					showErrorAlert(response)
					return
				}
				location.reload()
			},
			error: (response) => showErrorAlert(response)
		})
	})
}

//#endregion

function validApiKeyNotInserted() {
	const apiKeyform = $(`#${API_KEY_FORM_ID}`)
	const apiKeyInput = apiKeyform.find(`input[name=${API_KEY_INPUT_NAME}]:first`)

	if (!apiKeyform.data('validKeyInserted')) {
		apiKeyform.removeClass('was-validated')
		apiKeyform.find(".invalid-feedback:first").text("This key is invalid")
		apiKeyInput.removeClass('is-valid').addClass('is-invalid')
		return true
	}

	return false
}

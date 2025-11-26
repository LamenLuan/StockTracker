$(() => {

	apiKeyEvents();
	addCardFormEvents();
	viewCardEvents();

	const form = $(`#${API_KEY_FORM_ID}`);
	const input = form.find(`input[name=${API_KEY_INPUT_NAME}]:first`);
	form.data('validKeyInserted', !!input.val());
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
	input.setInputAsLoading(true);

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
		complete: () => input.setInputAsLoading(false)
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
	createTrackBtnEvent()
	percentageInputEvent()
}

function addCardButtonEvent() {
	const cards = $(`#${CARDS_ID}`)

	$(document).on('click', `#${ADD_CARD_ID}:not(.loading)`, e => {
		if (validApiKeyNotInserted()) return;
		const card = $(e.currentTarget).setInputAsLoading(true);

		$.get({
			url: `Home/${CREATE_CARD_URL}`,
			success: function (response) {
				if (response.result == false) {
					showErrorAlert(response);
					return;
				}
				card.closest('.stock-card').remove()
				const inputCard = $(response)
				cards.prepend(inputCard)
				inputCard.find(`#${STOCK_INPUT_ID}`).select2()
			},
			error: response => showErrorAlert(response),
			complete: () => card.setInputAsLoading(false)
		})
	})
}

function stockNameSelectEvent() {
	$(document).on('select2:select', `#${STOCK_INPUT_ID}`, function () {
		$(`#${PRICE_INPUT_ID}`).trigger("focus");
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

function createTrackBtnEvent() {
	$(document).on('click', `.${CARD_BTN_CLASS}`, function () {
		const form = $(`#${FORM_ID}`)
		const btns = form.find(`.${CARD_BTN_CLASS}:first`)

		if (!form[0].checkValidity()) {
			form.addClass('was-validated')
			return
		}

		btns.setInputAsLoading(true);

		$.post({
			url: `Home/${CREATE_STOCK_TRACK_URL}`,
			data: serializeObject(form),
			success: (response) => {
				if (!response.result) {
					showErrorAlert(response)
					return
				}
				location.reload();
			},
			error: (response) => showErrorAlert(response),
			complete: () => btns.setInputAsLoading(false)
		})
	});
}

//#endregion

//#region View card

function viewCardEvents() {
	viewCardMuteNotificationBtnEvent();
	viewCardRemoveBtnEvent();
}

function viewCardMuteNotificationBtnEvent() {
	$(document).on('click', `button[name=${MUTE_BTN_NAME}]`, (e) => {
		if (validApiKeyNotInserted()) return;

		const btn = $(e.currentTarget);
		const form = btn.closest('form');
		btn.setInputAsLoading(true);

		$.post({
			url: `Home/${CHANGE_MUTE_OPTION_TRACK_URL}`,
			data: serializeObject(form, true),
			success: (response) => {
				if (!response.result) {
					showErrorAlert(response)
					return
				}
				location.reload()
			},
			error: (response) => showErrorAlert(response),
			complete: () => btn.setInputAsLoading(false)
		})
	});
}

function viewCardRemoveBtnEvent() {
	$(document).on('click', `.${CARD_REMOVE_CLASS}`, (e) => {
		if (validApiKeyNotInserted()) return

		const btn = $(e.currentTarget)
		const form = btn.closest('form')

		btn.setInputAsLoading(true);

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
			error: response => {
				showErrorAlert(response);
				btn.setInputAsLoading(false);
			}
		})
	})
}

//#endregion

function validApiKeyNotInserted() {
	const apiKeyform = $(`#${API_KEY_FORM_ID}`)
	const apiKeyInput = apiKeyform.find(`input[name=${API_KEY_INPUT_NAME}]:first`)

	if (apiKeyInput.prop('disabled')) return true;

	if (!apiKeyform.data('validKeyInserted')) {
		apiKeyform.removeClass('was-validated')
		apiKeyform.find(".invalid-feedback:first").text("This key is invalid")
		apiKeyInput.removeClass('is-valid').addClass('is-invalid')
		return true
	}

	return false
}

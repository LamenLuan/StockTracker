function showErrorAlert(response) {
	const modal = $('#modal-alert')
	const modalTitle = modal.find('.modal-title:first')
	const modalBody = modal.find('.modal-body:first')
	modalTitle.text('Error')
	const errorMsg = `An error ocurred. ${response.content ? response.content : 'Please try again later'}`
	modalBody.text(errorMsg)
	modal.modal('show')
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

function serializeObject(form, serializeDisabled) {
	let objData = {}
	let disableds = form.find(':disabled')
	if (serializeDisabled)
		disableds.removeAttr('disabled')

	let seriArray = form.serializeArray()

	seriArray.forEach(data => {
		let oldValue = objData[data.name]
		if (oldValue) {
			if (Array.isArray(oldValue))
				objData[data.name].push(data.value)
			else
				objData[data.name] = [oldValue, data.value]
		}
		else objData[data.name] = data.value
	})

	disableds.prop('disabled', true)

	return objData
}

function areaPath() {
	var path = window.location.pathname;
	return path == '/'
		? '_/Home'
		: path.slice(0, -1) + path.at(-1).replace('/', '');
}

$.fn.setInputAsLoading = function (loading) {
	const jqThis = $(this);
	jqThis.prop('disabled', loading)
		.toggleClass('loading', loading);
	return jqThis;
};
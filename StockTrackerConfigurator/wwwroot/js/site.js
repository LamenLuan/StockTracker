function showErrorAlert(response) {
	const modal = $('#modal-alert')
	const modalTitle = modal.find('.modal-title:first')
	const modalBody = modal.find('.modal-body:first')
	modalTitle.text('Error')
	const errorMsg = `An error ocurred. ${response.content ? response.content : 'Please try again later'}`
	modalBody.text(errorMsg)
	new bootstrap.Modal('#modal-alert').show()
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
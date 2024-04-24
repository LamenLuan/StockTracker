function showErrorAlert(response) {
	const modal = $('#modal-alert')
	const modalTitle = modal.find('.modal-title:first')
	const modalBody = modal.find('.modal-body:first')
	modalTitle.text('Error')
	const errorMsg = `An error ocurred. ${response.content ? response.content : 'Please try again later'}`
	modalBody.text(errorMsg)
	new bootstrap.Modal('#modal-alert').show()
}
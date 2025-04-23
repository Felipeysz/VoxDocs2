// Função para abrir o modal de código
function openFilterModal() {
    const modal = new bootstrap.Modal(document.getElementById('documentCodeModal'));
    modal.show();
}

// Função para confirmar o código digitado
function confirmDocumentCode() {
    const inputCode = document.getElementById('documentCodeInput').value.trim();
    const errorMessage = document.getElementById('codeError');

    if (inputCode === "") {
        errorMessage.style.display = "block"; // Mostra mensagem de erro
    } else {
        errorMessage.style.display = "none";

        // Simular "abrir o documento"
        alert("Documento aberto com sucesso!");

        // Fechar o modal
        const modalElement = document.getElementById('documentCodeModal');
        const modalInstance = bootstrap.Modal.getInstance(modalElement);
        modalInstance.hide();

        // Limpa o campo para próxima vez
        document.getElementById('documentCodeInput').value = "";
    }
}

function generateRandomDocuments() {
    const types = ['DOC', 'PDF', 'XLS', 'PPT', 'JPG'];
    const categories = ['CONT', 'FISC', 'OPER', 'COMM', 'OUTR'];
    const documentsList = document.querySelector('.documents-list');
    documentsList.innerHTML = '';

    for (let i = 1; i <= 5; i++) {
        const type = types[Math.floor(Math.random() * types.length)];
        const category = categories[Math.floor(Math.random() * categories.length)];
        const docName = `${type}${category}042025ADM`;

        const docElement = document.createElement('div');
        docElement.className = 'document-item animate__animated animate__fadeInUp'; // Animação ao entrar
        docElement.textContent = `${docName} - ${type}`;

        // Efeito de abrir página ao clicar
        docElement.onclick = () => {
            docElement.classList.remove('animate__fadeInUp');
            docElement.classList.add('animate__animated', 'animate__flipOutY');

            setTimeout(() => {
                alert("Acessando documento: " + docName);
                docElement.classList.remove('animate__flipOutY');
                docElement.classList.add('animate__fadeInUp');
            }, 700);
        };

        documentsList.appendChild(docElement);
    }
}

window.onload = generateRandomDocuments;

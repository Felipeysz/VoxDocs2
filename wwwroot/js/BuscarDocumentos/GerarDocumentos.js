    // Função para gerar documentos aleatórios
    function GenerateRandomDocumento() {
        const types = ['DOC', 'PDF', 'XLS', 'PPT', 'JPG'];
        const categories = ['CONT', 'FISC', 'OPER', 'COMM', 'OUTR'];
        const documentsList = document.getElementById('documentsList');

        if (!documentsList) {
            console.error('Elemento documentsList não encontrado!');
            return;
        }

        documentsList.innerHTML = '';

        const documents = []; // Array para armazenar os documentos gerados

        for (let i = 1; i <= 5; i++) {
            const type = types[Math.floor(Math.random() * types.length)];
            const category = categories[Math.floor(Math.random() * categories.length)];
            const docName = `${type}${category}042025ADM`;

            // Criar objeto de documento e adicionar ao array
            documents.push({ nome: `${docName} - ${type}` });
        }

        // Renderizar os documentos gerados
        renderDocuments(documents);
    }

    // Função para exibir documentos na lista
    function renderDocuments(documents) {
        const documentsList = document.getElementById('documentsList');
        documentsList.innerHTML = '';
    
        if (!documents || documents.length === 0) {
            documentsList.innerHTML = '<p style="text-align:center; color: #888;">Nenhum documento encontrado.</p>';
            return;
        }
    
        documents.forEach(doc => {
            const item = document.createElement('div');
            item.className = 'document-item';
            item.textContent = doc.nome || 'Documento Sem Nome';
    
            // Aqui adicionamos o evento de clique para abrir o modal
            item.addEventListener('click', function() {
                selectedDocumentName = doc.nome; // salva o documento selecionado
                openFilterModal();
            });
            documentsList.appendChild(item);
        });
    }
    

    // Chamar a função para gerar os documentos assim que a página for carregada
    window.onload = function () {
        GenerateRandomDocumento();  // Gerar documentos ao carregar a página
    };
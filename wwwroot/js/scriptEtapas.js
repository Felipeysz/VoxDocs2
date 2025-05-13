let selectedAreaId = null;
let selectedTipoId = null;

// Etapa 1: Seleciona a área
function selectArea(button, areaId) {
    document.querySelectorAll('#step1 .doc-list button').forEach(btn =>
        btn.classList.remove('selected')
    );
    button.classList.add('selected');
    selectedAreaId = areaId;

    // Avança para etapa 2
    const wrapper = document.getElementById('steps-wrapper');
    const step1 = document.getElementById('step1');
    const step2 = document.getElementById('step2');
    step2.classList.remove('d-none');
    wrapper.classList.add('show-step2');
    step1.classList.add('animate__animated', 'animate__fadeOutLeft');
    step2.classList.add('animate__animated', 'animate__fadeInRight');
    setTimeout(() => {
        step1.classList.add('d-none');
        step1.classList.remove('animate__animated', 'animate__fadeOutLeft');
        step2.classList.remove('animate__animated', 'animate__fadeInRight');
    }, 800);
}

// Etapa 2: Seleciona o tipo e carrega a exibição dos documentos via AJAX
function selectTipo(button, tipoId) {
    document.querySelectorAll('#step2 .doc-list button').forEach(btn =>
        btn.classList.remove('selected')
    );
    button.classList.add('selected');
    selectedTipoId = tipoId;

    if (selectedAreaId && selectedTipoId) {
        // Carrega a partial da lista de documentos via AJAX
        fetch(`/DocumentosMvc/DocumentosExibir?areaId=${selectedAreaId}&tipoId=${selectedTipoId}`)
            .then(resp => resp.text())
            .then(html => {
                // Mostra etapa 3 e insere o HTML
                const etapa3 = document.getElementById('etapa3-container');
                document.getElementById('documentos-lista-container').innerHTML = html;
                etapa3.classList.remove('d-none');
                etapa3.classList.add('animate__animated', 'animate__fadeInRight');

                // Esconde etapa 2 com animação
                const step2 = document.getElementById('step2');
                step2.classList.add('animate__animated', 'animate__fadeOutLeft');
                setTimeout(() => {
                    step2.classList.add('d-none');
                    step2.classList.remove('animate__animated', 'animate__fadeOutLeft');
                    etapa3.classList.remove('animate__animated', 'animate__fadeInRight');
                }, 800);
            });
    }
}

// Funções auxiliares já existentes
function showTokenModal() { new bootstrap.Modal(document.getElementById('tokenModal')).show(); }
function showManualModal() { new bootstrap.Modal(document.getElementById('manualModal')).show(); }
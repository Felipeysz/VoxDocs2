import { FormUtils } from './ultilities.js';

export function initStep2() {
    document.getElementById('btnAdicionarPastaPrincipal')?.addEventListener('click', addPasta);
    document.querySelectorAll('.btn-adicionar-subpasta').forEach(btn => {
        btn.addEventListener('click', (e) => addSubpasta(e.target.closest('.card')));
    });
}

function addPasta() {
    const container = document.getElementById('pastasPrincipaisContainer');
    const template = document.getElementById('templatePastaPrincipal');
    if (container && template) {
        container.appendChild(template.content.cloneNode(true));
    }
}

function addSubpasta(parentCard) {
    const container = parentCard?.querySelector('.subpastas-container');
    const template = document.getElementById('templateSubpasta');
    if (container && template) {
        container.appendChild(template.content.cloneNode(true));
    }
}

export function validateStep2() {
    const pastas = document.querySelectorAll('#pastasPrincipaisContainer .pasta-nome');
    const errorElement = document.getElementById('step2Error');

    if (pastas.length === 0) {
        FormUtils.showAlert(errorElement, 'Adicione pelo menos uma pasta');
        return false;
    }

    return true;
}
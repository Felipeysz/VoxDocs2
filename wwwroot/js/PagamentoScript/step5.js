import { FormUtils } from './utilities.js';

export function initStep5() {
    document.querySelector('[name="MetodoPagamentoSelecionado"]')?.addEventListener('change', (e) => {
        const cartaoFields = document.getElementById('cartaoCreditoFields');
        cartaoFields?.classList.toggle('d-none', e.target.value !== 'CARTAO');
    });
}

export function validateStep5() {
    const metodo = document.querySelector('[name="MetodoPagamentoSelecionado"]')?.value;
    const errorElement = document.getElementById('step5Error');

    if (!metodo) {
        FormUtils.showAlert(errorElement, 'Selecione um método de pagamento');
        return false;
    }

    if (metodo === 'CARTAO') {
        const numeroCartao = document.querySelector('#cartaoCreditoFields input[type="text"]')?.value.trim();
        const validade = document.querySelector('#cartaoCreditoFields input[type="text"]:nth-child(2)')?.value.trim();
        const codigo = document.querySelector('#cartaoCreditoFields input[type="text"]:nth-child(3)')?.value.trim();

        if (!numeroCartao || !validade || !codigo) {
            FormUtils.showAlert(errorElement, 'Preencha todos os campos do cartão');
            return false;
        }
    }

    return true;
}

export function getStep5Data() {
    return {
        MetodoPagamentoSelecionado: document.querySelector('[name="MetodoPagamentoSelecionado"]').value
    };
}
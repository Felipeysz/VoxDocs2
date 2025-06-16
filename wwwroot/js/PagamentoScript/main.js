import { FormUtils } from './ultilities.js';

// Import dinâmico dos steps
const steps = {
    1: () => import('./step1.js'),
    2: () => import('./step2.js'),
    3: () => import('./step3.js'),
    4: () => import('./step4.js'),
    5: () => import('./step5.js')
};

export class PaymentFlow {
    constructor() {
        this.currentStep = 1;
        this.totalSteps = 5;
        this.init();
    }

    async init() {
        FormUtils.initPasswordToggle();
        this.setupEventListeners();
        await this.showStep(this.currentStep);
    }

    setupEventListeners() {
        document.getElementById('nextBtn')?.addEventListener('click', () => this.nextStep());
        document.getElementById('prevBtn')?.addEventListener('click', () => this.prevStep());
    }

    async showStep(step) {
        // Atualiza conteúdo dos passos
        document.querySelectorAll('.step-content').forEach(el => {
            el.classList.toggle('d-block', parseInt(el.dataset.step) === step);
            el.classList.toggle('d-none', parseInt(el.dataset.step) !== step);
        });

        // Atualiza visual do stepper
        document.querySelectorAll('.step').forEach((el, idx) => {
            el.classList.toggle('active', idx + 1 === step);
            el.classList.toggle('completed', idx + 1 < step);
        });

        // Atualiza linha de progresso
        const progress = document.querySelector('.stepper-line-progress');
        const progressWidth = ((step - 1) / (this.totalSteps - 1)) * 100;
        progress.style.width = `${progressWidth}%`;

        // Atualiza navegação
        document.getElementById('prevBtn').disabled = step === 1;
        document.getElementById('nextBtn').textContent = step === this.totalSteps ? 'Finalizar' : 'Próximo »';

        // Carrega e inicializa o passo atual
        await this.initStep(step);
    }

    async initStep(step) {
        try {
            const stepModule = await steps[step]();
            const initFunction = stepModule[`initStep${step}`];
            if (initFunction) {
                initFunction();
            }
        } catch (error) {
            console.error(`Erro ao carregar o step ${step}:`, error);
        }
    }

    async validateStep() {
        try {
            const stepModule = await steps[this.currentStep]();
            const validateFunction = stepModule[`validateStep${this.currentStep}`];
            if (validateFunction) {
                return validateFunction();
            }
            return true;
        } catch (error) {
            console.error(`Erro ao validar o step ${this.currentStep}:`, error);
            return false;
        }
    }

    async nextStep() {
        if (await this.validateStep()) {
            this.currentStep++;
            await this.showStep(this.currentStep);
        }
    }

    async prevStep() {
        if (this.currentStep > 1) {
            this.currentStep--;
            await this.showStep(this.currentStep);
        }
    }
}

// Inicializa quando o DOM estiver pronto
document.addEventListener('DOMContentLoaded', () => {
    new PaymentFlow();
});
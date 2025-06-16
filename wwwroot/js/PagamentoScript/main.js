import { FormUtils } from './utilities.js';
import { initStep1, validateStep1, getStep1Data } from './step1.js';
import { initStep2, validateStep2, getStep2Data } from './step2.js';
import { initStep3, validateStep3, getStep3Data } from './step3.js';
import { initStep4, validateStep4, getStep4Data } from './step4.js';
import { initStep5, validateStep5, getStep5Data } from './step5.js';

export class PaymentFlow {
  constructor() {
    this.currentStep = 1;
    this.totalSteps = 5;
    this.init();
  }

  init() {
    FormUtils.initPasswordToggle();
    this.setupEventListeners();
    this.showStep(this.currentStep);
  }

  setupEventListeners() {
    document.getElementById('nextBtn')?.addEventListener('click', () => this.nextStep());
    document.getElementById('prevBtn')?.addEventListener('click', () => this.prevStep());
  }

  showStep(step) {
    // Atualiza conteúdo dos passos
    document.querySelectorAll('.step-content').forEach(el => {
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
    document.getElementById('nextBtn').style.display = step === this.totalSteps ? 'none' : '';

    // Inicializa o passo atual
    this.initStep(step);
  }

  initStep(step) {
    switch(step) {
      case 1: initStep1(); break;
      case 2: initStep2(); break;
      case 3: initStep3(); break;
      case 4: initStep4(); break;
      case 5: initStep5(); break;
    }
  }

  validateStep() {
    switch(this.currentStep) {
      case 1: return validateStep1();
      case 2: return validateStep2();
      case 3: return validateStep3();
      case 4: return validateStep4();
      case 5: return validateStep5();
      default: return true;
    }
  }

  nextStep() {
    if (this.validateStep()) {
      this.currentStep++;
      this.showStep(this.currentStep);
    }
  }

  prevStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
      this.showStep(this.currentStep);
    }
  }
}

// Inicializa quando o DOM estiver pronto
document.addEventListener('DOMContentLoaded', () => new PaymentFlow());
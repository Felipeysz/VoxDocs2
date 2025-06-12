// main.js - Controlador principal do fluxo de pagamento
import { initStep1, validateStep1, getStep1Data } from './step1.js';
import { initStep2, validateStep2, getStep2Data } from './step2.js';
import { initStep3, validateStep3, getStep3Data } from './step3.js';
import { initStep4, validateStep4, getStep4Data } from './step4.js';
import { initStep5, validateStep5, getStep5Data } from './step5.js';
import { removeOldEventListeners } from './ultilities.js';

class PaymentFlow {
  constructor() {
    this.currentStep = 1;
    this.totalSteps = 5;
    this.formData = JSON.parse(sessionStorage.getItem('formData')) || {};
    this.init();
  }

  init() {
    document.addEventListener('DOMContentLoaded', () => {
      this.mergeFormDataWithViewBag();
      this.setupEventListeners();
      this.showStep(this.currentStep);
    });
  }

  mergeFormDataWithViewBag() {
    const id = document.getElementById('idPlano')?.value;
    const nomePlano = document.getElementById('nomePlano')?.value;
    const periodicidade = document.getElementById('periodicidadePlano')?.value;

    this.formData = {
      ...this.formData,
      Id: id,
      NomePlano: nomePlano,
      Periodicidade: periodicidade,
      EmpresaContratante: this.formData.EmpresaContratante || '',
      EmailEmpresa: this.formData.EmailEmpresa || '',
      PastasPrincipais: this.formData.PastasPrincipais || [],
      Administradores: this.formData.Administradores || [],
      Usuarios: this.formData.Usuarios || []
    };
  }

  setupEventListeners() {
    document.getElementById('nextBtn').addEventListener('click', () => this.goToNextStep());
    document.getElementById('prevBtn').addEventListener('click', () => this.goToPrevStep());
  }

  showStep(step) {
    // Esconde todos os steps
    document.querySelectorAll('.step-content').forEach(el => {
      el.classList.add('d-none');
    });

    // Exibe o step atual
    const content = document.querySelector(`.step-content[data-step="${step}"]`);
    if (content) content.classList.remove('d-none');

    // Atualiza o stepper
    this.updateStepperUI(step);
    this.updateNavigationButtons(step);

    // Inicializa o step
    this.initializeStep(step);
  }

  updateStepperUI(step) {
    document.querySelectorAll('.step').forEach((el, idx) => {
      const active = idx + 1 === step;
      el.classList.toggle('bg-primary', active);
      el.classList.toggle('text-white', active);
      el.classList.toggle('bg-light', !active);
      el.classList.toggle('text-muted', !active);
    });
  }

  updateNavigationButtons(step) {
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');

    prevBtn.disabled = (step === 1);

    if (step === this.totalSteps) {
      nextBtn.style.display = 'none';
    } else {
      nextBtn.style.display = '';
      nextBtn.textContent = 'Próximo »';
    }
  }

  initializeStep(step) {
    removeOldEventListeners();
    
    switch (step) {
      case 1: initStep1(this.formData); break;
      case 2: initStep2(this.formData); break;
      case 3: initStep3(this.formData); break;
      case 4: initStep4(this.formData); break;
      case 5: initStep5(this.formData); break;
    }
  }

  validateCurrentStep() {
    const validators = {
      1: validateStep1,
      2: validateStep2,
      3: validateStep3,
      4: validateStep4,
      5: validateStep5
    };
    return validators[this.currentStep]?.() ?? true;
  }

  saveStepData(step) {
    const dataGetters = {
      1: getStep1Data,
      2: getStep2Data,
      3: getStep3Data,
      4: getStep4Data,
      5: getStep5Data
    };

    const data = dataGetters[step]?.();
    if (data) {
      this.formData = { ...this.formData, ...data };
      sessionStorage.setItem('formData', JSON.stringify(this.formData));
    }
  }

  goToNextStep() {
    if (!this.validateCurrentStep()) return;

    this.saveStepData(this.currentStep);

    if (this.currentStep < this.totalSteps) {
      this.currentStep++;
      this.showStep(this.currentStep);
    }
  }

  goToPrevStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
      this.showStep(this.currentStep);
    }
  }
}

// Inicializa o fluxo
new PaymentFlow();
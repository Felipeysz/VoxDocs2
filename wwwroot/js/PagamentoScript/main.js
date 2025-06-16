import { FormUtils } from './utilities.js';
import { initStep1, validateStep1, getStep1Data } from './step1.js';
import { initStep2, validateStep2, getStep2Data } from './step2.js';
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
    document.querySelectorAll('.step-content').forEach(el => {
      el.classList.toggle('d-none', parseInt(el.dataset.step) !== step);
    });

    document.querySelectorAll('.step').forEach((el, idx) => {
      el.classList.toggle('active', idx + 1 === step);
    });

    document.getElementById('prevBtn').disabled = step === 1;
    document.getElementById('nextBtn').style.display = step === this.totalSteps ? 'none' : '';

    this.initStep(step);
  }

  initStep(step) {
    switch(step) {
      case 1: initStep1(); break;
      case 2: initStep2(); break;
      case 5: initStep5(); break;
    }
  }

  validateStep() {
    switch(this.currentStep) {
      case 1: return validateStep1();
      case 2: return validateStep2();
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

new PaymentFlow();
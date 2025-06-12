// utilities.js - Utilitários gerais
export class FormUtils {
  static initPasswordToggle() {
    document.addEventListener('click', e => {
      if (e.target.closest('.toggle-password')) {
        const btn = e.target.closest('.toggle-password');
        const input = btn.parentElement.querySelector('.password-field');
        const icon = btn.querySelector('i');
        
        if (input.type === 'password') {
          input.type = 'text';
          icon.classList.replace('fa-eye', 'fa-eye-slash');
        } else {
          input.type = 'password';
          icon.classList.replace('fa-eye-slash', 'fa-eye');
        }
      }
    });
  }

  static isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  }

  static isValidPassword(password) {
    return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/.test(password);
  }

  static showAlert(elementId, message, type = 'danger') {
    const el = document.getElementById(elementId);
    if (el) {
      el.textContent = message;
      el.className = `alert alert-${type}`;
      el.classList.remove('d-none');
      setTimeout(() => el.classList.add('d-none'), 5000);
    }
  }

  static clearAlerts(...elementIds) {
    elementIds.forEach(id => {
      const el = document.getElementById(id);
      if (el) {
        el.textContent = '';
        el.classList.add('d-none');
      }
    });
  }
}

// Gerenciamento de Event Listeners
export class EventManager {
  static removeEventListeners(element, eventType, handler) {
    if (element && handler) {
      element.removeEventListener(eventType, handler);
    }
  }

  static removeAllListeners(element) {
    if (element) {
      const newElement = element.cloneNode(true);
      element.parentNode.replaceChild(newElement, element);
      return newElement;
    }
    return null;
  }
}

// Inicializa os utilitários básicos
document.addEventListener('DOMContentLoaded', () => {
  FormUtils.initPasswordToggle();
});
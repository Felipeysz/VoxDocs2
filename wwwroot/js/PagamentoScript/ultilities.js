export class FormUtils {
    static initPasswordToggle() {
        document.addEventListener('click', (e) => {
            const btn = e.target.closest('.toggle-password');
            if (!btn) return;
            
            const input = btn.parentElement.querySelector('.password-field');
            const icon = btn.querySelector('i');
            
            if (input.type === 'password') {
                input.type = 'text';
                icon.classList.replace('fa-eye', 'fa-eye-slash');
            } else {
                input.type = 'password';
                icon.classList.replace('fa-eye-slash', 'fa-eye');
            }
        });
    }

    static isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    static showAlert(element, message, isSuccess = false) {
        if (!element) return;
        
        element.textContent = message;
        element.classList.remove('d-none', 'text-danger', 'text-success');
        element.classList.add(isSuccess ? 'text-success' : 'text-danger');
    }
}
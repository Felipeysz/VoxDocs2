// paymentUtils.js - Utilitários de pagamento e integração com API
export class PaymentService {
  static validCardNumber(num) {
    const sanitized = num.replace(/\D/g, '');
    let sum = 0, shouldDouble = false;
    for (let i = sanitized.length - 1; i >= 0; i--) {
      let digit = parseInt(sanitized.charAt(i), 10);
      if (shouldDouble) {
        digit *= 2;
        if (digit > 9) digit -= 9;
      }
      sum += digit;
      shouldDouble = !shouldDouble;
    }
    return sum % 10 === 0;
  }

  static detectBrand(num) {
    const sanitized = num.replace(/\D/g, '');
    if (/^4/.test(sanitized)) return 'visa';
    if (/^5[1-5]/.test(sanitized)) return 'mastercard';
    if (/^3[47]/.test(sanitized)) return 'amex';
    return '';
  }

  static validExpiry(v) {
    if (!/^\d{2}\/\d{2}$/.test(v)) return false;
    const [mm, yy] = v.split('/').map(n => parseInt(n, 10));
    if (mm < 1 || mm > 12) return false;
    const now = new Date();
    const exp = new Date(2000 + yy, mm, 0, 23, 59, 59);
    return exp > now;
  }

  static validCvv(cvv, brand = '') {
    const sanitized = cvv.replace(/\D/g, '');
    return brand === 'amex'
      ? /^\d{4}$/.test(sanitized)
      : /^\d{3}$/.test(sanitized);
  }

  static buildPaymentPayload(formData) {
    const pastasPrincipais = (formData.PastasPrincipais || []).map(pasta => ({
      nomePastaPrincipal: pasta.Nome || '',
      empresaContratante: formData.EmpresaContratante || ''
    }));

    const subPastas = (formData.PastasPrincipais || []).flatMap(pasta =>
      (pasta.Subpastas || []).map(sub => ({
        nomeSubPasta: sub,
        nomePastaPrincipal: pasta.Nome || '',
        empresaContratante: formData.EmpresaContratante || ''
      }))
    );

    const usuarios = [
      ...(formData.Administradores || []).map(u => ({
        usuario: u.Usuario || '',
        email: u.Email || '',
        senha: u.Senha || '',
        permissionAccount: u.PermissionAccount || '',
        empresaContratante: u.EmpresaContratante || formData.EmpresaContratante || '',
        planoPago: formData.NomePlano || ''
      })),
      ...(formData.Usuarios || []).map(u => ({
        usuario: u.Usuario || '',
        email: u.Email || '',
        senha: u.Senha || '',
        permissionAccount: u.PermissionAccount || '',
        empresaContratante: u.EmpresaContratante || formData.EmpresaContratante || '',
        planoPago: formData.NomePlano || ''
      }))
    ];

    return {
      id: formData.Id || '',
      metodoPagamento: formData.metodoPagamento || '',
      empresaContratantePlano: formData.EmpresaContratante || '',
      dataPagamento: new Date().toISOString(),
      nomePlano: formData.NomePlano || '',
      periodicidade: formData.Periodicidade || '',
      empresaContratante: {
        empresaContratante: formData.EmpresaContratante || '',
        email: formData.EmailEmpresa || ''
      },
      pastasPrincipais,
      subPastas,
      usuarios
    };
  }

  static async processPayment(formData) {
    try {
      if (formData.metodoPagamento === 'cartao') {
        const { numero, validade, cvv } = formData.cartao || {};
        if (!this.validCardNumber(numero)) throw new Error('Número do cartão inválido');
        if (!this.validExpiry(validade)) throw new Error('Data de validade inválida');
        if (!this.validCvv(cvv, this.detectBrand(numero))) throw new Error('CVV inválido');
      }

      const payload = this.buildPaymentPayload(formData);
      return await this.finalizePayment(payload);
    } catch (error) {
      console.error('Erro no processamento do pagamento:', error);
      throw error;
    }
  }

  static async finalizePayment(payload) {
    try {
      const response = await fetch('/api/Pagamentos/finalizar', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const errorData = await this.parseErrorResponse(response);
        throw new Error(errorData.message || 'Falha ao finalizar pagamento');
      }

      return await response.json();
    } catch (error) {
      console.error('Erro na finalização do pagamento:', error);
      throw error;
    }
  }

  static async parseErrorResponse(response) {
    try {
      const text = await response.text();
      try {
        const data = JSON.parse(text);
        return {
          message: data.message || data.title || 'Erro desconhecido',
          details: data.errors || data.detail
        };
      } catch {
        return {
          message: text.split('\n')[0] || 'Erro desconhecido',
          details: text
        };
      }
    } catch {
      return {
        message: 'Falha ao processar resposta do servidor',
        details: null
      };
    }
  }
}
using VoxDocs.Models;
using VoxDocs.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using VoxDocs.Services;
using VoxDocs.Repository;
using VoxDocs.Data.Repositories;

namespace VoxDocs.BusinessRules
{

    // Classes de implementação
   public class PastaPrincipalBusinessRules : IPastaPrincipalBusinessRules
{
    private readonly IPastaPrincipalRepository _repository;
    private readonly ILogger<PastaPrincipalBusinessRules> _logger;

    public PastaPrincipalBusinessRules(
        IPastaPrincipalRepository repository,
        ILogger<PastaPrincipalBusinessRules> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<BusinessResult<PastaPrincipalModel>> ValidateAndCreateAsync(PastaPrincipalModel model)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(model.NomePastaPrincipal))
            {
                return new BusinessResult<PastaPrincipalModel>(
                    null, 
                    false, 
                    "O nome da pasta principal é obrigatório.", 
                    400);
            }

            if (string.IsNullOrWhiteSpace(model.EmpresaContratante))
            {
                return new BusinessResult<PastaPrincipalModel>(
                    null, 
                    false, 
                    "A empresa contratante é obrigatória.", 
                    400);
            }

            // Check for duplicate name
            var existingPasta = await _repository.GetByNamePrincipalAsync(model.NomePastaPrincipal);
            if (existingPasta != null)
            {
                return new BusinessResult<PastaPrincipalModel>(
                    null, 
                    false, 
                    $"Já existe uma pasta principal com o nome '{model.NomePastaPrincipal}'.", 
                    409);
            }

            // Create the entity
            model.Id = Guid.NewGuid();
            model.SubPastas = new List<SubPastaModel>(); // Initialize empty list

            var createdPasta = await _repository.CreateAsync(model);

            return new BusinessResult<PastaPrincipalModel>(createdPasta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pasta principal");
            return new BusinessResult<PastaPrincipalModel>(
                null, 
                false, 
                "Ocorreu um erro interno ao criar a pasta principal.", 
                500);
        }
    }

    public async Task<BusinessResult<bool>> ValidateAndDeleteAsync(Guid id)
    {
        try
        {
            var pasta = await _repository.GetByIdAsync(id);
            if (pasta == null)
            {
                return new BusinessResult<bool>(
                    false, 
                    false, 
                    "Pasta principal não encontrada.", 
                    404);
            }

            // Check if folder has subfolders (business rule example)
            if (pasta.SubPastas != null && pasta.SubPastas.Any())
            {
                return new BusinessResult<bool>(
                    false, 
                    false, 
                    "Não é possível excluir a pasta principal porque ela contém subpastas.", 
                    400);
            }

            var result = await _repository.DeleteAsync(id);
            return new BusinessResult<bool>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao excluir pasta principal ID: {id}");
            return new BusinessResult<bool>(
                false, 
                false, 
                "Ocorreu um erro interno ao excluir a pasta principal.", 
                500);
        }
    }

    public async Task<BusinessResult<PastaPrincipalModel>> ValidateGetByIdAsync(Guid id)
    {
        try
        {
            var pasta = await _repository.GetByIdAsync(id);
            if (pasta == null)
            {
                return new BusinessResult<PastaPrincipalModel>(
                    null, 
                    false, 
                    "Pasta principal não encontrada.", 
                    404);
            }

            return new BusinessResult<PastaPrincipalModel>(pasta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao buscar pasta principal ID: {id}");
            return new BusinessResult<PastaPrincipalModel>(
                null, 
                false, 
                "Ocorreu um erro interno ao buscar a pasta principal.", 
                500);
        }
    }

    public async Task<BusinessResult<PastaPrincipalModel>> ValidateGetByNameAsync(string nomePasta)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nomePasta))
            {
                return new BusinessResult<PastaPrincipalModel>(
                    null, 
                    false, 
                    "O nome da pasta é obrigatório.", 
                    400);
            }

            var pasta = await _repository.GetByNamePrincipalAsync(nomePasta);
            if (pasta == null)
            {
                return new BusinessResult<PastaPrincipalModel>(
                    null, 
                    false, 
                    "Pasta principal não encontrada.", 
                    404);
            }

            return new BusinessResult<PastaPrincipalModel>(pasta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao buscar pasta principal por nome: {nomePasta}");
            return new BusinessResult<PastaPrincipalModel>(
                null, 
                false, 
                "Ocorreu um erro interno ao buscar a pasta principal.", 
                500);
        }
    }

    public async Task<BusinessResult<IEnumerable<PastaPrincipalModel>>> ValidateGetByEmpresaAsync(string empresaContratante)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(empresaContratante))
            {
                return new BusinessResult<IEnumerable<PastaPrincipalModel>>(
                    null, 
                    false, 
                    "O nome da empresa contratante é obrigatório.", 
                    400);
            }

            var pastas = await _repository.GetByEmpresaAsync(empresaContratante);
            if (pastas == null || !pastas.Any())
            {
                return new BusinessResult<IEnumerable<PastaPrincipalModel>>(
                    Enumerable.Empty<PastaPrincipalModel>(), 
                    true, 
                    "Nenhuma pasta principal encontrada para esta empresa.", 
                    200);
            }

            return new BusinessResult<IEnumerable<PastaPrincipalModel>>(pastas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao buscar pastas por empresa: {empresaContratante}");
            return new BusinessResult<IEnumerable<PastaPrincipalModel>>(
                null, 
                false, 
                "Ocorreu um erro interno ao buscar pastas por empresa.", 
                500);
        }
    }
}

    public class DocumentoBusinessRules : IDocumentoBusinessRules
    {
        public const string DocumentoNaoEncontradoMsg = "Documento não encontrado.";
        public const string TokenObrigatorioMsg = "Token de segurança é obrigatório para este documento.";
        public const string TokenInvalidoMsg = "Token de segurança inválido.";
        public const string ArquivoNaoEncontradoMsg = "Arquivo não encontrado no storage.";
        private readonly IDocumentoRepository _documentoRepository;


        public DocumentoBusinessRules(IDocumentoRepository documentoRepository)
        {
            _documentoRepository = documentoRepository;
        }

        public async Task<bool> ValidateTokenDocumentoAsync(string nomeArquivo, string token)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                throw new ArgumentException("Nome do arquivo não pode ser vazio.");

            var doc = (await _documentoRepository.GetAllAsync())
                .FirstOrDefault(d => d.NomeArquivo.ToLower() == nomeArquivo.ToLower());

            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (doc.NivelSeguranca == "Publico")
                return true;

            ValidateTokenSecurity(token, doc.TokenSeguranca);

            return true;
        }

        public async Task<DocumentoModel> GetByIdAsync(Guid id, string? token = null)
        {
            var doc = await _documentoRepository.GetByIdAsync(id);
            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (doc.NivelSeguranca != "Publico")
            {
                ValidateTokenSecurity(token, doc.TokenSeguranca);
            }

            return doc;
        }

        public async Task DeleteAsync(Guid id, string? token)
        {
            var doc = await _documentoRepository.GetByIdAsync(id);
            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (doc.NivelSeguranca != "Publico")
            {
                ValidateTokenSecurity(token, doc.TokenSeguranca);
            }

            await _documentoRepository.DeleteAsync(doc);
        }

        public async Task<IEnumerable<DocumentoModel>> GetAllAsync()
        {
            return await _documentoRepository.GetAllAsync();
        }

        public async Task<IEnumerable<DocumentoModel>> GetBySubPastaAsync(string subPasta)
        {
            if (string.IsNullOrWhiteSpace(subPasta))
                throw new ArgumentException("Nome da subpasta não pode ser vazio.");

            return await _documentoRepository.GetBySubPastaAsync(subPasta);
        }

        public async Task<IEnumerable<DocumentoModel>> GetByPastaPrincipalAsync(string pastaPrincipal)
        {
            if (string.IsNullOrWhiteSpace(pastaPrincipal))
                throw new ArgumentException("Nome da pasta principal não pode ser vazio.");

            return await _documentoRepository.GetByPastaPrincipalAsync(pastaPrincipal);
        }

        public async Task<DocumentoModel> CreateAsync(DocumentoModel documento, Stream arquivoStream, string usuario)
        {
            ValidateDocumentCreation(documento, arquivoStream);

            if (await _documentoRepository.ArquivoExisteAsync(documento.NomeArquivo))
                throw new InvalidOperationException("Já existe um documento com este nome.");

            if (documento.NivelSeguranca != "Publico" && string.IsNullOrWhiteSpace(documento.TokenSeguranca))
                throw new ArgumentException("Token de segurança é obrigatório para documentos não públicos.");

            documento.UsuarioCriador = usuario;
            documento.DataCriacao = DateTime.UtcNow;
            documento.UsuarioUltimaAlteracao = usuario;
            documento.DataUltimaAlteracao = DateTime.UtcNow;
            
            if (!string.IsNullOrWhiteSpace(documento.TokenSeguranca))
            {
                documento.TokenSeguranca = GenerateTokenHash(documento.TokenSeguranca);
            }

            await _documentoRepository.AddAsync(documento);

            return documento;
        }

        public async Task<DocumentoModel> UpdateAsync(DocumentoModel documento, Stream novoArquivoStream = null)
        {
            if (documento == null)
                throw new ArgumentNullException(nameof(documento));

            var existingDoc = await _documentoRepository.GetByIdAsync(documento.Id);
            if (existingDoc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (!string.IsNullOrWhiteSpace(documento.Descrição))
            {
                if (documento.Descrição.Length > 500)
                    throw new ArgumentException("Descrição não pode ter mais que 500 caracteres.");

                existingDoc.Descrição = documento.Descrição;
            }

            if (novoArquivoStream != null)
            {
                existingDoc.TamanhoArquivo = novoArquivoStream.Length;
                // Atualize outros campos relacionados ao arquivo conforme necessário
            }

            if (!string.IsNullOrEmpty(documento.NivelSeguranca))
            {
                existingDoc.NivelSeguranca = documento.NivelSeguranca;
            }

            if (!string.IsNullOrEmpty(documento.TokenSeguranca))
            {
                existingDoc.TokenSeguranca = GenerateTokenHash(documento.TokenSeguranca);
            }

            existingDoc.UsuarioUltimaAlteracao = documento.UsuarioUltimaAlteracao;
            existingDoc.DataUltimaAlteracao = DateTime.UtcNow;

            await _documentoRepository.UpdateAsync(existingDoc);

            return existingDoc;
        }

        public async Task<DocumentoEstatisticas> GetEstatisticasEmpresaAsync(string empresa)
        {
            if (string.IsNullOrWhiteSpace(empresa))
                throw new ArgumentException("Nome da empresa não pode ser vazio.");

            var documentos = (await _documentoRepository.GetAllAsync())
                .Where(d => d.Empresa == empresa)
                .ToList();

            return new DocumentoEstatisticas
            {
                EmpresaContratante = empresa,
                QuantidadeDocumentos = documentos.Count,
                TamanhoTotalGb = documentos.Sum(d => d.TamanhoArquivo) / 1024.0 / 1024.0 / 1024.0
            };
        }

        public async Task IncrementarAcessoAsync(Guid id)
        {
            await _documentoRepository.IncrementarAcessoAsync(id);
        }

        public async Task<bool> ArquivoExisteAsync(string nomeArquivo)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                throw new ArgumentException("Nome do arquivo não pode ser vazio.");

            return await _documentoRepository.ArquivoExisteAsync(nomeArquivo);
        }

        public async Task<(Stream stream, string contentType)> DownloadDocumentoProtegidoAsync(string nomeArquivo, string token = null)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                throw new ArgumentException("Nome do arquivo não pode ser vazio.");

            var doc = (await _documentoRepository.GetAllAsync())
                .FirstOrDefault(d => d.NomeArquivo == nomeArquivo);

            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (doc.NivelSeguranca != "Publico")
            {
                ValidateTokenSecurity(token, doc.TokenSeguranca);
            }

            return (Stream.Null, "application/octet-stream");
        }

        public string GenerateTokenHash(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(token);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public DateTime ConvertToBrasiliaTime(DateTime utcTime)
        {
            TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, brasiliaTimeZone);
        }

        public void ValidateDocumentCreation(DocumentoModel documento, Stream arquivoStream)
        {
            if (documento == null)
                throw new ArgumentNullException(nameof(documento));

            if (string.IsNullOrWhiteSpace(documento.UsuarioCriador))
                throw new ArgumentException("Usuário não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(documento.Empresa))
                throw new ArgumentException("Empresa contratante não pode ser vazia.");

            if (string.IsNullOrWhiteSpace(documento.NomePastaPrincipal))
                throw new ArgumentException("Nome da pasta principal não pode ser vazio.");

            if (arquivoStream == null || arquivoStream.Length == 0)
                throw new ArgumentException("Arquivo não pode ser vazio.");

            if (arquivoStream.Length > 100 * 1024 * 1024) // 100MB
                throw new ArgumentException("Tamanho máximo do arquivo é 100MB.");

            if (!IsValidFileType(documento.NomeArquivo))
                throw new ArgumentException("Tipo de arquivo não suportado.");
        }

        public string GetErrorMessage(string operation, Exception ex)
        {
            return $"Erro ao {operation} documento: {ex.Message}";
        }

        public void ValidateTokenSecurity(string token, string? tokenSeguranca)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException(TokenObrigatorioMsg);

            token = token.Trim();
            string hashedToken = GenerateTokenHash(token);

            if (tokenSeguranca != hashedToken)
                throw new UnauthorizedAccessException(TokenInvalidoMsg);
        }

        private bool IsValidFileType(string fileName)
        {
            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".pptx", ".txt" };
            var extension = Path.GetExtension(fileName)?.ToLower();
            return allowedExtensions.Contains(extension);
        }
    }

    public class SubPastaBusinessRules : ISubPastaBusinessRules
    {
        private const int MaxNomeLength = 100;
        private const int MaxEmpresaLength = 50;

        private readonly ISubPastaRepository _subPastaRepository;
        private readonly IPastaPrincipalRepository _pastaPrincipalRepository;

        public SubPastaBusinessRules(
            ISubPastaRepository subPastaRepository,
            IPastaPrincipalRepository pastaPrincipalRepository)
        {
            _subPastaRepository = subPastaRepository;
            _pastaPrincipalRepository = pastaPrincipalRepository;
        }

        public async Task<SubPastaModel> ValidateAndCreateSubPastaAsync(SubPastaModel subPasta)
        {
            await ValidateSubPastaAsync(subPasta);
            return await _subPastaRepository.CreateAsync(subPasta);
        }

        public async Task<bool> CanDeleteSubPastaAsync(Guid id)
        {
            var subPasta = await _subPastaRepository.GetByIdAsync(id);
            if (subPasta == null) 
                return false;

            // Verificação mais eficiente se a subpasta tem documentos
            var subPastaComDocumentos = await _subPastaRepository.GetByIdAsync(id);
            return subPastaComDocumentos?.Documentos?.Count == 0;
        }

        public async Task<bool> IsSubPastaNameUniqueAsync(string nomeSubPasta, string empresaContratante)
        {
            if (string.IsNullOrWhiteSpace(nomeSubPasta))
                return true;

            var existing = await _subPastaRepository.GetByNameAndEmpresaAsync(nomeSubPasta, empresaContratante);
            return existing == null;
        }

        public async Task<bool> DoesPastaPrincipalExistAsync(string nomePastaPrincipal, string empresaContratante)
        {
            var pastaPrincipal = await _pastaPrincipalRepository.GetByNameAndEmpresaAsync(nomePastaPrincipal, empresaContratante);
            return pastaPrincipal != null;
        }

        public async Task ValidateSubPastaAsync(SubPastaModel subPasta)
        {
            if (subPasta == null)
                throw new ArgumentNullException(nameof(subPasta), "A subpasta não pode ser nula.");

            // Validação do NomeSubPasta
            if (string.IsNullOrWhiteSpace(subPasta.NomeSubPasta))
                throw new ArgumentException("O nome da subpasta é obrigatório.");

            if (subPasta.NomeSubPasta.Length > MaxNomeLength)
                throw new ArgumentException($"O nome da subpasta não pode exceder {MaxNomeLength} caracteres.");

            // Validação do NomePastaPrincipal
            if (string.IsNullOrWhiteSpace(subPasta.NomePastaPrincipal))
                throw new ArgumentException("O nome da pasta principal é obrigatório.");

            if (subPasta.NomePastaPrincipal.Length > MaxNomeLength)
                throw new ArgumentException($"O nome da pasta principal não pode exceder {MaxNomeLength} caracteres.");

            // Validação da Empresa
            if (string.IsNullOrWhiteSpace(subPasta.EmpresaContratante))
                throw new ArgumentException("A empresa contratante é obrigatória.");

            if (subPasta.EmpresaContratante.Length > MaxEmpresaLength)
                throw new ArgumentException($"O nome da empresa não pode exceder {MaxEmpresaLength} caracteres.");

            // Validações de negócio
            if (!await IsSubPastaNameUniqueAsync(subPasta.NomeSubPasta, subPasta.EmpresaContratante))
                throw new InvalidOperationException($"Já existe uma subpasta com o nome '{subPasta.NomeSubPasta}' para a empresa '{subPasta.EmpresaContratante}'.");

            if (!await DoesPastaPrincipalExistAsync(subPasta.NomePastaPrincipal, subPasta.EmpresaContratante))
                throw new InvalidOperationException($"A pasta principal '{subPasta.NomePastaPrincipal}' não existe para a empresa '{subPasta.EmpresaContratante}'.");
        }
    }

    // Classes auxiliares
    public class BusinessResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusCode { get; set; }

        public BusinessResult(T data, bool success = true, string errorMessage = null, int statusCode = 200)
        {
            Data = data;
            Success = success;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }
    }

    public class DocumentoEstatisticas
    {
        public string EmpresaContratante { get; set; }
        public int QuantidadeDocumentos { get; set; }
        public double TamanhoTotalGb { get; set; }
    }
}
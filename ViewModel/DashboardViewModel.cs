namespace VoxDocs.Models.ViewModels
{
    public class DashboardViewModel
    {
        public string Plano { get; set; }
        public string ArmazenamentoUsado { get; set; }
        public string ArmazenamentoTotal { get; set; }
        public int UsuariosAtuais { get; set; }
        public int UsuariosPermitidos { get; set; }
        public string TokensDisponiveis { get; set; } = "infinito";
        public int TokensGerados { get; set; }
        public int DocumentosEnviados { get; set; }
        public int ArquivosAlterados { get; set; }
        public int ConsultasRealizadas { get; set; }
        public string UltimaAtualizacao { get; set; }
        public int TotalPastas { get; set; }
        public bool PagamentoPixConfirmado { get; set; }
    }
}

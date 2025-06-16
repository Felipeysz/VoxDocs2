using System;
using System.Collections.Generic;

namespace VoxDocs.Models
{
    public class EstatisticasAdminModel
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public int TotalAdministradores { get; set; }
        public List<UserModel> UsuariosRecentes { get; set; }
        public int TotalEmpresas { get; set; }
        public int TotalPlanosAtivos { get; set; }
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }

    public class EstatisticasPlanoModel
    {
        public string NomePlano { get; set; }
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public int AdministradoresAtivos { get; set; }
        public double PercentualUso { get; set; }
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }

    public class EstatisticasEmpresaModel
    {
        public string NomeEmpresa { get; set; }
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public string PlanoContratado { get; set; }
        public DateTime DataContratacao { get; set; }
        public DateTime DataRenovacao { get; set; }
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
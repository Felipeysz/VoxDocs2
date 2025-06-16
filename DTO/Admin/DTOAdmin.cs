// DTO/DTOEstatisticasAdmin.cs
using System;
using System.Collections.Generic;

namespace VoxDocs.DTO
{
    public class DTOEstatisticasAdmin
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public int TotalAdministradores { get; set; }
        public List<DTOUsuarioInfo> UsuariosRecentes { get; set; }
        public int TotalEmpresas { get; set; }
        public int TotalPlanosAtivos { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class DTOEstatisticasPlano
    {
        public string NomePlano { get; set; }
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public int AdministradoresAtivos { get; set; }
        public double PercentualUso { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class DTOEstatisticasEmpresa
    {
        public string NomeEmpresa { get; set; }
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public string PlanoContratado { get; set; }
        public DateTime DataContratacao { get; set; }
        public DateTime DataRenovacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
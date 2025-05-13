using Microsoft.EntityFrameworkCore;
using VoxDocs.Models;

namespace VoxDocs.Data
{
    public class VoxDocsContext : DbContext
    {
        public VoxDocsContext(DbContextOptions<VoxDocsContext> options)
            : base(options)
        {
        }

        public DbSet<VoxDocs.Models.UserModel> Users { get; set; }
        public DbSet<DocumentoModel> Documentos { get; set; }
        public DbSet<DocumentoUploadModel> DocumentosUploads { get; set; }
        public DbSet<TipoDocumentoModel> TiposDocumento { get; set; }
        public DbSet<AreasDocumentoModel> AreasDocumento { get; set; }
    }
}

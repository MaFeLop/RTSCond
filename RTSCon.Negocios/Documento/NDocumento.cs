using RTSCon.Datos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Negocios
{
    public class NDocumento
    {
        private readonly DDocumento _dal;
        private readonly string _baseDocsPath;

        // _baseDocsPath: carpeta raíz donde guardas documentos (ej: C:\RTSCond\docs)
        public NDocumento(DDocumento dal, string baseDocsPath)
        {
            _dal = dal;
            _baseDocsPath = baseDocsPath;
        }

        public int InsertarProvisional(string rutaOrigen, string usuario, out string rutaProvisional, int version = 1)
        {
            if (string.IsNullOrWhiteSpace(rutaOrigen) || !File.Exists(rutaOrigen))
                throw new FileNotFoundException("Archivo no encontrado", rutaOrigen);

            var nombre = Path.GetFileName(rutaOrigen);
            var ext = Path.GetExtension(rutaOrigen);
            var tipoContenido = InferContentType(ext); // básico por extensión
            var tam = new FileInfo(rutaOrigen).Length;

            // Copia a STAGING con nombre único
            var stagingDir = Path.Combine(_baseDocsPath, "staging");
            Directory.CreateDirectory(stagingDir);
            var nombreUnico = Guid.NewGuid().ToString("N") + ext;
            rutaProvisional = Path.Combine(stagingDir, nombreUnico);
            File.Copy(rutaOrigen, rutaProvisional, true);

            // Inserta metadata con Ubicacion = provisional (audit trail consistente)
            var docId = _dal.Insertar(nombre, tipoContenido, tam, rutaProvisional, version, usuario);
            return docId;
        }

        public void FinalizarUbicacion(int documentoId, int condominioId, int version, string rutaProvisional, string usuario)
        {
            var ext = Path.GetExtension(rutaProvisional);
            var destinoDir = Path.Combine(_baseDocsPath, "Condominio", condominioId.ToString(), "Reglamento");
            Directory.CreateDirectory(destinoDir);

            var nombreFinal = string.Format("{0}_v{1}{2}", documentoId, version, ext);
            var rutaFinal = Path.Combine(destinoDir, nombreFinal);

            if (File.Exists(rutaFinal))
                File.Delete(rutaFinal);

            File.Move(rutaProvisional, rutaFinal);

            _dal.ActualizarUbicacion(documentoId, rutaFinal, usuario);
        }

        private static string InferContentType(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return "application/octet-stream";
            ext = ext.ToLowerInvariant();
            if (ext == ".pdf") return "application/pdf";
            if (ext == ".doc") return "application/msword";
            if (ext == ".docx") return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            if (ext == ".png") return "image/png";
            if (ext == ".jpg" || ext == ".jpeg") return "image/jpeg";
            if (ext == ".txt") return "text/plain";
            return "application/octet-stream";
        }
    }

}

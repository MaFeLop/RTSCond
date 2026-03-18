using System;
using System.Data;
using System.Data.SqlClient;

namespace RTSCon.Datos
{
    public class DCondominio
    {
        private readonly string _cn;

        public DCondominio(string connectionString)
        {
            _cn = connectionString;
        }

        public DataTable Listar(string buscar, bool soloActivos, int page, int pageSize, int? propietarioId, out int totalRows)
        {
            totalRows = 0;

            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_listar", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@buscar", SqlDbType.NVarChar, 200).Value =
                    string.IsNullOrWhiteSpace(buscar) ? (object)DBNull.Value : buscar.Trim();

                cmd.Parameters.Add("@soloActivos", SqlDbType.Bit).Value = soloActivos;
                cmd.Parameters.Add("@page", SqlDbType.Int).Value = page;
                cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;

                cmd.Parameters.Add("@PropietarioId", SqlDbType.Int).Value =
                    propietarioId.HasValue ? (object)propietarioId.Value : DBNull.Value;

                var pTotal = cmd.Parameters.Add("@totalRows", SqlDbType.Int);
                pTotal.Direction = ParameterDirection.Output;

                var dt = new DataTable();

                cn.Open();
                da.Fill(dt);

                totalRows = (pTotal.Value == null || pTotal.Value == DBNull.Value)
                    ? 0
                    : Convert.ToInt32(pTotal.Value);

                return dt;
            }
        }

        public void Notificar(string to, string subject, string htmlBody, string mailProfile)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_notificar_accion", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Para", (object)to ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Asunto", (object)subject ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Cuerpo", (object)htmlBody ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Perfil", (object)mailProfile ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataRow PorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_por_id", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                var dt = new DataTable();

                cn.Open();
                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public int Insertar(
            string nombre,
            string direccion,
            string tipo,
            string adminResp,
            DateTime fechaConstitucion,
            decimal cuotaBase,
            string emailNotif,
            bool enviarNotifProp,
            int? reglamentoDocId,
            int? idPropietario,
            int? idSec1,
            int? idSec2,
            int? idSec3,
            string creador)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_insertar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Direccion", direccion);
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cmd.Parameters.AddWithValue("@AdministradorResponsable", adminResp);
                cmd.Parameters.AddWithValue("@FechaConstitucion", fechaConstitucion);
                cmd.Parameters.AddWithValue("@CuotaMantenimientoBase", cuotaBase);
                cmd.Parameters.AddWithValue("@EmailNotificaciones", (object)emailNotif ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EnviarNotifsAlPropietario", enviarNotifProp);
                cmd.Parameters.AddWithValue("@ReglamentoDocumentoId", (object)reglamentoDocId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_propietario", (object)idPropietario ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_usr_secretario1", (object)idSec1 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_usr_secretario2", (object)idSec2 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_usr_secretario3", (object)idSec3 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Creador", creador);

                var pId = cmd.Parameters.Add("@NuevoId", SqlDbType.Int);
                pId.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();

                return Convert.ToInt32(pId.Value);
            }
        }

        public DataTable BuscarPorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_por_id", cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                var dt = new DataTable();

                cn.Open();
                da.Fill(dt);

                return dt;
            }
        }

        public void Actualizar(
            int id,
            string nombre,
            string direccion,
            string tipo,
            string administradorResponsable,
            DateTime fechaConstitucion,
            decimal cuotaMantenimientoBase,
            string emailNotificaciones,
            bool enviarNotifsAlPropietario,
            int? reglamentoDocumentoId,
            int? idPropietario,
            int? idSecretario1,
            int? idSecretario2,
            int? idSecretario3,
            byte[] rowVersion,
            string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 200).Value = (object)nombre ?? DBNull.Value;
                cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar, 300).Value = (object)direccion ?? DBNull.Value;
                cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 30).Value = (object)tipo ?? DBNull.Value;
                cmd.Parameters.Add("@AdministradorResponsable", SqlDbType.NVarChar, 150).Value = (object)administradorResponsable ?? DBNull.Value;
                cmd.Parameters.Add("@FechaConstitucion", SqlDbType.Date).Value = fechaConstitucion;

                var pCuota = cmd.Parameters.Add("@CuotaMantenimientoBase", SqlDbType.Decimal);
                pCuota.Precision = 18;
                pCuota.Scale = 2;
                pCuota.Value = cuotaMantenimientoBase;

                cmd.Parameters.Add("@EmailNotificaciones", SqlDbType.NVarChar, 150).Value = (object)emailNotificaciones ?? DBNull.Value;
                cmd.Parameters.Add("@EnviarNotifsAlPropietario", SqlDbType.Bit).Value = enviarNotifsAlPropietario;
                cmd.Parameters.Add("@ReglamentoDocumentoId", SqlDbType.Int).Value = (object)reglamentoDocumentoId ?? DBNull.Value;
                cmd.Parameters.Add("@ID_propietario", SqlDbType.Int).Value = (object)idPropietario ?? DBNull.Value;
                cmd.Parameters.Add("@ID_usr_secretario1", SqlDbType.Int).Value = (object)idSecretario1 ?? DBNull.Value;
                cmd.Parameters.Add("@ID_usr_secretario2", SqlDbType.Int).Value = (object)idSecretario2 ?? DBNull.Value;
                cmd.Parameters.Add("@ID_usr_secretario3", SqlDbType.Int).Value = (object)idSecretario3 ?? DBNull.Value;
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = (object)rowVersion ?? DBNull.Value;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = (object)editor ?? DBNull.Value;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Desactivar(int id, byte[] rowVersion, string editor)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_desactivar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = (object)rowVersion ?? DBNull.Value;
                cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = (object)editor ?? DBNull.Value;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataRow ObtenerPorId(int id)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_condominio_obtener", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Id", id);

                var dt = new DataTable();
                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public DataTable Buscar(string buscar, bool soloActivos, int top = 20)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_condominio_buscar", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Buscar", (object)buscar ?? DBNull.Value);
                da.SelectCommand.Parameters.AddWithValue("@SoloActivos", soloActivos);
                da.SelectCommand.Parameters.AddWithValue("@Top", top);

                var dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        // --------- BÚSQUEDA DE PROPIETARIOS ----------
        // Ajusta únicamente el nombre del SP si en tu BD se llama distinto.
        public DataTable BuscarPropietarios(string buscar, bool soloActivos, int top = 50)
        {
            using (var cn = new SqlConnection(_cn))
            using (var da = new SqlDataAdapter("dbo.sp_propietario_buscar", cn))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                da.SelectCommand.Parameters.AddWithValue(
                    "@buscar",
                    string.IsNullOrWhiteSpace(buscar) ? (object)DBNull.Value : buscar.Trim()
                );

                da.SelectCommand.Parameters.AddWithValue("@soloActivos", soloActivos);

                var dt = new DataTable();
                da.Fill(dt);

                NormalizarColumnasPropietario(dt);
                return dt;
            }
        }

        private void NormalizarColumnasPropietario(DataTable dt)
        {
            if (dt == null)
                return;

            if (!dt.Columns.Contains("Id"))
            {
                if (dt.Columns.Contains("ID_usr"))
                    AgregarColumnaAlias(dt, "Id", "ID_usr");
                else if (dt.Columns.Contains("IdUsuario"))
                    AgregarColumnaAlias(dt, "Id", "IdUsuario");
                else if (dt.Columns.Contains("ID_propietario"))
                    AgregarColumnaAlias(dt, "Id", "ID_propietario");
            }

            if (!dt.Columns.Contains("Usuario"))
            {
                if (dt.Columns.Contains("usuario"))
                    AgregarColumnaAlias(dt, "Usuario", "usuario");
                else if (dt.Columns.Contains("Nombre"))
                    AgregarColumnaAlias(dt, "Usuario", "Nombre");
                else if (dt.Columns.Contains("NombreCompleto"))
                    AgregarColumnaAlias(dt, "Usuario", "NombreCompleto");
            }

            if (!dt.Columns.Contains("Correo"))
            {
                if (dt.Columns.Contains("Email"))
                    AgregarColumnaAlias(dt, "Correo", "Email");
                else if (dt.Columns.Contains("correo"))
                    AgregarColumnaAlias(dt, "Correo", "correo");
            }
        }

        private void AgregarColumnaAlias(DataTable dt, string nuevaColumna, string columnaOrigen)
        {
            if (dt.Columns.Contains(nuevaColumna) || !dt.Columns.Contains(columnaOrigen))
                return;

            dt.Columns.Add(nuevaColumna, typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                row[nuevaColumna] = row[columnaOrigen] == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(row[columnaOrigen]);
            }
        }

        public void NotificarAccion(int id, string accion, string editor, string mailProfile)
        {
            using (var cn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_condominio_notificar_accion", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Accion", (object)accion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Editor", (object)editor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MailProfile", (object)mailProfile ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class CondominioCrearRequest
    {
        public string Nombre { get; set; }
        public int? ReglamentoDocumentoId { get; set; }
        public string Usuario { get; set; }
    }
}
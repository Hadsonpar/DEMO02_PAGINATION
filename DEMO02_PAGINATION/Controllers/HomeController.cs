using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DEMO02_PAGINATION.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DEMO02_PAGINATION.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration) {
            Configuration = configuration;
        }

        // GET: /<controller>/
        //private int countUsuario()
        //{
        //    int intCount = 0;

        //    string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        string sql = "select count(1) from USUARIO";
        //        SqlCommand command = new SqlCommand(sql, connection);
        //        intCount = Convert.ToInt32(command.ExecuteScalar());
        //        connection.Close();
        //    }
        //    return intCount;
        //}

        public IActionResult Usuario() {
            return View();
        }

        [HttpGet]
        [ActionName("Usuario")]
        public IActionResult UsuarioPage(int page = 1)
        {
            //capturamos el valor PageSize, esta definido como 5
            int PageSize = Convert.ToInt16(Configuration["KeyConfig:PageSize"]);
            //creamos el objeto ViewModel
            UsuarioViewModel _usuarioViewModel = new UsuarioViewModel();
            DataSet ds = new DataSet();
            //listamos los campos de usuario
            List<ClsUsuario> ListclsUsuarios = new List<ClsUsuario>();

            //Conexión a la base de datos
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            //Estoy usando uso de ADO.Net para interactuar con la base de datos, puede usar cualquier otros ORM
            using (SqlConnection connection = new SqlConnection(connectionString))
            //using (SqlConnection con = new SqlConnection(Configuration["ConnectionStrings:DefaultConnection"]))
            {
                connection.Open();
                SqlCommand com = new SqlCommand("GET_USUARIO", connection);
                com.CommandType = CommandType.StoredProcedure;
                //Paso los parametros para seleccionar el rango
                com.Parameters.AddWithValue("@OffsetValue", (page - 1) * PageSize);
                com.Parameters.AddWithValue("@PagingSize", PageSize);
                SqlDataAdapter adapt = new SqlDataAdapter(com);
                //Cargo el conjunto de datos con fill y cierro la conexión a base de datos
                adapt.Fill(ds);
                connection.Close();
                //Ahora asocio los datos de usuario
                //Estamos retornando 2 conjunto de datos, una contiene los datos del usuario y otra contiene la cantidad total de registros existente en la tabla usuario
                if (ds != null && ds.Tables.Count == 2)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ClsUsuario clsUsuario = new ClsUsuario();
                        clsUsuario.id = Convert.IsDBNull(ds.Tables[0].Rows[i]["Id"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["Id"]);
                        clsUsuario.usuario = Convert.IsDBNull(ds.Tables[0].Rows[i]["Usuario"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Usuario"]);
                        clsUsuario.contrasena = Convert.IsDBNull(ds.Tables[0].Rows[i]["Contrasena"]) ? "" : Convert.ToString(ds.Tables[0].Rows[i]["Contrasena"]);
                        clsUsuario.intentos = Convert.IsDBNull(ds.Tables[0].Rows[i]["Intentos"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["Intentos"]);
                        clsUsuario.nivelSeg = Convert.IsDBNull(ds.Tables[0].Rows[i]["NivelSeg"]) ? 0 : Convert.ToDecimal(ds.Tables[0].Rows[i]["NivelSeg"]);
                        clsUsuario.fechaReg = Convert.IsDBNull(ds.Tables[0].Rows[i]["FechaReg"]) ? (DateTime?)null : Convert.ToDateTime(ds.Tables[0].Rows[i]["FechaReg"]);
                        ListclsUsuarios.Add(clsUsuario);
                    }
                    //Pasamos el total de registros para la página actual
                    var pager = new Pager((ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0) ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalRows"]) : 0, page, PageSize);
                    _usuarioViewModel.ListUsuario = ListclsUsuarios;
                    _usuarioViewModel.pager = pager;
                }
            }
            return View(_usuarioViewModel);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            List<ClsUsuario> UsuariosList = new List<ClsUsuario>();

            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString)) {

                connection.Open();

                string sql = "select * from USUARIO";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ClsUsuario clsUsuario = new ClsUsuario();
                        clsUsuario.id = Convert.ToInt32(dataReader["Id"]);
                        clsUsuario.usuario = Convert.ToString(dataReader["Usuario"]);
                        clsUsuario.contrasena = Convert.ToString(dataReader["Contrasena"]);
                        clsUsuario.intentos = Convert.ToInt32(dataReader["Intentos"]);
                        clsUsuario.nivelSeg = Convert.ToDecimal(dataReader["NivelSeg"]);
                        clsUsuario.fechaReg = Convert.ToDateTime(dataReader["FechaReg"]);

                        UsuariosList.Add(clsUsuario);
                    }
                }

                connection.Close();
            }
            return View(UsuariosList);
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClsUsuario clsUsuario)
        {
            if (ModelState.IsValid) {
                string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    string sql = $"Insert Into USUARIO(Usuario, Contrasena, Intentos, NivelSeg) Values('{clsUsuario.usuario}', '{clsUsuario.contrasena}', '{clsUsuario.intentos}', '{clsUsuario.nivelSeg}')";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    return RedirectToAction("Usuario");
                }
            }
            else
            return View();
        }

        public IActionResult Edit(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            ClsUsuario clsUsuario = new ClsUsuario();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * From USUARIO Where Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        clsUsuario.id = Convert.ToInt32(dataReader["Id"]);
                        clsUsuario.usuario = Convert.ToString(dataReader["Usuario"]);
                        clsUsuario.contrasena = Convert.ToString(dataReader["Contrasena"]);
                        clsUsuario.intentos = Convert.ToInt32(dataReader["Intentos"]);
                        clsUsuario.nivelSeg = Convert.ToDecimal(dataReader["NivelSeg"]);
                        clsUsuario.fechaReg = Convert.ToDateTime(dataReader["FechaReg"]);

                    }
                }
                connection.Close();
            }
            return View(clsUsuario);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult Edit_Post(ClsUsuario clsUsuario)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {             
                string sql = $"Update USUARIO SET Usuario='{clsUsuario.usuario}', Contrasena='{clsUsuario.contrasena}', Intentos='{clsUsuario.intentos}', NivelSeg='{clsUsuario.nivelSeg}' Where Id='{clsUsuario.id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Usuario");
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            ClsUsuario clsUsuario = new ClsUsuario();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * From USUARIO Where Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        clsUsuario.id = Convert.ToInt32(dataReader["Id"]);
                        clsUsuario.usuario = Convert.ToString(dataReader["Usuario"]);
                        clsUsuario.contrasena = Convert.ToString(dataReader["Contrasena"]);
                        clsUsuario.intentos = Convert.ToInt32(dataReader["Intentos"]);
                        clsUsuario.nivelSeg = Convert.ToDecimal(dataReader["NivelSeg"]);
                        clsUsuario.fechaReg = Convert.ToDateTime(dataReader["FechaReg"]);
                    }
                }
            }
            return View(clsUsuario);
        }

        [HttpGet]//Lo uso para ver el detalle del registro y para la eliminacion de registro
        public IActionResult Delete(int? id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            ClsUsuario clsUsuario = new ClsUsuario();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * From USUARIO Where Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        clsUsuario.id = Convert.ToInt32(dataReader["Id"]);
                        clsUsuario.usuario = Convert.ToString(dataReader["Usuario"]);
                        clsUsuario.contrasena = Convert.ToString(dataReader["Contrasena"]);
                        clsUsuario.intentos = Convert.ToInt32(dataReader["Intentos"]);
                        clsUsuario.nivelSeg = Convert.ToDecimal(dataReader["NivelSeg"]);
                        clsUsuario.fechaReg = Convert.ToDateTime(dataReader["FechaReg"]);
                    }
                }
            }
            return View(clsUsuario);
        }

        //[HttpPost]//Se puede usar directo, invocando desde el view Index a través de un boton con HttpPost
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Delete From USUARIO Where Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ViewBag.Result = "Error en la operación:" + ex.Message;
                    }
                    connection.Close();
                }
            }

            return RedirectToAction("Usuario");
        }
    }
}
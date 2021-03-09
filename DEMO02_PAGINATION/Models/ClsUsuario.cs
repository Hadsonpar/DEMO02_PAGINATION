using DEMO02_PAGINATION.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DEMO02_PAGINATION.Models
{
    public class ClsUsuario
    {
        public int TotalRecords { get; set; }

        public int id { get; set; }
   
        [Required]
        public string usuario { get; set; }

        [Required]
        [ContrasenaValidate(Allowed = new string[] { "hadson1", "erlita1", "cesar1" }, ErrorMessage = "Contraseña no valida")]
        public string contrasena { get; set; }

        [Range(3, 5)]
        public int intentos { get; set; }

        [Required]
        public decimal nivelSeg { get; set; }

        public DateTime? fechaReg { get; set; }
    }

    public class UsuarioViewModel
    {
        public List<ClsUsuario> ListUsuario { get; set; }
        public Pager pager { get; set; }
    }

    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            // Total de paginación a mostrar
            int _totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            // Pagina actual
            int _currentPage = page != null ? (int)page : 1;
            // Paginación inicia con
            int _startPage = _currentPage - 5;
            // Paginación termina con
            int _endPage = _currentPage + 4;
            if (_startPage <= 0)
            {
                _endPage -= (_startPage - 1);
                _startPage = 1;
            }
            if (_endPage > _totalPages)
            {
                _endPage = _totalPages;
                if (_endPage > 10)
                {
                    _startPage = _endPage - 9;
                }
            }
            // Propiedades de paginación
            TotalItems = totalItems;
            CurrentPage = _currentPage;
            PageSize = pageSize;
            TotalPages = _totalPages;
            StartPage = _startPage;
            EndPage = _endPage;
        }

        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> RegistroVistaDepartamento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            VistaDepartamento departamento =
            await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            return View(departamento);
        }

        public async Task<IActionResult> GrupoVistaDepartamentos(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numPagina = 1;
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            //string html = "<div>";
            //for (int i = 1; i <= numRegistros; i += 2)
            //{
            //    html += "<a href='GrupoVistaDepartamentos?posicion=" + i + "'>Página " + numPagina + " | </a>";
            //    numPagina += 1;
            //}
            //html += "</div>";
            //ViewData["LINKS"] = html;
            ViewData["NUMREGISTROS"] = numRegistros;
            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDepartamentoAsync(posicion.Value);
            return View(departamentos);

        }

        public async Task<IActionResult> GrupoDepartamentos(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numPagina = 1;
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();

            ViewData["NUMREGISTROS"] = numRegistros;
            List<Departamento> departamentos = await this.repo.GetGrupoDepartamentoAsync(posicion.Value);
            return View(departamentos);

        }

        public async Task<IActionResult> PaginarGrupoEmpleados(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int numRegistros = await this.repo.GetEmpleadosCountAsync();
            ViewData["REGISTROS"] = numRegistros;
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosAsync(posicion.Value);
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosOficio(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(posicion.Value, oficio);
            int registros = await this.repo.GetEmpleadosOficioCountAsync(oficio);
            ViewData["REGISTROS"] = registros;
            ViewData["OFICIO"] = oficio;
            return View(empleados);
        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio(string oficio)
        {
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(1, oficio);
            int registros = await this.repo.GetEmpleadosOficioCountAsync(oficio);
            ViewData["REGISTROS"] = registros;
            ViewData["OFICIO"] = oficio;
            return View(empleados);

        }

        public async Task<IActionResult> EmpleadosOficioOut(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(posicion.Value, oficio);
            ViewData["REGISTROS"] = model.NumeroRegistros;
            ViewData["OFICIO"] = oficio;
            return View(model.Empleados);
        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficioOut(string oficio)
        {
            ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(1, oficio);
            ViewData["REGISTROS"] = model.NumeroRegistros;
            ViewData["OFICIO"] = oficio;
            return View(model.Empleados);

        }

        public async Task<IActionResult> Details(int iddep, int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int numRegistros = await this.repo.GetEmpleadosRegistroDepartamentoCountAsync(iddep);
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["POSICION"] = posicion;
            Empleado emp =
            await this.repo.GetEmpleadosRegistroDepartamentoOutAsync(posicion.Value, iddep);
            ViewData["EMPLEADO"] = emp;
            Departamento dep = await this.repo.FindDepartamento(iddep);
            return View(dep);
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]

    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashboardServicio;

        public DashBoardController(IDashBoardService dashboardservicio)
        {
            _dashboardServicio = dashboardservicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashboard> gResponse = new GenericResponse<VMDashboard>();

            try
            {
                VMDashboard vmDashboard = new VMDashboard();

                vmDashboard.TotalVentas = await _dashboardServicio.TotalVentasUltimaSemana();
                vmDashboard.TotalIngresos = await _dashboardServicio.TotalIngresosUltimaSemana();
                vmDashboard.TotalProductos = await _dashboardServicio.TotalProductos();
                vmDashboard.TotalCategorias = await _dashboardServicio.TotalCategorias();

                List<VMVentasSemana> listaVentasSemana = new List<VMVentasSemana>();
                List<VMProductosSemana> listaProductosSemana = new List<VMProductosSemana>();

                foreach (KeyValuePair<string, int> item in await _dashboardServicio.VentasUltimaSemana())
                {
                    listaVentasSemana.Add(new VMVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value,
                    });
                }

                foreach (KeyValuePair<string, int> item in await _dashboardServicio.ProductosTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VMProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value,
                    });
                }

                vmDashboard.VentasUltimaSemana = listaVentasSemana;
                vmDashboard.ProductosTopUltimaSemana = listaProductosSemana;

                gResponse.Estado = true;
                gResponse.Objeto = vmDashboard;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

    }
}

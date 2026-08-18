using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PAV_PF_KendallBonilla_RandyArtavia.Models;

public class VentasController : Controller
{
    private PAV_PF_KendallBonilla_RandyArtaviaEntities db = new PAV_PF_KendallBonilla_RandyArtaviaEntities();

    // GET: Ventas
    public ActionResult Index()
    {
        string perfil = Session["Perfil"]?.ToString();
        int usuarioID = (int)(Session["UsuarioID"] ?? 0);

        IQueryable<Ventas> ventas;

        if (perfil == "Administrador")
        {
            // Mostrar todas las ventas si es Administrador
            ventas = db.Ventas.Include(v => v.Usuarios).Include(v => v.DetalleVentas.Select(d => d.Libros));
        }
        else if (perfil == "Usuario")
        {
            // Mostrar solo las ventas del usuario actual
            ventas = db.Ventas.Include(v => v.Usuarios)
                              .Include(v => v.DetalleVentas.Select(d => d.Libros))
                              .Where(v => v.UsuarioID == usuarioID);
        }
        else
        {
            // Si el perfil no está definido, redirigir a una vista de error o acceso denegado
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Acceso denegado");
        }

        return View(ventas.ToList());
    }


    // GET: Ventas/Create
    public ActionResult Create()
    {
        ViewBag.LibroID = new SelectList(db.Libros, "LibroID", "NombreLibro");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Ventas venta, List<int> LibroID, List<int> Cantidad, decimal TotalVenta)
    {
        try
        {
            // Redondear TotalVenta a dos decimales
            TotalVenta = Math.Round(TotalVenta, 2);

            // Depuración: Imprimir el valor redondeado
            Console.WriteLine($"Valor de TotalVenta redondeado: {TotalVenta}");

            // Validar que TotalVenta no sea menor o igual a cero
            if (TotalVenta <= 0)
            {
                ViewBag.Error = "El total de la venta no puede ser 0 o negativo.";
                ViewBag.LibroID = new SelectList(db.Libros, "LibroID", "NombreLibro");
                return View(venta);
            }

            if (LibroID == null || Cantidad == null || !LibroID.Any() || !Cantidad.Any())
            {
                ViewBag.Error = "Debe seleccionar al menos un libro y su cantidad.";
                ViewBag.LibroID = new SelectList(db.Libros, "LibroID", "NombreLibro");
                return View(venta);
            }

            // Asignar valores adicionales al modelo
            venta.UsuarioID = Convert.ToInt32(Session["UsuarioID"]);
            venta.FechaCompra = DateTime.Now;
            venta.NumeroFactura = GenerarNumeroFacturaUnico();
            venta.TotalVenta = TotalVenta; // Asignar el valor redondeado

            db.Ventas.Add(venta);
            db.SaveChanges();

            // Guardar los detalles de la venta
            for (int i = 0; i < LibroID.Count; i++)
            {
                var libro = db.Libros.Find(LibroID[i]);
                if (libro != null)
                {
                    var detalle = new DetalleVentas
                    {
                        VentaID = venta.VentaID,
                        LibroID = LibroID[i],
                        Cantidad = Cantidad[i],
                        Subtotal = Math.Round(Cantidad[i] * libro.Precio, 2) // Redondear subtotales
                    };
                    db.DetalleVentas.Add(detalle);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Controlar excepciones y mostrar mensajes de error
            ViewBag.Error = "Ocurrió un error al registrar la venta: " + ex.Message;
            Console.WriteLine($"Error en el controlador: {ex}");
        }

        ViewBag.LibroID = new SelectList(db.Libros, "LibroID", "NombreLibro");
        return View(venta);
    }

    // GET: Ventas/Details/5
    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        var venta = db.Ventas.Include(v => v.DetalleVentas.Select(d => d.Libros))
                             .FirstOrDefault(v => v.VentaID == id);

        if (venta == null)
        {
            return HttpNotFound();
        }

        return View(venta);
    }

    [HttpGet]
    public JsonResult GetPrecioLibro(int id)
    {
        var libro = db.Libros.FirstOrDefault(l => l.LibroID == id);
        if (libro == null)
        {
            return Json(new { error = "Libro no encontrado" }, JsonRequestBehavior.AllowGet);
        }

        return Json(new { precio = libro.Precio }, JsonRequestBehavior.AllowGet);
    }

    private string GenerarNumeroFacturaUnico()
    {
        string numeroFactura;
        bool existe;

        do
        {
            // Generar un número alfanumérico de 20 caracteres
            numeroFactura = GenerarAlfanumerico(20);

            // Validar si ya existe en la base de datos
            existe = db.Ventas.Any(v => v.NumeroFactura == numeroFactura);
        } while (existe); // Repetir hasta que no exista un duplicado

        return numeroFactura;
    }

    private string GenerarAlfanumerico(int longitud)
    {
        const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(caracteres, longitud)
                                    .Select(s => s[random.Next(s.Length)]).ToArray());
    }

}
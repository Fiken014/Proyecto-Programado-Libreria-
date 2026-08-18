using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PAV_PF_KendallBonilla_RandyArtavia.Models;


[Authorize] // Requiere que el usuario haya iniciado sesión
public class DetalleVentasController : Controller
{
    private PAV_PF_KendallBonilla_RandyArtaviaEntities db = new PAV_PF_KendallBonilla_RandyArtaviaEntities();

    // Método para verificar si el usuario es administrador
    private bool EsAdministrador()
    {
        return Session["Perfil"] != null && Session["Perfil"].ToString() == "Administrador";
    }

    // GET: DetalleVentas/Index/5
    // Lista todos los detalles de una venta específica
    public ActionResult Index(int? ventaId)
    {
        // Validar que se haya iniciado sesión
        if (Session["UsuarioID"] == null)
        {
            return RedirectToAction("Login", "Usuarios"); // Redirige al login si no hay sesión activa
        }

        // Validar que el usuario sea administrador
        if (!EsAdministrador())
        {
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Acceso denegado: Solo administradores pueden acceder a este módulo.");
        }

        if (ventaId == null) // Verifica que se pase un ID válido
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // Recupera los detalles de la venta especificada
        var detalles = db.DetalleVentas.Include(d => d.Libros)
                                       .Where(d => d.VentaID == ventaId).ToList();


        if (!detalles.Any()) // Verifica si hay detalles para la venta especificada
        {
            ViewBag.Mensaje = "No se encontraron detalles para esta venta.";
        }

        ViewBag.VentaID = ventaId; // Guarda el ID de la venta para referencias en la vista
        return View(detalles); // Devuelve la vista con los detalles
    }

    // GET: DetalleVentas/Create
    // Muestra el formulario para agregar un nuevo detalle de venta
    public ActionResult Create(int ventaId)
    {
        if (Session["UsuarioID"] == null)
        {
            return RedirectToAction("Login", "Usuarios");
        }

        if (!EsAdministrador())
        {
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Acceso denegado: Solo administradores pueden acceder a este módulo.");
        }
        ViewBag.LibroID = new SelectList(db.Libros, "LibroID", "NombreLibro"); // Lista desplegable de libros
        ViewBag.VentaID = ventaId; // Pasa el ID de la venta actual a la vista
        return View();
    }

    // POST: DetalleVentas/Create
    // Agrega un nuevo detalle de venta
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(DetalleVentas detalleVenta)
    {
        if (ModelState.IsValid)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (!EsAdministrador())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Acceso denegado: Solo administradores pueden acceder a este módulo.");
            }

            // Calcula el subtotal basado en la cantidad y el precio del libro
            var libro = db.Libros.Find(detalleVenta.LibroID);
            if (libro != null)
            {
                detalleVenta.Subtotal = detalleVenta.Cantidad * libro.Precio;
                db.DetalleVentas.Add(detalleVenta); // Agrega el detalle a la base de datos
                db.SaveChanges(); // Guarda los cambios
                return RedirectToAction("Index", new { ventaId = detalleVenta.VentaID }); // Redirige al listado de detalles
            }
        }

        // Si hay errores, devuelve los datos para corregirlos
        ViewBag.LibroID = new SelectList(db.Libros, "LibroID", "NombreLibro", detalleVenta.LibroID);
        ViewBag.VentaID = detalleVenta.VentaID;
        return View(detalleVenta);
    }

    // GET: DetalleVentas/Edit/5
    // Muestra el formulario para editar un detalle de venta
    public ActionResult Edit(int? id)
    {
        if (Session["UsuarioID"] == null)
        {
            return RedirectToAction("Login", "Usuarios");
        }

        if (!EsAdministrador())
        {
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Acceso denegado: Solo administradores pueden acceder a este módulo.");
        }

        if (id == null) // Verifica que se pase un ID válido
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        DetalleVentas detalleVenta = db.DetalleVentas.Find(id); // Busca el detalle en la base de datos
        if (detalleVenta == null) // Verifica si el detalle existe
        {
            return HttpNotFound();
        }

        ViewBag.LibroID = new SelectList(db.Libros, "LibroID", "NombreLibro", detalleVenta.LibroID); // Lista desplegable de libros
        return View(detalleVenta); // Devuelve la vista con los datos del detalle
    }

    // POST: DetalleVentas/Edit/5
    // Actualiza un detalle de venta existente
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(DetalleVentas detalleVenta)
    {
        if (Session["UsuarioID"] == null)
        {
            return RedirectToAction("Login", "Usuarios");
        }

        if (!EsAdministrador())
        {
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Acceso denegado: Solo administradores pueden acceder a este módulo.");
        }
        if (ModelState.IsValid)
        {
            // Calcula nuevamente el subtotal basado en los cambios realizados
            var libro = db.Libros.Find(detalleVenta.LibroID);
            if (libro != null)
            {
                detalleVenta.Subtotal = detalleVenta.Cantidad * libro.Precio;
            }

            db.Entry(detalleVenta).State = EntityState.Modified; // Marca el detalle como modificado
            db.SaveChanges(); // Guarda los cambios
            return RedirectToAction("Index", new { ventaId = detalleVenta.VentaID }); // Redirige al listado de detalles
        }

        // Si hay errores, devuelve los datos para corregirlos
        ViewBag.LibroID = new SelectList(db.Libros, "LibroID", "NombreLibro", detalleVenta.LibroID);
        return View(detalleVenta);
    }

    // GET: DetalleVentas/Delete/5
    // Muestra una confirmación para eliminar un detalle de venta
}
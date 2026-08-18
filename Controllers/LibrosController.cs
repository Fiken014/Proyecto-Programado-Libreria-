using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PAV_PF_KendallBonilla_RandyArtavia.Models;

namespace PAV_PF_KendallBonilla_RandyArtavia.Controllers
{

    /// Controlador para gestionar los libros en el sistema.

    public class LibrosController : Controller
    {

        private PAV_PF_KendallBonilla_RandyArtaviaEntities db = new PAV_PF_KendallBonilla_RandyArtaviaEntities();

        /// Muestra una lista de todos los libros en el sistema.
        public ActionResult Index()
        {
            // Obtiene la lista de libros junto con la información de los géneros literarios asociados.
            var libros = db.Libros.Include(l => l.GenerosLiterarios).ToList();
            return View(libros); // Retorna la vista con los libros.
        }

        /// Muestra el formulario para crear un nuevo libro.
        public ActionResult Create()
        {
            // Genera una lista de géneros literarios para un dropdown en la vista.
            ViewBag.GeneroID = new SelectList(db.GenerosLiterarios, "GeneroID", "DescripcionGenero");
            return View(); // Retorna la vista del formulario de creación.
        }

        /// Procesa los datos para agregar un nuevo libro.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CodigoLibro,GeneroID,NombreLibro,Precio")] Libros libro)
        {
            if (ModelState.IsValid) // Valida que los datos enviados sean correctos.
            {
                db.Libros.Add(libro); // Agrega el libro al contexto de datos.
                db.SaveChanges(); // Guarda los cambios en la base de datos.
                return RedirectToAction("Index"); // Redirige a la lista de libros.
            }

            // Si hay un error, se vuelve a generar la lista de géneros para la vista.
            ViewBag.GeneroID = new SelectList(db.GenerosLiterarios, "GeneroID", "DescripcionGenero", libro.GeneroID);
            return View(libro); // Retorna la vista con los datos enviados.
        }


        /// Muestra el formulario para editar un libro existente.
        public ActionResult Edit(int? id)
        {
            if (id == null) // Valida que el ID no sea nulo.
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Retorna un error de solicitud incorrecta.
            }

            Libros libro = db.Libros.Find(id); // Busca el libro por ID.
            if (libro == null) // Verifica si el libro existe.
            {
                return HttpNotFound(); // Retorna un error 404 si no se encuentra.
            }

            // Genera una lista de géneros literarios para un dropdown en la vista.
            ViewBag.GeneroID = new SelectList(db.GenerosLiterarios, "GeneroID", "DescripcionGenero", libro.GeneroID);
            return View(libro); // Retorna la vista del formulario de edición.
        }


        /// Procesa los datos para actualizar un libro existente.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LibroID,CodigoLibro,NombreLibro,GeneroID,Precio")] Libros libro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(libro).State = EntityState.Modified; // Marca el registro como modificado.
                    db.SaveChanges(); // Guarda los cambios.
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationError in ex.EntityValidationErrors)
                    {
                        foreach (var error in validationError.ValidationErrors)
                        {
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                }
            }

            ViewBag.GeneroID = new SelectList(db.GenerosLiterarios, "GeneroID", "DescripcionGenero", libro.GeneroID);
            return View(libro);
        }

        /// Muestra los detalles de un libro específico.
        public ActionResult Details(int? id)
        {
            if (id == null) // Valida que el ID no sea nulo.
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Retorna un error de solicitud incorrecta.
            }

            // Busca el libro por ID y carga la información del género literario relacionado.
            Libros libro = db.Libros.Include(l => l.GenerosLiterarios).FirstOrDefault(l => l.LibroID == id);
            if (libro == null) // Verifica si el libro existe.
            {
                return HttpNotFound(); // Retorna un error 404 si no se encuentra.
            }

            return View(libro); // Retorna la vista con los detalles del libro.
        }


        /// Libera los recursos del controlador, como el contexto de la base de datos.
        protected override void Dispose(bool disposing)
        {
            if (disposing) // Si se deben liberar recursos administrados.
            {
                db.Dispose(); // Libera el contexto de datos.
            }
            base.Dispose(disposing); // Llama al método Dispose de la clase base.
        }
    }
}
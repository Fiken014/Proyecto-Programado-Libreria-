using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PAV_PF_KendallBonilla_RandyArtavia.Models;

namespace PAV_PF_KendallBonilla_RandyArtavia.Controllers
{

    public class GenerosLiterariosController : Controller
    {

        private PAV_PF_KendallBonilla_RandyArtaviaEntities db = new PAV_PF_KendallBonilla_RandyArtaviaEntities();


        public ActionResult Index()
        {
            var generos = db.GenerosLiterarios.ToList(); // Obtiene todos los géneros literarios.
            return View(generos); // Retorna la vista con los datos obtenidos.
        }


        /// Muestra el formulario para la creación de un nuevo género literario.
        public ActionResult Create()
        {
            return View(); // Retorna la vista del formulario de creación.
        }

        /// Procesa la solicitud para crear un nuevo género literario.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CodigoGenero,DescripcionGenero")] GenerosLiterarios genero)
        {
            if (ModelState.IsValid) // Verifica si los datos son válidos.
            {
                db.GenerosLiterarios.Add(genero); // Agrega el género al contexto de datos.
                db.SaveChanges(); // Guarda los cambios en la base de datos.
                return RedirectToAction("Index"); // Redirige al índice de géneros.
            }
            return View(genero); // Si no es válido, retorna la vista con el modelo enviado.
        }

        /// Muestra el formulario para editar un género literario existente.
        public ActionResult Edit(int? id)
        {
            if (id == null) // Valida que el ID no sea nulo.
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Retorna un error de solicitud incorrecta.
            }

            GenerosLiterarios genero = db.GenerosLiterarios.Find(id); // Busca el género por ID.
            if (genero == null) // Verifica que el género exista.
            {
                return HttpNotFound(); // Retorna un error 404 si no se encuentra.
            }
            return View(genero); // Retorna la vista con los datos del género.
        }


        /// Procesa la solicitud para editar un género literario existente.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GeneroID,CodigoGenero,DescripcionGenero")] GenerosLiterarios genero)
        {
            if (ModelState.IsValid) // Verifica si los datos son válidos.
            {
                var generoExistente = db.GenerosLiterarios.Find(genero.GeneroID); // Busca el género por ID.

                if (generoExistente == null) // Verifica si el género existe.
                {
                    return HttpNotFound(); // Retorna un error 404 si no se encuentra.
                }

                // Actualiza solo la descripción del género.
                generoExistente.DescripcionGenero = genero.DescripcionGenero;

                db.Entry(generoExistente).State = EntityState.Modified; // Marca el registro como modificado.
                db.SaveChanges(); // Guarda los cambios en la base de datos.
                return RedirectToAction("Index"); // Redirige al índice de géneros.
            }
            return View(genero); // Si no es válido, retorna la vista con los datos enviados.
        }

        /// Muestra los detalles de un género literario específico.
        public ActionResult Details(int? id)
        {
            if (id == null) // Valida que el ID no sea nulo.
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Retorna un error de solicitud incorrecta.
            }

            GenerosLiterarios genero = db.GenerosLiterarios.Find(id); // Busca el género por ID.
            if (genero == null) // Verifica si el género existe.
            {
                return HttpNotFound(); // Retorna un error 404 si no se encuentra.
            }
            return View(genero); // Retorna la vista con los detalles del género.
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
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PAV_PF_KendallBonilla_RandyArtavia.Models;

public class UsuariosController : Controller
{
    private PAV_PF_KendallBonilla_RandyArtaviaEntities db = new PAV_PF_KendallBonilla_RandyArtaviaEntities();

    // Método GET para mostrar la pantalla de login
    public ActionResult Login()
    {
        return View(); // Devuelve la vista Login
    }

    // Método POST para procesar el login
    [HttpPost]
    [ValidateAntiForgeryToken] // Protege contra ataques CSRF
    public ActionResult Login(string CorreoElectronico, string Contraseña)
    {
        // Validación básica de campos vacíos
        if (string.IsNullOrEmpty(CorreoElectronico) || string.IsNullOrEmpty(Contraseña))
        {
            ViewBag.Error = "Por favor, ingrese su correo electrónico y contraseña.";
            return View();
        }

        // Busca un usuario con las credenciales ingresadas
        var usuario = db.Usuarios
            .FirstOrDefault(u => u.CorreoElectronico == CorreoElectronico && u.Contrasena == Contraseña);

        if (usuario != null)
        {
            // Almacena datos en la sesión para identificar al usuario
            Session["UsuarioID"] = usuario.UsuarioID;
            Session["Perfil"] = usuario.Perfil;
            Session["NombreCompleto"] = usuario.NombreCompleto;

            // Redirige a la página de inicio
            return RedirectToAction("Index", "Home");
        }

        // Si las credenciales son incorrectas, muestra un mensaje de error
        ViewBag.Error = "Credenciales incorrectas.";
        return View();
    }

    // Método GET para mostrar el formulario de registro
    public ActionResult Register()
    {
        return View(); // Devuelve la vista Register
    }

    // Método POST para registrar un nuevo usuario
    [HttpPost]
    [ValidateAntiForgeryToken] // Protege contra ataques CSRF
    public ActionResult Register(Usuarios usuario)
    {
        if (ModelState.IsValid) // Verifica que el modelo cumpla con las validaciones
        {
            usuario.Perfil = "Usuario"; // Asigna el perfil "Usuario" por defecto
            db.Usuarios.Add(usuario); // Agrega el usuario a la base de datos
            db.SaveChanges(); // Guarda los cambios en la base de datos
            return RedirectToAction("Login"); // Redirige al formulario de login
        }
        return View(usuario); // Si hay errores, regresa al formulario de registro
    }

    // Método GET para listar todos los usuarios (solo para administradores)
    public ActionResult Index()
    {
        // Recupera el perfil y el ID del usuario desde la sesión
        string perfil = Session["Perfil"]?.ToString();
        int usuarioID = (int)(Session["UsuarioID"] ?? 0);

        // Filtra los datos según el perfil
        List<Usuarios> usuarios;
        if (perfil == "Administrador")
        {
            usuarios = db.Usuarios.ToList(); // Los administradores ven todos los usuarios
        }
        else if (perfil == "Usuario")
        {
            usuarios = db.Usuarios.Where(u => u.UsuarioID == usuarioID).ToList(); // Usuarios solo ven su información
        }
        else
        {
            return RedirectToAction("Login"); // Si no hay sesión, redirige al login
        }

        return View(usuarios); // Devuelve la lista filtrada
    }

    // Método GET para mostrar el formulario de edición de un usuario
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return RedirectToAction("Login"); // Si no hay sesión, redirige al login
        }

        var usuario = db.Usuarios.Find(id);
        if (usuario == null)
        {
            return HttpNotFound();
        }

        return View(usuario);
    }

    // Método POST para guardar los cambios de un usuario editado
    [HttpPost]
    public ActionResult Edit(Usuarios usuario)
    {
        if (ModelState.IsValid)
        {
            // Solo actualizar campos específicos, excluyendo NumeroIdentificacion
            var usuarioExistente = db.Usuarios.Find(usuario.UsuarioID);

            if (usuarioExistente != null)
            {
                usuarioExistente.NombreCompleto = usuario.NombreCompleto;
                usuarioExistente.CorreoElectronico = usuario.CorreoElectronico;
                usuarioExistente.Genero = usuario.Genero;
                usuarioExistente.TipoTarjeta = usuario.TipoTarjeta;
                usuarioExistente.NumeroTarjeta = usuario.NumeroTarjeta;
                usuarioExistente.Contrasena = usuario.Contrasena;

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        return View(usuario);
    }

    // Método GET para cerrar la sesión del usuario
    public ActionResult Logout()
    {
        Session.Clear(); // Limpia todas las variables de sesión
        return RedirectToAction("Login"); // Redirige al formulario de login
    }
}

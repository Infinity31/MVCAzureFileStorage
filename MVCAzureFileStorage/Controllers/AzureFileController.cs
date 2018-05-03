using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MVCAzureFileStorage.Models;

namespace MVCAzureFileStorage.Controllers{
    public class AzureFileController : Controller{
        ModeloFileAzure modelo;

        public AzureFileController(){
            this.modelo = new ModeloFileAzure();
        }

        public ActionResult SubirFichero(){
            return View();
        }

        [HttpPost]
        public ActionResult SubirFichero(HttpPostedFileBase archivo){
            //GUARDAMOS EL CONTENIDO DEL ARCHIVO EN FORMATO STREAM
            Stream stream = archivo.InputStream;
            //LLAMAMOS A NUESTRO METODO PARA SUBIR FICHEROS A AZURE DE NUESTRO MODELO
            //LE MANDAMOS EL NOMBRE DEL ARCHIVO Y SU CONTENIDO EN FORMATO STREAM
            modelo.SubirFicheroAzure(archivo.FileName, stream);
            //GUARDAMOS EN VIEWBAG UN MENSAJE 
            ViewBag.Correcto = "Fichero subido correctamente";
            return View();
        }

        public ActionResult ArchivosAzure(){
            List<String> archivos = modelo.GetArchivosAzure();
            return View(archivos);
        }

        public ActionResult DatosTXT(String nombrearchivo){
            String contenido = this.modelo.LeerTXT(nombrearchivo);
            ViewBag.Contenido = contenido;
            return View();
        }

        public ActionResult DatosXML(String nombrearchivo){
            List<Libro> libros = modelo.LeerXML(nombrearchivo);
            return View(libros);
        }
    }
}
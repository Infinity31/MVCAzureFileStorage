using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Newtonsoft.Json;

namespace MVCAzureFileStorage.Models{
    public class ModeloFileAzure{
        CloudFileDirectory directorio;
        CloudFile fichero;

        public ModeloFileAzure(){
            //PARA OBTENER ACCESO A LA CUENTA STORAGE DEBEMOS USAR SUS CLAVES
            //PARA ELLO RECUPERAMOS LA CADENA DE CONEXION QUE HEMOS INCLUIDO EN EL Web.config (USAMOS EL VALOR DE KEY)
            String claves = CloudConfigurationManager.GetSetting("storagefile");
            //CON LAS CLAVES ACCEDEMOS A LA CUENTA STORAGE
            CloudStorageAccount cuenta = CloudStorageAccount.Parse(claves);
            //PARA ACCEDER A NUESTRO RECURSO DEBEMOS CREAR UN CLIENTE DEL TIPO DE RECURSO QUE QUERAMOS (EN ESTE CASO FILE)
            CloudFileClient cliente = cuenta.CreateCloudFileClient();
            //CON NUESTRO CLIENTE DE TIPO FILE RECUPERAMOS EL RECURSO COMPARTIDO QUE HEMOS CREADO ANTES LLAMADO FICHEROS
            CloudFileShare recurso = cliente.GetShareReference("ficheros");
            //PARA GUARDAR ARCHIVOS EN LA RAÍZ DE NUESTRO RECURSO ACCEDEMOS AL DIRECTORIO
            this.directorio = recurso.GetRootDirectoryReference();
        }

        public void SubirFicheroAzure(String nombre, Stream contenido){
            //ACCEDEMOS A LA REFERENCIA DEL ARCHIVO POR SU NOMBRE
            fichero = this.directorio.GetFileReference(nombre);
            //ESCRIBIMOS EL CONTENIDO
            fichero.UploadFromStream(contenido);
        }

        public List<String> GetArchivosAzure(){
            List<String> archivos = new List<String>();
            //RECUPERAMOS LOS ARCHIVOS DEL DIRECTORIO
            IEnumerable<IListFileItem> datos = this.directorio.ListFilesAndDirectories();
            //RECORREMOS LOS DATOS
            foreach(IListFileItem item in datos){
                //COGEMOS LA RUTA URI DE CADA RECURSO
                String rutauri = item.Uri.ToString();
                //GUARDAMOS EL NOMBRE DEL ARCHIVO QUE OBTENEMOS DE LA RUTA
                int pos = rutauri.LastIndexOf("/") + 1;
                String nombrearchivo = rutauri.Substring(pos);
                //AÑADIMOS EL NOMBRE A LA COLECCION
                archivos.Add(nombrearchivo);
            }
            return archivos;
        }

        public String LeerTXT(String nombrearchivo){
            //BUSCAMOS EL NOMBRE DE NUESTRO ARCHIVO EN NUESTRO DIRECTORIO
            CloudFile archivo = this.directorio.GetFileReference(nombrearchivo);
            //DESCARGAMOS EL CONTENIDO DEL ARCHIVO COMO TEXTO
            String contenido = archivo.DownloadTextAsync().Result;
            return contenido;
        }

        public List<Libro> LeerXML(String nombrearchivo){
            //CREAMOS UN CONTENIDO EN LA MEMORIA PARA LEER EL DOCUMENTO
            MemoryStream contenido = new MemoryStream();
            //ACCEDEMOS AL FICHERO POR SU NOMBRE
            CloudFile archivo = this.directorio.GetFileReference(nombrearchivo);
            //DESCARGAMOS SU CONTENIDO EN UN STRING
            String datos = archivo.DownloadText(System.Text.Encoding.UTF8);
            //CREAMOS UN NUEVO DOCUMENTO XML A PARTIR DEL STREAM DE AZURE
            XDocument doc = XDocument.Parse(datos);

            //HACEMOS LA CONSULTA PARA OBTENER LA LISTA
            var consulta = from d in doc.Descendants("libro")
                           select new Libro{
                               Nombre = (String)d.Element("nombre"),
                               Autor = (String)d.Element("autor"),
                               NumeroPaginas = (int)d.Element("numeropaginas"),
                               Imagen = (String)d.Element("img"),
                               Capitulos = new List<Capitulo>(
                                   from cap in d.Descendants("capitulo")
                                   select new Capitulo{
                                       NombreCapitulo = cap.Element("nombre").Value,
                                       ImagenCapitulo = cap.Element("imagen").Value
                                   })
                           };
            return consulta.ToList();
        }
    }
}
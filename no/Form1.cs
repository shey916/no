using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json; // libreria para leer y escribir archivos json
using ClosedXML.Excel; // libreria para leer y escribir archivos excel

namespace no
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            lst.View = View.Details; // muestra las columnas
            lst.LabelEdit = true;
            lst.FullRowSelect = true;
            lst.GridLines = true;
            lst.Columns.Add("Nombre", -2, HorizontalAlignment.Left);
            string ruta = AppDomain.CurrentDomain.BaseDirectory; // ruta: guarda la ruta del directorio de la app
                         // obtiene la ruta donde está la app
            DirectoryInfo dir = new DirectoryInfo(ruta); // dir: guarda la informacion 

            // limpia la lista antes de añadir nuevos elementos 
            lst.Items.Clear();
            // muestra solo en el lst solo los archivos json, xlsx y txt
            foreach (var fil in dir.GetFiles("*.json")) 
            {
                lst.Items.Add(fil.Name);
            }
            foreach (var fil in dir.GetFiles("*.txt"))
            {
                lst.Items.Add(fil.Name);
            }
            foreach (var fil in dir.GetFiles("*.xlsx"))
            {
                lst.Items.Add(fil.Name);
            }
        }
        private void btnJson_Click(object sender, EventArgs e)
        {
            string ruta = AppDomain.CurrentDomain.BaseDirectory; // obtiene la ruta de la carpeta donde está la app
            DirectoryInfo dir = new DirectoryInfo(ruta); // crea un objeto que busca archivos/carpetas dentro de esa ruta

            lst.Items.Clear(); // quita los archivos q no son json

            foreach (var fil in dir.GetFiles("*.json"))
            {
                lst.Items.Add(fil.Name); // los muestra en el lst
            }
        }
        private void btnTexto_Click(object sender, EventArgs e)
        {
            string ruta = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dir = new DirectoryInfo(ruta);

            lst.Items.Clear();

            foreach (var fil in dir.GetFiles("*.txt"))
            {
                lst.Items.Add(fil.Name);
            }
        }
        private void btnExcel_Click(object sender, EventArgs e)
        {
            string ruta = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dir = new DirectoryInfo(ruta);

            lst.Items.Clear();

            foreach (var fil in dir.GetFiles("*.xlsx"))
            {
                lst.Items.Add(fil.Name);
            }
        }
        private void btnConvertir_Click(object sender, EventArgs e)
        {
            if (lst.SelectedItems.Count == 0) // esto es para ver si hay un archivo seleccionado
            {
                MessageBox.Show("Favor de seleccionar un archivo."); // si no lo hay muestra este mensaje
                return;
            }

            string fileName = lst.SelectedItems[0].Text; // fileName: nombre del archivo seleccionado
            string ruta = AppDomain.CurrentDomain.BaseDirectory; // ruta: carpeta de la app
            string fullPath = Path.Combine(ruta, fileName); // fullPath: ruta completa 

            if (fileName.EndsWith(".json")) // si es json lo convierte a xlsx
            {
                // lee el JSON como texto
                string jsonContent = File.ReadAllText(fullPath);

                // lo convierte a una lista de diccionarios 
                var lista = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonContent);

                if (lista == null || lista.Count == 0) // si no esta selecciondo el archivo o si esta mal escrito,
                // tiene una condicional lo que quiere decir que una de esas dos cosas es cierta
                {
                    MessageBox.Show("El JSON está vacío o mal formado.");
                    return;
                }
                // crea un libro de excel
                var workbook = new ClosedXML.Excel.XLWorkbook(); // crea el libro
                var hoja = workbook.Worksheets.Add("Datos"); // crea la hoja

                // aqui agrega las columnas
                int col = 1;
                foreach (var encabezado in lista[0].Keys)
                {
                    hoja.Cell(1, col).Value = encabezado;
                    col++;
                }
                // agrega los datos por fila
                int fila = 2;
                foreach (var item in lista)
                {
                    col = 1;
                    foreach (var valor in item.Values)
                    {
                        hoja.Cell(fila, col).Value = valor;
                        col++;
                    }
                    fila++;
                }
                string nuevoArchivo = Path.Combine(ruta, Path.GetFileNameWithoutExtension(fileName) + "_convertido.xlsx"); // Construimos la ruta completa donde se guardará el nuevo archivo excel, obtiene el nombre sin su extencion
                workbook.SaveAs(nuevoArchivo); // Escribimos el contenido de 'excel' 
                MessageBox.Show("Convertido a Excel."); // guardo el archivo con otro nombre
            }
            else if (fileName.EndsWith(".xlsx"))
            {
                // Leer Excel
                var workbook = new ClosedXML.Excel.XLWorkbook(fullPath); // carga primero el excel existente
                var hoja = workbook.Worksheet(1);

                var rango = hoja.RangeUsed(); // obtiene el rango de las celdas con los datos
                if (rango == null)
                {
                    MessageBox.Show("El archivo Excel está vacío.");
                    return;
                } 
                int filas = rango.RowCount(); // cuenta las filas y las columnas
                int columnas = rango.ColumnCount();
                // lee los encabezados
                List<string> encabezados = new List<string>();
                for (int c = 1; c <= columnas; c++) //  recorre desde 1 hasta columnas
                {
                    encabezados.Add(hoja.Cell(1, c).GetString()); // lee el contenido de una celda en la fila 1
                    // y despues lo conviterte a una cadena de texto con el GetString y por ultimo lo guarda el lista encabezados
                }
                // lee los datos por fila
                List<Dictionary<string, string>> lista = new List<Dictionary<string, string>>();
                for (int f = 2; f <= filas; f++) // recorre todas las filas de la hoja de Excel desde la 2
                {
                    // diccionario temporal para almacenar los datos de la fila actual
                    Dictionary<string, string> filaDatos = new Dictionary<string, string>();
                    for (int c = 1; c <= columnas; c++) //  recorre todas las columnas de la fila actual
                    {
                        string encabezado = encabezados[c - 1]; // obtenemos el nombre del encabezado que esta en columna actual
                        string valor = hoja.Cell(f, c).GetString(); // obtenemos valor de la celda en la fila 'f' y columna 'c' y convertimos a string
                        filaDatos[encabezado] = valor; // luego se añade al diccionario
                    }
                    lista.Add(filaDatos); // luego se la agrega fila completa con sus valores
                }
                string jsonNuevo = JsonConvert.SerializeObject(lista, Formatting.Indented);
                string nuevoArchivo = Path.Combine(ruta, Path.GetFileNameWithoutExtension(fileName) + "_convertido.json"); // Construimos la ruta completa donde se guardará el nuevo archivo JSON, obtiene el nombre sin su extencion
                File.WriteAllText(nuevoArchivo, jsonNuevo); // Escribimos el contenido de 'jsonNuevo' en un archivo de texto
                MessageBox.Show("Convertido a JSON."); // Si el archivo no existe, lo crea; si ya existe, lo sobrescribe
            }
            else // si es otro archivo q no sea excel o xlsx
            {
                MessageBox.Show("El archivo debe ser .json o .xlsx.");
            }
        }
        public class Alumno // primero definimos como estructuraremos los daros de un alumno
        {
            public string Nombre { get; set; }
            public string Matricula { get; set; }
            public string Carrera { get; set; }
        }
        private void btnar_Click(object sender, EventArgs e)
        {

        {
            if (lst.SelectedItems.Count == 0) // checa si se selecciono un archivo
            {
                // si no se selecciono nada aparece el siguiente mensaje
                MessageBox.Show("Favor de seleccionar un archivo en el ListView.");
                return;
            }
            // obtiene el nombre y ruta del archivo
            string fileName = lst.SelectedItems[0].Text;
            string ruta = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(ruta, fileName);

            // crea un objeto (alumno) con los datos ingresaods 
            Alumno nuevoAlumno = new Alumno
            {
                Nombre = txtNombre.Text,
                Matricula = txtMatricula.Text,
                Carrera = txtCarrera.Text
                
            };

            if (fileName.EndsWith(".json")) // aqui lo guarda SOLO si el nombre termina con .json
            {
                // Leer JSON existente
                List<Alumno> lista;
                if (File.Exists(fullPath)) // verifica si el archivo especificado por ruta ya existe 
                    {
                    string json = File.ReadAllText(fullPath); // lee todo el contenido del archivo JSON existente como una sola cadena y lo almacena en la variable
                    lista = JsonConvert.DeserializeObject<List<Alumno>>(json) ?? new List<Alumno>();
                        // (convertimos) la cadena JSON 'json' a una lista de objetos de tipo Alumno
                        // luego intenta leer el JSON y convertirlo en una lista
                        // devuelve null (por ejemplo, si 'json' está vacío o mal formado),
                        // entonces se asigna una nueva lista vacía para evitar errores 
                    }
                    else // Si no se cumple la condición anterior 
                         // entonces se inicializa 'lista' como una nueva lista vacía de Alumno.
                    {
                        lista = new List<Alumno>();
                }

                lista.Add(nuevoAlumno); // agrega el registro

                string nuevoJson = JsonConvert.SerializeObject(lista, Formatting.Indented); // hace que sea mas facil d leer si se abre con un editor d txt
                File.WriteAllText(fullPath, nuevoJson); // escribe la cadena en el archivo especificado fullPath

                MessageBox.Show("Registro guardado en JSON.");
            }
            else if (fileName.EndsWith(".txt")) // esto es por si no se cumple la primera condicion
            {
                // crea una variable d cadena llamada registro
                string registro = $"{nuevoAlumno.Nombre},{nuevoAlumno.Matricula},{nuevoAlumno.Carrera}"; // los datos son separados por comas
                File.AppendAllText(fullPath, registro + Environment.NewLine); // añade texto a un archivo existente o lo crea si no existe
                MessageBox.Show("Registro guardado en archivo de texto.");
            }
            else if (fileName.EndsWith(".xlsx")) // condicional
            {
                var workbook = new ClosedXML.Excel.XLWorkbook(fullPath); // abre el archivo de Excel ubicado en fullPath
                var hoja = workbook.Worksheet(1); //aqui se obtiene la primera hoja de calculo

                int fila = hoja.LastRowUsed().RowNumber() + 1; //  busca la fila que contiene datos, obtiene el número de fila real de la última fila utilizada
                // se colocan los datos registrados en la hoja de excel 
                hoja.Cell(fila, 1).Value = nuevoAlumno.Nombre;
                hoja.Cell(fila, 2).Value = nuevoAlumno.Matricula;
                hoja.Cell(fila, 3).Value = nuevoAlumno.Carrera;

                workbook.SaveAs(fullPath); // guarda el libro
                MessageBox.Show("Registro guardado en Excel.");
            }
            else 
            {
                MessageBox.Show("Archivo no compatible para guardar registro.");
            }
        }
            txtNombre.Clear(); // limpia las cajas de texto luego de guardar la informacion
            txtMatricula.Clear();
            txtCarrera.Clear();
        }
        private void btnbuscar_Click(object sender, EventArgs e)
        {
            string BusquedaArchivo = txtb.Text.Trim(); // obtiene lo q escribimos en el txt
            if (String.IsNullOrEmpty(BusquedaArchivo)) // verifica q si haya texto
            {
                MessageBox.Show("Ingrese el nombre del archivo que desea buscar. ");
                return;
            }
            string ruta = AppDomain.CurrentDomain.BaseDirectory;
            // obtiene la lista de los q coinciden
            DirectoryInfo dir = new DirectoryInfo(ruta);

            // obtiene todos los archivos del directorio actual y los filtra
            FileInfo[] archivosCoincidentes = dir.GetFiles()
            // filtra los archivos
            .Where(f => f.Name.StartsWith(BusquedaArchivo, StringComparison.OrdinalIgnoreCase))
            .ToArray(); // el resultado t lo convierte en un array
            

            lst.Items.Clear();
            foreach (FileInfo archivo in archivosCoincidentes)
            {
                lst.Items.Add(archivo.Name);
            }
            if (lst.Items.Count == 0) // muestra un mensaje si no se encontraron archivos
            {
                MessageBox.Show("No se encontraron archivos que coincidan.");
            }
            txtb.Clear(); // limpia el txt de busqueda
        }
        private void lst_Click(object sender, EventArgs e)
        {
            // Limpiamos el TextBox antes de mostrar nuevo contenido
            txtbuscar.Clear();
            // Obtenemos la ruta del archivo seleccionado en el ListView
            string rutaArchivo = lst.SelectedItems[0].Text;
            // Obtenemos la extensión del archivo y la convertimos a minúsculas para comparación
            string extension = Path.GetExtension(rutaArchivo).ToLower();

            if (extension == ".xlsx")
            {
                // Si es un archivo Excel (.xlsx)
                using (var libro = new ClosedXML.Excel.XLWorkbook(rutaArchivo))
                {
                    // Seleccionamos la primera hoja del libro
                    var hoja = libro.Worksheet(1);

                    // Recorremos todas las filas usadas en la hoja
                    foreach (var fila in hoja.RowsUsed())
                    {
                        // Recorremos todas las celdas usadas en cada fila
                        foreach (var celda in fila.CellsUsed())
                        {
                            // Mostramos el valor de la celda en el TextBox, separado por espacio
                            txtbuscar.AppendText(celda.Value.ToString() + "  ");
                        }
                        // Al terminar cada fila, agregamos un salto de línea
                        txtbuscar.AppendText(Environment.NewLine);
                    }
                }
            }
            else
            {
                // Si es un archivo de texto plano (.txt, .json, etc.)
                StreamReader fichero = File.OpenText(rutaArchivo);
                string lectura = "";

                // Mientras no se llegue al final del archivo
                while (!fichero.EndOfStream)
                {
                    // Leemos cada línea del archivo
                    lectura = fichero.ReadLine();
                    // se muestra el contenido e ¿n el textvox
                    txtbuscar.AppendText(lectura + Environment.NewLine);
                }
                // Cerramos el archivo
               // fichero.Close();
            }

        }
    }
}
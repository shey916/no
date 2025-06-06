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
            lst.View = View.Details; // 
            lst.LabelEdit = true;
            lst.FullRowSelect = true;
            lst.GridLines = true;
            lst.Columns.Add("Nombre", -2, HorizontalAlignment.Left);
            string ruta = AppDomain.CurrentDomain.BaseDirectory; // grd-rt-dir-dap
            DirectoryInfo dir = new DirectoryInfo(ruta); // grd-inf

            // 
            lst.Items.Clear();
            // 
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
            string ruta = AppDomain.CurrentDomain.BaseDirectory; // obtrt-dla-cpt-dnd-ela-ap
            DirectoryInfo dir = new DirectoryInfo(ruta); // crobj-q-bs-archycar-dntd-rt

            lst.Items.Clear(); // 

            foreach (var fil in dir.GetFiles("*.json"))
            {
                lst.Items.Add(fil.Name); // 
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
            if (lst.SelectedItems.Count == 0) // 
            {
                MessageBox.Show("Favor de seleccionar un archivo."); // 
                return;
            }

            string fileName = lst.SelectedItems[0].Text; // nm-archsel
            string ruta = AppDomain.CurrentDomain.BaseDirectory; // crp-d-lap
            string fullPath = Path.Combine(ruta, fileName); // rt.comp

            if (fileName.EndsWith(".json")) // 
            {
                string jsonContent = File.ReadAllText(fullPath); // le.to.cont.d.arch grd.cd.tx(jsonContent)

                // lconv-lst.d-dic
                var lista = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonContent);

                if (lista == null || lista.Count == 0) // 
                {
                    MessageBox.Show("El JSON está vacío o mal formado.");
                    return;
                }
                // 
                var workbook = new ClosedXML.Excel.XLWorkbook(); // 
                var hoja = workbook.Worksheets.Add("Datos"); // 

                // agcol
                int col = 1;
                foreach (var encabezado in lista[0].Keys)
                {
                    hoja.Cell(1, col).Value = encabezado;
                    col++;
                }
                // agdt*fl
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
                string nuevoArchivo = Path.Combine(ruta, Path.GetFileNameWithoutExtension(fileName) + "_convertido.xlsx"); // cons.rt.comp.dnd.grd.nv.archex - ob.nom.sin.ext
                workbook.SaveAs(nuevoArchivo); //esc-con.d.ex y sgrd
                MessageBox.Show("Convertido a Excel."); 
            }
            else if (fileName.EndsWith(".xlsx"))
            {
                // leex
                var workbook = new ClosedXML.Excel.XLWorkbook(fullPath); // crg1.ex.exis
                var hoja = workbook.Worksheet(1); // obt/hj/dl.arch/ex

                var rango = hoja.RangeUsed(); // ob.rng.d.cel-q.cont los-dt
                if (rango == null) // 
                {
                    MessageBox.Show("El archivo Excel está vacío.");
                    return;
                } 
                int filas = rango.RowCount(); // cnt.fl.clm
                int columnas = rango.ColumnCount();
                // le.enc
                List<string> encabezados = new List<string>(); // cr-lst.vaci-y.grd.ele.d.msm.tip
                for (int c = 1; c <= columnas; c++) // rcr.dsd1.hst.col
                {
                    encabezados.Add(hoja.Cell(1, c).GetString()); // le.con.d.celC-y-fl1
                    // dsp.conv.cd-tx(gtstr)-y.grd.e.lst.d.en
                }
                //  l.dt*fl
                List<Dictionary<string, string>> lista = new List<Dictionary<string, string>>();
                for (int f = 2; f <= filas; f++) // rc.to.fl-d-hj.d.ex.dsdla2
                {
                    // cre.un.dic-tem-p alm.dts.dla.fl.act
                    Dictionary<string, string> filaDatos = new Dictionary<string, string>();
                    for (int c = 1; c <= columnas; c++) // rec.to.col.d.fl.act
                    {
                        string encabezado = encabezados[c - 1]; // ob.nom.de.enc-q-est-en-col.act
                        string valor = hoja.Cell(f, c).GetString(); // obt.val.d.cel.en.fil"F"ycol"c"
                        filaDatos[encabezado] = valor; // lg.añ.a.dic
                    }
                    lista.Add(filaDatos); // lg.agg.fil.com.c.val
                }
                string jsonNuevo = JsonConvert.SerializeObject(lista, Formatting.Indented);
                string nuevoArchivo = Path.Combine(ruta, Path.GetFileNameWithoutExtension(fileName) + "_convertido.json"); //  const.rt.com.dnd.gu.nv.archjsn.obt.nom.sin.ext
                File.WriteAllText(nuevoArchivo, jsonNuevo); // esc.cont.d.jsnv.en.arch-tx
                MessageBox.Show("Convertido a JSON."); 
            }
            else //
            {
                MessageBox.Show("El archivo debe ser .json o .xlsx.");
            }
        }
        public class Alumno // 
        {
            public string Nombre { get; set; }
            public string Matricula { get; set; }
            public string Carrera { get; set; }
        }
        private void btnar_Click(object sender, EventArgs e)
        {

        {
            if (lst.SelectedItems.Count == 0) // 
            {
                //
                MessageBox.Show("Favor de seleccionar un archivo en el ListView.");
                return;
            }
            // ob.nom-y-rt.d.arch
            string fileName = lst.SelectedItems[0].Text;
            string ruta = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(ruta, fileName);

            // cr,obj,dts,ing
            Alumno nuevoAlumno = new Alumno
            {
                Nombre = txtNombre.Text,
                Matricula = txtMatricula.Text,
                Carrera = txtCarrera.Text
                
            };

            if (fileName.EndsWith(".json")) // aqui lo guarda SOLO si el nombre termina con .json
            {
                // le.jsn.exis
                List<Alumno> lista;
                if (File.Exists(fullPath)) // ver.arch.esp.*rt.exis
                    {
                    string json = File.ReadAllText(fullPath); // le.tdo.conte.arch.jsnexis.grd.cont.var.ti-stri(json)
                    lista = JsonConvert.DeserializeObject<List<Alumno>>(json) ?? new List<Alumno>();
                        //  conv.cdnjson.a.un.lst.objt,tpalu
                        // lg.int.le.json.conv.en.lst
                    }
                    else // // se.inic.lst.cmo.nva.lst.vac-d-alu
                    {
                        lista = new List<Alumno>();
                }

                lista.Add(nuevoAlumno); // 

                string nuevoJson = JsonConvert.SerializeObject(lista, Formatting.Indented); // hac.+.fcl.d.le.s.se.ab.cn.edtxt
                File.WriteAllText(fullPath, nuevoJson); // esc.cdn.tx.arch.espe

                MessageBox.Show("Registro guardado en JSON.");
            }
            else if (fileName.EndsWith(".txt")) // 
            {
                // cr,var,cdn(rgistro)
                string registro = $"{nuevoAlumno.Nombre},{nuevoAlumno.Matricula},{nuevoAlumno.Carrera}"; // 
                File.AppendAllText(fullPath, registro + Environment.NewLine); // añ,tx,arch-ex
                MessageBox.Show("Registro guardado en archivo de texto.");
            }
            else if (fileName.EndsWith(".xlsx")) //
            {
                var workbook = new ClosedXML.Excel.XLWorkbook(fullPath); // abr.arch.ex.ub.flpth
                var hoja = workbook.Worksheet(1); //  obt.1.hj

                int fila = hoja.LastRowUsed().RowNumber() + 1; // bcs.fl.cont.dt  ob.#.fl.d.la.ult-fl-ut
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
            txtNombre.Clear(); // limpia las cajas 
            txtMatricula.Clear();
            txtCarrera.Clear();
        }
        private void btnbuscar_Click(object sender, EventArgs e)
        {
            string BusquedaArchivo = txtb.Text.Trim(); // obt.lo.q.esc.en.tx
            if (String.IsNullOrEmpty(BusquedaArchivo)) // ver.si.hay.txt
            {
                MessageBox.Show("Ingrese el nombre del archivo que desea buscar. ");
                return;
            }
            string ruta = AppDomain.CurrentDomain.BaseDirectory;
            // obt.lst.d.coinc
            DirectoryInfo dir = new DirectoryInfo(ruta);

            // obt.to.arch.d-dir-act.y.ls-fltr
            FileInfo[] archivosCoincidentes = dir.GetFiles()
            // filtra los archivos fltr-arch
            .Where(f => f.Name.StartsWith(BusquedaArchivo, StringComparison.OrdinalIgnoreCase))
            .ToArray(); // el resultado t lo convierte en un array
            

            lst.Items.Clear();
            foreach (FileInfo archivo in archivosCoincidentes)
            {
                lst.Items.Add(archivo.Name);
            }
            if (lst.Items.Count == 0) // si no se encuentran arch
            {
                MessageBox.Show("No se encontraron archivos que coincidan.");
            }
            txtb.Clear(); // 
        }
        private void lst_Click(object sender, EventArgs e)
        {
            //
            txtbuscar.Clear();
            // obt.rt.dl.arch.sel.en-lst
            string rutaArchivo = lst.SelectedItems[0].Text;
            // obt.ext.d.arch-y-conv.a.min.for.comp
            string extension = Path.GetExtension(rutaArchivo).ToLower();

            if (extension == ".xlsx") {
                // Si es un archivo Excel (.xlsx)
                using (var libro = new ClosedXML.Excel.XLWorkbook(rutaArchivo))
                {
                    // sel.1.hj.dl.lb
                    var hoja = libro.Worksheet(1);

                    // a rcr.to.fl.usd.en-hj
                    foreach (var fila in hoja.RowsUsed())
                    {
                        // rcr.to.clds.usd.en.cd.fl
                        foreach (var celda in fila.CellsUsed())
                        {
                            // mstr.val.d.cld.en.txb,,,,sprd*esp
                            txtbuscar.AppendText(celda.Value.ToString() + "  ");
                        }
                        // Al terminar cada fila, agregamos un salto de línea
                        txtbuscar.AppendText(Environment.NewLine);
                    }
                }
            }
            else {
                // Si es un archivo de texto plano (.txt, .json, etc.)
                StreamReader fichero = File.OpenText(rutaArchivo);
                string lectura = "";

                // while no-llg.,al.fnl.arch
                while (!fichero.EndOfStream)
                {
                    // lee.cd.li.d.arch
                    lectura = fichero.ReadLine();
                    // s.mstr.cont.e-txtbox
                    txtbuscar.AppendText(lectura + Environment.NewLine);
                }
                // Cerramos el archivo   //fichero.Close();
            }
        }
    }}
using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPWinForm_Equipo20A
{
    public partial class Form2 : Form
    {
        private Articulo articulo = null;
        public Form2()
        {
            InitializeComponent();
            Text = "Carga de datos";
            btnAgrModif.Text = "Agregar";
        }

        public Form2(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Edicion de datos";
            btnAgrModif.Text = "Modificar";
        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {
            //convertir en titulo "Modificar" o "Agregar" dependiendo del botón que se haya presionado en el form1
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CategoriaNegocio categorianegocio = new CategoriaNegocio();
            MarcaNegocio marcanegocio = new MarcaNegocio();

            try
            {
                cboCategoria.DataSource = categorianegocio.listar();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";
                cboMarca.DataSource = marcanegocio.listar();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";

                if (articulo != null)
                {
                    txtCodigo.Text=articulo.Codigo;
                    txtNombre.Text=articulo.Nombre;

                    if(articulo.Imagenes != null)
                    {
                        foreach (Imagen img in articulo.Imagenes)
                        {
                            cboImagenVistaPrevia.Items.Add(img.UrlImagen);
                        }
                        if(cboImagenVistaPrevia.Items.Count > 0)
                            cboImagenVistaPrevia.SelectedIndex = 0;
                    }
                    

                    cboCategoria.SelectedValue = articulo.Categoria.Id;
                    cboMarca.SelectedValue = articulo.Marca.Id;
                    txtPrecio.Text=articulo.Precio.ToString();
                    txtDescripcion.Text=articulo.Descripcion;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void btnAgrModif_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if(articulo ==  null)
                    articulo = new Articulo();
                articulo.Codigo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Imagenes.Clear();
                foreach (var item in cboImagenVistaPrevia.Items)
                {
                    Imagen aux = new Imagen();
                    aux.UrlImagen = item.ToString();
                    articulo.Imagenes.Add(aux);
                }
                articulo.Categoria = (Categoria)cboCategoria.SelectedItem;
                articulo.Marca = (Marca)cboMarca.SelectedItem;
                articulo.Precio = decimal.Parse(txtPrecio.Text);
                articulo.Descripcion = txtDescripcion.Text;
                
                if(articulo.Id == 0)
                {
                    negocio.agregar(articulo);
                    MessageBox.Show("Articulo agregado con exito!");
                }
                else
                {
                    negocio.modificar(articulo);
                    MessageBox.Show("Articulo modificado con exito!");
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            string url = txtUrlImagen.Text;
            if (!string.IsNullOrEmpty(txtUrlImagen.Text))
            {
                cboImagenVistaPrevia.Items.Add(url);
                cboImagenVistaPrevia.SelectedIndex = cboImagenVistaPrevia.Items.Count - 1;
                txtUrlImagen.Clear();
            }
        }

        private void cboImagenVistaPrevia_SelectedIndexChanged(object sender, EventArgs e)
        {
            string url = cboImagenVistaPrevia.SelectedItem.ToString();
            cargarImagen(url);
        }

        private void cargarImagen(string url)
        {
            try
            {
                pbImagen.Load(url);
            }
            catch (Exception)
            {
                pbImagen.Load("https://img.ridingwarehouse.com/watermark/rs.php?path=-1.jpg&nw=455");
            }
        }

        private void lblAgregado_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cboImagenVistaPrevia.SelectedIndex != -1)
            {
                int indice = cboImagenVistaPrevia.SelectedIndex;
                cboImagenVistaPrevia.Items.RemoveAt(indice);
                cargarImagen("");
                if (cboImagenVistaPrevia.Items.Count > 0)
                {
                    cboImagenVistaPrevia.SelectedIndex = 0;
                }
            }
            else
            {
                MessageBox.Show("No hay imagenes para eliminar");
            }
        }
    }
}

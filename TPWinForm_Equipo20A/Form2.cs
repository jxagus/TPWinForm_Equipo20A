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
        public Form2()
        {
            InitializeComponent();
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
                cboMarca.DataSource = marcanegocio.listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void btnAgrModif_Click(object sender, EventArgs e)
        {
            Articulo nuevo = new Articulo();
            ArticuloNegocio articulo = new ArticuloNegocio();
            try
            {
                nuevo.Codigo = txtCodigo.Text;
                nuevo.Nombre = txtNombre.Text;
                //nuevo.ImagenUrl = txtUrlImagen.Text;
                nuevo.Categoria = (Categoria)cboCategoria.SelectedItem;
                nuevo.Marca = (Marca)cboMarca.SelectedItem;
                nuevo.Precio = decimal.Parse(txtPrecio.Text);
                nuevo.Descripcion = txtDescripcion.Text;
                
                articulo.agregar(nuevo);
                MessageBox.Show("Articulo agregado con exito");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            try
            {
                pbImagen.Load(txtUrlImagen.Text);
            }
            catch(Exception ex) 
            {
                pbImagen.Load("https://media.istockphoto.com/id/1055079680/vector/black-linear-photo-camera-like-no-image-available.jpg?s=612x612&w=0&k=20&c=P1DebpeMIAtXj_ZbVsKVvg-duuL0v9DlrOZUvPG6UJk=");
            }
        }
    }
}

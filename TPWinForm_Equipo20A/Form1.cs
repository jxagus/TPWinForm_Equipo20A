using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace TPWinForm_Equipo20A
{
    public partial class Form1 : Form

    {
        private List<Articulo> listaArticulo;

        public Form1()
        {
            InitializeComponent();
        }

        private void dgvLista_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();
        }

        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();

            listaArticulo = negocio.listar();
            dgvLista.DataSource = listaArticulo;

            if (listaArticulo.Count > 0 &&
                listaArticulo[0].Imagenes != null &&
                listaArticulo[0].Imagenes.Count > 0)
            {
                pbImagen.Load(listaArticulo[0].Imagenes[0].UrlImagen);
            }

            ocultarColumnas();
        }
        private void ocultarColumnas()
        {
            dgvLista.Columns["Id"].Visible = false;
            //dgvLista.Columns["ImagenUrl"].Visible = false; //ocultamos la url           
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Form2 agregar = new Form2();
            agregar.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvLista.CurrentRow.DataBoundItem;

            ImagenNegocio img = new ImagenNegocio();
            seleccionado.Imagenes = img.listar(seleccionado.Id);

            Form2 modificar = new Form2(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnBorrarFiltro_Click(object sender, EventArgs e)
        {
            cargar();
        }

        private void tbBuscar_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada;
            string filtro = tbBuscar.Text;

            if (filtro.Length >= 2)
            {
                listaFiltrada = listaArticulo.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaArticulo;
            }


            dgvLista.DataSource = null;
            dgvLista.DataSource = listaFiltrada;
            ocultarColumnas();
        }
    }
}

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

namespace TpIntegrador
{
    public partial class fmrCatalogo : Form
    {
        private List<Articulo> listaArticulo;
        public fmrCatalogo()
        {
            InitializeComponent();
        }

        private void fmrCatalogo_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripcion");
            cboCampo.Items.Add("Precio");
        }

        private void dgvCatalogo_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvCatalogo.CurrentRow != null)
            {
                Articulo seleccionado =(Articulo)dgvCatalogo.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.ImagenUrl);
            }
        }

        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                listaArticulo = negocio.listar();
                dgvCatalogo.DataSource = listaArticulo;
                ocultarColumnas();
                cargarImagen(listaArticulo[0].ImagenUrl);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            dgvCatalogo.Columns["ImagenUrl"].Visible = false;
            dgvCatalogo.Columns["Id"].Visible = false;
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulo.Load(imagen);
            }
            catch (Exception ex)
            {

                pbxArticulo.Load("https://www.came-educativa.com.ar/upsoazej/2022/03/placeholder-4.png");
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvCatalogo.CurrentRow.DataBoundItem;

            frmAltaArticulo modificar = new frmAltaArticulo(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            try
            {
                if(dgvCatalogo.CurrentRow != null)
                {
                    DataGridViewRow seleccionado =dgvCatalogo.SelectedRows[0];
                    string CodigoArticulo = seleccionado.Cells["CodigoArticulo"].Value.ToString();

                    string Nombre = seleccionado.Cells["Nombre"].Value.ToString();
                    string Descripcion = seleccionado.Cells["Descripcion"].Value.ToString();
                    string Marcas = seleccionado.Cells["Marcas"].Value.ToString();
                    string Categoria = seleccionado.Cells["Categoria"].Value.ToString();
                    string Precio = seleccionado.Cells["Precio"].Value.ToString();
                    string precioBorrado = string.Format("{0:N2}", Convert.ToDecimal(Precio));

                    string detalle = "Categoría: " + seleccionado.Cells["Categoria"].Value.ToString() + "\r\n" + "Marcas: " + seleccionado.Cells["Marcas"].Value.ToString() + "\r\n" + "Código de artículo " + CodigoArticulo + "\r\n" + "Nombre: " + seleccionado.Cells["Nombre"].Value.ToString() + "\r\n" + "Descripción: " + seleccionado.Cells["Descripcion"].Value.ToString() + "\r\n" + "Precio: $" + precioBorrado;

                    //string detalle = "Categoría: " + Categoria + "\r\n" + "Marca: " + Marcas + "\r\n" + "Código de artículo " + CodigoArticulo + "\r\n" + "Nombre: " + Nombre + "\r\n" + "Descripción: " + Descripcion + "\r\n" + "Precio: $" + precioBorrado;
                    //lblDetalle.Text = detalle;
                    txtDetalles.Text = detalle; 

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;
            try
            {
                DialogResult respuestas = MessageBox.Show("¿Desea eliminar este artículo?", "Elinando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if(respuestas == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvCatalogo.CurrentRow.DataBoundItem;
                    negocio.eliminar(seleccionado.Id);
                    cargar();

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void btnFiltroRapido_Click(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada;
            string filtro = txtFiltroRapido.Text;

            if (filtro != "")
            {
                listaFiltrada = listaArticulo.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Marcas.Descripcion.ToUpper().Contains(filtro.ToUpper()) || x.Categoria.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaArticulo;
            }  

            dgvCatalogo.DataSource = null;
            dgvCatalogo.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if(opcion == "Precio")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione un campo para filtrar.");
                return true;
            }
            if(cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione un criterio para filtrar.");
                return true;
            }
            if(cboCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar un precio");
                    return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Por favor, ingrese solo números");
                     return true;

                }

            }

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        private void btnFiltroAvanzado_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();    
            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvCatalogo.DataSource = negocio.filtrar(campo, criterio, filtro);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}

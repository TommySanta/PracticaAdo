using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using PracticaAdo.Models;
using PracticaAdo.Repositories;

#region PROCEDIMIENTOS ALMACENADOS 
/*
create procedure SP_ALL_CLIENTES
as
	select * from clientes
go

*/

/*
CREATE PROCEDURE SP_GET_INFO_CLIENTE
    @nombreEmpresa NVARCHAR(255),
    @CodigoCliente NVARCHAR(50) OUTPUT,
    @Empresa NVARCHAR(255) OUTPUT,
    @Contacto NVARCHAR(255) OUTPUT,
    @Cargo NVARCHAR(255) OUTPUT,
    @Ciudad NVARCHAR(255) OUTPUT,
    @Telefono NVARCHAR(15) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        @CodigoCliente = CodigoCliente, 
        @Empresa = Empresa, 
        @Contacto = Contacto, 
        @Cargo = Cargo, 
        @Ciudad = Ciudad, 
        @Telefono = CAST(Telefono AS NVARCHAR)
    FROM clientes
    WHERE Empresa = @nombreEmpresa;
    SELECT 
        CodigoPedido, 
        CodigoCliente, 
        FechaEntrega, 
        FormaEnvio, 
        Importe
    FROM pedidos
    WHERE CodigoCliente = @CodigoCliente;
END;
*/

/*
CREATE PROCEDURE SP_GET_INFO_PEDIDO
    @codigoPedido NVARCHAR(50)
AS
BEGIN
    SELECT 
        CodigoPedido, 
        FechaEntrega, 
        FormaEnvio, 
        Importe
    FROM 
        pedidos
    WHERE 
        CodigoPedido = @codigoPedido;
END

*/
#endregion
namespace Test
{
    public partial class FormPractica : Form
    {
        RepositoryClientesPedidos repository;
        public FormPractica()
        {
            InitializeComponent();
            this.repository = new RepositoryClientesPedidos();
            this.LoadClientes();
        }
        private async void LoadClientes()
        {
            List<string> clientes = await this.repository.GetClientes();
            clientes = clientes.Distinct().ToList();
            this.cmbclientes.Items.Clear();
            foreach (string c in clientes)
            {
                this.cmbclientes.Items.Add(c);
            }
        }
        private async void cmbclientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cliente cliente = await this.repository.GetClienteInfo(this.cmbclientes.SelectedItem.ToString());
            this.lstpedidos.Items.Clear();
            foreach (Pedido p in cliente.Pedidos)
            {
                this.lstpedidos.Items.Add(p.CodigoPedido);
            }

            this.txtempresa.Text = cliente.Empresa;
            this.txtcontacto.Text = cliente.Contacto;
            this.txtcargo.Text = cliente.Cargo;
            this.txtciudad.Text = cliente.Ciudad;
            this.txttelefono.Text = cliente.Telefono;
        }

        private async void btnnuevopedido_Click(object sender, EventArgs e)
        {
            string codigoPedido = this.txtcodigopedido.Text;
            string fechaEntrega = this.txtfechaentrega.Text;
            string formaEnvio = this.txtformaenvio.Text;
            string importe = this.txtimporte.Text;

            bool pedidoAgregado = await this.repository.AgregarPedido(codigoPedido, fechaEntrega, formaEnvio, importe);
            if (pedidoAgregado)
            {
                MessageBox.Show("Pedido agregado con éxito.");
            }
            else
            {
                MessageBox.Show("Hubo un error al agregar el pedido.");
            }
        }

        private async void btneliminarpedido_Click(object sender, EventArgs e)
        {
            string codigoPedido = this.txtcodigopedido.Text;

            bool pedidoEliminado = await this.repository.EliminarPedido(codigoPedido);
            if (pedidoEliminado)
            {
                MessageBox.Show("Pedido eliminado con éxito.");
            }
            else
            {
                MessageBox.Show("Hubo un error al eliminar el pedido.");
            }
            this.LoadClientes();
            this.LoadClientes();
        }

        private async void lstpedidos_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pedido pedido = await this.repository.GetPedidoInfo(this.lstpedidos.SelectedItem.ToString());
            this.txtcodigopedido.Text = pedido.CodigoPedido;
            this.txtfechaentrega.Text = pedido.FechaEntrega;
            this.txtformaenvio.Text = pedido.FormaEnvio;
            this.txtimporte.Text = pedido.Importe;
        }
    }
}

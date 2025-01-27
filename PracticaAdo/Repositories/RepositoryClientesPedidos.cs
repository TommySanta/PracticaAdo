using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoFinalAdoNet.Helpers;
using PracticaAdo.Models;

namespace PracticaAdo.Repositories
{
    public class RepositoryClientesPedidos
    {
        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;
        public string mensaje;
        public RepositoryClientesPedidos() 
        {
            this.cn = new SqlConnection(HelperConfig.GetStringConnection());
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
            this.mensaje = "";
            this.cn.InfoMessage += Cn_InfoMessage;
        }
        public async Task<List<string>> GetClientes()
        {
            List<string> clientes = new List<string>();
            string sql = "SP_ALL_CLIENTES";

            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();

            while (await this.reader.ReadAsync())
            {
                clientes.Add(this.reader["Empresa"].ToString());
            }

            await this.cn.CloseAsync();
            await this.reader.CloseAsync();

            return clientes;
        }
        public async Task<Cliente> GetClienteInfo(string nombreEmpresa)
        {
            List<Pedido> pedidos = new List<Pedido>();
            Cliente cliente = new Cliente();

            string sql = "SP_GET_INFO_CLIENTE";

            this.com.Parameters.AddWithValue("@nombreEmpresa", nombreEmpresa);

            SqlParameter paramCodigoCliente = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                ParameterName = "@CodigoCliente",
                Size = 50
            };
            this.com.Parameters.Add(paramCodigoCliente);

            SqlParameter paramEmpresa = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                ParameterName = "@Empresa",
                Size = 255
            };
            this.com.Parameters.Add(paramEmpresa);

            SqlParameter paramContacto = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                ParameterName = "@Contacto",
                Size = 255
            };
            this.com.Parameters.Add(paramContacto);

            SqlParameter paramCargo = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                ParameterName = "@Cargo",
                Size = 255
            };
            this.com.Parameters.Add(paramCargo);

            SqlParameter paramCiudad = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                ParameterName = "@Ciudad",
                Size = 255
            };
            this.com.Parameters.Add(paramCiudad);

            SqlParameter paramTelefono = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                ParameterName = "@Telefono",
                Size = 15
            };
            this.com.Parameters.Add(paramTelefono);

            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();

            while (await this.reader.ReadAsync())
            {
                Pedido pedido = new Pedido
                {
                    CodigoPedido = this.reader["CodigoPedido"].ToString(),
                    CodigoCliente = this.reader["CodigoCliente"].ToString(),
                    FechaEntrega = this.reader["FechaEntrega"].ToString(),
                    FormaEnvio = this.reader["FormaEnvio"].ToString(),
                    Importe = this.reader["Importe"].ToString()
                };

                pedidos.Add(pedido);
            }

            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();

            cliente.CodigoCliente = paramCodigoCliente.Value.ToString();
            cliente.Empresa = paramEmpresa.Value.ToString();
            cliente.Contacto = paramContacto.Value.ToString();
            cliente.Cargo = paramCargo.Value.ToString();
            cliente.Ciudad = paramCiudad.Value.ToString();
            cliente.Telefono = paramTelefono.Value.ToString();
            cliente.Pedidos = pedidos;

            return cliente;
        }
        public async Task<Pedido> GetPedidoInfo(string codigoPedido)
        {
            Pedido pedido = new Pedido();

            string sql = "SP_GET_INFO_PEDIDO";

            this.com.Parameters.AddWithValue("@codigoPedido", codigoPedido);

            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();

            if (await this.reader.ReadAsync())
            {
                pedido.CodigoPedido = this.reader["CodigoPedido"].ToString();
                pedido.FechaEntrega = this.reader["FechaEntrega"].ToString();
                pedido.FormaEnvio = this.reader["FormaEnvio"].ToString();
                pedido.Importe = this.reader["Importe"].ToString();
            }

            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();

            return pedido;
        }
        public async Task<bool> AgregarPedido(string codigoPedido, string fechaEntrega, string formaEnvio, string importe)
        {
           
           string sql = "INSERT INTO PEDIDOS (CodigoPedido, FechaEntrega, FormaEnvio, Importe) VALUES (@CodigoPedido, @FechaEntrega, @FormaEnvio, @Importe)";
           SqlParameter pamCodigoPedido = new SqlParameter("@CodigoPedido", codigoPedido);
           SqlParameter pamFechaEntrega = new SqlParameter("@FechaEntrega", fechaEntrega);
           SqlParameter pamFormaEnvio = new SqlParameter("@FormaEnvio", formaEnvio);
           SqlParameter pamImporte = new SqlParameter("@Importe", importe);

           this.com.Parameters.Add(pamCodigoPedido);
           this.com.Parameters.Add(pamFechaEntrega);
           this.com.Parameters.Add(pamFormaEnvio);
           this.com.Parameters.Add(pamImporte);

           this.com.CommandType = CommandType.Text;
           this.com.CommandText = sql;

           await this.cn.OpenAsync();
           int filasAfectadas = await this.com.ExecuteNonQueryAsync();
           await this.cn.CloseAsync();

           this.com.Parameters.Clear();

           return filasAfectadas > 0;
            
        }
        public async Task<bool> EliminarPedido(string codigoPedido)
        {
            string sql = "DELETE FROM PEDIDOS WHERE CodigoPedido = @CodigoPedido";
            SqlParameter pamCodigoPedido = new SqlParameter("@CodigoPedido", codigoPedido);
            this.com.Parameters.Add(pamCodigoPedido);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            int filasAfectadas = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
            return filasAfectadas > 0;  
           
        }

        private void Cn_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            this.mensaje = e.Message;
        }
    }
}

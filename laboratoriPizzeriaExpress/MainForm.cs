/*
 * Pizzería Campus Express - Gestión de pedidos con Queue y Stack
 * Compatible con SharpDevelop 4.4 / .NET Framework 2.0+
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using laboratoriPizzeriaCampusExpress;

//Agregar una segunda cola de 'pedidos premium' tomar prioridad sobre los normales,usa dos colas y un metodo atender_siguiente que primero es verificar si hay clientes premium

namespace laboratoriPizzeriaCampusExpress

{

    public partial class MainForm : Form

    {

        // Colecciones principales: FIFO para pedidos, LIFO para bitácora

        private Queue<string> colaPedidos = new Queue<string>();
        private Queue<string> colaPremium = new Queue<string>();

        private Stack<string> pilaBitacora = new Stack<string>();


        public MainForm()

        {

            InitializeComponent();

            ActualizarUI();

        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            // Soluciona el error del compilador
        }


        // PASO 1: Nuevo pedido (FIFO entrada)

        private void BtnNuevoPedido_Click(object sender, EventArgs e)

        {

            string cliente = txtCliente.Text.Trim();


            // Validar entrada

            if (cliente == "")

            {

                lblEstado.Text = string.Format("⚠️ Debe ingresar un nombre de cliente.");

                return;

            }


            // Agregar a la cola según prioridad del nombre
            if (cliente.StartsWith("*"))
            {
                colaPremium.Enqueue(cliente);
                pilaBitacora.Push(string.Format("PREMIUM: {0}", cliente));
                lblEstado.Text = string.Format("✅ Pedido PREMIUM registrado para {0}", cliente);
            }
            else
            {
                colaPedidos.Enqueue(cliente);
                pilaBitacora.Push(string.Format("PEDIDO: {0}", cliente));
                lblEstado.Text = string.Format("✅ Pedido registrado para {0}", cliente);
            }


            // Limpiar campo y actualizar

            txtCliente.Clear();

            ActualizarUI();

        }

        // Método requerido por la guía para verificar prioridad
        private string atender_siguiente()
        {
            if (colaPremium.Count > 0) return colaPremium.Dequeue();
            return colaPedidos.Count > 0 ? colaPedidos.Dequeue() : null;
        }


        // PASO 2: Entregar pedido (FIFO salida)

        private void BtnEntregar_Click(object sender, EventArgs e)

        {

            if (colaPedidos.Count == 0 && colaPremium.Count == 0)

            {

                lblEstado.Text = string.Format("❌ No hay pedidos pendientes.");

                return;

            }


            string cliente = atender_siguiente();

            pilaBitacora.Push(string.Format("ENTREGADO: {0}", cliente));

            lblEstado.Text = string.Format("🍕 Pedido entregado a {0}", cliente);

            ActualizarUI();

        }


        // PASO 3: Deshacer última acción (LIFO + lógica de reversión)

        private void BtnDeshacer_Click(object sender, EventArgs e)

        {

            if (pilaBitacora.Count == 0)

            {

                lblEstado.Text = string.Format("📭 No hay acciones para deshacer.");

                return;

            }


            string ultimaAccion = pilaBitacora.Pop();


            if (ultimaAccion.StartsWith("PEDIDO:"))

            {

                // Extraer nombre del cliente

                string nombre = ultimaAccion.Replace("PEDIDO: ", "").Trim();

                // Reconstruir cola excluyendo ese pedido

                string[] temporal = colaPedidos.ToArray();

                colaPedidos.Clear();

                foreach (string p in temporal)

                {

                    if (p != nombre)

                        colaPedidos.Enqueue(p);

                }

                lblEstado.Text = string.Format("↩️ Se deshizo el pedido de {0}", nombre);

            }

            else if (ultimaAccion.StartsWith("PREMIUM:"))
            {
                string nombre = ultimaAccion.Replace("PREMIUM: ", "").Trim();
                string[] temporal = colaPremium.ToArray();
                colaPremium.Clear();
                foreach (string p in temporal)
                {
                    if (p != nombre)
                        colaPremium.Enqueue(p);
                }
                lblEstado.Text = string.Format("↩️ Se deshizo el pedido PREMIUM de {0}", nombre);
            }

            else if (ultimaAccion.StartsWith("ENTREGADO:"))

            {

                // Extraer nombre del cliente

                string nombre = ultimaAccion.Replace("ENTREGADO: ", "").Trim();

                // Volver a encolar según corresponda
                if (nombre.StartsWith("*"))
                    colaPremium.Enqueue(nombre);
                else
                    colaPedidos.Enqueue(nombre);

                lblEstado.Text = string.Format("↩️ Se deshizo la entrega a {0}", nombre);

            }

            else

            {

                lblEstado.Text = string.Format("⚠️ Acción desconocida en bitácora.");

            }


            ActualizarUI();

        }


        // PASO 4: Limpiar todo (reiniciar sistema)

        private void BtnLimpiar_Click(object sender, EventArgs e)

        {

            colaPedidos.Clear();
            colaPremium.Clear();

            pilaBitacora.Clear();

            lblEstado.Text = string.Format("🧹 Sistema reiniciado.");

            ActualizarUI();

        }


        // Sincronizar la interfaz con el estado actual

        private void ActualizarUI()

        {

            // Limpiar listas visuales

            lstPedidos.Items.Clear();

            lstBitacora.Items.Clear();


            // Mostrar cola de pedidos premium primero
            foreach (string p in colaPremium)
                lstPedidos.Items.Add(p);

            // Mostrar cola de pedidos normales

            foreach (string p in colaPedidos)

                lstPedidos.Items.Add(p);

            if (colaPedidos.Count == 0 && colaPremium.Count == 0)

                lstPedidos.Items.Add("(Sin pedidos pendientes)");


            // Mostrar bitácora (pila)

            foreach (string accion in pilaBitacora)

                lstBitacora.Items.Add(accion);

            if (pilaBitacora.Count == 0)

                lstBitacora.Items.Add("(Sin acciones registradas)");


            // Actualizar contador

            lblContador.Text = string.Format("Pedidos: {0} | Premium: {1} | Bitácora: {2}",

                colaPedidos.Count, colaPremium.Count, pilaBitacora.Count);

            

           

        }

    }

}
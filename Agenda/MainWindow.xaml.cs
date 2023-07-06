using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agenda
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private string operacao;
        public MainWindow()
        {
            InitializeComponent();
            txtNome.Focus();
            AlteraBotoes(1);
        }


        #region Buttons Actions
        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {

            if (ValidaCampos())
            {
                if (operacao == "Inserir")
                {
                    contato c = new contato();
                    c.nome = txtNome.Text;
                    c.email = txtEmail.Text;
                    c.telefone = txtTelefone.Text;

                    using (agendaEntities ctx = new agendaEntities())
                    {
                        ctx.contato.Add(c);
                        ctx.SaveChanges();
                    }

                    MessageBox.Show("Contado cadastrado");

                }
            }


            if (operacao == "Alterar")
            {
                using (agendaEntities ctx = new agendaEntities())
                {
                    contato c = ctx.contato.Find(Convert.ToInt32(txtId.Text));
                    {
                        c.nome = txtNome.Text;
                        c.email = txtEmail.Text;
                        c.telefone = txtTelefone.Text;
                        ctx.SaveChanges();
                        MessageBox.Show("Dados alterados com sucesso!");

                    }
                }
            }

            this.ListarDados();
            this.AlteraBotoes(1);
            this.LimpaDados();
        }
        private void btnInserir_Click(object sender, RoutedEventArgs e)
        {
            this.operacao = "Inserir";
            this.AlteraBotoes(2);
            txtId.IsEnabled = false;
            this.LimpaDados();
        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.AlteraBotoes(1);
            this.LimpaDados();
        }
        private void btnLocalizar_Click(object sender, RoutedEventArgs e)
        {
            if (txtId.Text.Trim().Count() > 0)
            {
                //busca pelo codigo
                try
                {
                    int id = Convert.ToInt32(txtId.Text);
                    using (agendaEntities ctx = new agendaEntities())
                    {
                        contato c = ctx.contato.Find(id);
                        dgDados.ItemsSource = new contato[1] { c };
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (txtNome.Text.Trim().Count() > 0)
            {
                try
                {
                    using (agendaEntities ctx = new agendaEntities())
                    {
                        var consulta = from c in ctx.contato
                                       where c.nome.Contains(txtNome.Text)
                                       select c;
                        dgDados.ItemsSource = consulta.ToList();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            if (txtEmail.Text.Trim().Count() > 0)
            {
                try
                {
                    using (agendaEntities ctx = new agendaEntities())
                    {
                        var consulta = from c in ctx.contato
                                       where c.email.Contains(txtEmail.Text)
                                       select c;
                        dgDados.ItemsSource = consulta.ToList();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            if (txtTelefone.Text.Trim().Count() > 0)
            {
                try
                {
                    using (agendaEntities ctx = new agendaEntities())
                    {
                        var consulta = from c in ctx.contato
                                       where c.telefone.Contains(txtTelefone.Text)
                                       select c;
                        dgDados.ItemsSource = consulta.ToList();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {
            this.operacao = "Alterar";
            this.AlteraBotoes(2);
            txtId.IsEnabled = false;
        }
        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            using (agendaEntities ctx = new agendaEntities())
            {
                contato c = ctx.contato.Find(Convert.ToInt32(txtId.Text));
                {
                    if (c != null)
                    {
                        ctx.contato.Remove(c);
                        ctx.SaveChanges();
                        MessageBox.Show("Contato excluido com sucesso!");
                    }
                    this.LimpaDados();
                    this.ListarDados();
                    this.AlteraBotoes(1);


                }
            }
        }
        #endregion


        #region Methods
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ListarDados();
        }
        private void ListarDados()
        {
            using (agendaEntities ctx = new agendaEntities())
            {
                int a = ctx.contato.Count();
                qtdContatos.Content = "Total: " + a.ToString();
                var consulta = ctx.contato;
                dgDados.ItemsSource = consulta.ToList();
            }
        }
        private void AlteraBotoes(int op)
        {
            btnAlterar.IsEnabled = false;
            btnInserir.IsEnabled = false;
            btnExcluir.IsEnabled = false;
            btnCancelar.IsEnabled = false;
            btnLocalizar.IsEnabled = false;
            btnSalvar.IsEnabled = false;

            if (op == 1)
            {
                //ativa opções iniciais
                btnInserir.IsEnabled = true;
                btnLocalizar.IsEnabled = true;
            }
            if (op == 2)
            {
                //inserir um valor
                btnCancelar.IsEnabled = true;
                btnSalvar.IsEnabled = true;
            }
            if (op == 3)
            {
                btnAlterar.IsEnabled = true;
                btnExcluir.IsEnabled = true;
            }
        }
        private void LimpaDados()
        {
            txtId.Clear();
            txtNome.Clear();
            txtEmail.Clear();
            txtTelefone.Clear();
            txtNome.Focus();
        }
        private bool ValidaCampos()
        {
            if (txtNome.Text != "" && txtEmail.Text != "" && txtTelefone.Text != "")
            {
                return true;
            }
            else
            {
                MessageBox.Show("Por favor, preencher todos os campos!");
                return false;
            }
        }
        private void dgDados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgDados.SelectedIndex >= 0)
            {
                contato c = (contato)dgDados.SelectedItem;
                txtId.Text = c.id.ToString();
                txtNome.Text = c.nome;
                txtEmail.Text = c.email;
                txtTelefone.Text = c.telefone;
                this.AlteraBotoes(3);
            }
        }
        #endregion
    }
}

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JogoDaVelhaFrontend
{
    public partial class Form1 : Form
    {
        // Importações da DLL do C
        [DllImport("jogo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void iniciar_jogo();

        [DllImport("jogo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void finalizar_jogo();

        [DllImport("jogo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int preenche_pos(int jogador, int linha, int coluna);

        [DllImport("jogo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_posicao(int linha, int coluna);

        [DllImport("jogo.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int verificar_vitoria();

        // Controles da Tela de Login
        private Panel panelLogin;
        private TextBox txtJogador1;
        private TextBox txtJogador2;
        private Button btnIniciar;

        // Controles da Tela do Jogo
        private Panel panelJogo;
        private Label lblTurno;
        private Label[,] visualTabuleiro = new Label[3, 3];
        private TextBox txtLinha;
        private TextBox txtColuna;
        private Button btnJogar;

        // Estado do Frontend
        private string nomeJogador1;
        private string nomeJogador2;
        private int jogadorAtual = 1; // 1 = X, 2 = O

        // Paleta de Cores do Tema (PETEEL + Dark Mode)
        private Color corFundo = Color.FromArgb(25, 25, 25);       // Preto/Chumbo Escuro
        private Color corAmareloPeteel = Color.FromArgb(255, 215, 0); // Amarelo Ouro
        private Color corAzulContraste = Color.FromArgb(0, 200, 255); // Azul Ciano para o Jogador O
        private Color corTexto = Color.WhiteSmoke;                 // Branco gelo para textos
        private Color corCaixaTexto = Color.FromArgb(45, 45, 45);  // Cinza escuro para inputs

        public Form1()
        {
            this.Text = "Jogo da Velha - PETEEL";
            this.Size = new Size(400, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = corFundo; // Aplica o fundo escuro na janela inteira
            
            ConstruirInterfaceGrafica();
        }

        private void ConstruirInterfaceGrafica()
        {
            // --- MONTAGEM DO PAINEL DE LOGIN ---
            panelLogin = new Panel { Dock = DockStyle.Fill };
            
            Label lblTitulo = new Label { 
                Text = "JOGO DA VELHA", 
                Location = new Point(80, 40), 
                AutoSize = true, 
                Font = new Font("Segoe UI Black", 18, FontStyle.Bold),
                ForeColor = corAmareloPeteel
            };
            
            Label lblJ1 = new Label { Text = "Jogador 1 (X):", Location = new Point(50, 110), AutoSize = true, ForeColor = corTexto, Font = new Font("Segoe UI", 10) };
            txtJogador1 = new TextBox { Location = new Point(160, 107), Width = 170, BackColor = corCaixaTexto, ForeColor = corAmareloPeteel, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            
            Label lblJ2 = new Label { Text = "Jogador 2 (O):", Location = new Point(50, 160), AutoSize = true, ForeColor = corTexto, Font = new Font("Segoe UI", 10) };
            txtJogador2 = new TextBox { Location = new Point(160, 157), Width = 170, BackColor = corCaixaTexto, ForeColor = corAzulContraste, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            
            btnIniciar = new Button { 
                Text = "INICIAR PARTIDA", 
                Location = new Point(100, 230), 
                Width = 180, 
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                BackColor = corAmareloPeteel,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnIniciar.FlatAppearance.BorderSize = 0;
            btnIniciar.Click += BtnIniciar_Click;

            panelLogin.Controls.AddRange(new Control[] { lblTitulo, lblJ1, txtJogador1, lblJ2, txtJogador2, btnIniciar });

            // --- MONTAGEM DO PAINEL DE JOGO ---
            panelJogo = new Panel { Dock = DockStyle.Fill, Visible = false };
            
            lblTurno = new Label { Location = new Point(45, 20), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = corAmareloPeteel };

            // Desenhando o tabuleiro com estilo Dark
            for (int l = 0; l < 3; l++)
            {
                for (int c = 0; c < 3; c++)
                {
                    visualTabuleiro[l, c] = new Label
                    {
                        Bounds = new Rectangle(110 + (c * 60), 70 + (l * 60), 55, 55),
                        BorderStyle = BorderStyle.FixedSingle,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Comic Sans MS", 26, FontStyle.Bold), // Fonte mais lúdica para o jogo
                        BackColor = Color.FromArgb(35, 35, 35),
                        ForeColor = corTexto
                    };
                    panelJogo.Controls.Add(visualTabuleiro[l, c]);
                }
            }

            // Entradas de Linha e Coluna ajustadas para o usuário (1-3)
            Label lblLinha = new Label { Text = "Linha (1-3):", Location = new Point(60, 280), AutoSize = true, ForeColor = corTexto, Font = new Font("Segoe UI", 10) };
            txtLinha = new TextBox { Location = new Point(140, 277), Width = 40, BackColor = corCaixaTexto, ForeColor = corTexto, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = HorizontalAlignment.Center };

            Label lblColuna = new Label { Text = "Coluna (1-3):", Location = new Point(195, 280), AutoSize = true, ForeColor = corTexto, Font = new Font("Segoe UI", 10) };
            txtColuna = new TextBox { Location = new Point(290, 277), Width = 40, BackColor = corCaixaTexto, ForeColor = corTexto, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = HorizontalAlignment.Center };

            btnJogar = new Button { 
                Text = "FAZER JOGADA", 
                Location = new Point(110, 340), 
                Width = 160, 
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                BackColor = corAmareloPeteel,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnJogar.FlatAppearance.BorderSize = 0;
            btnJogar.Click += BtnJogar_Click;

            panelJogo.Controls.AddRange(new Control[] { lblTurno, lblLinha, txtLinha, lblColuna, txtColuna, btnJogar });

            this.Controls.Add(panelLogin);
            this.Controls.Add(panelJogo);
        }

        private void BtnIniciar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtJogador1.Text) || string.IsNullOrWhiteSpace(txtJogador2.Text))
            {
                MessageBox.Show("Os nomes dos jogadores não podem estar vazios!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            nomeJogador1 = txtJogador1.Text.Trim();
            nomeJogador2 = txtJogador2.Text.Trim();

            iniciar_jogo(); 
            jogadorAtual = 1;
            
            AtualizarInterfaceDoJogo();
            
            panelLogin.Visible = false;
            panelJogo.Visible = true;
        }

        private void BtnJogar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLinha.Text, out int linhaDigitada) || !int.TryParse(txtColuna.Text, out int colunaDigitada))
            {
                MessageBox.Show("Digite números válidos para a linha e coluna.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // O PADRÃO ADAPTER: Converte a entrada do usuário (1 a 3) para o índice da matriz no C (0 a 2)
            int linhaBackend = linhaDigitada - 1;
            int colunaBackend = colunaDigitada - 1;

            // Envia a coordenada já adaptada para a DLL
            int resultado = preenche_pos(jogadorAtual, linhaBackend, colunaBackend);

            if (resultado == -1)
            {
                MessageBox.Show("Jogada inválida! Verifique se a posição está entre 1 e 3 e se já não está ocupada.", "Movimento Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLinha.Clear(); txtColuna.Clear(); txtLinha.Focus();
                return;
            }

            AtualizarInterfaceDoJogo();
            int statusVitoria = verificar_vitoria();

            if (statusVitoria != 0) 
            {
                FinalizarRodada(statusVitoria);
            }
            else 
            {
                jogadorAtual = (jogadorAtual == 1) ? 2 : 1;
                AtualizarInterfaceDoJogo();
            }

            txtLinha.Clear();
            txtColuna.Clear();
            txtLinha.Focus();
        }

        private void AtualizarInterfaceDoJogo()
        {
            string nomeDaVez = (jogadorAtual == 1) ? nomeJogador1 : nomeJogador2;
            string simboloDaVez = (jogadorAtual == 1) ? "X" : "O";
            
            lblTurno.Text = $"Vez de: {nomeDaVez} ({simboloDaVez})";
            // Muda a cor do título dependendo de quem é a vez para dar um feedback visual legal
            lblTurno.ForeColor = (jogadorAtual == 1) ? corAmareloPeteel : corAzulContraste;

            for (int l = 0; l < 3; l++)
            {
                for (int c = 0; c < 3; c++)
                {
                    int estado = get_posicao(l, c);
                    if (estado == 1) 
                    {
                        visualTabuleiro[l, c].Text = "X";
                        visualTabuleiro[l, c].ForeColor = corAmareloPeteel;
                    }
                    else if (estado == 2) 
                    {
                        visualTabuleiro[l, c].Text = "O";
                        visualTabuleiro[l, c].ForeColor = corAzulContraste;
                    }
                    else 
                    {
                        visualTabuleiro[l, c].Text = "";
                    }
                }
            }
        }

        private void FinalizarRodada(int status)
        {
            string mensagem;
            if (status == 1) mensagem = $"🏆 {nomeJogador1} venceu a partida!";
            else if (status == 2) mensagem = $"🏆 {nomeJogador2} venceu a partida!";
            else mensagem = "O jogo terminou em EMPATE (Deu velha)!";

            DialogResult resposta = MessageBox.Show(
                mensagem + "\n\nDesejam jogar novamente com os mesmos jogadores?\n(Clique em 'Não' para trocar os nomes dos jogadores)", 
                "Fim da Partida", 
                MessageBoxButtons.YesNoCancel, 
                MessageBoxIcon.Information);

            if (resposta == DialogResult.Yes)
            {
                iniciar_jogo(); 
                jogadorAtual = 1;
                AtualizarInterfaceDoJogo();
            }
            else if (resposta == DialogResult.No)
            {
                finalizar_jogo();
                txtJogador1.Clear();
                txtJogador2.Clear();
                panelJogo.Visible = false;
                panelLogin.Visible = true;
            }
            else
            {
                finalizar_jogo();
                Application.Exit();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            finalizar_jogo();
            base.OnFormClosed(e);
        }
    }
}
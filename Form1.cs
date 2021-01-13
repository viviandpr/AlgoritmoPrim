using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.IO;
using System.Configuration;
using System.Runtime.InteropServices;


namespace AlgoritmoPrim{


        public partial class Form1 : Form{
            public Form1(){
                InitializeComponent();
            }

            public No[] nos;
            public void inicializar(){
                //List<No> nos = new List<No>();
                int numeroNos = 280;
                string[] leitura = File.ReadAllLines(@"C:\Users\Dell\Desktop\Mestrado\dadosTSP.txt"); // o read é método da classe File
                nos = new No[numeroNos];  //Variável Nos, do tipo No, com 280 posições. Instanciar, declarar, inicializar.
            
                for (int i = 0; i < numeroNos; i++){
                    string[] atual = leitura[i + 6].Trim().Replace("   ", " ").Replace("  ", " ").Split(' ');
                    //Nos[i] = new No();  //vetor Nos, na posição i vai recber um novo nó. Um novo elemento para classe No.
                    nos[i] = new No(int.Parse(atual[1]), int.Parse(atual[2]), int.Parse(atual[0]));
                }
                MessageBox.Show("Finalizada a leitura do arquivo!");
            
            
                //Plotar cada um dos nós na tela.
                foreach (No n in nos){
                   desenharNo(n);
                   desenhaLabel(n);
               
                }
            }
            
            public int pixelsPorNo = 3;
            public void desenharNo(No n){
                Graphics g = pictureBox1.CreateGraphics();
                g.FillEllipse(Brushes.Blue, (n.x - 2)*(pixelsPorNo), (n.y - 2) * pixelsPorNo, 2 * 2, 2 * 2);

            }

            public void desenharLinha(No n1, No n2){
                Graphics g = pictureBox1.CreateGraphics();
                g.DrawLine(Pens.PaleVioletRed, (n1.x) * (pixelsPorNo) - 4, (n1.y) * (pixelsPorNo) - 4, (n2.x) * (pixelsPorNo) - 4, (n2.y) * (pixelsPorNo) - 4);
            }

            public double calcularDistanciaEntreNos(No n1, No n2){
                double deltaX = n1.x - n2.x;
                double deltaY = n1.y - n2.y;
                return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            }
            public double calcularTamanhoPossibilidade(int[] caminho){
                double trajeto = 0;
                int idx1 = 0;
                int idx2 = 0;
                for (int i = 1; i < caminho.Length; i++){
                    idx1 = caminho[i - 1];
                    idx2 = caminho[i];
                    trajeto += calcularDistanciaEntreNos(nos[idx1], nos[idx2]);
                }
                return trajeto;
            }

            public void desenhaLabel(No n){
                Label lbl = new Label();
                lbl.Name = "boi_" + n.x + "," + n.y;
                lbl.Text = n.numero.ToString();
                //lbl.Location = new Point(n.x , n.y);
                lbl.Location = new Point(pictureBox1.Location.X + (n.x)*(pixelsPorNo) , pictureBox1.Location.Y + (n.y)*(pixelsPorNo));
                lbl.AutoSize = true;
                lbl.Font = new Font("Calibri", 6);
                this.Controls.Add(lbl);
                lbl.BringToFront();
            }


        //Algoritmo de Prim
        public List<int[]> EncontrarMinimaArborescenciaPrim(){

                //Matriz de distâncias.
                int dimensao = nos.Length;
                double[,] matrizDistancias = new double[dimensao, dimensao];

                //double[,] CopiaMatrizDistancias = new double[Dimensao, Dimensao];
                //double[,] MatrizPredecessor = new double[Dimensao, Dimensao];
                for (int i = 0; i < dimensao; i++){
                    for (int j = 0; j < dimensao; j++){
                        if (i == j){
                            matrizDistancias[i, j] = 0;
                            //richTextBox1.AppendText("Distância entre" + i.ToString() + " e " + j.ToString() + " = 0" + "\r\n"); richTextBox1.ScrollToCaret();
                        }
                        else{
                            matrizDistancias[i, j] = calcularDistanciaEntreNos(nos[i], nos[j]);
                            //richTextBox1.AppendText("Distância entre" + i.ToString() + " e " + j.ToString() + " = " + MatrizDistancias[i, j].ToString() + "\r\n"); richTextBox1.ScrollToCaret();

                        }
                    }
                }
                //Inicializar variáveis
                List<VerticeNaoConectado> VerticesNaoConectados = new List<VerticeNaoConectado>();
                List<int[]> arvore = new List<int[]>();
                //Escolher Vértice Inicial Arbitrário
                int VerticeInicial = 0;
                for (int i = 1; i < dimensao; i++)
                {
                    VerticeNaoConectado v = new VerticeNaoConectado();
                    v.Vertice = i;
                    v.Distancia = matrizDistancias[VerticeInicial, i];
                    v.VizinhoMaisProximo = VerticeInicial;
                    VerticesNaoConectados.Add(v);
                }

                //Laço principal
                while (VerticesNaoConectados.Count > 0)
                {
                    //Encontrar Vertice não conectado mais próximo de algum vértice já conectado
                    int PosicaoVerticeMaisPerto = -1;
                    double MenorDistancia = double.MaxValue;
                    for (int i = 0; i < VerticesNaoConectados.Count; i++)
                    {
                        if (VerticesNaoConectados[i].Distancia < MenorDistancia)
                        {
                            MenorDistancia = VerticesNaoConectados[i].Distancia;
                            PosicaoVerticeMaisPerto = i;
                        }
                    }
                    int VerticeConectado = VerticesNaoConectados[PosicaoVerticeMaisPerto].VizinhoMaisProximo;
                    int VerticeParaConectar = VerticesNaoConectados[PosicaoVerticeMaisPerto].Vertice;
                    //Adicionar aresta à árvore
                    arvore.Add(new int[2] { VerticeConectado, VerticeParaConectar });
                    //Atualizar Vértices Não conectados
                    VerticesNaoConectados.RemoveAt(PosicaoVerticeMaisPerto);
                    foreach (VerticeNaoConectado v in VerticesNaoConectados)
                    {
                        if (matrizDistancias[VerticeParaConectar, v.Vertice] < v.Distancia)
                        {
                            v.Distancia = matrizDistancias[VerticeParaConectar, v.Vertice];
                            v.VizinhoMaisProximo = VerticeParaConectar;
                        }
                    }
                }
                return arvore;
            }


            //Botões
            private void Ler_Click(object sender, EventArgs e){
                inicializar();
            }

            private void Desenhar_Click(object sender, EventArgs e){
                List<int[]> arvore = EncontrarMinimaArborescenciaPrim();
                No n1;
                No n2;
                //Desenhar a árvore e imprimir o custo do menor caminho.
                double custo = 0;
                foreach (int[] conjunto in arvore){
                    n1 = nos[conjunto[0]];
                    n2 = nos[conjunto[1]];
                    desenharLinha(n1, n2);
                    custo += calcularTamanhoPossibilidade(conjunto);
            }
                
               MessageBox.Show("O custo total do caminho é: " + custo.ToString()); 
        }
 
    }

        public class VerticeNaoConectado
        {
            public int Vertice;
            public double Distancia;
            public int VizinhoMaisProximo;
        }

        /* Documentação

         */
        public class No
        {//A classe No é um pacote que contém todas as informações de um nó.
            public int x;
            public int y;
            public int numero;
            //public bool grupoAtivo = true;
            //public int head;//índice

            public No(int x, int y, int numero)
            { // Função de inicialização da classe.
                this.x = x;
                this.y = y;
                this.numero = numero;
            }
        }
}


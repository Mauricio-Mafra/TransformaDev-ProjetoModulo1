using System;
using System.IO;
using System.Numerics;
using System.Runtime;
using System.Media;

namespace ProjetoFinal
{
    class JogoDaForca
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = true;
            string palavraSecreta = "";
            string situacaoAtual = "";
            string letrasTestadas = "";
            int tentativasRestantes = 6;
            int xCentroTela = Console.BufferWidth / 2;
            int yCentroTela = Console.BufferHeight / 2;



            ImprimirMenu();
            palavraSecreta = DefinirPalavraAleatoria();
            string hipotese = "";
            string buffer;


            while (true)
            {
                //Atualizar tela
                Console.Clear();
                situacaoAtual = AtualizarMascara(hipotese, palavraSecreta, situacaoAtual);
                EscreverNaPosicao(situacaoAtual, (xCentroTela - situacaoAtual.Length / 2), 13); // Palavra Mascarada
                ImprimirBoneco(tentativasRestantes);

                buffer = "Digite uma letra:";
                EscreverNaPosicao(buffer, (xCentroTela - buffer.Length / 2), 22);

                buffer = "Letras já testadas: " + letrasTestadas;
                EscreverNaPosicao(buffer, (xCentroTela - buffer.Length / 2), yCentroTela + 2);

                buffer = "Vidas restantes: " + ImprimirVidas(tentativasRestantes);
                EscreverNaPosicao(buffer, (xCentroTela - buffer.Length / 2), yCentroTela + 4);

                while (true)
                {


                    try
                    {
                        Console.SetCursorPosition(xCentroTela + 10, 22);
                        hipotese = Console.ReadLine().ToLower(); // Limpar entrada


                        if (hipotese == "")
                            throw new Exception(message: "Digite uma letra!");

                        if (hipotese.Length > 1)
                            throw new Exception(message: "Digite apenas uma letra!");

                        if (int.TryParse(hipotese, out int saida))
                            throw new Exception(message: "Digite apenas letras!");

                        if (letrasTestadas.Contains(hipotese))
                            throw new Exception(message: "Letra já utilizada anteriormente!");

                        letrasTestadas += hipotese + " ";
                        break;
                    }
                    catch (Exception ex)
                    {
                        EscreverNaPosicao("                                                      ", (xCentroTela - 30), 25);
                        EscreverNaPosicao(ex.Message, (xCentroTela - ex.Message.Length / 2), 25);
                    }
                }


                if (!VerificarTentativa(hipotese, palavraSecreta))
                {
                    tentativasRestantes--;
                    buffer = "Vidas restantes: " + ImprimirVidas(tentativasRestantes);
                    EscreverNaPosicao(buffer, (xCentroTela - buffer.Length / 2), yCentroTela + 4);
                }


                situacaoAtual = AtualizarMascara(hipotese, palavraSecreta, situacaoAtual);

                if (tentativasRestantes < 0)
                    tentativasRestantes = 0;

                if (tentativasRestantes == 0)
                {
                    Console.Clear();
                    ImprimirBoneco(tentativasRestantes);
                    EscreverNaPosicao("Não foi dessa vez...", xCentroTela - 10, yCentroTela);
                    EscreverNaPosicao("Aperte uma tecla para finalizar...", xCentroTela - 17, yCentroTela + 5);
                    Console.ReadKey();
                    break;
                }

                if (VerificarVitoria(palavraSecreta, situacaoAtual))
                {
                    Console.Clear();
                    EscreverNaPosicao("VITÓRIA!!!", xCentroTela - 5, yCentroTela);
                    EscreverNaPosicao("Aperte uma tecla para finalizar...", xCentroTela - 17, yCentroTela + 5);
                    SoundPlayer soundPlayer = new SoundPlayer("vitoria.wav");
                    soundPlayer.Play();
                    Console.ReadKey();
                    break;
                }
            }
        }

        static string DefinirPalavraAleatoria()
        {
            Random rand = new Random();
            int indicePalavra = 0;
            int contador = 0;
            string[] palavrasPossiveis = new string[50];
            string line = "";
            string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "palavras.txt");

            try
            {
                StreamReader sr = new StreamReader(caminhoArquivo);

                while (line != null)
                {

                    line = sr.ReadLine();
                    Console.WriteLine(line);
                    if (line != null)
                    {
                        palavrasPossiveis[contador] = line.ToString();
                        contador++;
                    }
                }

                indicePalavra = rand.Next(contador);
                return palavrasPossiveis[indicePalavra];

            }
            catch (IOException ex)
            {
                Console.WriteLine("Exception IO: " + ex.Message);
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine("Exception Memory: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            return "";
        }

        static bool VerificarTentativa(string hipotese, string palavraSecreta)
        {
            if (palavraSecreta.Contains(hipotese.ToString()))
                return true;
            else
                return false;
        }

        static string AtualizarMascara(string hipotese, string palavraSecreta, string situacaoAtual)
        {
            string novaMascara = "";

            if (situacaoAtual == "")
            {
                for (int i = 0; i < palavraSecreta.Length; i++)
                {
                    novaMascara += "_";
                }
            }
            else
            {
                for (int i = 0; i < palavraSecreta.Length; i++)
                {
                    if (palavraSecreta[i].ToString() == hipotese.ToString())
                    {
                        novaMascara += hipotese.ToString();
                    }
                    else if (situacaoAtual[i].ToString() != "_")
                    {
                        novaMascara += situacaoAtual[i];
                    }
                    else
                    {
                        novaMascara += "_";
                    }
                }
            }

            return novaMascara;
        }

        static bool VerificarVitoria(string palavraSecreta, string situacaoAtual)
        {
            if (palavraSecreta == situacaoAtual)
                return true;
            else
                return false;
        }

        static void ImprimirMenu()
        {
            Dictionary<int, string> opcoes = new Dictionary<int, string>();
            int posicaoYMenu = Console.BufferHeight / 2;
            int posicaoXMenu = Console.BufferWidth / 2;
            int itemSelecionado = 0;

            opcoes.Add(0, "Iniciar Jogo");
            opcoes.Add(1, "Como jogar?");
            opcoes.Add(2, "Sair");

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();
                int posicao = CentralizarTexto("     __                    __       ____                 ");

                EscreverNaPosicao(@"         __                    __       ____                 
                                     __ / /__  ___ ____    ___/ /__ _  / __/__  ___________ _
                                    / // / _ \/ _ `/ _ \  / _  / _ `/ / _// _ \/ __/ __/ _ `/
                                    \___/\___/\_, /\___/  \_,_/\_,_/ /_/  \___/_/  \__/\_,_/ 
                                             /___/                                          ",
                 posicao,
                 5);

                ImprimirControlesMenu();

                Console.SetCursorPosition(Console.BufferWidth / 2, Console.BufferHeight / 2);
                for (int i = 0; i < opcoes.Count; i++)
                {
                    Console.SetCursorPosition(CentralizarTexto(opcoes[i]), (Console.BufferHeight / 2 - opcoes.Count + i));
                    if (itemSelecionado == i)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;

                        Console.WriteLine(opcoes[i]);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;

                    }
                    else
                    {
                        Console.WriteLine(opcoes[i]);
                    }
                }


                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        itemSelecionado--;

                        if (itemSelecionado < 0)
                            itemSelecionado = opcoes.Count - 1;

                        break;
                    case ConsoleKey.DownArrow:
                        itemSelecionado++;
                        if (itemSelecionado == opcoes.Count)
                            itemSelecionado = 0;
                        break;
                    case ConsoleKey.Enter:
                        if (itemSelecionado == 0)
                            return;

                        if (itemSelecionado == 1)
                            ImprimirComoJogar();

                        if (itemSelecionado == 2)
                            System.Environment.Exit(1);
                        break;
                    default:
                        break;
                }
            }
        }

        static void ImprimirBoneco(int tentativasRestantes)
        {
            int xCentroTela = Console.BufferWidth / 2;
            int yCentroTela = Console.BufferHeight / 2;

            Console.SetCursorPosition((Console.BufferWidth / 2 - 4), 5);
            switch (tentativasRestantes)
            {
                case 6:
                    EscreverNaPosicao("▃▃▃▃▃▃▃▃", xCentroTela - 4, 3);
                    EscreverNaPosicao("▋       ▋", xCentroTela - 4, 4);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 5);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 6);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 7);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 8);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 9);
                    break;
                case 5:
                    EscreverNaPosicao("▃▃▃▃▃▃▃▃", xCentroTela - 4, 3);
                    EscreverNaPosicao("▋       ▋", xCentroTela - 4, 4);
                    EscreverNaPosicao("▋       O", xCentroTela - 4, 5);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 6);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 7);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 8);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 9);
                    break;

                case 4:
                    EscreverNaPosicao("▃▃▃▃▃▃▃▃", xCentroTela - 4, 3);
                    EscreverNaPosicao("▋       ▋", xCentroTela - 4, 4);
                    EscreverNaPosicao("▋       O", xCentroTela - 4, 5);
                    EscreverNaPosicao("▋       |", xCentroTela - 4, 6);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 7);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 8);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 9);
                    break;

                case 3:
                    EscreverNaPosicao("▃▃▃▃▃▃▃▃", xCentroTela - 4, 3);
                    EscreverNaPosicao("▋       ▋", xCentroTela - 4, 4);
                    EscreverNaPosicao("▋       O", xCentroTela - 4, 5);
                    EscreverNaPosicao("▋      /|", xCentroTela - 4, 6);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 7);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 8);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 9);
                    break;

                case 2:
                    EscreverNaPosicao("▃▃▃▃▃▃▃▃", xCentroTela - 4, 3);
                    EscreverNaPosicao("▋       ▋", xCentroTela - 4, 4);
                    EscreverNaPosicao("▋       O", xCentroTela - 4, 5);
                    EscreverNaPosicao("▋      /|\\", xCentroTela - 4, 6);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 7);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 8);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 9);
                    break;

                case 1:
                    EscreverNaPosicao("           /￣￣￣￣￣￣￣￣￣￣￣￣\\", xCentroTela - 4, 2);
                    EscreverNaPosicao("▃▃▃▃▃▃▃▃   | Rapaz... Tá certo isso?/", xCentroTela - 4, 3);
                    EscreverNaPosicao("▋       ▋  /へ.＿＿＿＿＿＿＿＿＿＿/", xCentroTela - 4, 4);
                    EscreverNaPosicao("▋       O", xCentroTela - 4, 5);
                    EscreverNaPosicao("▋      /|\\", xCentroTela - 4, 6);
                    EscreverNaPosicao("▋      / ", xCentroTela - 4, 7);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 8);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 9);
                    break;

                case 0:
                    EscreverNaPosicao("▃▃▃▃▃▃▃▃", xCentroTela - 4, 3);
                    EscreverNaPosicao("▋       ▋", xCentroTela - 4, 4);
                    EscreverNaPosicao("▋       O", xCentroTela - 4, 5);
                    EscreverNaPosicao("▋      /|\\", xCentroTela - 4, 6);
                    EscreverNaPosicao("▋      / \\", xCentroTela - 4, 7);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 8);
                    EscreverNaPosicao("▋       ", xCentroTela - 4, 9);
                    break;

            }
        }

        static void EscreverNaPosicao(string texto, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.WriteLine(texto);
        }

        static string ImprimirVidas(int tentativasRestantes)
        {
            string vidas = "";
            for (int i = 0; i <= tentativasRestantes; i++)
            {
                vidas += "♡ ";
            }
            return vidas;
        }

        static void ImprimirComoJogar()
        {
            int xCentroTela = Console.BufferWidth / 2;
            int yCentroTela = Console.BufferHeight / 2;

            Console.Clear();

            EscreverNaPosicao("Tente advinhar as letras presentes na palavra secreta.", xCentroTela - 28, yCentroTela - 3);
            EscreverNaPosicao("A cada tentativa errada, você perderá uma vida.", xCentroTela - 23, yCentroTela - 1);
            EscreverNaPosicao("Complete a palavra antes que isso aconteça!", xCentroTela - 22, yCentroTela + 1);
            EscreverNaPosicao("Boa sorte!!!", xCentroTela - 6, yCentroTela + 7);


            EscreverNaPosicao("Aperte ENTER tecla para voltar.", xCentroTela - 16, yCentroTela + 10);

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    break;
            }
        }

        static int CentralizarTexto(string labelOpcao)
        {
            int posicao = (Console.BufferWidth / 2) - labelOpcao.Length / 2;

            return posicao;
        }

        static void ImprimirControlesMenu()
        {
            Console.SetCursorPosition(3, Console.BufferHeight - 1);
            Console.WriteLine("< \u2191 / \u2193 > Selecionar opção         <ENTER> Confirmar");
        }
    }
}
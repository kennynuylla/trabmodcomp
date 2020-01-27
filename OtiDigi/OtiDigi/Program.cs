using AlgoritmoGenetico.Estruturas_de_Dados;
using AlgoritmoGenetico.Helpers.Taxas_Adaptáveis;
using CircuitoDigitalCombinacional.Estrutura_de_Dados.Array_Bits;
using CircuitoDigitalCombinacional.Estruturas_de_Dados.Portas;
using OtiCircDigitais;
using QuineMcCluskey;
using System;

namespace OtiDigi
{
    class Program
    {
        private const int máximoEntradasPorta = 4;
        private const int quantidadeBitsSomador = 2;
        private const int qtdEntradasCircuito = 2*quantidadeBitsSomador;
        private const string caminho = "C:\\Users\\mathe\\Desktop\\";

        static void Main(string[] args)
        {
            var gabaritos = GerarGabaritosSomador(quantidadeBitsSomador); // Overflow | MSB | ... | LSB
            var entradas = Entrada.RetornarEntradas(qtdEntradasCircuito);
            var custoTotalQuine = 0;
            var custoTotalAg = 0;

            foreach (var item in gabaritos)
            {
                var quine = new QuineMcCluskeySolver(qtdEntradasCircuito, item);
                var circuito = quine.Rodar(entradas, caminho, máximoEntradasPorta);

                custoTotalQuine += circuito.Custo;
            }

            var crossover = new CrossoverCircuitoHelper(máximoEntradasPorta, entradas, caminho);
            var criterioParada = new CriterioParada(100);
            var taxaLimpeza = new TaxaLimpeza(20);
            var taxaMutacao = new TaxaConstanteHelper<double>(0.02);
            var taxaCrossover = new TaxaConstanteHelper<double>(.9);
            var taxaPopulacao = new TaxaConstanteHelper<int>(100);
            var taxaInsercao = new TaxaConstanteHelper<double>(0.1);

            foreach (var item in gabaritos)
            {
                var motorGA = new GACircuito(criterioParada, máximoEntradasPorta, qtdEntradasCircuito, caminho, crossover, gabaritos[1], 2, entradas);
                motorGA.SetarTaxas(taxaMutacao, taxaCrossover, taxaPopulacao, taxaInsercao, taxaLimpeza);
                motorGA.ProbabilidadeCircuitoMintermoMaxtermo = .1;
                motorGA.GerarPopulacao();
                motorGA.SetarMostrarProgresso(true, 1);
                motorGA.Rodar();

                var campeao = motorGA.RetornarCampeao();
                custoTotalAg += campeao.Circuito.Custo;
                Console.WriteLine(campeao.Circuito.GerarExpressao());

                //break;
            }
            Console.WriteLine($"Quine: {custoTotalQuine}");
            Console.WriteLine($"AG: {custoTotalAg}");

            Sair();
        }

        private static Gabarito[] GerarGabaritosSomador(int quantidadeBits)
        {
            var quantidadeBitsResultado = quantidadeBits + 1;
            var gabaritos = new Gabarito[quantidadeBitsResultado];
            var entradaMáxima = (int)Math.Pow(2, quantidadeBits) - 1;
            var índiceGabarito = 0;

            for (int i = 0; i < quantidadeBitsResultado; i++)
            {
                gabaritos[i] = new Gabarito();
            }
            
            for (int i = 0; i <= entradaMáxima; i++)
            {
                for (int e = 0; e <= entradaMáxima; e++)
                {
                    var resultado = new ArrayBits(quantidadeBitsResultado);
                    resultado.PreencherAPartirNumero(i + e);
                    foreach (var item in resultado.Bits)
                    {
                        gabaritos[índiceGabarito].AdicionarEntrada(item);
                        índiceGabarito++;
                    }
                    índiceGabarito = 0;
                }
            }

            return gabaritos;
        }

        private static void Sair()
        {
            Console.WriteLine("Pressione ENTER para Sair...");
            Console.ReadLine();
        }
    }
}

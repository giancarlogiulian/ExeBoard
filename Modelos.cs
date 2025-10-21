using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExeBoard
{
    // Colamos todas as nossas classes de modelo de dados aqui
    // Agora elas são "públicas" para todo o projeto.

    public class ServidorItem
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public bool ReplicarParaCopia { get; set; }
        public string SubDiretorios { get; set; }
        public string CaminhoCompletoAplicacao { get; set; }
        public override string ToString() { return Nome; }
    }

    public class ClienteItem
    {
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string SubDiretorios { get; set; }
        public string CaminhoCompletoCliente { get; set; }
        public override string ToString() { return Nome; }
    }

    public class Colaborador
    {
        public string Nome { get; set; }
        public string Funcao { get; set; }
        public string GitHub { get; set; }
        public override string ToString() { return Nome; }
    }

    public class Atualizador
    {
        public string Nome { get; set; }
        public string Caminho { get; set; }
        public override string ToString() { return Nome; }
    }
}
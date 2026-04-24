using System;
using System.Collections.Generic;

public interface IObservador
{
    void Atualizar(PCD pcd, string tipoMedicao, double valorAnterior, double valorAtual);
}

public interface ISujeito
{
    void Inscrever(IObservador observador);
    void Cancelar(IObservador observador);
    void Notificar(string tipoMedicao, double valorAnterior, double valorAtual);
}

public class PCD : ISujeito
{
    //Identificação
    public string Id       { get; }
    public string Nome     { get; }
    public string Rio      { get; }
    public string Regiao   { get; }

    //Medições
    private double _temperaturaAgua;
    private double _phAgua;
    private double _umidadeRelativa;

    //Lista de observadores
    private readonly List<IObservador> _observadores = new List<IObservador>();

    public PCD(string id, string nome, string rio, string regiao,
               double temperaturaAgua, double phAgua, double umidadeRelativa)
    {
        Id                = id;
        Nome              = nome;
        Rio               = rio;
        Regiao            = regiao;
        _temperaturaAgua  = temperaturaAgua;
        _phAgua           = phAgua;
        _umidadeRelativa  = umidadeRelativa;
    }


    public double TemperaturaAgua
    {
        get => _temperaturaAgua;
        set
        {
            double anterior  = _temperaturaAgua;
            _temperaturaAgua = value;
            if (Math.Abs(anterior - value) > 1e-9)
                Notificar("Temperatura da Água (°C)", anterior, value);
        }
    }

    public double PhAgua
    {
        get => _phAgua;
        set
        {
            double anterior = _phAgua;
            _phAgua         = value;
            if (Math.Abs(anterior - value) > 1e-9)
                Notificar("pH da Água", anterior, value);
        }
    }

    public double UmidadeRelativa
    {
        get => _umidadeRelativa;
        set
        {
            double anterior  = _umidadeRelativa;
            _umidadeRelativa = value;
            if (Math.Abs(anterior - value) > 1e-9)
                Notificar("Umidade Relativa do Ar (%)", anterior, value);
        }
    }


    public void Inscrever(IObservador observador)
    {
        if (!_observadores.Contains(observador))
        {
            _observadores.Add(observador);
            Console.WriteLine($"  [+] {((Universidade)observador).Nome} inscrita em {Nome}");
        }
    }

    public void Cancelar(IObservador observador)
    {
        _observadores.Remove(observador);
        Console.WriteLine($"  [-] {((Universidade)observador).Nome} cancelou inscrição em {Nome}");
    }

    public void Notificar(string tipoMedicao, double valorAnterior, double valorAtual)
    {
        Console.WriteLine($"\n  [{Nome}] Mudança detectada em '{tipoMedicao}':" +
                          $" {valorAnterior:F2} → {valorAtual:F2}");
        Console.WriteLine($"     Notificando {_observadores.Count} universidade(s) interessada(s)...");

        foreach (var obs in _observadores)
            obs.Atualizar(this, tipoMedicao, valorAnterior, valorAtual);
    }

    public override string ToString() =>
        $"PCD [{Id}] {Nome} | Rio: {Rio} | Região: {Regiao} " +
        $"| T°C: {_temperaturaAgua:F1} | pH: {_phAgua:F1} | Umidade: {_umidadeRelativa:F1}%";
}

public class Universidade : IObservador
{
    public string Nome    { get; }
    public string Cidade  { get; }

    public Universidade(string nome, string cidade)
    {
        Nome   = nome;
        Cidade = cidade;
    }

    /// <summary>
    /// Chamado automaticamente pela PCD quando um valor muda.
    /// </summary>
    public void Atualizar(PCD pcd, string tipoMedicao, double valorAnterior, double valorAtual)
    {
        Console.WriteLine($"    [{Nome} - {Cidade}] " +
                          $"Notificação recebida: '{tipoMedicao}' da {pcd.Nome} " +
                          $"({pcd.Rio}, {pcd.Regiao}) mudou de {valorAnterior:F2} para {valorAtual:F2}.");
    }

    public override string ToString() => $"Universidade {Nome} ({Cidade})";
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Cabecalho("SISTEMA DE MONITORAMENTO DE PCDs NA AMAZÔNIA");

        Secao("1. Instanciando as Plataformas de Coleta de Dados (PCDs)");

        var pcdSolimoes = new PCD(
            id: "PCD-AM-001", nome: "PCD Solimões I",
            rio: "Rio Solimões", regiao: "Amazonas",
            temperaturaAgua: 28.5, phAgua: 6.8, umidadeRelativa: 85.0);

        var pcdNegro = new PCD(
            id: "PCD-AM-002", nome: "PCD Negro II",
            rio: "Rio Negro", regiao: "Amazonas",
            temperaturaAgua: 27.0, phAgua: 4.2, umidadeRelativa: 90.0);

        var pcdXingu = new PCD(
            id: "PCD-PA-001", nome: "PCD Xingu I",
            rio: "Rio Xingu", regiao: "Pará",
            temperaturaAgua: 29.2, phAgua: 7.1, umidadeRelativa: 82.0);

        Console.WriteLine($"  {pcdSolimoes}");
        Console.WriteLine($"  {pcdNegro}");
        Console.WriteLine($"  {pcdXingu}");

        Secao("2. Instanciando as Universidades");

        var inpe   = new Universidade("INPE",   "São José dos Campos");
        var ufrj   = new Universidade("UFRJ",   "Rio de Janeiro");
        var usp    = new Universidade("USP",    "São Paulo");
        var ufpr   = new Universidade("UFPR",   "Curitiba");
        var ufam   = new Universidade("UFAM",   "Manaus");

        Console.WriteLine($"  {inpe}");
        Console.WriteLine($"  {ufrj}");
        Console.WriteLine($"  {usp}");
        Console.WriteLine($"  {ufpr}");
        Console.WriteLine($"  {ufam}");


        Secao("3. Universidades se inscrevendo nas PCDs de interesse");

        Console.WriteLine("\n  → Interesses na PCD Solimões I:");
        pcdSolimoes.Inscrever(inpe);
        pcdSolimoes.Inscrever(usp);
        pcdSolimoes.Inscrever(ufam);

        Console.WriteLine("\n  → Interesses na PCD Negro II:");
        pcdNegro.Inscrever(ufrj);
        pcdNegro.Inscrever(ufam);
        pcdNegro.Inscrever(ufpr);

        Console.WriteLine("\n  → Interesses na PCD Xingu I:");
        pcdXingu.Inscrever(inpe);
        pcdXingu.Inscrever(ufrj);
        pcdXingu.Inscrever(usp);


        Secao("4. Simulando mudanças nas medições das PCDs");

        Console.WriteLine("\n Alterando temperatura da PCD Solimões I de 28.5°C para 31.2°C:");
        pcdSolimoes.TemperaturaAgua = 31.2;

        Console.WriteLine("\n Alterando pH da PCD Negro II de 4.2 para 3.9:");
        pcdNegro.PhAgua = 3.9;

        Console.WriteLine("\n Alterando umidade relativa da PCD Xingu I de 82.0% para 76.5%:");
        pcdXingu.UmidadeRelativa = 76.5;

        Console.WriteLine("\n Alterando temperatura da PCD Negro II de 27.0°C para 29.8°C:");
        pcdNegro.TemperaturaAgua = 29.8;

        Console.WriteLine("\n Alterando pH da PCD Xingu I de 7.1 para 7.4:");
        pcdXingu.PhAgua = 7.4;

        Secao("5. UFAM cancela interesse na PCD Negro II");

        pcdNegro.Cancelar(ufam);

        Console.WriteLine("\n Temperatura do Negro sobe para 30.5°C — UFAM NÃO deve ser notificada:");
        pcdNegro.TemperaturaAgua = 30.5;

        Secao("6. Estado final das PCDs");

        Console.WriteLine($"  {pcdSolimoes}");
        Console.WriteLine($"  {pcdNegro}");
        Console.WriteLine($"  {pcdXingu}");

        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("  Simulação concluída com sucesso.");
        Console.WriteLine(new string('=', 70));
    }

    static void Cabecalho(string texto)
    {
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"  {texto}");
        Console.WriteLine(new string('=', 70));
    }

    static void Secao(string texto)
    {
        Console.WriteLine($"\n{'─',0}{new string('─', 69)}");
        Console.WriteLine($"  {texto}");
        Console.WriteLine(new string('─', 70));
    }
}

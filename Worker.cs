using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Reflection.Metadata;
using System;
using Desafio10FastGingersRPA.Models;
using OfficeOpenXml;

namespace Desafio10FastGingersRPA
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private int _tempoExecucao = 1;
        private string _chromeDriverPath;
        private ChromeOptions _chromeOptions;
        private string _url = "https://10fastfingers.com/typing-test/portuguese";
        public Worker(ILogger<Worker> logger)
        {
            _chromeDriverPath = @".\Driver\chromedriver-win64\chromedriver.exe";
            _chromeOptions = new ChromeOptions();
            //_chromeOptions.AddArgument("--headless");

            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Servi�o iniciado: {time}", DateTimeOffset.Now);

                    ResultModel result = PegarInforma��es();

                    SalvarDados(result);

                    _logger.LogInformation("Servi�o finalizado: {time}", DateTimeOffset.Now);

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }

        public ResultModel PegarInforma��es()
        {
            ResultModel result = new ResultModel();

            using (var driver = new ChromeDriver(_chromeDriverPath, _chromeOptions))
            {
                driver.Navigate().GoToUrl(_url);

                if (VerificarPalavraNaPagina(driver, "closeIconHit")) 
                {
                    driver.FindElement(By.Id("closeIconHit")).Click();
                }

                if (VerificarPalavraNaPagina(driver, "CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll"))
                {
                    driver.FindElement(By.Id("CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll")).Click();
                }

                if (VerificarPalavraNaPagina(driver, "highlight"))
                {
                    DateTime horaInicio = DateTime.Now;
                    while (!PassaramXMinutos(horaInicio, _tempoExecucao))
                    {
                        var palavraSelecionada = driver.FindElement(By.CssSelector("span[class='highlight']"));

                        var barra = driver.FindElement(By.Id("inputfield"));

                        barra.SendKeys(palavraSelecionada.Text + " ");

                        Thread.Sleep(100);
                    }
                }
                else 
                {
                    result.ErrorMesssage = "N�o encontrado palavra selecionada.";
                }

                if (VerificarPalavraNaPagina(driver, "auswertung-result")) 
                {
                    result = new ResultModel()
                    {
                        Wpm             = driver.FindElement(By.XPath("//*[@id=\"wpm\"]/strong")).Text,
                        Keystrokes      = driver.FindElement(By.XPath("//*[@id=\"keystrokes\"]/td[2]/small/span[1]")).Text,
                        Accuracy        = driver.FindElement(By.XPath("//*[@id=\"accuracy\"]/td[2]/strong")).Text,
                        CorrectWords    = driver.FindElement(By.XPath("//*[@id=\"correct\"]/td[2]/strong")).Text,
                        WrongWords      = driver.FindElement(By.XPath("//*[@id=\"wrong\"]/td[2]/strong")).Text
                    };
                }
                else
                {
                    result.ErrorMesssage = "Nao encontrado caixa com resultados.";
                }

                driver.Quit();
            }

            return result;
        }

        private bool PassaramXMinutos(DateTime tempoAnterior, int minutos)
        {
            TimeSpan diferenca = DateTime.Now - tempoAnterior;
            return diferenca.TotalMinutes >= minutos;
        }

        private bool VerificarPalavraNaPagina(IWebDriver driver, string palavra)
        {
            return driver.PageSource.Contains(palavra);
        }

        private void SalvarDados(ResultModel result) 
        {
            string caminhoDownloads = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";

            FileInfo arquivo = new FileInfo(caminhoDownloads + "\\Tabela.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage pacote = new ExcelPackage(arquivo))
            {
                ExcelWorksheet planilha = pacote.Workbook.Worksheets.Add(DateTime.Now.ToString("ddMMyyyy HH:mm:ss"));

                planilha.Cells["A1"].Value = "Wpm";
                planilha.Cells["B1"].Value = "Keystrokes";
                planilha.Cells["C1"].Value = "Accuracy";
                planilha.Cells["D1"].Value = "CorrectWords";
                planilha.Cells["E1"].Value = "KeystrWrongWordsokes";

                planilha.Cells["A2"].Value = result.Wpm;
                planilha.Cells["B2"].Value = result.Keystrokes;
                planilha.Cells["C2"].Value = result.Accuracy;
                planilha.Cells["D2"].Value = result.CorrectWords;
                planilha.Cells["E2"].Value = result.Keystrokes;

                pacote.Save();
            }
        }
    }
}
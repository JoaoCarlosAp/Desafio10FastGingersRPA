using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Reflection.Metadata;
using System;
using Desafio10FastGingersRPA.Models;

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
                _logger.LogInformation("Serviço iniciado: {time}", DateTimeOffset.Now);

                MainFlow();

                _logger.LogInformation("Serviço finalizado: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        public void MainFlow()
        {
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
                
                DateTime horaInicio = DateTime.Now;
                while (!PassaramXMinutos(horaInicio, _tempoExecucao))
                {
                    if (VerificarPalavraNaPagina(driver, "highlight"))
                    {
                        var palavraSelecionada = driver.FindElement(By.CssSelector("span[class='highlight']"));

                        var barra = driver.FindElement(By.Id("inputfield"));

                        barra.SendKeys(palavraSelecionada.Text + " ");
                    }
                }

                if (VerificarPalavraNaPagina(driver, "auswertung-result")) 
                {
                    ResultModel resultModel = new ResultModel()
                    {
                        Wpm = driver.FindElement(By.XPath("//*[@id=\"wpm\"]/strong")).Text,
                        Keystrokes = driver.FindElement(By.XPath("//*[@id=\"keystrokes\"]/td[2]/small/span[1]")).Text,
                        Accuracy = driver.FindElement(By.XPath("//*[@id=\"accuracy\"]/td[2]/strong")).Text,
                        CorrectWords = driver.FindElement(By.XPath("//*[@id=\"correct\"]/td[2]/strong")).Text,
                        WrongWords = driver.FindElement(By.XPath("//*[@id=\"wrong\"]/td[2]/strong")).Text
                    };
                }

                // Salvar no banco de dados

                // Fechar o navegador
                driver.Quit();
            }
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
    }
}
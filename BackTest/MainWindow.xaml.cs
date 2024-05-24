﻿using BackTest.Data;
using BackTest.Framework;
using BackTest.Trading;
using System.Windows;

namespace BackTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Use this as composition root since this is a simple
        // application which won't need a DI container
        public MainWindow()
        {
            // I don't have the rights to redistribute the data, so load
            // from a neutral location
            var dataSource = new FileSpecificDataSource(@"C:\Temp\BackTestData");

            var marketData = new MarketData(dataSource);
            var marketAtTime = new MarketAtTime(marketData);
            var startingCapital = 100000000;
            var companyCount = 500;

            WindowState = WindowState.Maximized;
            DataContext = new MainWindowVM(marketAtTime, marketData, new List<IPriceSeries>()
            {
                /*Trader.IndexTrader(marketAtTime, startingCapital, companyCount, 30),
                Trader.IndexTrader(marketAtTime, startingCapital, companyCount, 90),
                Trader.IndexTrader(marketAtTime, startingCapital, companyCount, 365),
                Trader.IndexTrader(marketAtTime, startingCapital, companyCount, 365 * 10),
                Trader.IndexTrader(marketAtTime, startingCapital, companyCount, 365 * 50),*/

                //Trader.IndexTrader(marketAtTime, startingCapital, companyCount, 90),
                //Trader.IndexTrader2(marketAtTime, startingCapital, companyCount, 90),
                //Trader.IndexTrader3(marketAtTime, startingCapital, companyCount, 90),
                Trader.IndexTrader(marketAtTime, startingCapital, companyCount, 365 * 50),
            });

            InitializeComponent();
        }
    }
}
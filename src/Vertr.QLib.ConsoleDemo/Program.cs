using QuantLib;

using Vertr.QLib.ConsoleDemo;

internal static class Program
{
    private static void Main()
    {
        Date todaysDate = new Date(15, Month.September, 2024);
        Settings.instance().setEvaluationDate(todaysDate);

        using var demo = new EquityOptionDemo(
            Option.Type.Put,
            settlementDate: new Date(17, Month.September, 2024),
            maturityDate: new Date(17, Month.September, 2025),
            strikePrice: 40,
            underlyingPrice: 36,
            dividendYield: 0,
            riskFreeRate: 0.06,
            volatility: 0.2);

        demo.ReportParams();

        EquityOptionReportHelper.ReportHeadings();

        demo.BlackScholesForEuropean();
        demo.BaroneAdesiWhaleyForAmerican();
        demo.BjerksundStenslandForAmerican();
        demo.Integral();
        demo.FiniteDifferences();
        demo.BinomialJarrowRudd();
        demo.BinomialCoxRossRubinstein();
    }
}

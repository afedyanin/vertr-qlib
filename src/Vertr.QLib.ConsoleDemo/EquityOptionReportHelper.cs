using QuantLib;

namespace Vertr.QLib.ConsoleDemo;
internal static class EquityOptionReportHelper
{
    private static readonly int[] ColumnWidths = [35, 14, 14, 14];

    public static void ReportParameters(
        Option.Type optionType,
        double underlyingPrice,
        double strikePrice,
        double dividendYield,
        double riskFreeRate,
        double volatility,
        Date maturityDate)
    {
        Console.WriteLine();
        Console.WriteLine("Option type = {0}", optionType);
        Console.WriteLine("Maturity = {0} {1} {2}", maturityDate.year(), maturityDate.month(), maturityDate.dayOfMonth());
        Console.WriteLine("Underlying price = ${0}", underlyingPrice);
        Console.WriteLine("Strike = ${0}", strikePrice);
        Console.WriteLine("Risk-free interest rate = {0}%", riskFreeRate * 100.0);
        Console.WriteLine("Dividend yield = {0}%", dividendYield * 100.0);
        Console.WriteLine("Volatility = {0}%", volatility * 100);
        Console.WriteLine();
    }


    public static void ReportHeadings()
    {
        Console.Write("Method".PadRight(ColumnWidths[0]));
        Console.Write("European".PadRight(ColumnWidths[1]));
        Console.Write("Bermudan".PadRight(ColumnWidths[2]));
        Console.Write("American".PadRight(ColumnWidths[3]));
        Console.WriteLine();
    }

    public static void ReportResults(
        string methodName,
        double? european,
        double? bermudan,
        double? american)
    {
        string strNA = "N/A";
        string format = "{0:N6}";

        Console.Write(methodName.PadRight(ColumnWidths[0]));
        Console.Write(string.Format((european == null) ? strNA : format, european).PadRight(ColumnWidths[1]));
        Console.Write(string.Format((bermudan == null) ? strNA : format, bermudan).PadRight(ColumnWidths[2]));
        Console.Write(string.Format((american == null) ? strNA : format, american).PadRight(ColumnWidths[3]));
        Console.WriteLine();
    }
}

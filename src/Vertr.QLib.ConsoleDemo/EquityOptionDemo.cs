using QuantLib;

namespace Vertr.QLib.ConsoleDemo;
internal sealed class EquityOptionDemo : IDisposable
{
    private readonly BlackScholesMertonProcess _stochasticProcess;

    private readonly Option.Type _optionType;
    private readonly Date _settlementDate;
    private readonly Date _maturityDate;
    private readonly double _dividendYield;
    private readonly double _riskFreeRate;
    private readonly double _volatility;
    private readonly double _underlyingPrice;
    private readonly double _strikePrice;

    private readonly Calendar _calendar = new TARGET();
    private readonly PlainVanillaPayoff _payoff;

    private readonly EuropeanExercise _europeanExercise;
    private readonly AmericanExercise _americanExercise;
    private readonly BermudanExercise _bermudanExercise;

    public EquityOptionDemo(
        Option.Type optionType,
        Date settlementDate,
        Date maturityDate,
        double strikePrice,
        double underlyingPrice,
        double dividendYield,
        double riskFreeRate,
        double volatility)
    {
        _optionType = optionType;
        _settlementDate = settlementDate;
        _maturityDate = maturityDate;
        _dividendYield = dividendYield;
        _riskFreeRate = riskFreeRate;
        _volatility = volatility;
        _underlyingPrice = underlyingPrice;
        _strikePrice = strikePrice;

        _payoff = new PlainVanillaPayoff(_optionType, _strikePrice);

        _europeanExercise = new EuropeanExercise(_maturityDate);
        _americanExercise = new AmericanExercise(_settlementDate, _maturityDate);

        var exerciseDates = new DateVector(4);
        for (int i = 1; i <= 4; i++)
        {
            var forwardPeriod = new Period(3 * i, TimeUnit.Months);
            var forwardDate = settlementDate.Add(forwardPeriod);
            exerciseDates.Add(forwardDate);
        }

        _bermudanExercise = new BermudanExercise(exerciseDates);

        _stochasticProcess = InitStochasticProcess();
    }

    public void ReportParams()
    {
        EquityOptionReportHelper.ReportParameters(
            _optionType,
            _underlyingPrice,
            _strikePrice,
            _dividendYield,
            _riskFreeRate,
            _volatility,
            _maturityDate);
    }

    /// <summary>
    /// Black-Scholes for European
    /// </summary>
    public void BlackScholesForEuropean()
    {
        try
        {
            using var europeanOption = new VanillaOption(_payoff, _europeanExercise);
            europeanOption.setPricingEngine(new AnalyticEuropeanEngine(_stochasticProcess));

            EquityOptionReportHelper.ReportResults("Black-Scholes", europeanOption.NPV(), null, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    /// <summary>
    /// Barone-Adesi and Whaley approximation for American
    /// </summary>
    public void BaroneAdesiWhaleyForAmerican()
    {
        try
        {
            using var americanOption = new VanillaOption(_payoff, _americanExercise);
            americanOption.setPricingEngine(new BaroneAdesiWhaleyApproximationEngine(_stochasticProcess));

            EquityOptionReportHelper.ReportResults("Barone-Adesi/Whaley", null, null, americanOption.NPV());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    /// <summary>
    /// Bjerksund and Stensland approximation for American
    /// </summary>
    public void BjerksundStenslandForAmerican()
    {
        try
        {
            using var americanOption = new VanillaOption(_payoff, _americanExercise);
            americanOption.setPricingEngine(new BjerksundStenslandApproximationEngine(_stochasticProcess));

            EquityOptionReportHelper.ReportResults("Bjerksund/Stensland", null, null, americanOption.NPV());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    /// <summary>
    /// Integral
    /// </summary>
    public void Integral()
    {
        try
        {
            using var europeanOption = new VanillaOption(_payoff, _europeanExercise);
            europeanOption.setPricingEngine(new IntegralEngine(_stochasticProcess));

            EquityOptionReportHelper.ReportResults("Integral", europeanOption.NPV(), null, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    /// <summary>
    /// Finite differences
    /// </summary>
    public void FiniteDifferences()
    {
        uint timeSteps = 801;

        // Finite differences
        try
        {
            using var europeanOption = new VanillaOption(_payoff, _europeanExercise);
            europeanOption.setPricingEngine(new FdBlackScholesVanillaEngine(_stochasticProcess, timeSteps, timeSteps - 1));

            using var bermudanOption = new VanillaOption(_payoff, _bermudanExercise);
            bermudanOption.setPricingEngine(new FdBlackScholesVanillaEngine(_stochasticProcess, timeSteps, timeSteps - 1));

            using var americanOption = new VanillaOption(_payoff, _americanExercise);
            americanOption.setPricingEngine(new FdBlackScholesVanillaEngine(_stochasticProcess, timeSteps, timeSteps - 1));

            EquityOptionReportHelper.ReportResults("Finite differences",
                europeanOption.NPV(),
                bermudanOption.NPV(),
                americanOption.NPV());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    /// <summary>
    /// Binomial Jarrow-Rudd
    /// </summary>
    public void BinomialJarrowRudd()
    {
        try
        {
            uint timeSteps = 801;

            using var europeanOption = new VanillaOption(_payoff, _europeanExercise);
            europeanOption.setPricingEngine(
                new BinomialJRVanillaEngine(_stochasticProcess, timeSteps));

            using var bermudanOption = new VanillaOption(_payoff, _bermudanExercise);
            bermudanOption.setPricingEngine(
                new BinomialJRVanillaEngine(_stochasticProcess, timeSteps));

            using var americanOption = new VanillaOption(_payoff, _americanExercise);
            americanOption.setPricingEngine(
                new BinomialJRVanillaEngine(_stochasticProcess, timeSteps));

            EquityOptionReportHelper.ReportResults("Binomial Jarrow-Rudd",
                europeanOption.NPV(),
                bermudanOption.NPV(),
                americanOption.NPV());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    /// <summary>
    /// Binomial Cox-Ross-Rubinstein
    /// </summary>
    public void BinomialCoxRossRubinstein()
    {
        try
        {
            uint timeSteps = 801;

            using var europeanOption = new VanillaOption(_payoff, _europeanExercise);
            europeanOption.setPricingEngine(
               new BinomialCRRVanillaEngine(_stochasticProcess, timeSteps));

            using var bermudanOption = new VanillaOption(_payoff, _bermudanExercise);
            bermudanOption.setPricingEngine(
               new BinomialCRRVanillaEngine(_stochasticProcess, timeSteps));

            using var americanOption = new VanillaOption(_payoff, _americanExercise);
            americanOption.setPricingEngine(
               new BinomialCRRVanillaEngine(_stochasticProcess, timeSteps));

            EquityOptionReportHelper.ReportResults("Binomial Cox-Ross-Rubinstein",
                europeanOption.NPV(),
                bermudanOption.NPV(),
                americanOption.NPV());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private BlackScholesMertonProcess InitStochasticProcess()
    {
        // bootstrap the yield/dividend/vol curves and create a
        // BlackScholesMerton stochastic process
        DayCounter dayCounter = new Actual365Fixed();

        var flatRateTSH = new YieldTermStructureHandle(
            new FlatForward(_settlementDate, _riskFreeRate, dayCounter));

        var flatDividendTSH = new YieldTermStructureHandle(
            new FlatForward(_settlementDate, _dividendYield, dayCounter));

        var flatVolTSH = new BlackVolTermStructureHandle(
            new BlackConstantVol(_settlementDate, _calendar, _volatility, dayCounter));

        var underlyingQuoteH = new QuoteHandle(new SimpleQuote(_underlyingPrice));

        var stochasticProcess = new BlackScholesMertonProcess(
            underlyingQuoteH,
            flatDividendTSH,
            flatRateTSH,
            flatVolTSH);

        return stochasticProcess;
    }

    public void Dispose()
    {
        _calendar.Dispose();
    }
}

using QL = QuantLib;

namespace Vertr.QLib.Tests;

[TestFixture(Category = "Unit")]
public class TimesTests
{
    [Test]
    public void CanSortTenors()
    {
        var tenorNull = null as QL.Period;
        var tenor91D = new QL.Period("91D");
        var tenor03M = new QL.Period("03M");
        var tenor06M = new QL.Period("06M");
        var tenor12M = new QL.Period("12M");
        var tenor01Y = new QL.Period("01Y");
        var tenor02Y = new QL.Period("02Y");

#pragma warning disable CS8604 // Possible null reference argument.
        var periods = new List<QL.Period>() { tenor01Y, tenorNull, tenor02Y, tenor06M, tenor03M };
#pragma warning restore CS8604 // Possible null reference argument.

        periods.Sort();

        Assert.Multiple(() =>
        {
            Assert.That(periods[0], Is.EqualTo(tenorNull));
            Assert.That(periods[1], Is.EqualTo(tenor03M));
            Assert.That(periods[2], Is.EqualTo(tenor06M));
            Assert.That(periods[3], Is.EqualTo(tenor01Y));
            Assert.That(periods[4], Is.EqualTo(tenor02Y));
        });
    }

    [Test]
    public void CanCompareTenors()
    {
        var tenorNull = null as QL.Period;
        var tenor91D = new QL.Period("91D");
        var tenor03M = new QL.Period("03M");
        var tenor06M = new QL.Period("06M");
        var tenor12M = new QL.Period("12M");
        var tenor01Y = new QL.Period("01Y");
        var tenor02Y = new QL.Period("02Y");

        Assert.Multiple(() =>
        {
            Assert.That(tenor12M.CompareTo(tenorNull), Is.GreaterThan(0));
            Assert.That(tenor12M.CompareTo(tenor03M), Is.GreaterThan(0));
            Assert.That(tenor12M.CompareTo(tenor06M), Is.GreaterThan(0));
            Assert.That(tenor12M.CompareTo(tenor01Y), Is.EqualTo(0));
            Assert.That(tenor01Y.CompareTo(tenor01Y), Is.EqualTo(0));
            Assert.That(tenor12M.CompareTo(tenor02Y), Is.LessThan(0));
        });
    }
}

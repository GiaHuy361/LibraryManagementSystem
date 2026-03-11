namespace LMS.Application.Strategies;

/// <summary>
/// Strategy Pattern for fine calculation.
/// Allows different fine calculation algorithms to be swapped at runtime.
/// </summary>
public interface IFineCalculationStrategy
{
    decimal Calculate(int daysOverdue);
}

/// <summary>
/// Default implementation: flat rate per overdue day.
/// Rate is configurable via appsettings.
/// </summary>
public class DailyFineStrategy : IFineCalculationStrategy
{
    private readonly decimal _ratePerDay;

    public DailyFineStrategy(decimal ratePerDay = 5000m)
    {
        _ratePerDay = ratePerDay;
    }

    public decimal Calculate(int daysOverdue)
    {
        if (daysOverdue <= 0) return 0;
        return daysOverdue * _ratePerDay;
    }
}

/// <summary>
/// Progressive fine strategy: doubles rate after 7 days overdue.
/// </summary>
public class ProgressiveFineStrategy : IFineCalculationStrategy
{
    private readonly decimal _baseRate;
    private readonly decimal _doubleRate;

    public ProgressiveFineStrategy(decimal baseRate = 5000m, decimal doubleRate = 10000m)
    {
        _baseRate = baseRate;
        _doubleRate = doubleRate;
    }

    public decimal Calculate(int daysOverdue)
    {
        if (daysOverdue <= 0) return 0;
        if (daysOverdue <= 7) return daysOverdue * _baseRate;
        return (7 * _baseRate) + ((daysOverdue - 7) * _doubleRate);
    }
}

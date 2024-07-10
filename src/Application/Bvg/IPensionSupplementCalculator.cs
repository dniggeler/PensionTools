namespace Application.Bvg;

public interface IPensionSupplementCalculator
{
    decimal CalculatePensionSupplement(DateTime dateOfBirth, decimal finalRetirementCapital);

    bool IsBirthdateEligible(DateTime dateOfBirth);
}

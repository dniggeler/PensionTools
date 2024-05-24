namespace Application.Bvg;

public interface IPensionSupplementCalculator
{
    decimal CalculatePensionSupplement(DateTime dateOfBirth, decimal finalRetirementCapital);
}

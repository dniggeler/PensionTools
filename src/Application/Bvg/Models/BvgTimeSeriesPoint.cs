using Domain.Models.Bvg;

namespace Application.Bvg.Models;

public record BvgTimeSeriesPoint(TechnicalAge Age, DateTime Date, decimal Value);

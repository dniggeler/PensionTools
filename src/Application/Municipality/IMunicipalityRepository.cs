using Domain.Models.Municipality;

namespace Application.Municipality;

public interface IMunicipalityRepository
{
    IEnumerable<MunicipalityEntity> GetAll();

    MunicipalityEntity Get(int bfsNumber, int year);

    IEnumerable<MunicipalityEntity> GetAllSupportTaxCalculation();

    IEnumerable<MunicipalityEntity> Search(MunicipalitySearchFilter searchFilter);
}

using System;
using System.Linq;
using Tax.Data;

namespace Tax.AppConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dbContext = new TaxRateDbContext())
            {
                foreach (var taxItem in dbContext.Blogs.Where(item => item.Canton=="ZH" && item.Year==2017))
                {
                    Console.WriteLine($"{taxItem.Municipality}: {taxItem.TaxRateMunicipality}");
                }
            }
        }
    }
}

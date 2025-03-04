using Aspose.Cells;
using Domain.Enums;
using TaxType = Domain.Enums.TaxType;
using FlowType = Domain.Enums.FlowType;

namespace Application.MultiPeriodCalculator.ExcelManager;

public class ExcelCashFlowManager
{
    private const int WorkSheetPerson = 0;
    private const int WorkSheetTaxMunicipality = 1;
    private const int WorkSheetAccount = 3;
    private const int WorkSheetCashFlow = 4;
    private const int WorkSheetTaxAction = 7;

    public static void WriteTransactions(string workbookName, IEnumerable<ExcelResponseTransaction> transactions)
    {
        using Workbook workbook = new Workbook(workbookName);
        // add a new worksheet
        Worksheet worksheet = workbook.Worksheets.Add("Transktionen");
        // add header to wo
        worksheet.Cells[0, 0].PutValue("Konto");
        worksheet.Cells[0, 1].PutValue("Konto-Typ");
        worksheet.Cells[0, 2].PutValue("Konto-Name");

        foreach (ExcelResponseTransaction trx in transactions)
        {
            // add the transaction to the worksheet
            worksheet.Cells[worksheet.Cells.MaxDataRow + 1, 0].PutValue(trx.AccountId);
            worksheet.Cells[worksheet.Cells.MaxDataRow, 1].PutValue(trx.AccountType);
            worksheet.Cells[worksheet.Cells.MaxDataRow, 2].PutValue(trx.AccountName);
        }

        // save the workbook
        workbook.Save(workbookName);
    }

    public static IEnumerable<ExcelTaxMunicipality> ReadTaxMunicipalities(string workbookName)
    {
        Workbook workbook = new Workbook(workbookName);
        Worksheet worksheet = workbook.Worksheets[WorkSheetTaxMunicipality];
        Cells cells = worksheet.Cells;
        
        List<ExcelTaxMunicipality> taxMunicipalityList = [];

        int rowCount = cells.MaxDataRow + 1;
        for (int i = 1; i < rowCount; i++)
        {
            int municipalityId = cells[i, 0].IntValue;
            int taxLocationId = cells[i, 1].IntValue;
            string canton = cells[i, 2].StringValue;

            var excelTaxMunicipality = Create(municipalityId, taxLocationId, canton);
            if (excelTaxMunicipality is null)
            {
                continue;
            }
            taxMunicipalityList.Add(excelTaxMunicipality);
        }
        return taxMunicipalityList;
    }

    public static IEnumerable<ExcelPerson> ReadPersons(string workbookName)
    {
        Workbook workbook = new Workbook(workbookName);
        Worksheet worksheet = workbook.Worksheets[WorkSheetPerson];
        Cells cells = worksheet.Cells;
        List<ExcelPerson> personList = [];

        int rowCount = cells.MaxDataRow + 1;
        for (int i = 1; i < rowCount; i++)
        {
            int number = cells[i, 0].IntValue;
            string name = cells[i, 1].StringValue;
            string birthdate = cells[i, 2].StringValue;
            string civilStatus = cells[i, 3].StringValue;
            string gender = cells[i, 4].StringValue;
            string religiousGroupType = cells[i, 5].StringValue;
            string partnerReligiousGroupType = cells[i, 6].StringValue;

            var excelPerson = Create(number, name, birthdate, civilStatus, gender, religiousGroupType, partnerReligiousGroupType);
            if (excelPerson is null)
            {
                continue;
            }

            personList.Add(excelPerson);
        }

        return personList;
    }

    public static IEnumerable<ExcelAccount> ReadAccounts(string workbookName)
    {
        Workbook workbook = new Workbook(workbookName);
        Worksheet worksheet = workbook.Worksheets[WorkSheetAccount];
        Cells cells = worksheet.Cells;

        List<ExcelAccount> definitionList = [];

        int rowCount = cells.MaxDataRow + 1;
        for (int i = 1; i < rowCount; i++)
        {
            string counter = cells[i, 0].StringValue;
            string accountName = cells[i, 1].StringValue;
            string accountType = cells[i, 2].StringValue;

            var excelAccountDefinition = Create(counter, accountName, accountType);
            if (excelAccountDefinition is null)
            {
                continue;
            }

            definitionList.Add(excelAccountDefinition);
        }

        return definitionList;
    }

    public static IEnumerable<ExcelFixAmountCashFlow> ReadCashFlows(string workbookName)
    {

        Workbook workbook = new Workbook(workbookName);
        Worksheet worksheet = workbook.Worksheets[WorkSheetCashFlow];
        Cells cells = worksheet.Cells;

        List<ExcelFixAmountCashFlow> cashFlowList = [];

        int rowCount = cells.MaxDataRow + 1;
        for (int i = 1; i < rowCount; i++)
        {
            string processDate = cells[i, 0].StringValue;
            string description = cells[i, 1].StringValue;
            string debitAccountNumber = cells[i, 2].StringValue;
            string creditAccountNumber = cells[i, 3].StringValue;
            string amount = cells[i, 4].StringValue;
            string taxType = cells[i, 5].StringValue;
            string flowType = cells[i, 6].StringValue;

            var excelCashFlow = Create(processDate, debitAccountNumber, creditAccountNumber, description, amount, taxType, flowType);
            if (excelCashFlow is null)
            {
                continue;
            }

            cashFlowList.Add(excelCashFlow);
        }

        return cashFlowList;
    }

    public static IEnumerable<ExcelTaxAction> ReadTaxActions(string workbookName)
    {
        Workbook workbook = new Workbook(workbookName);
        Worksheet worksheet = workbook.Worksheets[WorkSheetTaxAction];
        Cells cells = worksheet.Cells;

        List<ExcelTaxAction> actions = [];

        int rowCount = cells.MaxDataRow + 1;
        for (int i = 1; i < rowCount; i++)
        {
            var row = cells.Rows[i];
            string periodBeginDate = cells[i, 0].StringValue;
            string periodEndDate = cells[i, 1].StringValue;
            string kindOfPeriod = cells[i, 2].StringValue;
            string description = cells[i, 3].StringValue;
            int? debitAccountNumber = row.GetCellOrNull(4)?.IntValue;
            int? creditAccountNumber = row.GetCellOrNull(5)?.IntValue;
            string taxType = cells[i, 6].StringValue;

            ExcelTaxAction excelAction = Create(periodBeginDate, periodEndDate, debitAccountNumber, creditAccountNumber, description, taxType);
            if (excelAction is null)
            {
                continue;
            }

            actions.Add(excelAction);
        }

        return actions;
    }

    public static Guid StringToGuid(string input)
    {
        // Convert the input string to a byte array
        byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

        // Create a byte array to hold the GUID components
        byte[] guidBytes = new byte[16];

        // Copy the input bytes into the GUID byte array
        for (int i = 0; i < Math.Min(inputBytes.Length, guidBytes.Length); i++)
        {
            guidBytes[i] = inputBytes[i];
        }

        // Create the GUID from the byte array
        return new Guid(guidBytes);
    }

    public static Guid IntToGuid(int input)
    {
        return StringToGuid(input.ToString());
    }

    private static ExcelPerson Create(
        int? counter,
        string name,
        string birthdateString,
        string civilStatusString,
        string genderString,
        string religiousGroupTypeString,
        string partnerReligiousGroupTypeString)
    {
        if (!counter.HasValue)
        {
            return null;
        }

        string personName = name;

        if (string.IsNullOrEmpty(name))
        {
            personName = $"Test-{counter}";
        }

        if (!DateOnly.TryParse(birthdateString, out DateOnly birthdate))
        {
            birthdate = new DateOnly(DateTime.Today.Year - 50, 7, 1);
        }

        if (string.IsNullOrEmpty(birthdateString))
        {
            personName = $"Test-{counter}";
        }

        CivilStatus civilStatus = civilStatusString switch
        {
            "Ledig" => CivilStatus.Single,
            "Verheiratet" => CivilStatus.Married,

            _ => throw new ArgumentException(nameof(CivilStatus))
        };

        Gender gender = genderString switch
        {
            "M" => Gender.Male,
            "W" => Gender.Female,
            _ => Gender.Undefined
        };

        ReligiousGroupType religiousGroupType = religiousGroupTypeString switch
        {
            "Katholisch" => ReligiousGroupType.Catholic,
            "Reformiert" => ReligiousGroupType.Protestant,
            "Andere" or "Keine" => ReligiousGroupType.Other,
            _ => throw new ArgumentException(nameof(religiousGroupType))
        };

        ReligiousGroupType partnerReligiousGroupType = partnerReligiousGroupTypeString switch
        {
            "Katholisch" => ReligiousGroupType.Catholic,
            "Reformiert" => ReligiousGroupType.Protestant,
            "Andere" or "Keine" => ReligiousGroupType.Other,
            _ => ReligiousGroupType.Other
        };

        return new ExcelPerson(counter, personName, birthdate, civilStatus, gender, religiousGroupType, partnerReligiousGroupType);
    }

    private static ExcelTaxMunicipality Create(int municipalityId, int taxLocationId, string cantonString)
    {
        if (string.IsNullOrEmpty(cantonString))
        {
            return null;
        }

        Canton canton = Enum.Parse<Canton>(cantonString);

        return new ExcelTaxMunicipality(municipalityId, taxLocationId, canton);
    }

    private static ExcelAccount Create(string counter, string accountName, string accountTypeName)
    {
        if (string.IsNullOrEmpty(accountTypeName))
        {
            return null;
        }

        AccountType accountType = accountTypeName switch
        {
            "Lohn" => AccountType.Income,
            "Vermögen" => AccountType.Wealth,
            "Exogenous" => AccountType.Exogenous,
            "3a" => AccountType.ThirdPillar,
            "PK" => AccountType.OccupationalPension,

            _ => throw new ArgumentException(nameof(accountTypeName))
        };

        return new ExcelAccount(StringToGuid(counter), accountName ?? "na", accountType);
    }

    private static ExcelFixAmountCashFlow Create(
        string processDateString,
        string debitAccountNumberString,
        string creditAccountNumberString,
        string description,
        string amountString,
        string taxTypeString,
        string taxFlowTypeString)
    {
        if (string.IsNullOrEmpty(description) ||
            debitAccountNumberString is null ||
            creditAccountNumberString is null)
        {
            return null;
        }

        if (!DateOnly.TryParse(processDateString, out DateOnly processDate))
        {
            return null;
        }

        if (!int.TryParse(debitAccountNumberString, out int debitAccountNumber))
        {
            return null;
        }

        if (!int.TryParse(creditAccountNumberString, out int creditAccountNumber))
        {
            return null;
        }

        if (!decimal.TryParse(amountString, out decimal amount))
        {
            amount = decimal.Zero;
        }

        TaxType taxType = MapTaxType(taxTypeString);

        FlowType flowType = taxFlowTypeString switch
        {
            "IN" => FlowType.InFlow,
            "OUT" => FlowType.OutFlow,
            _ => FlowType.Undefined
        };

        return new ExcelFixAmountCashFlow(
            processDate,
            IntToGuid(debitAccountNumber),
            IntToGuid(creditAccountNumber),
            description,
            amount,
            taxType,
            flowType);
    }

    private static ExcelTaxAction Create(
        string periodBeginDateString,
        string periodEndDateString,
        int? debitAccountNumber,
        int? creditAccountNumber,
        string description,
        string taxTypeString)
    {
        if (string.IsNullOrEmpty(periodEndDateString) ||
            debitAccountNumber is null ||
            creditAccountNumber is null)
        {
            return null;
        }

        if (!DateOnly.TryParse(periodBeginDateString, out DateOnly periodBeginDate))
        {
            periodBeginDate = DateOnly.MinValue;
        }

        if (!DateOnly.TryParse(periodEndDateString, out DateOnly periodEndDate))
        {
            return null;
        }

        TaxType taxType = MapTaxType(taxTypeString);

        return new ExcelTaxAction(
            IntToGuid(debitAccountNumber.Value),
            IntToGuid(creditAccountNumber.Value),
            description,
            periodBeginDate,
            periodEndDate,
            taxType);
    }

    private static TaxType MapTaxType(string taxTypeString)
    {
        TaxType taxType = taxTypeString switch
        {
            "Einkommen" => TaxType.Income,
            "Vermögen" => TaxType.Wealth,
            "Kapitalbezug" => TaxType.CapitalBenefits,
            _ => TaxType.None
        };
        return taxType;
    }
}

namespace MyBudgetTracker.Models
{
    public class UserDataModel
    {
        public string? UserId { get; set; }
        public object? UserMasterData { get; set; }
    }
    public class UserMasterData
    {
        public List<string>? BudgetingCategorie { get; set; }
        public List<int>? BudgetCategoriePercentage { get; set; }
        public List<string>? IncomeCategories { get; set; }
        public List<SpendData>? TotalSpend { get; set; }
        public List<IncomeData>? TotalIncome { get; set; }
        public string? SelectedCurrency { get; set; }
    }

    public class SpendData
    {
        public DateTime? Date { get; set; }
        public string? Notes { get; set; }
        public string? Category { get; set; }
        public decimal? Amount { get; set; }
        public string? Source { get; set; }
    }

    public class IncomeData
    {
        public DateTime? DateTime { get; set; }
        public string? Notes { get; set; }
        public string? Category { get; set; }
        public decimal? Amount { get; set; }
    }

    public class BudgetDataRequest
    {
        public string? UserData { get; set; }
        public UserMasterData? UserMasterData { get; set; }
    }

}

using System.Collections.Generic;
public static class CurrencyFormatter 
{
    private static readonly string[] Units =
    {
        "", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L",
        "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
        "AA", "AB", "AC"
    };

    public static string Format(double amount)
    {
        if (amount < 100d)
            return amount.ToString("F0");

        int unitIndex = 0;
        double tempAmount = amount;
      
        while (tempAmount >= 100 && unitIndex < Units.Length - 1)
        {
            tempAmount /= 100;
            unitIndex++;
        }

        return $"{tempAmount:F2}{Units[unitIndex]}";
    }
}

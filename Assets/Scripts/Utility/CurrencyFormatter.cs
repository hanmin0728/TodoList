using System.Collections.Generic;
public static class CurrencyFormatter 
{
    private static readonly string[] Units =
       {
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L",
        "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
        "AA", "AB", "AC"
    };

    public static string Format(double amount)
    {
        int unitIndex = 0;
        double tempAmount = amount;
      
        // 100으로 나눠가며 나눌때마다 알파벳을 올림
        while (tempAmount >= 100 && unitIndex < Units.Length - 1)
        {
            tempAmount /= 100;
            unitIndex++;
        }

        return $"{tempAmount:F2}{Units[unitIndex]}";
    }
}

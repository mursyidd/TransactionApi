namespace TransactionApi.Utils;

public static class DiscountHelper
{
    public static long CalculateDiscount(long total)
    {
        double discountRate = 0;

        if (total >= 20000 && total <= 50000) discountRate += 0.05;
        else if (total <= 80000) discountRate += 0.07;
        else if (total <= 120000) discountRate += 0.10;
        else if (total > 120000) discountRate += 0.15;

        if (total > 50000 && IsPrime(total)) discountRate += 0.08;
        if (total > 90000 && total % 10 == 5) discountRate += 0.10;

        if (discountRate > 0.20) discountRate = 0.20;

        return (long)(total * discountRate);
    }

    private static bool IsPrime(long number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;
        var boundary = (long)Math.Sqrt(number);
        for (long i = 3; i <= boundary; i += 2)
            if (number % i == 0) return false;
        return true;
    }
}
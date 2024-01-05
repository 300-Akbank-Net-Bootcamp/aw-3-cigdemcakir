namespace Vb.Base.Common;

public static class CommonValidators
{
    public static bool HaveValidScale(decimal amount)
    {
        var fractionalPart = amount - Math.Floor(amount);
        var scale = BitConverter.GetBytes(decimal.GetBits(fractionalPart)[3])[2];
        return scale <= 4; 
    }

    public static bool HaveValidPrecision(decimal amount)
    {
        var precision = Math.Floor(amount).ToString().Length + BitConverter.GetBytes(decimal.GetBits(amount)[3])[2];
        return precision <= 18; 
    }
}
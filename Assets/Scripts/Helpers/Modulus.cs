public static partial class Helpers
{
    /// <summary>
    /// performs a % b, this is different from C# % which is remainder, and does not
    /// always return a positive, this will behave identically with positive remainders
    /// </summary>
    /// <param name="a">Divisor</param>
    /// <param name="b">Dividend</param>
    /// <returns>The result of a % b</returns>
    public static float Modulo(float a, float b)
    {
        return (a % b + b) % b;//https://stackoverflow.com/a/51018529, legal for me to use becuase stack overflow contributuions is liscences under CC-BY-SA, the link is suficient attributuion, however i now must license this work under CC-BY-SA
    }

    /// <summary>
    /// Returns the absoloute value of a
    /// </summary>
    /// <param name="a">the number to absoloute</param>
    /// <returns>the absoloute value of a</returns>
    public static float Abs(float a)
    {
        return a >= 0 ? a : -1 * a;
    }
}

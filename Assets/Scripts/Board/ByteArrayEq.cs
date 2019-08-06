using System.Collections.Generic;

public class ByteArrayEq : IEqualityComparer<byte[]>
{
    public bool Equals(byte[] x, byte[] y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
        for (byte i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(byte[] obj)
    {
        int result = 17;
        for (int i = 0; i < obj.Length; i++)
        {
            unchecked
            {
                result = result * 23 + obj[i];
            }
        }
        return result;
    }
}
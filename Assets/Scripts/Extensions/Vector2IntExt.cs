using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SerializableVector2Int
{
   private Vector2Int _vector2Int;
   public Vector2Int Vector2Int { get => _vector2Int; }

   public SerializableVector2Int(int x, int y)
   {
      _vector2Int = new Vector2Int(x, y);
   }

   public static object Deserialize(byte[] data)
   {
      return new SerializableVector2Int(BitConverter.ToInt32(data, 0), BitConverter.ToInt32(data, 4));
   }

   public static byte[] Serialize(object customType)
   {
      var vec = (SerializableVector2Int) customType;

      byte[] x = BitConverter.GetBytes(vec._vector2Int.x);
      byte[] y = BitConverter.GetBytes(vec._vector2Int.y);

      byte[] byteVec = new byte[x.Length + y.Length];
      System.Buffer.BlockCopy(x, 0, byteVec, 0, x.Length);
      System.Buffer.BlockCopy(y, 0, byteVec, x.Length, y.Length);

      return byteVec;
   }
}

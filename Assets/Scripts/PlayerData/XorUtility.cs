using UnityEngine;

public class XorUtility : MonoBehaviour {
    public static byte[] XorEncrypt( byte[] data, byte key = 0xAA ) {
        byte[] result = new byte[data.Length];
        for ( int i = 0; i < data.Length; i++ ) {
            result[i] = (byte)(data[i] ^ key);
        }
        return result;
    }
}

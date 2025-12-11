using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Crypto
{
    private const string AES_KEY = "";
    private const string AES_IV = "";

    private static readonly byte[] Key;
    private static readonly byte[] IV;

    static Crypto()
    {
        Key = Encoding.UTF8.GetBytes(AES_KEY);
        IV = Encoding.UTF8.GetBytes(AES_IV);
    }
    
    
    public static string EncryptSha256(string input)
    {
        StringBuilder sb = new StringBuilder();
        
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"{input}:X2"));

            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
        }
        
        return sb.ToString();
    }

    public static byte[] EncryptAes(string input)
    {
        try
        {
            using Aes aes = Aes.Create();
            
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using StreamWriter sw = new StreamWriter(cs);
            
            sw.Write(input);

            return ms.ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public static string DecryptAes(byte[] data)
    {
        try
        {
            using Aes aes = Aes.Create();
            
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            
            using MemoryStream ms = new MemoryStream(data);
            using CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs);
            
            return sr.ReadToEnd();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}

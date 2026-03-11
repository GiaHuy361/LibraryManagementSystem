using System;
using System.IO;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        string pw = "Admin@123";
        string hash = BCrypt.Net.BCrypt.HashPassword(pw);
        File.WriteAllText("hash.txt", hash);
    }
}

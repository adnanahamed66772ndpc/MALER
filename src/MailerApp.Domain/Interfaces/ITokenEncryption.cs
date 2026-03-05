namespace MailerApp.Domain.Interfaces;

public interface ITokenEncryption
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

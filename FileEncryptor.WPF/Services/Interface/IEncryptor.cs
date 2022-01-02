namespace FileEncryptor.WPF.Services.Interface
{
    interface IEncryptor
    {
        void Encrypt(string SourcePath, string DestinationPath, string Password, int BuffeLength = 104200);
        bool Dencrypt(string SourcePath, string DestinationPath, string Password, int BuffeLength = 104200);
    }
}
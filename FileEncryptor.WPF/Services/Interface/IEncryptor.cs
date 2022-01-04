﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileEncryptor.WPF.Services.Interface
{
    interface IEncryptor
    {
        void Encrypt(string SourcePath, string DestinationPath, string Password, int BuffeLength = 104200);
        bool Dencrypt(string SourcePath, string DestinationPath, string Password, int BuffeLength = 104200);

        Task EncryptAsync(string SourcePath, string DestinationPath, string Password, int BufferLength = 104200, IProgress<double> Progress = null, CancellationToken Cancel = default);

        Task<bool> DencryptAsync(string SourcePath, string DestinationPath, string Password, int BuffeLength = 104200, IProgress<double> Progress = null, CancellationToken Cancel = default);
    }
}
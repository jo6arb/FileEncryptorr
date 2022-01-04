﻿using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using FileEncryptor.WPF.Services.Interface;

namespace FileEncryptor.WPF.Services
{
    internal class Rfc2898Encryptor : IEncryptor
    {
        private static readonly byte[] __Salt =
        {
            0x26, 0xdc, 0xff, 0x00,
            0xad, 0xed, 0x7a, 0xee,
            0xc5, 0xfe, 0x07, 0xaf,
            0x4d, 0x08, 0x22, 0x3c
        };

        private static ICryptoTransform GetEncryptor(string password, byte[] Slat = null)
        {
            var pdb = new Rfc2898DeriveBytes(password, Slat ?? __Salt);
            var algorithm = Rijndael.Create();
            algorithm.Key = pdb.GetBytes(32);
            algorithm.IV = pdb.GetBytes(16);
            return algorithm.CreateEncryptor();
        }

        private static ICryptoTransform GetDecryptor(string password, byte[] Slat = null)
        {
            var pdb = new Rfc2898DeriveBytes(password, Slat ?? __Salt);
            var algorithm = Rijndael.Create();
            algorithm.Key = pdb.GetBytes(32);
            algorithm.IV = pdb.GetBytes(16);
            return algorithm.CreateDecryptor();
        }

        public void Encrypt(string SourcePath, string DestinationPath, string Password, int BuffeLength = 104200)
        {
            var encryptor = GetEncryptor(Password);

            using var destination_encrypted = File.Create(DestinationPath, BuffeLength);
            using var destination = new CryptoStream(destination_encrypted, encryptor, CryptoStreamMode.Write);
            using var source = File.OpenRead(SourcePath);

            var buffer = new byte[BuffeLength];
            int readed;
            do
            {
                readed = source.Read(buffer, 0, BuffeLength);
                destination.Write(buffer, 0, readed);
            }
            while (readed > 0);
            destination.FlushFinalBlock();
        }

        public bool Dencrypt(string SourcePath, string DestinationPath, string Password, int BufferLength = 104200)
        {
            var decryptor = GetDecryptor(Password);

            using var destination_decrypted = File.Create(DestinationPath, BufferLength);
            using var destination = new CryptoStream(destination_decrypted, decryptor, CryptoStreamMode.Write);
            using var encrypted_source = File.OpenRead(SourcePath);

            var buffer = new byte[BufferLength];
            int readed;
            do
            {
                readed = encrypted_source.Read(buffer, 0, BufferLength);
                destination.Write(buffer, 0, readed);
            }
            while (readed > 0);

            try
            {
                destination.FlushFinalBlock();
            }
            catch (CryptographicException)
            {
                return false;
            }

            return true;
        }

        public async Task EncryptAsync(
            string SourcePath,
            string DestinationPath,
            string Password,
            int BufferLength = 104200,
            IProgress<double> Progress = null,
            CancellationToken Cansel = default)
        {

            if (!File.Exists(SourcePath)) throw new FileNotFoundException("Файл-источник для процесса шифрования не найден", SourcePath);
            if (BufferLength <= 0) throw new ArgumentOutOfRangeException(nameof(BufferLength), BufferLength, "Размер буфера чтения должен быть больше 0");

            Cansel.ThrowIfCancellationRequested();

            var encryptor = GetEncryptor(Password);

            try
            {
                await using var destination_encrypted = File.Create(DestinationPath, BufferLength);
                await using var destination = new CryptoStream(destination_encrypted, encryptor, CryptoStreamMode.Write);
                await using var source = File.OpenRead(SourcePath);

                var file_length = source.Length;

                var buffer = new byte[BufferLength];
                int readed;
                var last_percent = 0.0;
                do
                {
                    readed = await source.ReadAsync(buffer, 0, BufferLength, Cansel).ConfigureAwait(false);
                    await destination.WriteAsync(buffer, 0, readed, Cansel).ConfigureAwait(false);

                    var position = source.Position;

                    var percent = (double)position / file_length;
                    if (percent - last_percent >= 0.001)
                    {
                        Progress?.Report(percent);
                        last_percent = percent;
                    }
                    
                    if (Cansel.IsCancellationRequested)
                    {
                        Cansel.ThrowIfCancellationRequested();
                    }
                }
                while (readed > 0);

                destination.FlushFinalBlock();

                Progress?.Report(1);
            }
            catch (OperationCanceledException)
            {
                File.Delete(DestinationPath);
                throw;
            }
            catch (Exception error)
            {
                Debug.WriteLine("Ошибка в EncryptAsync: \r\n{0}", error);
                throw;
            }
        }

        public async Task<bool> DencryptAsync(string SourcePath,
            string DestinationPath,
            string Password,
            int BufferLength = 104200,
            IProgress<double> Progress = null,
            CancellationToken Cansel = default)
        {
            if (!File.Exists(SourcePath)) throw new FileNotFoundException("Файл-источник для процесса дешифрования не найден", SourcePath);
            if (BufferLength <= 0) throw new ArgumentOutOfRangeException(nameof(BufferLength), BufferLength, "Размер буфера чтения должен быть больше 0");

            Cansel.ThrowIfCancellationRequested();

            var decryptor = GetDecryptor(Password);

            try
            {
                await using var destination_decrypted = File.Create(DestinationPath, BufferLength);
                await using var destination = new CryptoStream(destination_decrypted, decryptor, CryptoStreamMode.Write);
                await using var encrypted_source = File.OpenRead(SourcePath);

                var file_length = encrypted_source.Length;

                var buffer = new byte[BufferLength];
                int readed;
                var last_percent = 0.0;
                do
                {
                    readed = await encrypted_source.ReadAsync(buffer, 0, BufferLength, Cansel).ConfigureAwait(false);
                    await destination.WriteAsync(buffer, 0, readed, Cansel).ConfigureAwait(false);

                    var position = encrypted_source.Position;

                    var percent = (double)position / file_length;
                    if (percent - last_percent >= 0.001)
                    {
                        Progress?.Report(percent);
                        last_percent = percent;
                    }

                    Cansel.ThrowIfCancellationRequested();
                
                }
                while (readed > 0);

                try
                {
                    destination.FlushFinalBlock();
                }
                catch (CryptographicException)
                {
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                File.Delete(DestinationPath);
                throw;
            }
            catch (Exception error)
            {
                Debug.WriteLine("Ошибка в DecryprAsynk: \r\n {0}",error);
                throw;
            }

            return true;
        }
    }
}
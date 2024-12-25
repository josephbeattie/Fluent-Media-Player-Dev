using System;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Rise.Common.Extensions
{
    public static class CryptographyExtensions
    {
        /// <summary>
        /// Gets an encoded hash from a string using the specified
        /// algorithm.
        /// </summary>
        /// <param name="str">String to hash.</param>
        /// <param name="alg">Algorithm to use. Must be a valid value
        /// from <see cref="HashAlgorithmNames"/>.</param>
        /// <returns>The encoded hash as a hexadecimal string.</returns>
        public static string GetEncodedHash(this string str, string alg)
        {
            // Convert the message string to binary data
            var utf8Buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);

            // Create a HashAlgorithmProvider using the specified algorithm
            var algProvider = HashAlgorithmProvider.OpenAlgorithm(alg);

            // Hash the message
            var hashBuff = algProvider.HashData(utf8Buff);

            // Verify that the hash length equals the length specified for the algorithm
            if (hashBuff.Length != algProvider.HashLength)
                throw new Exception("There was an error creating the hash");

            // Convert the hash to a string and return it
            return CryptographicBuffer.EncodeToHexString(hashBuff);
        }
    }
}

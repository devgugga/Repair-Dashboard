using System.Security.Cryptography;
using System.Text;

using Konscious.Security.Cryptography;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Server.Domain.Interfaces.Services.Security;
using Server.Domain.ValueObjects.Security;

namespace Server.Infrastructure.Services.Security;

public class Argon2PasswordHashService(
    IOptions<PasswordOptions> options,
    ILogger<Argon2PasswordHashService> logger)
    : IPasswordHashService
{
    private readonly PasswordOptions _options = options.Value;

    public async Task<string> HashPasswordAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        try
        {
            // Gerar salt aleatório
            byte[] salt = GenerateRandomSalt();

            // Configurar Argon2id
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = _options.DegreeOfParallelism,
                MemorySize = _options.MemorySize,
                Iterations = _options.Iterations
            };

            // Gerar hash
            byte[]? hash = await argon2.GetBytesAsync(_options.HashLength);

            // Retornar no formato: $argon2id$v=19$m=65536,t=3,p=4$salt$hash
            string result = FormatHash(salt, hash);

            logger.LogDebug("Password hashed successfully with Argon2id");
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error hashing password");
            throw;
        }
    }

    public async Task<bool> VerifyPasswordAsync(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            // Parse do hash para extrair parâmetros
            (byte[] salt, byte[] storedHash, int memorySize, int iterations, int parallelism) = ParseHash(hash);

            // Recriar hash com os mesmos parâmetros
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt, DegreeOfParallelism = parallelism, MemorySize = memorySize, Iterations = iterations
            };

            byte[]? computedHash = await argon2.GetBytesAsync(storedHash.Length);

            // Comparação constante para evitar timing attacks
            bool result = ConstantTimeEquals(computedHash, storedHash);

            logger.LogDebug("Password verification completed: {Result}", result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error verifying password");
            return false;
        }
    }

    public bool NeedsRehash(string hash)
    {
        try
        {
            (_, _, int memorySize, int iterations, int parallelism) = ParseHash(hash);

            return memorySize != _options.MemorySize ||
                   iterations != _options.Iterations ||
                   parallelism != _options.DegreeOfParallelism;
        }
        catch
        {
            // Se não conseguir fazer parse, precisa de rehash
            return true;
        }
    }

    private byte[] GenerateRandomSalt()
    {
        byte[] salt = new byte[_options.SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }

    private string FormatHash(byte[] salt, byte[] hash)
    {
        string saltBase64 = Convert.ToBase64String(salt);
        string hashBase64 = Convert.ToBase64String(hash);

        return
            $"$argon2id$v=19$m={_options.MemorySize},t={_options.Iterations},p={_options.DegreeOfParallelism}${saltBase64}${hashBase64}";
    }

    private (byte[] salt, byte[] hash, int memorySize, int iterations, int parallelism) ParseHash(string hashedPassword)
    {
        string[] parts = hashedPassword.Split('$');

        if (parts.Length != 6 || parts[1] != "argon2id" || parts[2] != "v=19")
            throw new FormatException("Invalid hash format");

        // Parse parâmetros: m=65536,t=3,p=4
        string[] parameters = parts[3].Split(',');
        int memorySize = int.Parse(parameters[0].Substring(2)); // Remove "m="
        int iterations = int.Parse(parameters[1].Substring(2)); // Remove "t="
        int parallelism = int.Parse(parameters[2].Substring(2)); // Remove "p="

        byte[] salt = Convert.FromBase64String(parts[4]);
        byte[] hash = Convert.FromBase64String(parts[5]);

        return (salt, hash, memorySize, iterations, parallelism);
    }

    /// <summary>
    ///     Comparação em tempo constante para evitar timing attacks
    /// </summary>
    private static bool ConstantTimeEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        int result = 0;
        for (int i = 0; i < a.Length; i++) result |= a[i] ^ b[i];

        return result == 0;
    }
}

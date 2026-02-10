namespace Server.Domain.Interfaces.Services.Security;

public interface IPasswordHashService
{
    /// <summary>
    ///     Gera hash da senha usando Argon2id
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <returns>Hash da senha (inclui salt e parâmetros)</returns>
    Task<string> HashPasswordAsync(string password);

    /// <summary>
    ///     Verifica se a senha corresponde ao hash
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <param name="hash">Hash armazenado</param>
    /// <returns>True se a senha estiver correta</returns>
    Task<bool> VerifyPasswordAsync(string password, string hash);

    /// <summary>
    ///     Verifica se o hash precisa ser atualizado (parâmetros mudaram)
    /// </summary>
    /// <param name="hash">Hash atual</param>
    /// <returns>True se precisar rehash</returns>
    bool NeedsRehash(string hash);
}

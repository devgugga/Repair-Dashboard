using System.ComponentModel.DataAnnotations.Schema;

using Server.Domain.Enums.Core;

namespace Server.Domain.Entities.Core;

[Table("Persons")]
public class Person : BaseEntity
{
    public string Name { get; set; }

    public string Document { get; set; }

    public DocumentType DocumentType { get; set; }

    // Só deve ser preenchido caso documentType seja Cpf
    public DateTimeOffset? BirthDate { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string Country { get; set; }

    public string State { get; set; }

    public string City { get; set; }

    public string ZipCode { get; set; }

    public string Address { get; set; }

    public string AddressNumber { get; set; }

    public string? AddressComplement { get; set; }

    public virtual User? User { get; set; }
    public virtual Client? Client { get; set; }
}

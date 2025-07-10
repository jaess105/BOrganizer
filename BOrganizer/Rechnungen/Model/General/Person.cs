namespace Rechnungen.Model.General;

public record Person(long? Id, string FirstName, string LastName)
{
    public Person(string firstName, string lastName) : this(null, firstName, lastName)
    {
    }

    public string FullName => $"{FirstName} {LastName}";
}
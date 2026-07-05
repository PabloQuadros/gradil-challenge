using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace GradilChallenge.Domain.Common;

public abstract class Enumeration : IEquatable<Enumeration>, IComparable<Enumeration>
{
    public int Id { get; }
    public string Name { get; }

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static bool TryFromId<T>(int id, string conceptName,
    [NotNullWhen(true)] out T? value,
    [NotNullWhen(false)] out DomainError? error) where T : Enumeration
    {
        value = GetAll<T>().SingleOrDefault(item => item.Id == id);
        error = value is null
            ? new DomainError($"{typeof(T).Name.ToLowerInvariant()}.invalid",
                              $"'{id}' não é um(a) {conceptName} válido(a).")
            : null;
        return value is not null;
    }

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(
            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        return fields.Select(field => field.GetValue(null)).Cast<T>();
    }

    public override bool Equals(object? obj) => Equals(obj as Enumeration);

    public bool Equals(Enumeration? other)
    {
        if (other is null) return false;
        return GetType() == other.GetType() && Id == other.Id;
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public int CompareTo(Enumeration? other) => Id.CompareTo(other?.Id);

    public static bool operator ==(Enumeration? left, Enumeration? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Enumeration? left, Enumeration? right) => !(left == right);
}

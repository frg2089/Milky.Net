namespace Milky.Net.ModelGenerator.Models;

public interface IDescribable
{
    string Description { get; }
}

public interface IMilkyType : IDescribable
{

}

public interface ISimplyMilkyType : IMilkyType
{
    IReadOnlyList<Field> Fields { get; }
}


public interface IDerivableModel : IMilkyType
{
    string TagValue { get; }
}

namespace Milky.Net.Model;

[Serializable]
public class MilkyException : Exception
{
    public MilkyException() { }
    public MilkyException(string message) : base(message) { }
    public MilkyException(string message, Exception inner) : base(message, inner) { }
}

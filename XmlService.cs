using System.Xml.Linq;

internal class XmlService
{
    private XDocument document;

    public static XmlService? Load(string path)
    {
        XDocument document;
        try
        {
            document = XDocument.Load(path);
        }
        catch (Exception exception)
        {
            BigBrother.Instance.DebugLog(exception.Message);
            return null;
        }
        return new XmlService(document);
    }

    private XmlService(XDocument document)
    {
        this.document = document;
    }

    private T GetValue<T>(XElement element)
    {
        if (typeof(T) == typeof(string))
            return (T)(object)element.Value;
        if (typeof(T) == typeof(ulong))
            return (T)(object)ulong.Parse(element.Value);

        throw new Exception("I don't know what to do, help");
    }

    public T? GetValue<T>(string elementName)
    {
        XElement? element = document.Root.Element(elementName);
        if (element == null)
            return default;

        return GetValue<T>(element);
    }

    public IEnumerable<T?> GetValues<T>(string elementName)
    {
        foreach (XElement element in document.Root.Elements(elementName))
            yield return GetValue<T>(element);
    }
}

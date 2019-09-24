using System.Xml.Serialization;

namespace Module
{
    public class Foo
    {
        public Foo()
        {
            // This cause ALC to crash
            var xmlSerializer = new XmlSerializer(typeof(Bar));
        }
    }
}
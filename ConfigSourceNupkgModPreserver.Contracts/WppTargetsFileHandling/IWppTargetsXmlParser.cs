namespace ConfigSourceNupkgModPreserver.Contracts.WppTargetsFileHandling
{
    public interface IWppTargetsXmlParser
    {
        WppTargetsInfo GetInfo(string xml);
    }
}
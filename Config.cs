namespace Fleur
{
    internal class Config
    {
        public enum EOperationMode
        {
            JustCache,
            JustDump,
            Full
        }

        public string CachePath = "cache";
        public string OutputPath = "html";
        public string TemplatesPath = "templates";
        public string ResourcesPath = "resources";

        public EOperationMode OperationMode = EOperationMode.Full;
        public bool LogToFile = false;

        public string[] SessionCookies = { "example1", "example2", "example3" };
    }
}

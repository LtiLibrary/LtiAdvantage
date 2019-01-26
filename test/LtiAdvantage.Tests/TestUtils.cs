using System.IO;

namespace LtiAdvantage.Tests
{
    internal static class TestUtils
    {
        public static string LoadReferenceJsonFile(string filename)
        {
            return File.ReadAllText("ReferenceJson/" + filename + ".json");
        }

        public static string LoadReferenceTextFile(string filename)
        {
            return File.ReadAllText("ReferenceText/" + filename + ".txt");
        }
    }
}
using DeepEqual.Syntax;
//using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;
using JsonDiffPatchDotNet;

namespace JsonComparer
{
    public static class Comparer
    {
        public static string CompareFiles(string path1, string path2)
        {
            if (string.IsNullOrWhiteSpace(path1))
            {
                throw new ArgumentException("Must not be null or white space", nameof(path1));
            }

            if (string.IsNullOrWhiteSpace(path2))
            {
                throw new ArgumentException("Must not be null or white space", nameof(path2));
            }

            if (!System.IO.File.Exists(path1))
            {
                throw new ArgumentException("File must exist", nameof(path1));
            }

            if (!System.IO.File.Exists(path2))
            {
                throw new ArgumentException("File must exist", nameof(path2));
            }

            var content1 = System.IO.File.ReadAllText(path1);

            var content2 = System.IO.File.ReadAllText(path2);

            return CompareStrings(content1, content2);
        }

        public static string CompareStrings(string content1, string content2)
        {
            if (string.IsNullOrWhiteSpace(content1))
            {
                throw new ArgumentException("Must not be null or white space", nameof(content1));
            }

            if (string.IsNullOrWhiteSpace(content2))
            {
                throw new ArgumentException("Must not be null or white space", nameof(content2));
            }

            CleanJsonString(content1);
            CleanJsonString(content2);

            var json1 = JsonConvert.DeserializeObject<JToken>(content1);
            var json2 = JsonConvert.DeserializeObject<JToken>(content2);

            return CompareJson(json1, json2);
        }

        private static void CleanJsonString(string content)
        {
            // todo Make the cleanups configurable?

            // todo Remove names with empty values? "xyz": null
            content = content.Replace(": null", "");

            // todo Empty Arrays? "xyz": []
            content = content.Replace(": []", "");

            // todo Empty Containers? "xyz: {}
            content = content.Replace(": {}", "");

            // todo Generalize Date/Time: Timezone offset to Zulu?
            // 2019-01-02T17:03:50.667000+00:00 => 2019-01-02T17:03:50.667Z
            content = content.Replace("000+00:00", "Z");

            // todo Convert all times to Zulu?
            // 2019-01-02T12:03:50.667-05:00 => 2019-01-02T17:03:50.667Z Even though it's -5, we add 5 to the time to get Zulu
        }

        private static string CompareJson(JToken json1, JToken json2)
        {
            if (json1 == null)
            {
                throw new ArgumentException("Must not be null", nameof(json1));
            }

            if (json2 == null)
            {
                throw new ArgumentException("Must not be null", nameof(json2));
            }

            // This doesn't work because it doesn't support dynamic objects.
            /*
            var config = new ComparisonConfig
            {
                MaxStructDepth = 5,
                MaxDifferences = 100
            };

            var compareLogic = new CompareLogic(config);
            var  if (JToken.DeepEquals(json1, json2))
            {
                return null;
            }
            elseresult = compareLogic.Compare(json1, json2);
            */

            // This just compares but doesn't show differences.
            // var result1 = json1.IsDeepEqual(json2);

            var x = new JsonDiffPatchDotNet.Options()
            {
                TextDiff = TextDiffMode.Simple,
                ArrayDiff = ArrayDiffMode.Simple,
                MinEfficientTextDiffLength = long.MaxValue
            };
            var jdp = new JsonDiffPatch(x);

            var jsonResult = jdp.Diff(json1, json2);

            var result = JsonConvert.SerializeObject(jsonResult, Formatting.Indented);

            return result;
        }
    }
}

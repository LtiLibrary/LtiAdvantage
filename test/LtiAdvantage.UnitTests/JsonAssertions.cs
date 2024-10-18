using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace LtiAdvantage.UnitTests
{
    public static class JsonAssert
    {
        public static void Equal(string expectedJson, string actualJson)
        {
            using (JsonDocument expectedDoc = JsonDocument.Parse(expectedJson))
            using (JsonDocument actualDoc = JsonDocument.Parse(actualJson))
            {
                JsonElement expectedRootElement = expectedDoc.RootElement;
                JsonElement actualRootElement = actualDoc.RootElement;

                CompareJson(expectedRootElement, actualRootElement, "$");
            }
        }

        private static void CompareJson(JsonElement expected, JsonElement actual, string path)
        {
            if (expected.ValueKind != actual.ValueKind)
            {
                Assert.Fail(GetMessage(path, expected, actual));
            }

            switch (expected.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty expectedProperty in expected.EnumerateObject())
                    {
                        JsonElement actualProperty;

                        if (!actual.TryGetProperty(expectedProperty.Name, out actualProperty))
                        {
                            Assert.Fail(GetMessage(path, expectedProperty, "Property not found"));
                        }

                        CompareJson(expectedProperty.Value, actualProperty, $"{path}.{expectedProperty.Name}");
                    }
                    break;

                case JsonValueKind.Array:
                    JsonElement[] expectedArray = expected.EnumerateArray().ToArray();
                    JsonElement[] actualArray = actual.EnumerateArray().ToArray();

                    if (expectedArray.Length != actualArray.Length)
                    {
                        Assert.Fail(GetMessage(path, $"Array length {expectedArray.Length}", $"Array length {actualArray.Length}"));
                    }

                    for (int i = 0; i < expectedArray.Length; i++)
                    {
                        CompareJson(expectedArray[i], actualArray[i], $"{path}[{i}]");
                    }
                    break;

                default:
                    if (!JsonElementEqualityComparer.Instance.Equals(expected, actual))
                    {
                        Assert.Fail(GetMessage(path, expected, actual));
                    }
                    break;
            }
        }

        private static string GetMessage(string path, object expected, object actual)
        {
            return $"JSON did not match at path '{path}'. Expected: {expected}, Actual: {actual}";
        }

        // A custom JsonElementEqualityComparer is needed because JsonElement's default equality comparer
        // does not consider structural equality.
        private class JsonElementEqualityComparer : IEqualityComparer<JsonElement>
        {
            public static JsonElementEqualityComparer Instance { get; } = new JsonElementEqualityComparer();

            public bool Equals(JsonElement x, JsonElement y)
            {
                if (x.ValueKind != y.ValueKind)
                    return false;

                switch (x.ValueKind)
                {
                    case JsonValueKind.Object:
                    case JsonValueKind.Array:
                        // We already handle structural comparisons for objects and arrays
                        // in the CompareJson method.
                        return true;
                    case JsonValueKind.String:
                        var xString = x.GetString();
                        var yString = y.GetString();

                        if (DateTime.TryParse(xString, out var xDate) && DateTime.TryParse(yString, out var yDate))
                            return xDate == yDate;

                        return xString == yString;
                    case JsonValueKind.Number:
                        return x.GetRawText() == y.GetRawText(); // Raw text comparison is more precise for numbers
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return x.GetBoolean() == y.GetBoolean();
                    case JsonValueKind.Null:
                        return true; // If both are null, they are equal
                    default:
                        throw new InvalidOperationException($"Unsupported JsonValueKind: {x.ValueKind}");
                }
            }

            public int GetHashCode(JsonElement obj)
            {
                // This is not used in the context of JSON comparison here, so leaving unimplemented.
                return obj.GetHashCode();
            }
        }
    }
}

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LtiAdvantage.UnitTests
{
    internal static class JsonAssertions
    {
        public static void Equal(string expectedJson, string actualJson)
        {
            try
            {
                var actualJObject = JObject.Parse(actualJson);
                var expectedJObject = JObject.Parse(expectedJson);

                AssertSameObject(expectedJObject, actualJObject);
            }
            catch (JsonReaderException)
            {
                var actualJArray = JArray.Parse(actualJson);
                var expectedJArray = JArray.Parse(expectedJson);

                AssertSameArray(expectedJArray, actualJArray);
            }
        }

        private static void AssertSameArray(JArray expected, JArray actual)
        {
            var diff = ObjectDiffPatch.GenerateDiff(expected, actual);
            Assert.True(diff.ActualValues == null && diff.ExpectedValues == null, "Expected:\n" + diff.ExpectedValues + "\nActual:\n" + diff.ActualValues);
        }

        private static void AssertSameObject(JObject expected, JObject actual)
        {
            var diff = ObjectDiffPatch.GenerateDiff(expected, actual);
            Assert.True(diff.ActualValues == null && diff.ExpectedValues == null, "Expected:\n" + diff.ExpectedValues + "\nActual:\n" + diff.ActualValues);
        }

        private static class ObjectDiffPatch
        {
            private const string PrefixArraySize = "@@ Count";
            private const string PrefixNullObject = "@@ Null";
            private const string PrefixRemovedFields = "@@ Removed";

            /// <summary>
            /// Compares two objects and generates the differences between them.
            /// </summary>
            /// <typeparam name="T">The type of the T.</typeparam>
            /// <param name="expected">The expected object.</param>
            /// <param name="actual">The actual object.</param>
            /// <returns></returns>
            public static ObjectDiffPatchResult GenerateDiff<T>(T expected, T actual) where T : class
            {
                if (typeof(JObject).IsAssignableFrom(typeof(T)))
                {
                    var expectedJson = expected as JObject;
                    var actualJson = actual as JObject;
                    return Diff(expectedJson, actualJson);
                }

                if (typeof(JArray).IsAssignableFrom(typeof(T)))
                {
                    var expectedJson = expected as JArray;
                    var actualJson = actual as JArray;
                    var diff = new ObjectDiffPatchResult();

                    if (expectedJson == null && actualJson == null)
                    {
                        return diff;
                    }

                    if (expectedJson == null || actualJson == null)
                    {
                        diff.ExpectedValues = new JObject {{PrefixNullObject, expectedJson == null ? "null" : "not null"}};
                        diff.ActualValues = new JObject {{PrefixNullObject, actualJson == null ? "null" : "not null" }};
                        return diff;
                    }

                    if (expectedJson.Count != actualJson.Count)
                    {
                        diff.ExpectedValues = new JObject {{PrefixArraySize, expectedJson.Count}};
                        diff.ActualValues = new JObject {{PrefixArraySize, actualJson.Count}};
                        return diff;
                    }

                    for (var index = 0; index < expectedJson.Count; index++)
                    {
                        var expectedJObject = expectedJson[index] as JObject;
                        var actualJObject = actualJson[index] as JObject;

                        var result = Diff(expectedJObject, actualJObject);
                        if (result.ExpectedValues != null)
                        {
                            foreach (var pair in result.ExpectedValues)
                            {
                                if (diff.ExpectedValues == null)
                                {
                                    diff.ExpectedValues = new JObject();
                                }

                                diff.ExpectedValues.Add($"{pair.Key}[{index}]", pair.Value);
                            }
                        }

                        if (result.ActualValues != null)
                        {
                            foreach (var pair in result.ActualValues)
                            {
                                if (diff.ActualValues == null)
                                {
                                    diff.ActualValues = new JObject();
                                }

                                diff.ActualValues.Add($"{pair.Key}[{index}]", pair.Value);
                            }
                        }
                    }

                    return diff;
                }
                else
                {
                    // not a JObject or a JArray
                    // ensure the serializer will not ignore null values
                    var writer = GetJsonSerializer();
                    var expectedJson = expected != null ? JObject.FromObject(expected, writer) : null;
                    var actualJson = actual != null ? JObject.FromObject(actual, writer) : null;
                    return Diff(expectedJson, actualJson);
                }
            }

            private static ObjectDiffPatchResult Diff(JObject expected, JObject actual)
            {
                var result = new ObjectDiffPatchResult();
                // check for null values
                if (expected == null && actual == null)
                {
                    return result;
                }
                if (expected == null || actual == null)
                {
                    result.ExpectedValues = expected;
                    result.ActualValues = actual;
                    return result;
                }

                // compare internal fields           
                var removedNew = new JArray();
                var removedOld = new JArray();
                JToken token;
                // start by iterating in source fields
                foreach (var pair in expected)
                {
                    // ignore null values (i.e. null values = missing value)
                    if (pair.Value.Type == JTokenType.Null)
                        continue;

                    // check if field exists
                    if (!actual.TryGetValue(pair.Key, out token))
                    {
                        AddExpectedToken(result, pair.Value, pair.Key);
                        removedNew.Add(pair.Key);
                    }
                    // compare field values
                    else
                    {
                        DiffField(pair.Key, pair.Value, token, result);
                    }
                }
                // then iterate in target fields that are not present in source
                foreach (var pair in actual)
                {
                    // ignore null values (i.e. null values = missing value)
                    if (pair.Value.Type == JTokenType.Null)
                        continue;
                    // ignore already compared values
                    if (expected.TryGetValue(pair.Key, out token))
                        continue;
                    // add missing tokens
                    removedOld.Add(pair.Key);
                    AddActualToken(result, pair.Value, pair.Key);
                }

                if (removedOld.Count > 0)
                    AddExpectedToken(result, removedOld, PrefixRemovedFields);
                if (removedNew.Count > 0)
                    AddActualToken(result, removedNew, PrefixRemovedFields);

                return result;
            }

            private static void DiffField(string fieldName, JToken source, JToken target, ObjectDiffPatchResult result = null)
            {
                if (result == null)
                    result = new ObjectDiffPatchResult();
                if (source == null)
                {
                    if (target != null)
                    {
                        AddToken(result, fieldName, null, target);
                    }
                }
                else if (target == null)
                {
                    AddToken(result, fieldName, source, null);
                }
                else if (source.Type == JTokenType.Object)
                {
                    var v = target as JObject;
                    var r = Diff(source as JObject, v);
                    if (!r.AreEqual)
                        AddToken(result, fieldName, r);
                }
                else
                {
                    var aT = target as JArray;
                    var aS = source as JArray;
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (source.Type)
                    {
                        case JTokenType.Array when aS == null || aT == null:
                        case JTokenType.Array when (aS.Count == 0 || aT.Count == 0) && aS.Count != aT.Count:
                            AddToken(result, fieldName, source, target);
                            break;
                        case JTokenType.Array:
                        {
                            var arrayDiff = new ObjectDiffPatchResult();
                            var minCount = Math.Min(aS.Count, aT.Count);
                            for (var i = 0; i < Math.Max(aS.Count, aT.Count); i++)
                            {
                                if (i < minCount)
                                {
                                    DiffField(i.ToString(), aS[i], aT[i], arrayDiff);
                                }
                                else if (i >= aS.Count)
                                {
                                    AddActualToken(arrayDiff, aT[i], i.ToString());
                                }
                                else
                                {
                                    AddExpectedToken(arrayDiff, aS[i], i.ToString());
                                }
                            }

                            if (arrayDiff.AreEqual) return;

                            if (aS.Count != aT.Count)
                                AddToken(arrayDiff, PrefixArraySize, aS.Count, aT.Count);
                            AddToken(result, fieldName, arrayDiff);
                            break;
                        }
                        case JTokenType.Integer when target.Type != JTokenType.Integer:
                        {
                            var sourceValue = (JValue) source;
                            if (target is JValue targetValue)
                            {
                                try
                                {
                                    var objA = Convert.ChangeType(sourceValue.Value, targetValue.Value.GetType());
                                    if (!objA.Equals(targetValue.Value))
                                    {
                                        AddToken(result, fieldName, source, target);
                                    }
                                }
                                catch
                                {
                                    AddToken(result, fieldName, source, target);
                                }
                            }
                            else if (!JToken.DeepEquals(source, target))
                            {
                                AddToken(result, fieldName, source, target);
                            }

                            break;
                        }
                        default:
                        {
                            if (!JToken.DeepEquals(source, target))
                            {
                                AddToken(result, fieldName, source, target);
                            }

                            break;
                        }
                    }
                }
            }

            private static JsonSerializer GetJsonSerializer()
            {
                // ensure the serializer will not ignore null values
                var settings = JsonConvert.DefaultSettings != null ? JsonConvert.DefaultSettings() : new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Include;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                settings.Formatting = Formatting.None;
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

                // create our custom serializer
                var writer = JsonSerializer.Create(settings);
                return writer;
            }

            private static void AddActualToken(ObjectDiffPatchResult item, JToken actualToken, string fieldName)
            {
                if (item.ActualValues == null)
                    item.ActualValues = new JObject();
                item.ActualValues[fieldName] = actualToken;
            }

            private static void AddExpectedToken(ObjectDiffPatchResult item, JToken expectedToken, string fieldName)
            {
                if (item.ExpectedValues == null)
                    item.ExpectedValues = new JObject();
                item.ExpectedValues[fieldName] = expectedToken;
            }

            private static void AddToken(ObjectDiffPatchResult item, string fieldName, JToken expectedToken, JToken actualToken)
            {
                AddExpectedToken(item, expectedToken, fieldName);

                AddActualToken(item, actualToken, fieldName);
            }

            private static void AddToken(ObjectDiffPatchResult item, string fieldName, ObjectDiffPatchResult diff)
            {
                AddToken(item, fieldName, diff.ExpectedValues, diff.ActualValues);
            }
        }

        /// <summary>
        /// Result of a diff operation between two objects
        /// </summary>
        private class ObjectDiffPatchResult
        {
            /// <summary>
            /// If the compared objects are equal.
            /// </summary>
            /// <value>true if the obects are equal; otherwise, false.</value>
            public bool AreEqual => ExpectedValues == null && ActualValues == null;

            /// <summary>
            /// The values that are different in the expected object.
            /// </summary>
            public JObject ExpectedValues { get; set; }

            /// <summary>
            /// The values that are different in the actual object.
            /// </summary>
            public JObject ActualValues { get; set; }
        }
    }
}

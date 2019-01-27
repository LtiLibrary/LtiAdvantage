using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LtiAdvantage.Tests
{
    internal static class JsonAssertions
    {
        public static void Equal(string expectedJson, string actualJson)
        {
            var actualJObject = JObject.Parse(actualJson);
            var expectedJObject = JObject.Parse(expectedJson);

            AssertSameObject(expectedJObject, actualJObject);
        }

        private static void AssertSameObject(JObject expected, JObject actual)
        {
            var diff = ObjectDiffPatch.GenerateDiff(expected, actual);
            Assert.True(diff.NewValues == null && diff.OldValues == null, "Expected:\n" + diff.OldValues + "\nActual:\n" + diff.NewValues);
        }

        private static class ObjectDiffPatch
        {
            private const string PrefixArraySize = "@@ Count";
            private const string PrefixRemovedFields = "@@ Removed";

            /// <summary>
            /// Compares two objects and generates the differences between them.
            /// </summary>
            /// <typeparam name="T">The type of the T.</typeparam>
            /// <param name="original">The original.</param>
            /// <param name="updated">The updated.</param>
            /// <returns></returns>
            public static ObjectDiffPatchResult GenerateDiff<T>(T original, T updated) where T : class
            {
                // ensure the serializer will not ignore null values
                var writer = GetJsonSerializer();
                // parse our objects
                JObject originalJson, updatedJson;
                if (typeof(JObject).IsAssignableFrom(typeof(T)))
                {
                    originalJson = original as JObject;
                    updatedJson = updated as JObject;
                }
                else
                {
                    originalJson = original != null ? JObject.FromObject(original, writer) : null;
                    updatedJson = updated != null ? JObject.FromObject(updated, writer) : null;
                }
                // analyse their differences!
                var result = Diff(originalJson, updatedJson);
                return result;
            }

            private static ObjectDiffPatchResult Diff(JObject source, JObject target)
            {
                var result = new ObjectDiffPatchResult();
                // check for null values
                if (source == null && target == null)
                {
                    return result;
                }
                if (source == null || target == null)
                {
                    result.OldValues = source;
                    result.NewValues = target;
                    return result;
                }

                // compare internal fields           
                var removedNew = new JArray();
                var removedOld = new JArray();
                JToken token;
                // start by iterating in source fields
                foreach (var pair in source)
                {
                    // check if field exists
                    if (!target.TryGetValue(pair.Key, out token))
                    {
                        AddOldValuesToken(result, pair.Value, pair.Key);
                        removedNew.Add(pair.Key);
                    }
                    // compare field values
                    else
                    {
                        DiffField(pair.Key, pair.Value, token, result);
                    }
                }
                // then iterate in target fields that are not present in source
                foreach (var pair in target)
                {
                    // ignore alredy compared values
                    if (source.TryGetValue(pair.Key, out token))
                        continue;
                    // add missing tokens
                    removedOld.Add(pair.Key);
                    AddNewValuesToken(result, pair.Value, pair.Key);
                }

                if (removedOld.Count > 0)
                    AddOldValuesToken(result, removedOld, PrefixRemovedFields);
                if (removedNew.Count > 0)
                    AddNewValuesToken(result, removedNew, PrefixRemovedFields);

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
                    if (source.Type == JTokenType.Array)
                    {
                        if (aS == null || aT == null)
                        {
                            AddToken(result, fieldName, source, target);
                        }
                        else if ((aS.Count == 0 || aT.Count == 0) && aS.Count != aT.Count)
                        {
                            AddToken(result, fieldName, source, target);
                        }
                        else
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
                                    AddNewValuesToken(arrayDiff, aT[i], i.ToString());
                                }
                                else
                                {
                                    AddOldValuesToken(arrayDiff, aS[i], i.ToString());
                                }
                            }

                            if (arrayDiff.AreEqual) return;

                            if (aS.Count != aT.Count)
                                AddToken(arrayDiff, PrefixArraySize, aS.Count, aT.Count);
                            AddToken(result, fieldName, arrayDiff);
                        }
                    }
                    else if (source.Type == JTokenType.Integer && target.Type != JTokenType.Integer)
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
                    }
                    else
                    {
                        if (!JToken.DeepEquals(source, target))
                        {
                            AddToken(result, fieldName, source, target);
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

            private static void AddNewValuesToken(ObjectDiffPatchResult item, JToken newToken, string fieldName)
            {
                if (item.NewValues == null)
                    item.NewValues = new JObject();
                item.NewValues[fieldName] = newToken;
            }

            private static void AddOldValuesToken(ObjectDiffPatchResult item, JToken oldToken, string fieldName)
            {
                if (item.OldValues == null)
                    item.OldValues = new JObject();
                item.OldValues[fieldName] = oldToken;
            }

            private static void AddToken(ObjectDiffPatchResult item, string fieldName, JToken oldToken, JToken newToken)
            {
                AddOldValuesToken(item, oldToken, fieldName);

                AddNewValuesToken(item, newToken, fieldName);
            }

            private static void AddToken(ObjectDiffPatchResult item, string fieldName, ObjectDiffPatchResult diff)
            {
                AddToken(item, fieldName, diff.OldValues, diff.NewValues);
            }
        }

        /// <summary>
        /// Result of a diff operation between two objects
        /// </summary>
        public class ObjectDiffPatchResult
        {
            /// <summary>
            /// If the compared objects are equal.
            /// </summary>
            /// <value>true if the obects are equal; otherwise, false.</value>
            public bool AreEqual => OldValues == null && NewValues == null;

            /// <summary>
            /// The values modified in the original object.
            /// </summary>
            public JObject OldValues { get; set; }

            /// <summary>
            /// The values modified in the updated object.
            /// </summary>
            public JObject NewValues { get; set; }
        }

        internal class ObjectDiffPatchJTokenComparer : IEqualityComparer<JToken>
        {
            public bool Equals(JToken x, JToken y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                return JToken.DeepEquals(x, y);
            }
            public int GetHashCode(JToken i)
            {
                return i.ToString().GetHashCode();
            }
        }
    }
}

using System;
using System.Globalization;
using System.Text.Json;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.SubmissionReview;
using Xunit;

namespace LtiAdvantage.UnitTests.AssignmentGradeServices
{
    public class LineItemShould
    {
        /// <summary>
        /// Deserialize the example lineitem from IMS documentation and check values.
        /// See https://www.imsglobal.org/spec/lti-ags/v2p0/#example-application-vnd-ims-lis-v2-lineitem-json-representation
        /// </summary>
        [Fact]
        public void DeserializeFromValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LineItem");
            var lineitem = JsonSerializer.Deserialize<LineItem>(referenceJson);

            Assert.Equal("https://lms.example.com/context/2923/lineitems/1", lineitem.Id);
            Assert.Equal(60.0, lineitem.ScoreMaximum);
            Assert.Equal("Chapter 5 Test", lineitem.Label);
            Assert.Equal("a-9334df-33", lineitem.ResourceId);
            Assert.Equal("grade", lineitem.Tag);
            Assert.Equal("1g3k4dlk49fk", lineitem.ResourceLinkId);
            Assert.Equal(DateTime.Parse("2018-03-06T20:05:02Z", null, DateTimeStyles.AdjustToUniversal), lineitem.StartDateTime);
            Assert.Equal(DateTime.Parse("2018-04-06T22:05:03Z", null, DateTimeStyles.AdjustToUniversal), lineitem.EndDateTime);
        }

        /// <summary>
        /// Deserialize the example lineitem from IMS documentation and then serialize back to JSON
        /// and compare JSON.
        /// </summary>
        [Fact]
        public void SerializeToValidJson()
        {
            var referenceJson = TestUtils.LoadReferenceJsonFile("LineItem");
            var lineItem = JsonSerializer.Deserialize<LineItem>(referenceJson);
            var lineItemJson = JsonSerializer.Serialize(lineItem);

            JsonAssert.Equal(referenceJson, lineItemJson);
        }

        [Fact]
        public void RoundTripSubmissionReviewProperty()
        {
            var lineItem = new LineItem
            {
                Id = "https://platform.example.com/lineitems/1",
                Label = "Essay",
                ScoreMaximum = 10,
                SubmissionReviewExtension = new SubmissionReviewProperty
                {
                    Url = "https://tool.example.com/review",
                    Custom = new System.Collections.Generic.Dictionary<string, string> { ["a"] = "1" }
                }
            };

            var json = JsonSerializer.Serialize(lineItem);
            var roundTripped = JsonSerializer.Deserialize<LineItem>(json);

            Assert.NotNull(roundTripped.SubmissionReviewExtension);
            Assert.Equal("https://tool.example.com/review", roundTripped.SubmissionReviewExtension.Url);
            Assert.Equal("1", roundTripped.SubmissionReviewExtension.Custom["a"]);
        }

        /// <summary>
        /// The spec's submissionReview properties (label, url, custom) must survive a round trip.
        /// See https://www.imsglobal.org/spec/lti-sr/v1p0/#submissionreview-extension
        /// </summary>
        [Fact]
        public void RoundTripSubmissionReview_LabelUrlAndCustom()
        {
            const string json = """
                {
                  "scoreMaximum": 60,
                  "label": "Chapter 5 Test",
                  "submissionReview": {
                    "label": "Open My Tool Viewer",
                    "url": "https://platform.example.com/act/849023/sub",
                    "custom": { "action": "review", "a_id": "23942" }
                  }
                }
                """;

            var lineItem = JsonSerializer.Deserialize<LineItem>(json);
            var roundTripped = JsonSerializer.Deserialize<LineItem>(JsonSerializer.Serialize(lineItem));

            var review = roundTripped.SubmissionReviewExtension;
            Assert.Equal("Open My Tool Viewer", review.Label);
            Assert.Equal("https://platform.example.com/act/849023/sub", review.Url);
            Assert.Equal("review", review.Custom["action"]);
        }

        /// <summary>Unset optional properties are left out rather than written as null.</summary>
        [Fact]
        public void NotWriteNulls_ForUnsetProperties()
        {
            var json = JsonSerializer.Serialize(new LineItem { Label = "Quiz", ScoreMaximum = 10 });

            Assert.DoesNotContain("null", json);
            JsonAssert.Equal("{\"label\":\"Quiz\",\"scoreMaximum\":10}", json);
        }

        [Fact]
        public void RoundTripGradesReleased()
        {
            var json = JsonSerializer.Serialize(new LineItem { Label = "Quiz", ScoreMaximum = 10, GradesReleased = false });

            Assert.Contains("\"gradesReleased\":false", json);
            Assert.False(JsonSerializer.Deserialize<LineItem>(json).GradesReleased);
        }
    }
}

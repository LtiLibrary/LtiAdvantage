using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LtiAdvantage.Lti
{
    /// <summary>
    /// Implements custom property substitutions as described in
    /// https://www.imsglobal.org/spec/lti/v1p3#custom-property-substitution.
    /// </summary>
    /// <remarks>
    /// You must supply substitution values for any substituion variables you
    /// support. Be careful not to provide personally identifiable information
    /// that is not available via the normal launch.
    /// </remarks>
    public class CustomPropertySubstitutions
    {
        // These properties are used during custom variable substition. They are not normally
        // part of an LTI request.
        #region Custom Substitution Values

        /// <summary>
        /// Substitution values for Course Groups Service variables.
        /// </summary>
        public CourseGroup CourseGroup { get; set; }

        /// <summary>
        /// Substitution values for LIS Course Offering variables.
        /// </summary>
        public LisCourseOffering LisCourseOffering { get; set; }

        /// <summary>
        /// Substitution values for LIS Course Section variables.
        /// </summary>
        public LisCourseSection LisCourseSection { get; set; }

        /// <summary>
        /// Substitution values for LIS Course Template variables.
        /// </summary>
        public LisCourseTemplate LisCourseTemplate { get; set; }

        /// <summary>
        /// Substitution values for LIS Group variables.
        /// </summary>
        public LisGroup LisGroup { get; set; }

        /// <summary>
        /// Substitution values for LIS Membership variables.
        /// </summary>
        public LisMembership LisMembership { get; set; }

        /// <summary>
        /// Substitution values for LIS Message variables.
        /// </summary>
        public LisMessage LisMessage { get; set; }

        /// <summary>
        /// Substitution values for LIS Person variables.
        /// </summary>
        public LisPerson LisPerson { get; set; }

        /// <summary>
        /// Substitution values for LTI Context variables.
        /// </summary>
        public LtiContext LtiContext { get; set; }

        /// <summary>
        /// Substitution values for LTI ResourceLink variables.
        /// </summary>
        public LtiResourceLink LtiResourceLink { get; set; }

        /// <summary>
        /// Substitution values for LTI User variables.
        /// </summary>
        public LtiUser LtiUser { get; set; }

        /// <summary>
        /// Substitution values for Tool Platform variables.
        /// </summary>
        public ToolPlatfom ToolPlatform { get; set; }

        /// <summary>
        /// Substitution values for Tool Platform Instance variables.
        /// </summary>
        public ToolPlatfomInstance ToolPlatfomInstance { get; set; }

        #endregion

        /// <summary>
        /// Replace custom property variables with custom property values.
        /// </summary>
        /// <param name="source">Custom properties.</param>
        /// <returns>Custom properties with the variables replaced.</returns>
        public Dictionary<string, string> ReplaceCustomPropertyValues(Dictionary<string, string> source)
        {
            var dest = new Dictionary<string, string>();

            if (source != null)
            {
                foreach (var pair in source)
                {
                    // Substitute
                    dest.Add(pair.Key, SubstituteCustomVariable(pair.Value));
                }
            }

            return dest;
        }

        /// <summary>
        /// Records a variable's value, unless the variable was already recorded.
        /// </summary>
        private static void Add(IDictionary<string, string> variables, string variable, string replacement)
        {
            if (!variables.ContainsKey(variable))
            {
                variables.Add(variable, replacement ?? string.Empty);
            }
        }

        /// <summary>
        /// Replaces every known variable in <paramref name="value"/> in a single pass.
        /// </summary>
        /// <remarks>
        /// One pass over the original text means a substituted value is never scanned again, so a
        /// context titled "$User.id" stays "$User.id" instead of expanding to the user's id. The
        /// alternation lists longer names first, so where one name is a prefix of another
        /// (<c>$Context.id</c> and <c>$Context.id.history</c>) the longer one wins regardless of
        /// the order the variables were recorded in. Names are regex-escaped, and values are
        /// inserted through a match evaluator, so neither is read as regex syntax.
        /// </remarks>
        private static string SubstituteAll(string value, IDictionary<string, string> variables)
        {
            if (string.IsNullOrEmpty(value) || variables.Count == 0 || value.IndexOf('$') < 0)
            {
                return value;
            }

            var pattern = string.Join("|", variables.Keys
                .OrderByDescending(name => name.Length)
                .Select(Regex.Escape));

            return Regex.Replace(value, pattern, match => variables[match.Value], RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Replace variable with value. Unknown variables are ignored.
        /// </summary>
        /// <remarks>
        /// Each section records its variables, and <see cref="SubstituteAll"/> then replaces them
        /// all in one pass. Variables of sections that are not set stay as they are.
        /// </remarks>
        /// <param name="value">Variable to replace.</param>
        /// <returns>Value if variable is known.</returns>
        private string SubstituteCustomVariable(string value)
        {
            var variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // LIS Course Offering variable subsitutions
            if (LisCourseOffering != null)
            {
                Add(variables, "$CourseOffering.academicSession", LisCourseOffering.AcademicSession);
                Add(variables, "$CourseOffering.courseNumber", LisCourseOffering.CourseNumber);
                Add(variables, "$CourseOffering.credits", LisCourseOffering.Credits);
                Add(variables, "$CourseOffering.label", LisCourseOffering.Label);
                Add(variables, "$CourseOffering.longDescription", LisCourseOffering.LongDescription);
                Add(variables, "$CourseOffering.shortDescription", LisCourseOffering.ShortDescription);
                Add(variables, "$CourseOffering.sourcedId", LisCourseOffering.SourcedId);
                Add(variables, "$CourseOffering.title", LisCourseOffering.Title);
            }

            // LIS Course Section variable subsitutions
            if (LisCourseSection != null)
            {
                Add(variables, "$CourseSection.courseNumber", LisCourseSection.CourseNumber);
                Add(variables, "$CourseSection.credits", LisCourseSection.Credits);
                Add(variables, "$CourseSection.dataSource", LisCourseSection.DataSource);
                Add(variables, "$CourseSection.dept", LisCourseSection.Dept);
                Add(variables, "$CourseSection.enrollControl.accept", LisCourseSection.EnrollControlAccept);
                Add(variables, "$CourseSection.enrollControl.allowed", LisCourseSection.EnrollControlAllowed);
                Add(variables, "$CourseSection.label", LisCourseSection.Label);
                Add(variables, "$CourseSection.longDescription", LisCourseSection.LongDescription);
                Add(variables, "$CourseSection.maxNumberofStudents", LisCourseSection.MaxNumberOfStudents);
                Add(variables, "$CourseSection.numberofStudents", LisCourseSection.NumberOfStudents);
                Add(variables, "$CourseSection.shortDescription", LisCourseSection.ShortDescription);
                Add(variables, "$CourseSection.sourceSectionId", LisCourseSection.SourceSectionId);
                Add(variables, "$CourseSection.sourcedId", LisCourseSection.SourcedId);
                Add(variables, "$CourseSection.timeFrame.begin", LisCourseSection.TimeFrameBegin);
                Add(variables, "$CourseSection.timeFrame.end", LisCourseSection.TimeFrameEnd);
                Add(variables, "$CourseSection.title", LisCourseSection.Title);
            }

            // LIS Course Template variable subsitutions
            if (LisCourseTemplate != null)
            {
                Add(variables, "$CourseTemplate.courseNumber", LisCourseTemplate.CourseNumber);
                Add(variables, "$CourseTemplate.credits", LisCourseTemplate.Credits);
                Add(variables, "$CourseTemplate.label", LisCourseTemplate.Label);
                Add(variables, "$CourseTemplate.longDescription", LisCourseTemplate.LongDescription);
                Add(variables, "$CourseTemplate.shortDescription", LisCourseTemplate.ShortDescription);
                Add(variables, "$CourseTemplate.sourcedId", LisCourseTemplate.SourcedId);
                Add(variables, "$CourseTemplate.title", LisCourseTemplate.Title);
            }

            // LIS Group vaiable subsitutions
            if (LisGroup != null)
            {
                Add(variables, "$Group.email", LisGroup.Email);
                Add(variables, "$Group.enrollControl.accept", LisGroup.EnrollControlAccept);
                Add(variables, "$Group.enrollControl.allowed", LisGroup.EnrollControlAllowed);
                Add(variables, "$Group.grouptype.level", LisGroup.Level);
                Add(variables, "$Group.grouptype.scheme", LisGroup.Scheme);
                Add(variables, "$Group.grouptype.typevalue", LisGroup.Typevalue);
                Add(variables, "$Group.longDescription", LisGroup.LongDescription);
                Add(variables, "$Group.parentId", LisGroup.ParentId);
                Add(variables, "$Group.shortDescription", LisGroup.ShortDescription);
                Add(variables, "$Group.sourcedId", LisGroup.SourcedId);
                Add(variables, "$Group.timeFrame.begin", LisGroup.TimeFrameBegin);
                Add(variables, "$Group.timeFrame.end", LisGroup.TimeFrameEnd);
                Add(variables, "$Group.url", LisGroup.Url);
            }

            // LIS Membership variable subsitutions
            if (LisMembership != null)
            {
                Add(variables, "$Membership.collectionSourcedId", LisMembership.CollectionSourcedId);
                Add(variables, "$Membership.course.groupIds", LisMembership.CourseGroupIds);
                Add(variables, "$Membership.createdTimestamp", LisMembership.CreatedTimestamp);
                Add(variables, "$Membership.dataSource", LisMembership.DataSource);
                Add(variables, "$Membership.personSourcedId", LisMembership.PersonSourcedId);
                Add(variables, "$Membership.role.scope.mentor", LisMembership.RoleScopeMentor);
                Add(variables, "$Membership.role", LisMembership.Role);
                Add(variables, "$Membership.sourcedId", LisMembership.SourcedId);
                Add(variables, "$Membership.status", LisMembership.Status);
            }

            // LIS Message variable subsitutions
            if (LisMessage != null)
            {
                Add(variables, "$Message.documentTarget", LisMessage.DocumentTarget);
                Add(variables, "$Message.height", LisMessage.Height);
                Add(variables, "$Message.locale", LisMessage.Locale);
                Add(variables, "$Message.returnUrl", LisMessage.ReturnUrl);
                Add(variables, "$Message.width", LisMessage.Width);
            }

            // LIS Person variable subsitutions
            if (LisPerson != null)
            {
                Add(variables, "$Person.address.country", LisPerson.AddressCountry);
                Add(variables, "$Person.address.locality", LisPerson.AddressLocality);
                Add(variables, "$Person.address.postcode", LisPerson.AddressPostCode);
                Add(variables, "$Person.address.statepr", LisPerson.AddressStatePr);
                Add(variables, "$Person.address.street1", LisPerson.AddressStreet1);
                Add(variables, "$Person.address.street2", LisPerson.AddressStreet2);
                Add(variables, "$Person.address.street3", LisPerson.AddressStreet3);
                Add(variables, "$Person.address.street4", LisPerson.AddressStreet4);
                Add(variables, "$Person.address.timezone", LisPerson.AddressTimezone);
                Add(variables, "$Person.email.personal", LisPerson.EmailPersonal);
                Add(variables, "$Person.email.primary", LisPerson.EmailPrimary);
                Add(variables, "$Person.name.full", LisPerson.NameFull);
                Add(variables, "$Person.name.family", LisPerson.NameFamily);
                Add(variables, "$Person.name.given", LisPerson.NameGiven);
                Add(variables, "$Person.name.middle", LisPerson.NameMiddle);
                Add(variables, "$Person.name.prefix", LisPerson.NamePrefix);
                Add(variables, "$Person.name.suffix", LisPerson.NameSuffix);
                Add(variables, "$Person.phone.home", LisPerson.PhoneHome);
                Add(variables, "$Person.phone.mobile", LisPerson.PhoneMobile);
                Add(variables, "$Person.phone.primary", LisPerson.PhonePrimary);
                Add(variables, "$Person.phone.work", LisPerson.PhoneWork);
                Add(variables, "$Person.sms", LisPerson.Sms);
                Add(variables, "$Person.sourcedId", LisPerson.SourcedId);
                Add(variables, "$Person.webaddress", LisPerson.WebAddress);
            }

            // Course Groups Service variable substitutions
            if (CourseGroup != null)
            {
                Add(variables, "$CourseGroup.id", CourseGroup.Id);
            }

            // LTI Context variable substitutions
            if (LtiContext != null)
            {
                // $Context.id.history before $Context.id: see the remarks on this method.
                Add(variables, "$Context.id.history", LtiContext.IdHistory);
                Add(variables, "$Context.id", LtiContext.Id);
                Add(variables, "$Context.label", LtiContext.Label);
                Add(variables, "$Context.org", LtiContext.Org);
                Add(variables, "$Context.sourcedId", LtiContext.SourcedId);
                Add(variables, "$Context.title", LtiContext.Title);
                Add(variables, "$Context.type", LtiContext.Type);
            }

            // LTI ResourceLink variable subsitutions
            if (LtiResourceLink != null)
            {
                Add(variables, "$ResourceLink.description", LtiResourceLink.Description);
                // $ResourceLink.id.history before $ResourceLink.id: see the remarks on this method.
                Add(variables, "$ResourceLink.id.history", LtiResourceLink.IdHistory);
                Add(variables, "$ResourceLink.id", LtiResourceLink.Id);
                Add(variables, "$ResourceLink.title", LtiResourceLink.Title);
            }

            // LTI User variable substitutions
            if (LtiUser != null)
            {
                Add(variables, "$User.id", LtiUser.Id);
                Add(variables, "$User.image", LtiUser.Image);
                Add(variables, "$User.org", LtiUser.Org);
                Add(variables, "$User.scope.mentor", LtiUser.ScopeMentor);
                Add(variables, "$User.username", LtiUser.Username);
            }

            // Tool Platform variable substitutions
            if (ToolPlatform != null)
            {
                Add(variables, "$ToolPlatform.productFamilyCode", ToolPlatform.ProductFamilyCode);
                Add(variables, "$ToolPlatform.version", ToolPlatform.Version);
            }

            // Tool Platform Instance variable substitutions
            if (ToolPlatfomInstance != null)
            {
                Add(variables, "$ToolPlatformInstance.contactEmail", ToolPlatfomInstance.ContactEmail);
                Add(variables, "$ToolPlatformInstance.description", ToolPlatfomInstance.Description);
                Add(variables, "$ToolPlatformInstance.guid", ToolPlatfomInstance.Guid);
                Add(variables, "$ToolPlatformInstance.name", ToolPlatfomInstance.Name);
                Add(variables, "$ToolPlatformInstance.url", ToolPlatfomInstance.Url);
            }

            return SubstituteAll(value, variables);
        }
    }

    #region Substitution value classes

    /// <summary>
    /// Substitution values for Course Groups Service variables.
    /// </summary>
    /// <remarks>
    /// Added by Course Groups Service 1.0.
    /// See https://www.imsglobal.org/spec/lti-gs/v1p0#custom-parameter-substitutions.
    /// </remarks>
    public class CourseGroup
    {
        /// <summary>
        /// The id of any one context group associated with the launch, or an empty string when
        /// the launch is not associated with a group.
        /// <para>
        /// Custom parameter substitution: $CourseGroup.id.
        /// </para>
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// Substitution values for LIS Course Offering variables.
    /// </summary>
    public class LisCourseOffering
    {
        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.academicSession.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AcademicSession { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string CourseNumber { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Credits { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LongDescription { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// $CourseOffering.sourcedId
        /// </summary>
        public string SourcedId { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Substitution values for LIS Course Section variables.
    /// </summary>
    public class LisCourseSection
    {
        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string CourseNumber { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Credits { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.dept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Dept { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.enrollControl.accept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string EnrollControlAccept { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.enrollControl.allowed.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string EnrollControlAllowed { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LongDescription { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.maxNumberofStudents.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string MaxNumberOfStudents { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.numberofStudents.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string NumberOfStudents { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// $CourseSection.sourceSectionId
        /// </summary>
        public string SourceSectionId { get; set; }

        /// <summary>
        /// $CourseSection.sourcedId
        /// </summary>
        public string SourcedId { get; set; }

        /// <summary>
        /// $CourseSection.timeFrame.begin
        /// </summary>
        public string TimeFrameBegin { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.timeFrame.end.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string TimeFrameEnd { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Substitution values for LIS Course Template variables.
    /// </summary>
    public class LisCourseTemplate
    {
        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string CourseNumber { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Credits { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LongDescription { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string SourcedId { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Substitution values for LIS Group variables.
    /// </summary>
    public class LisGroup
    {
        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.email.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.enrollControl.accept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string EnrollControlAccept { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.enrollControl.Allowed.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string EnrollControlAllowed { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.level.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.scheme.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.typevalue.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Typevalue { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LongDescription { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.parentId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string SourcedId { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.timeFrame.begin.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string TimeFrameBegin { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.timeFrame.end.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string TimeFrameEnd { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.url.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// Substitution values for LIS Membership variables.
    /// </summary>
    public class LisMembership
    {
        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.collectionSourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string CollectionSourcedId { get; set; }

        /// <summary>
        /// A comma separated list of the group ids the person is enrolled in, aligning to the
        /// group_enrollments array of the Names and Role Provisioning Service response.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.course.groupIds.
        /// Versions: added by Course Groups Service 1.0.
        /// </para>
        /// </summary>
        public string CourseGroupIds { get; set; }

        /// <summary>
        /// LIS membership variable.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.createdTimestamp.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string CreatedTimestamp { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.personSourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string PersonSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.role.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// role_scope_mentor property
        /// </summary>
        public string RoleScopeMentor { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string SourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.status.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// Substitution values for LIS Message variables.
    /// </summary>
    public class LisMessage
    {
        /// <summary>
        /// $Message.documentTarget
        /// </summary>
        public string DocumentTarget { get; set; }

        /// <summary>
        /// $Message.height
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// $Message.locale
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// $Message.returnUrl
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// $Message.width
        /// </summary>
        public string Width { get; set; }
    }

    /// <summary>
    /// Substitution values for LIS Persion variables.
    /// </summary>
    public class LisPerson
    {
        /// <summary>
        /// This field contains the country of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.country.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AddressCountry { get; set; }

        /// <summary>
        /// This field contains the locality of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.locality.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AddressLocality { get; set; }

        /// <summary>
        /// This field contains the postal code of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.postcode.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AddressPostCode { get; set; }

        /// <summary>
        /// This field contains the state or province of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.statepr.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AddressStatePr { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street1.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AddressStreet1 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street2.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AddressStreet2 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street3.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AddressStreet3 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street4.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AddressStreet4 { get; set; }

        /// <summary>
        /// This field contains the timezone of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.timezone.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string AddressTimezone { get; set; }

        /// <summary>
        /// This field contains the personal email address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.email.personal.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string EmailPersonal { get; set; }

        /// <summary>
        /// $Person.email.primary
        /// </summary>
        public string EmailPrimary { get; set; }

        /// <summary>
        /// $Person.name.family
        /// </summary>
        public string NameFamily { get; set; }

        /// <summary>
        /// $Person.name.full
        /// </summary>
        public string NameFull { get; set; }

        /// <summary>
        /// $Person.name.given
        /// </summary>
        public string NameGiven { get; set; }

        /// <summary>
        /// This field contains the middle name of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.middle.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string NameMiddle { get; set; }

        /// <summary>
        /// This field contains the name prefix of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.prefix.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string NamePrefix { get; set; }

        /// <summary>
        /// This field contains the name suffix of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.suffix.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string NameSuffix { get; set; }

        /// <summary>
        /// This field contains the home phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.home.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string PhoneHome { get; set; }

        /// <summary>
        /// This field contains the mobile phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.mobile.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string PhoneMobile { get; set; }

        /// <summary>
        /// This field contains the primary phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.primary.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string PhonePrimary { get; set; }

        /// <summary>
        /// This field contains the work phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.work.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string PhoneWork { get; set; }

        /// <summary>
        /// This field contains the SMS number for the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.sms.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string Sms { get; set; }

        /// <summary>
        /// $Person.sourcedId
        /// </summary>
        public string SourcedId { get; set; }

        /// <summary>
        /// This field contains the website address for the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.webaddress.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string WebAddress { get; set; }
    }

    /// <summary>
    /// Substitution values for LTI Context variables.
    /// </summary>
    public class LtiContext
    {
        /// <summary>
        /// $Context.id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// $Context.id.history
        /// </summary>
        public string IdHistory { get; set; }

        /// <summary>
        /// $Context.label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// $Context.org
        /// </summary>
        public string Org { get; set; }

        /// <summary>
        /// $Context.sourcedId
        /// </summary>
        public string SourcedId { get; set; }

        /// <summary>
        /// $Context.title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// $Context.type
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Substitution values for LTI ResourceLink variables.
    /// </summary>
    public class LtiResourceLink
    {
        /// <summary>
        /// $ResourceLink.description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// $ResourceLink.id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// $ResourceLink.id.history
        /// </summary>
        public string IdHistory { get; set; }

        /// <summary>
        /// $ResourceLink.title
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Substitution values for LTI User variables.
    /// </summary>
    public class LtiUser
    {
        /// <summary>
        /// $User.id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// $User.image
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// $User.org
        /// </summary>
        public string Org { get; set; }

        /// <summary>
        /// $User.scope.mentor
        /// </summary>
        public string ScopeMentor { get; set; }

        /// <summary>
        /// $User.username
        /// </summary>
        public string Username { get; set; }
    }

    /// <summary>
    /// Substitution values for Tool Platform variables.
    /// </summary>
    public class ToolPlatfom
    {
        /// <summary>
        /// $ToolPlatform.productFamilyCode
        /// </summary>
        public string ProductFamilyCode { get; set; }

        /// <summary>
        /// $ToolPlatform.version
        /// </summary>
        public string Version { get; set; }
    }

    /// <summary>
    /// Substitution values for Tool Platform Instance variables.
    /// </summary>
    public class ToolPlatfomInstance
    {
        /// <summary>
        /// $ToolPlatformInstance.contactEmail
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// $ToolPlatformInstance.description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// $ToolPlatformInstance.guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// $ToolPlatformInstance.name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// $ToolPlatformInstance.url
        /// </summary>
        public string Url { get; set; }
    }

    #endregion
}

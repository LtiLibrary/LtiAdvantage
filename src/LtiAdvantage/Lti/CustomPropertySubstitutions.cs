using System.Collections.Generic;
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
        /// Replace one variable with its value, wherever it occurs.
        /// </summary>
        /// <remarks>
        /// The variable name is escaped before it is used as a pattern, so the dots in a name
        /// like <c>$Context.id</c> match dots rather than any character. The value is applied
        /// through a match evaluator rather than as a replacement string, so a value that
        /// happens to contain <c>$1</c> or <c>$&amp;</c> is inserted literally instead of being
        /// read as a regex substitution.
        /// </remarks>
        /// <param name="value">The custom property value being substituted into.</param>
        /// <param name="variable">The variable to replace, including the leading '$'.</param>
        /// <param name="replacement">The value to insert. Null is treated as empty.</param>
        private static string Replace(string value, string variable, string replacement)
        {
            var substitution = replacement ?? string.Empty;

            return Regex.Replace(value, Regex.Escape(variable), _ => substitution, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Replace variable with value. Unknown variables are ignored.
        /// </summary>
        /// <remarks>
        /// Where one variable name is a prefix of another, the longer name is substituted first.
        /// Otherwise the shorter one consumes the start of the longer name and leaves its tail
        /// behind: substituting <c>$Context.id</c> before <c>$Context.id.history</c> turns
        /// "$Context.id.history" into "&lt;the context id&gt;.history".
        /// </remarks>
        /// <param name="value">Variable to replace.</param>
        /// <returns>Value if variable is known.</returns>
        private string SubstituteCustomVariable(string value)
        {
            // LIS Course Offering variable subsitutions
            if (LisCourseOffering != null)
            {
                value = Replace(value, "$CourseOffering.academicSession", LisCourseOffering.AcademicSession);
                value = Replace(value, "$CourseOffering.courseNumber", LisCourseOffering.CourseNumber);
                value = Replace(value, "$CourseOffering.credits", LisCourseOffering.Credits);
                value = Replace(value, "$CourseOffering.label", LisCourseOffering.Label);
                value = Replace(value, "$CourseOffering.longDescription", LisCourseOffering.LongDescription);
                value = Replace(value, "$CourseOffering.shortDescription", LisCourseOffering.ShortDescription);
                value = Replace(value, "$CourseOffering.sourcedId", LisCourseOffering.SourcedId);
                value = Replace(value, "$CourseOffering.title", LisCourseOffering.Title);
            }

            // LIS Course Section variable subsitutions
            if (LisCourseSection != null)
            {
                value = Replace(value, "$CourseSection.courseNumber", LisCourseSection.CourseNumber);
                value = Replace(value, "$CourseSection.credits", LisCourseSection.Credits);
                value = Replace(value, "$CourseSection.dataSource", LisCourseSection.DataSource);
                value = Replace(value, "$CourseSection.dept", LisCourseSection.Dept);
                value = Replace(value, "$CourseSection.enrollControl.accept", LisCourseSection.EnrollControlAccept);
                value = Replace(value, "$CourseSection.enrollControl.allowed", LisCourseSection.EnrollControlAllowed);
                value = Replace(value, "$CourseSection.label", LisCourseSection.Label);
                value = Replace(value, "$CourseSection.longDescription", LisCourseSection.LongDescription);
                value = Replace(value, "$CourseSection.maxNumberofStudents", LisCourseSection.MaxNumberOfStudents);
                value = Replace(value, "$CourseSection.numberofStudents", LisCourseSection.NumberOfStudents);
                value = Replace(value, "$CourseSection.shortDescription", LisCourseSection.ShortDescription);
                value = Replace(value, "$CourseSection.sourceSectionId", LisCourseSection.SourceSectionId);
                value = Replace(value, "$CourseSection.sourcedId", LisCourseSection.SourcedId);
                value = Replace(value, "$CourseSection.timeFrame.begin", LisCourseSection.TimeFrameBegin);
                value = Replace(value, "$CourseSection.timeFrame.end", LisCourseSection.TimeFrameEnd);
                value = Replace(value, "$CourseSection.title", LisCourseSection.Title);
            }

            // LIS Course Template variable subsitutions
            if (LisCourseTemplate != null)
            {
                value = Replace(value, "$CourseTemplate.courseNumber", LisCourseTemplate.CourseNumber);
                value = Replace(value, "$CourseTemplate.credits", LisCourseTemplate.Credits);
                value = Replace(value, "$CourseTemplate.label", LisCourseTemplate.Label);
                value = Replace(value, "$CourseTemplate.longDescription", LisCourseTemplate.LongDescription);
                value = Replace(value, "$CourseTemplate.shortDescription", LisCourseTemplate.ShortDescription);
                value = Replace(value, "$CourseTemplate.sourcedId", LisCourseTemplate.SourcedId);
                value = Replace(value, "$CourseTemplate.title", LisCourseTemplate.Title);
            }

            // LIS Group vaiable subsitutions
            if (LisGroup != null)
            {
                value = Replace(value, "$Group.email", LisGroup.Email);
                value = Replace(value, "$Group.enrollControl.accept", LisGroup.EnrollControlAccept);
                value = Replace(value, "$Group.enrollControl.allowed", LisGroup.EnrollControlAllowed);
                value = Replace(value, "$Group.grouptype.level", LisGroup.Level);
                value = Replace(value, "$Group.grouptype.scheme", LisGroup.Scheme);
                value = Replace(value, "$Group.grouptype.typevalue", LisGroup.Typevalue);
                value = Replace(value, "$Group.longDescription", LisGroup.LongDescription);
                value = Replace(value, "$Group.parentId", LisGroup.ParentId);
                value = Replace(value, "$Group.shortDescription", LisGroup.ShortDescription);
                value = Replace(value, "$Group.sourcedId", LisGroup.SourcedId);
                value = Replace(value, "$Group.timeFrame.begin", LisGroup.TimeFrameBegin);
                value = Replace(value, "$Group.timeFrame.end", LisGroup.TimeFrameEnd);
                value = Replace(value, "$Group.url", LisGroup.Url);
            }

            // LIS Membership variable subsitutions
            if (LisMembership != null)
            {
                value = Replace(value, "$Membership.collectionSourcedId", LisMembership.CollectionSourcedId);
                value = Replace(value, "$Membership.course.groupIds", LisMembership.CourseGroupIds);
                value = Replace(value, "$Membership.createdTimestamp", LisMembership.CreatedTimestamp);
                value = Replace(value, "$Membership.dataSource", LisMembership.DataSource);
                value = Replace(value, "$Membership.personSourcedId", LisMembership.PersonSourcedId);
                value = Replace(value, "$Membership.role.scope.mentor", LisMembership.RoleScopeMentor);
                value = Replace(value, "$Membership.role", LisMembership.Role);
                value = Replace(value, "$Membership.sourcedId", LisMembership.SourcedId);
                value = Replace(value, "$Membership.status", LisMembership.Status);
            }

            // LIS Message variable subsitutions
            if (LisMessage != null)
            {
                value = Replace(value, "$Message.documentTarget", LisMessage.DocumentTarget);
                value = Replace(value, "$Message.height", LisMessage.Height);
                value = Replace(value, "$Message.locale", LisMessage.Locale);
                value = Replace(value, "$Message.returnUrl", LisMessage.ReturnUrl);
                value = Replace(value, "$Message.width", LisMessage.Width);
            }

            // LIS Person variable subsitutions
            if (LisPerson != null)
            {
                value = Replace(value, "$Person.address.country", LisPerson.AddressCountry);
                value = Replace(value, "$Person.address.locality", LisPerson.AddressLocality);
                value = Replace(value, "$Person.address.postcode", LisPerson.AddressPostCode);
                value = Replace(value, "$Person.address.statepr", LisPerson.AddressStatePr);
                value = Replace(value, "$Person.address.street1", LisPerson.AddressStreet1);
                value = Replace(value, "$Person.address.street2", LisPerson.AddressStreet2);
                value = Replace(value, "$Person.address.street3", LisPerson.AddressStreet3);
                value = Replace(value, "$Person.address.street4", LisPerson.AddressStreet4);
                value = Replace(value, "$Person.address.timezone", LisPerson.AddressTimezone);
                value = Replace(value, "$Person.email.personal", LisPerson.EmailPersonal);
                value = Replace(value, "$Person.email.primary", LisPerson.EmailPrimary);
                value = Replace(value, "$Person.name.full", LisPerson.NameFull);
                value = Replace(value, "$Person.name.family", LisPerson.NameFamily);
                value = Replace(value, "$Person.name.given", LisPerson.NameGiven);
                value = Replace(value, "$Person.name.middle", LisPerson.NameMiddle);
                value = Replace(value, "$Person.name.prefix", LisPerson.NamePrefix);
                value = Replace(value, "$Person.name.suffix", LisPerson.NameSuffix);
                value = Replace(value, "$Person.phone.home", LisPerson.PhoneHome);
                value = Replace(value, "$Person.phone.mobile", LisPerson.PhoneMobile);
                value = Replace(value, "$Person.phone.primary", LisPerson.PhonePrimary);
                value = Replace(value, "$Person.phone.work", LisPerson.PhoneWork);
                value = Replace(value, "$Person.sms", LisPerson.Sms);
                value = Replace(value, "$Person.sourcedId", LisPerson.SourcedId);
                value = Replace(value, "$Person.webaddress", LisPerson.WebAddress);
            }

            // Course Groups Service variable substitutions
            if (CourseGroup != null)
            {
                value = Replace(value, "$CourseGroup.id", CourseGroup.Id);
            }

            // LTI Context variable substitutions
            if (LtiContext != null)
            {
                // $Context.id.history before $Context.id: see the remarks on this method.
                value = Replace(value, "$Context.id.history", LtiContext.IdHistory);
                value = Replace(value, "$Context.id", LtiContext.Id);
                value = Replace(value, "$Context.label", LtiContext.Label);
                value = Replace(value, "$Context.org", LtiContext.Org);
                value = Replace(value, "$Context.sourcedId", LtiContext.SourcedId);
                value = Replace(value, "$Context.title", LtiContext.Title);
                value = Replace(value, "$Context.type", LtiContext.Type);
            }

            // LTI ResourceLink variable subsitutions
            if (LtiResourceLink != null)
            {
                value = Replace(value, "$ResourceLink.description", LtiResourceLink.Description);
                // $ResourceLink.id.history before $ResourceLink.id: see the remarks on this method.
                value = Replace(value, "$ResourceLink.id.history", LtiResourceLink.IdHistory);
                value = Replace(value, "$ResourceLink.id", LtiResourceLink.Id);
                value = Replace(value, "$ResourceLink.title", LtiResourceLink.Title);
            }

            // LTI User variable substitutions
            if (LtiUser != null)
            {
                value = Replace(value, "$User.id", LtiUser.Id);
                value = Replace(value, "$User.image", LtiUser.Image);
                value = Replace(value, "$User.org", LtiUser.Org);
                value = Replace(value, "$User.scope.mentor", LtiUser.ScopeMentor);
                value = Replace(value, "$User.username", LtiUser.Username);
            }

            // Tool Platform variable substitutions
            if (ToolPlatform != null)
            {
                value = Replace(value, "$ToolPlatform.productFamilyCode", ToolPlatform.ProductFamilyCode);
                value = Replace(value, "$ToolPlatform.version", ToolPlatform.Version);
            }

            // Tool Platform Instance variable substitutions
            if (ToolPlatfomInstance != null)
            {
                value = Replace(value, "$ToolPlatformInstance.contactEmail", ToolPlatfomInstance.ContactEmail);
                value = Replace(value, "$ToolPlatformInstance.description", ToolPlatfomInstance.Description);
                value = Replace(value, "$ToolPlatformInstance.guid", ToolPlatfomInstance.Guid);
                value = Replace(value, "$ToolPlatformInstance.name", ToolPlatfomInstance.Name);
                value = Replace(value, "$ToolPlatformInstance.url", ToolPlatfomInstance.Url);
            }

            return value;
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

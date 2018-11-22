using System.Collections.Generic;
using System.Text.RegularExpressions;
using LtiAdvantageLibrary.Utilities;
using Newtonsoft.Json;

namespace LtiAdvantageLibrary.Lti
{
    public partial class LtiResourceLinkRequest
    {
        /// <summary>
        /// This is a map of key/value custom parameters which are to be included with the launch.
        /// </summary>
        public Dictionary<string, string> Custom
        {
            get { return this.GetClaimValue<Dictionary<string, string>>(Constants.LtiClaims.Custom); }
            set { this.SetClaimValue(Constants.LtiClaims.Custom, value);}
        }
        
        // These properties are used during custom variable substition. They are not normally
        // part of an LTI request.
        #region Custom Substitution Values

        /// <summary>
        /// A URI describing the context's organisational properties; for example, an ldap:// URI. 
        /// Multiple URIs can be separated using commas.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Context.org.
        /// Versions: 1.2.
        /// </para>
        /// </summary>
        public string ContextOrg { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.academicSession.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingAcademicSession { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingCourseNumber { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingCredits { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingLabel { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingLongDescription { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingShortDescription { get; set; }

        /// <summary>
        /// LIS course offering variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseOffering.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseOfferingTitle { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionCourseNumber { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionCredits { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionDataSource { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.dept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionDept { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.enrollControl.accept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionEnrollControlAccept { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.enrollControl.allowed.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionEnrollControlAllowed { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionLabel { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionLongDescription { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.maxNumberofStudents.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionMaxNumberOfStudents { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.numberofStudents.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionNumberOfStudents { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionShortDescription { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.sourceSectionId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionSourceSectionId { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.timeFrame.begin.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionTimeFrameBegin { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.timeFrame.end.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionTimeFrameEnd { get; set; }

        /// <summary>
        /// LIS course section variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseSection.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseSectionTitle { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.courseNumber.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateCourseNumber { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.credits.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateCredits { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.label.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateLabel { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateLongDescription { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateShortDescription { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateSourcedId { get; set; }

        /// <summary>
        /// LIS course template variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $CourseTemplate.title.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisCourseTemplateTitle { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.email.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupEmail { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.enrollControl.accept.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupEnrollControlAccept { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.enrollControl.Allowed.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupEnrollControlAllowed { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.level.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupGrouptypeLevel { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.scheme.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupGrouptypeScheme { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.grouptype.typevalue.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupGrouptypeTypevalue { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.longDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupLongDescription { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.parentId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupParentId { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.shortDescription.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupShortDescription { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupSourcedId { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.timeFrame.begin.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupTimeFrameBegin { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.timeFrame.end.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupTimeFrameEnd { get; set; }

        /// <summary>
        /// LIS group variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Group.url.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisGroupUrl { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.collectionSourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipCollectionSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.createdTimestamp.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipCreatedTimestamp { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.dataSource.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipDataSource { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.personSourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipPersonSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.role.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipRole { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.sourcedId.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipSourcedId { get; set; }

        /// <summary>
        /// LIS membership variable. 
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Membership.status.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisMembershipStatus { get; set; }

        /// <summary>
        /// This field contains the country of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.country.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressCountry { get; set; }

        /// <summary>
        /// This field contains the locality of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.locality.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressLocality { get; set; }

        /// <summary>
        /// This field contains the postal code of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.postcode.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressPostCode { get; set; }

        /// <summary>
        /// This field contains the state or province of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.statepr.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStatePr { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street1.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStreet1 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street2.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStreet2 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street3.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStreet3 { get; set; }

        /// <summary>
        /// This field contains the street address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.street4.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressStreet4 { get; set; }

        /// <summary>
        /// This field contains the timezone of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.address.timezone.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonAddressTimezone { get; set; }

        /// <summary>
        /// This field contains the personal email address of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.email.personal.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonEmailPersonal { get; set; }

        /// <summary>
        /// This field contains the middle name of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.middle.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonNameMiddle { get; set; }

        /// <summary>
        /// This field contains the name prefix of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.prefix.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonNamePrefix { get; set; }

        /// <summary>
        /// This field contains the name suffix of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.name.suffix.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonNameSuffix { get; set; }

        /// <summary>
        /// This field contains the home phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.home.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonPhoneHome { get; set; }

        /// <summary>
        /// This field contains the mobile phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.mobile.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonPhoneMobile { get; set; }

        /// <summary>
        /// This field contains the primary phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.primary.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonPhonePrimary { get; set; }

        /// <summary>
        /// This field contains the work phone number of the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.phone.work.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonPhoneWork { get; set; }

        /// <summary>
        /// This field contains the SMS number for the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.sms.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonSms { get; set; }

        /// <summary>
        /// This field contains the website address for the user account that is performing this launch.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $Person.webaddress.
        /// Versions: 1.0, 1.1, 1.2.
        /// </para>
        /// </summary>
        public string LisPersonWebAddress { get; set; }

        /// <summary>
        /// The username by which the user is known to the TC.
        /// Typically the name entered by the user when they log in.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $User.username.
        /// Versions: 1.2.
        /// </para>
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// A URI describing the user's organisational properties; for example, an ldap:// URI. 
        /// Multiple URIs can be separated using commas.
        /// <para>
        /// Parameter: n/a.
        /// Custom parameter substitution: $User.org.
        /// Versions: 1.2.
        /// </para>
        /// </summary>
        public string UserOrg { get; set; }

        #endregion

        public Dictionary<string, string> ReplaceCustomValues(Dictionary<string, string> source)
        {
            var dest = new Dictionary<string, string>();

            foreach (var pair in source)
            {
                // Substitute
                dest.Add(pair.Key, SubstituteCustomVariable(pair.Value));
            }

            return dest;
        }

        /// <summary>
        /// Substitute known custom value tokens. Per the LTI 1.1 spec, unknown
        /// tokens are ignored.
        /// </summary>
        /// <param name="value">Custom value to scan.</param>
        /// <returns>Custom value with the known tokens replaced by their
        /// current value.</returns>
        private string SubstituteCustomVariable(string value)
        {
            // LTI User variables
            value = Regex.Replace(value, "\\$User.id", UserId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.image", Picture ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.org", UserOrg ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.scope.mentor", JsonConvert.SerializeObject(RoleScopeMentor), RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.username", UserName ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Person variables
            value = Regex.Replace(value, "\\$Person.address.country", LisPersonAddressCountry ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.locality", LisPersonAddressLocality ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.postcode", LisPersonAddressPostCode ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.statepr", LisPersonAddressStatePr ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street1", LisPersonAddressStreet1 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street2", LisPersonAddressStreet2 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street3", LisPersonAddressStreet3 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street4", LisPersonAddressStreet4 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.timezone", LisPersonAddressTimezone ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.email.personal", LisPersonEmailPersonal ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.email.primary", Email ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.full", Name ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.family", FamilyName ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.given", GivenName ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.middle", LisPersonNameMiddle ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.prefix", LisPersonNamePrefix ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.suffix", LisPersonNameSuffix ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.home", LisPersonPhoneHome ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.mobile", LisPersonPhoneMobile ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.primary", LisPersonPhonePrimary ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.work", LisPersonPhoneWork ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.sms", LisPersonSms ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.sourcedId", Lis?.PersonSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.webaddress", LisPersonWebAddress ?? string.Empty, RegexOptions.IgnoreCase);

            // LTI Context variables
            value = Regex.Replace(value, "\\$Context.id", Context.Id ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Context.org", ContextOrg ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Context.label", Context.Label ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Context.title", Context.Title ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Context.type", JsonConvert.SerializeObject(Context.Type), RegexOptions.IgnoreCase);

            // LTI ResourceLink variables
            value = Regex.Replace(value, "\\$ResourceLink.description", ResourceLink.Description ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$ResourceLink.id", ResourceLink.Id ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$ResourceLink.title", ResourceLink.Title ?? string.Empty, RegexOptions.IgnoreCase);

            // LTI Course Template variables
            value = Regex.Replace(value, "\\$CourseTemplate.courseNumber", LisCourseTemplateCourseNumber ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.credits", LisCourseTemplateCredits ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.label", LisCourseTemplateLabel ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.longDescription", LisCourseTemplateLongDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.shortDescription", LisCourseTemplateShortDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.sourcedId", LisCourseTemplateSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseTemplate.title", LisCourseTemplateTitle ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Course Offering variables
            value = Regex.Replace(value, "\\$CourseOffering.academicSession", LisCourseOfferingAcademicSession ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.courseNumber", LisCourseOfferingCourseNumber ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.credits", LisCourseOfferingCredits ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.label", LisCourseOfferingLabel ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.longDescription", LisCourseOfferingLongDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.shortDescription", LisCourseOfferingShortDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.sourcedId", Lis?.CourseOfferingSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseOffering.title", LisCourseOfferingTitle ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Course Section variables
            value = Regex.Replace(value, "\\$CourseSection.courseNumber", LisCourseSectionCourseNumber ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.credits", LisCourseSectionCredits ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.dataSource", LisCourseSectionDataSource ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.dept", LisCourseSectionDept ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.enrollControl.accept", LisCourseSectionEnrollControlAccept ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.enrollControl.allowed", LisCourseSectionEnrollControlAllowed ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.label", LisCourseSectionLabel ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.longDescription", LisCourseSectionLongDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.maxNumberofStudents", LisCourseSectionMaxNumberOfStudents ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.numberofStudents", LisCourseSectionNumberOfStudents ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.shortDescription", LisCourseSectionShortDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.sourceSectionId", LisCourseSectionSourceSectionId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.sourcedId", Lis?.CourseSectionSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.timeFrame.begin", LisCourseSectionTimeFrameBegin ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.timeFrame.end", LisCourseSectionTimeFrameEnd ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.title", LisCourseSectionTitle ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Group vaiables
            value = Regex.Replace(value, "\\$Group.email", LisGroupEmail ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.enrollControl.accept", LisGroupEnrollControlAccept ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.enrollControl.allowed", LisGroupEnrollControlAllowed ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.grouptype.level", LisGroupGrouptypeLevel ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.grouptype.scheme", LisGroupGrouptypeScheme ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.grouptype.typevalue", LisGroupGrouptypeTypevalue ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.longDescription", LisGroupLongDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.parentId", LisGroupParentId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.shortDescription", LisGroupShortDescription ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.sourcedId", LisGroupSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.timeFrame.begin", LisGroupTimeFrameBegin ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.timeFrame.end", LisGroupTimeFrameEnd ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Group.url", LisGroupUrl ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Membership variables
            value = Regex.Replace(value, "\\$Membership.collectionSourcedId", LisMembershipCollectionSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.createdTimestamp", LisMembershipCreatedTimestamp ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.dataSource", LisMembershipDataSource ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.personSourcedId", LisMembershipPersonSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.role.scope.mentor", JsonConvert.SerializeObject(RoleScopeMentor), RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.role", LisMembershipRole ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.sourcedId", LisMembershipSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Membership.status", LisMembershipStatus ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Message variables
            if (LaunchPresentation != null)
            {
                value = Regex.Replace(value, "\\$Message.documentTarget", JsonConvert.SerializeObject(LaunchPresentation.DocumentTarget), RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "\\$Message.height", LaunchPresentation.Height.ToString(), RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "\\$Message.locale", LaunchPresentation.Locale ?? string.Empty, RegexOptions.IgnoreCase); 
                value = Regex.Replace(value, "\\$Message.returnUrl", LaunchPresentation.ReturnUrl ?? string.Empty, RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "\\$Message.width", LaunchPresentation.Width.ToString(), RegexOptions.IgnoreCase);
            }

            // Tool Platform variables
            if (Platform != null)
            {
                value = Regex.Replace(value, "\\$ToolPlatform.productFamilyCode", Platform.ProductFamilyCode ?? string.Empty, RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "\\$ToolPlatform.version", Platform.Version ?? string.Empty, RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "\\$ToolPlatformInstance.guid", Platform.Guid ?? string.Empty, RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "\\$ToolPlatformInstance.name", Platform.Name ?? string.Empty, RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "\\$ToolPlatformInstance.description", Platform.Description ?? string.Empty, RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "\\$ToolPlatformInstance.url", Platform.Url ?? string.Empty, RegexOptions.IgnoreCase);
                value = Regex.Replace(value, "\\$ToolPlatformInstance.contactEmail", Platform.ContactEmail ?? string.Empty, RegexOptions.IgnoreCase);
            }

            return value;
        }
    }
}

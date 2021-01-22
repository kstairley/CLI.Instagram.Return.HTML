using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TechShare.Parser.Instagram.Return.HTML.Support;
using TechShare.Utility.Tools.Exceptions;
using TechShare.Utility.Tools.HtmlParse;
using TechShare.Utility.Tools.Logs;
using TechShare.Utility.Tools.Zip;

namespace TechShare.Parser.Instagram.Return.HTML.Sections
{
    public class IndexParser : SectionParser
    {
        public IndexParser(CommandLineLogger logger, ExtractFileInfo fileInfo, bool isPreservation, string preservationPrefix, ParserVersionEnum version)
            : base(logger, fileInfo, isPreservation, preservationPrefix, version)
        {

        }

        #region Properties

        // private string VideosDefinition { get; set; }
        // added
        // private string Videos { get; set; }

        private string Service { get; set; }
        private string Target { get; set; }
        private string AccountIdentifier { get; set; }
        private string AccountType { get; set; }
        private string DateGeneratedUTC { get; set; }
        private string DateRangeUTC { get; set; }
        private string FirstName { get; set; }
        private string MiddleName { get; set; }
        private string LastName { get; set; }
        // Name Definition, Emails Definition, 
        // Registration Date Definition, Registration Ip Definition, Account End Date Definition
        // Ip Addresses Definition,   -- put photos back in 
        private string NameDefinition { get; set; }
        private string EmailsDefinition { get; set; }
        private string IpAddressesDefinition { get; set; }      
        // adding for LIVE VIDEOS fix
        private string LiveVideos { get; set; }
        private string VideosDefinition { get; set; }

        // added
        private string Videos { get; set; }
        private string PhotosDefinition { get; set; }
        private string Unified_Messages { get; set; }

        private IEnumerable<RegisterdEmail> RegisteredEmailAddresses { get; set; }
        private string RegisteredEmailAddressString
        {
            get
            {
                return RegisteredEmailAddresses != null && RegisteredEmailAddresses.Any() ? string.Join("; ", RegisteredEmailAddresses.Where(x => x.HasData).Select(x => x.Email)) : null;
            }
        }
        private string VanityName { get; set; }
        // new VanityDefinition, Registration Date Definition, Registration Ip Definition, Account End Date Definition, Phone Numbers Definition, Photos

        private string Photos { get; set; }
        private string VanityDefinition { get; set; }
        // added
        private string VanityChangesDefinition { get; set; }
        private string VanityChanges { get; set; }
        private string RegistrationDateDefinition { get; set; }
        private string RegistrationDate { get; set; }
        private string RegistrationIP { get; set; }
        private string RegistrationIpDefinition { get; set; }
        private string AccountClosureDateUTC { get; set; }
        private string AccountEndDateDefinition { get; set; }
        private string IsAccountActive { get; set; }
        private string Street { get; set; }
        private string City { get; set; }
        private string State { get; set; }
        private string Zip { get; set; }
        private string Country { get; set; }
        private string CurrentCity { get; set; }
        private IEnumerable<Phone> PhoneNumbers { get; set; }
        private string PhoneNumbersDefinition { get; set; }
        private string PhoneNumberString
        {
            get
            {
                return (PhoneNumbers != null && PhoneNumbers.Any()) ? string.Join("; ", PhoneNumbers.Where(x => x.HasData).Select(x => x.ToString())) : null;
            }
        }
        private IEnumerable<CreditCard> CreditCards { get; set; }
        private string CreditCardString
        {
            get
            {
                return CreditCards != null && CreditCards.Any() ? string.Join("; ", CreditCards.Where(x => x.HasData).Select(x => x.ToString())) : null;
            }
        }
        private IEnumerable<IPAddress> IPAddresses { get; set; }
        protected override bool HasData
        {
            get
            {
                return (
                    !string.IsNullOrEmpty(AccountClosureDateUTC) ||
                    !string.IsNullOrEmpty(AccountIdentifier) ||
                    !string.IsNullOrEmpty(AccountType) ||
                    !string.IsNullOrEmpty(City) ||
                    !string.IsNullOrEmpty(Country) ||
                    (CreditCards != null && CreditCards.Any()) ||
                    !string.IsNullOrEmpty(CurrentCity) ||
                    !string.IsNullOrEmpty(DateGeneratedUTC) ||
                    !string.IsNullOrEmpty(DateRangeUTC) ||
                    !string.IsNullOrEmpty(FirstName) ||
                    (IPAddresses != null && IPAddresses.Any()) ||
                    !string.IsNullOrEmpty(IsAccountActive) ||
                    !string.IsNullOrEmpty(LastName) ||
                    !string.IsNullOrEmpty(MiddleName) ||
                    // new NameDefinition, EmailsDefinition, Phone Numbers Definition
                    // Registration Date Definition, Registration Ip Definition, Account End Date Definition
                    // Ip Addresses Definition, PhotosDefinition and VideosDefinition
                    !string.IsNullOrEmpty(RegistrationDateDefinition) ||
                    !string.IsNullOrEmpty(RegistrationIpDefinition) ||
                    !string.IsNullOrEmpty(AccountEndDateDefinition) ||
                    !string.IsNullOrEmpty(IpAddressesDefinition) ||
                    !string.IsNullOrEmpty(NameDefinition) ||
                    !string.IsNullOrEmpty(PhotosDefinition) ||
                    !string.IsNullOrEmpty(LiveVideos) ||
                    !string.IsNullOrEmpty(VideosDefinition) ||
                    !string.IsNullOrEmpty(Videos) ||
                    !string.IsNullOrEmpty(EmailsDefinition) ||
                    !string.IsNullOrEmpty(PhoneNumbersDefinition) ||
                    (PhoneNumbers != null && PhoneNumbers.Any()) ||
                    (RegisteredEmailAddresses != null && RegisteredEmailAddresses.Any()) ||
                    !string.IsNullOrEmpty(RegistrationDate) ||
                    !string.IsNullOrEmpty(RegistrationIP) ||
                    // added
                    !string.IsNullOrEmpty(VideosDefinition) ||
                    !string.IsNullOrEmpty(Videos) ||
                    !string.IsNullOrEmpty(Service) ||
                    !string.IsNullOrEmpty(State) ||
                    !string.IsNullOrEmpty(Street) ||
                    !string.IsNullOrEmpty(Target) ||
                    !string.IsNullOrEmpty(VanityName) ||
                    // new
                    !string.IsNullOrEmpty(Photos) ||
                    !string.IsNullOrEmpty(VanityDefinition) ||
                    !string.IsNullOrEmpty(VanityChangesDefinition) ||
                    !string.IsNullOrEmpty(VanityChanges) ||
                    // added Unified_Messages
                    !string.IsNullOrEmpty(Unified_Messages) ||
                    !string.IsNullOrEmpty(Zip)
                    );
            }
        }
        #endregion

        #region Functions
        protected override void ProcessHTML()
        {
            if (HtmlDoc != null && HtmlDoc.HasData)
            {
                foreach (ParseDataItem dataItem in HtmlDoc.Items)
                {
                    switch (dataItem.Header.ToUpper().Trim())
                    {
                        case "SERVICE":
                            Service = dataItem.Value;
                            break;
                        case "TARGET":
                            Target = dataItem.Value;
                            break;
                        case "ACCOUNT IDENTIFIER":
                            AccountIdentifier = dataItem.Value;
                            break;
                        case "ACCOUNT TYPE":
                            AccountType = dataItem.Value;
                            break;
                        case "GENERATED":
                            DateGeneratedUTC = dataItem.Value;
                            break;
                        case "DATE RANGE":
                            DateRangeUTC = dataItem.Value;
                            break;
                        case "PHONE NUMBERS DEFINITION":
                            PhoneNumbersDefinition = dataItem.Value;
                            break;
                        case "EMAILS DEFINITION":
                            EmailsDefinition = dataItem.Value;
                            break;
                        case "NAME DEFINITION":
                            NameDefinition = dataItem.Value;
                            break;
                        case "NAME":
                            if (dataItem.HasChildren)
                            {
                                foreach (ParseDataItem childDataItem in dataItem.Children)
                                {
                                    switch (childDataItem.Header.ToUpper().Trim())
                                    {
                                        case "FIRST":
                                            FirstName = childDataItem.Value;
                                            break;
                                        case "MIDDLE":
                                            MiddleName = childDataItem.Value;
                                            break;
                                        case "LAST":
                                            LastName = childDataItem.Value;
                                            break;
                                        default:
                                            Logger.LogWarning("Unexpected Html Element - \"" + DisplaySectionName + ": " + dataItem.Header + " - " + childDataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                                throw new ApplicationException(dataItem.Header);
#endif
                                                break;
                                        }
                                    }
                                }
                                break;

                        case "REGISTERED EMAIL ADDRESSES":
                            if (dataItem.HasValues)
                            {
                                List<RegisterdEmail> registeredEmails = new List<RegisterdEmail>();
                                foreach (string value in dataItem.Values)
                                {
                                    if (!string.IsNullOrEmpty(value) && !value.StartsWith("No responsive records", StringComparison.InvariantCultureIgnoreCase))
                                        registeredEmails.Add(new RegisterdEmail(value));
                                }
                                if (registeredEmails.Count() > 0)
                                    RegisteredEmailAddresses = registeredEmails;
                            }
                            break;
                        case "VANITY NAME":
                            // VanityName = dataItem.Value;
							VanityName = !string.IsNullOrEmpty(dataItem.Value) && !dataItem.Value.StartsWith("No responsive records", StringComparison.InvariantCultureIgnoreCase) ? dataItem.Value : null;
                            break;
                        // new
                        case "VANITY DEFINITION":
                            VanityDefinition = dataItem.Value;
                            break;
                        // added
                        case "VANITY CHANGES DEFINITION":
                            VanityChangesDefinition = dataItem.Value;
                            break;
                        case "VANITY CHANGES":
                            VanityChanges = dataItem.Value;
                            break;
                        case "PHOTOS":
                            Photos = dataItem.Value;  // also in PhotosParse.cs
                            break;
                        case "PHOTOS DEFINITION":
                            PhotosDefinition = dataItem.Value;
                            break;
                        case "UNIFIED MESSAGES":
                            Unified_Messages = dataItem.Value;
                            break;
                        case "LIVE VIDEOS":
                            LiveVideos = dataItem.Value;
                            break;
                        case "VIDEOS DEFINITION":
                            VideosDefinition = dataItem.Value;
                            break;
                        case "VIDEOS":
                            Videos = dataItem.Value;
                            break;
                        case "REGISTRATION DATE DEFINITION":
                            RegistrationIP = dataItem.Value;
                            break;
                        case "REGISTRATION IP DEFINITION":
                            RegistrationIpDefinition = dataItem.Value;
                            break;
                        case "ACCOUNT END DATE DEFINITION":
                            AccountEndDateDefinition = dataItem.Value;
                            break;
                        case "IP ADDRESSES DEFINITION":
                            IpAddressesDefinition = dataItem.Value;
                            break;
                        case "REGISTRATION DATE":
                            RegistrationDate = dataItem.Value;
                            break;
                        case "REGISTRATION IP":
                            RegistrationIP = dataItem.Value;
                            break;
                        case "ACCOUNT CLOSURE DATE":
                            if (dataItem.HasChildren)
                            {
                                foreach (ParseDataItem childDataItem in dataItem.Children)
                                {
                                    switch (childDataItem.Header.ToUpper().Trim())
                                    {
                                        case "ACCOUNT STILL ACTIVE":
                                            IsAccountActive = childDataItem.Value;
                                            break;
                                        default:
                                            Logger.LogWarning("Unexpected Html Element - \"" + DisplaySectionName + ": " + dataItem.Header + " - " + childDataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                                throw new ApplicationException(dataItem.Header);
#endif
                                                break;
                                        }
                                    }
                                }
                                break;
                            case "PHONE NUMBERS":
                                if (dataItem.HasValues)
                                {
                                    List<Phone> phoneRecords = new List<Phone>();
                                    foreach (string value in dataItem.Values)
                                    {
                                        if (!string.IsNullOrEmpty(value) && !value.StartsWith("No responsive records", StringComparison.InvariantCultureIgnoreCase))
                                            phoneRecords.Add(new Phone(value));
                                    }
                                    if (phoneRecords.Count() > 0)
                                        PhoneNumbers = phoneRecords;
                                }
                                break;
                            case "IP ADDRESSES":
                                IEnumerable<IPAddress> IPAddressList = IPAddress.BuildList(Logger, DisplaySectionName, dataItem);
                                if (IPAddressList.Count() > 0)
                                    IPAddresses = IPAddressList;
                                break;
                            case "ADDITIONAL PROPERTIES":
                            case "PREVIOUS PRESERVATIONS":
                                break;
                            default:
                                Logger.LogWarning("Unexpected Html Element - \"" + DisplaySectionName + ": " + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                throw new ApplicationException(dataItem.Header);
#endif
                                break;

                    }
                }
            }

            if (!HasData)
                throw new SectionEmptyException(DisplaySectionName);
        }

        public override IEnumerable<DataTable> GenerateDataTables()
        {
            List<DataTable> retVal = new List<DataTable>();
            retVal.Add(GenerateIndexTable());
            retVal.Add(GenerateNewIndexTable());
            retVal.Add(GenerateRegisteredEmailAddressesTable());
            retVal.Add(GeneratePhoneNumbersTable());
            retVal.Add(GenerateCreditCardsTable());
            retVal.Add(GenerateCreditCardTransactionsTable());
            retVal.Add(GenerateIPAddressesTable());
            return retVal;
        }
        public override IEnumerable<LocationDataPoint> GenerateLocationInformation()
        {
            return null;
        }

        private DataTable GenerateIndexTable()
        {
            DataTable retVal = new DataTable(MainTableName);
            retVal.Columns.Add("Service", typeof(string));
            retVal.Columns.Add("TargetId", typeof(string));
            retVal.Columns.Add("DateIndexGeneratedUTC", typeof(string));
            retVal.Columns.Add("DateRangeUTC", typeof(string));
            retVal.Columns.Add("AcctOwnerFirstName", typeof(string));
            retVal.Columns.Add("AcctOwnerMidName", typeof(string));
            retVal.Columns.Add("AcctOwnerLastName", typeof(string));
            retVal.Columns.Add("RegisteredEmailAddresses", typeof(string));
            retVal.Columns.Add("VanityName", typeof(string));
            // new
            retVal.Columns.Add("RegistrationDateDefinition", typeof(string));
            retVal.Columns.Add("RegistrationIpDefinition", typeof(string));
            retVal.Columns.Add("AccountEndDateDefinition", typeof(string));
            retVal.Columns.Add("IpAddressesDefinition", typeof(string));
            retVal.Columns.Add("VanityDefinition", typeof(string));
            retVal.Columns.Add("VanityChangesDefinition", typeof(string));
            retVal.Columns.Add("VanityChanges", typeof(string));
            // added Photos ... Videos
            retVal.Columns.Add("Photos", typeof(string));
            retVal.Columns.Add("PhoneNumbersDefinition", typeof(string));
            retVal.Columns.Add("NameDefinition", typeof(string));
            retVal.Columns.Add("PhotosDefinition", typeof(string));
            retVal.Columns.Add("Unified_Messages", typeof(string));
            retVal.Columns.Add("LiveVideos", typeof(string));
            retVal.Columns.Add("VideosDefinition", typeof(string));
            retVal.Columns.Add("Videos", typeof(string));
            retVal.Columns.Add("EmailsDefinition", typeof(string));
            retVal.Columns.Add("RegistrationDateUTC", typeof(string));
            retVal.Columns.Add("RegistrationIP", typeof(string));
            retVal.Columns.Add("IsAccountStillActive", typeof(string));
            retVal.Columns.Add("AcctOwnerAddressStreet", typeof(string));
            retVal.Columns.Add("AcctOwnerCity", typeof(string));
            retVal.Columns.Add("AcctOwnerState", typeof(string));
            retVal.Columns.Add("AcctOwnerZip", typeof(string));
            retVal.Columns.Add("AcctOwnerCountry", typeof(string));
            retVal.Columns.Add("CurrentCity", typeof(string));
            retVal.Columns.Add("PhoneNumbers", typeof(string));
            retVal.Columns.Add("CreditCards", typeof(string));
            retVal.Columns.Add("File", typeof(string));

            if (HasData)
            {
                DataRow row = retVal.NewRow();
                row["Service"] = !string.IsNullOrEmpty(Service) ? Service : null;
                row["TargetId"] = !string.IsNullOrEmpty(Target) ? Target : null;
                row["DateIndexGeneratedUTC"] = !string.IsNullOrEmpty(DateGeneratedUTC) ? DateGeneratedUTC : null;
                row["DateRangeUTC"] = !string.IsNullOrEmpty(DateRangeUTC) ? DateRangeUTC : null;
                row["AcctOwnerFirstName"] = !string.IsNullOrEmpty(FirstName) ? FirstName : null;
                row["AcctOwnerMidName"] = !string.IsNullOrEmpty(MiddleName) ? MiddleName : null;
                row["AcctOwnerLastName"] = !string.IsNullOrEmpty(LastName) ? LastName : null;
                row["RegisteredEmailAddresses"] = !string.IsNullOrEmpty(RegisteredEmailAddressString) ? RegisteredEmailAddressString : null;
                row["VanityName"] = !string.IsNullOrEmpty(VanityName) ? VanityName : null;
                // new
                row["RegistrationDateDefinition"] = !string.IsNullOrEmpty(RegistrationDateDefinition) ? RegistrationDateDefinition : null;
                row["RegistrationIpDefinition"] = !string.IsNullOrEmpty(RegistrationIpDefinition) ? RegistrationIpDefinition : null;
                row["AccountEndDateDefinition"] = !string.IsNullOrEmpty(AccountEndDateDefinition) ? AccountEndDateDefinition : null;
                row["IpAddressesDefinition"] = !string.IsNullOrEmpty(IpAddressesDefinition) ? IpAddressesDefinition: null;
                row["VanityDefinition"] = !string.IsNullOrEmpty(VanityDefinition) ? VanityDefinition : null;
                // added
                row["VanityChangesDefinition"] = !string.IsNullOrEmpty(VanityChangesDefinition) ? VanityChangesDefinition : null;
                row["VanityChanges"] = !string.IsNullOrEmpty(VanityChanges) ? VanityChanges : null;
                row["Photos"] = !string.IsNullOrEmpty(Photos) ? Photos : null;
                row["PhotosDefinition"] = !string.IsNullOrEmpty(PhotosDefinition) ? PhotosDefinition : null;
                row["Unified_Messages"] = !string.IsNullOrEmpty(Unified_Messages) ? Unified_Messages : null;
                // error w/2 "VideosDefinition" ??
                row["Videos"] = !string.IsNullOrEmpty(Videos) ? Videos : null;
                row["VideosDefinition"] = !string.IsNullOrEmpty(VideosDefinition) ? VideosDefinition : null;
                row["LiveVideos"] = !string.IsNullOrEmpty(LiveVideos) ? LiveVideos : null;
                // temp fix?
               // row["Live Videos"] = !string.IsNullOrEmpty(LiveVideos) ? LiveVideos : null;
                row["EmailsDefinition"] = !string.IsNullOrEmpty(EmailsDefinition) ? EmailsDefinition : null;
                row["PhoneNumbersDefinition"] = !string.IsNullOrEmpty(PhoneNumbersDefinition) ? PhoneNumbersDefinition : null;
                row["RegistrationDateUTC"] = !string.IsNullOrEmpty(RegistrationDate) ? RegistrationDate : null;
                row["RegistrationIP"] = !string.IsNullOrEmpty(RegistrationIP) ? RegistrationIP : null;
                row["IsAccountStillActive"] = !string.IsNullOrEmpty(IsAccountActive) ? IsAccountActive : null;
                row["AcctOwnerAddressStreet"] = !string.IsNullOrEmpty(Street) ? Street : null;
                row["AcctOwnerCity"] = !string.IsNullOrEmpty(City) ? City : null;
                row["AcctOwnerState"] = !string.IsNullOrEmpty(State) ? State : null;
                row["AcctOwnerZip"] = !string.IsNullOrEmpty(Zip) ? Zip : null;
                row["AcctOwnerCountry"] = !string.IsNullOrEmpty(Country) ? Country : null;
                row["CurrentCity"] = !string.IsNullOrEmpty(CurrentCity) ? CurrentCity : null;
                row["PhoneNumbers"] = !string.IsNullOrEmpty(PhoneNumberString) ? PhoneNumberString : null;
                row["CreditCards"] = !string.IsNullOrEmpty(CreditCardString) ? CreditCardString : null;
                row["File"] = !string.IsNullOrEmpty(SourceFile) ? SourceFile : null;
                retVal.Rows.Add(row);
            }

            return retVal;
        }
        private DataTable GenerateNewIndexTable()
        {
            DataTable retVal = new DataTable(MainTableName + "New");
            retVal.Columns.Add("Service", typeof(string));
            retVal.Columns.Add("TargetId", typeof(string));
            retVal.Columns.Add("AccountIdentifier", typeof(string));
            retVal.Columns.Add("AccountType", typeof(string));
            retVal.Columns.Add("DateIndexGeneratedUTC", typeof(string));
            retVal.Columns.Add("DateRangeUTC", typeof(string));
            retVal.Columns.Add("AcctOwnerFirstName", typeof(string));
            retVal.Columns.Add("AcctOwnerMidName", typeof(string));
            retVal.Columns.Add("AcctOwnerLastName", typeof(string));
            retVal.Columns.Add("VanityName", typeof(string));
            // new
            retVal.Columns.Add("PhotosDefinition", typeof(string));
            retVal.Columns.Add("Unified_Messages", typeof(string));
            retVal.Columns.Add("Videos", typeof(string));
            retVal.Columns.Add("LiveVideos", typeof(string));
            retVal.Columns.Add("Live Videos", typeof(string)); // don't think we can add a column name that has a space
            retVal.Columns.Add("VideosDefinition", typeof(string));
            retVal.Columns.Add("RegistrationDateDefinition", typeof(string));
            retVal.Columns.Add("RegistrationIpDefinition", typeof(string));
            retVal.Columns.Add("AccountEndDateDefinition", typeof(string));
            retVal.Columns.Add("IpAddressesDefinition", typeof(string));
            retVal.Columns.Add("VanityDefinition", typeof(string));
            // added
            retVal.Columns.Add("VanityChangesDefinition", typeof(string));
            retVal.Columns.Add("VanityChanges", typeof(string));
            retVal.Columns.Add("Photos", typeof(string));
            retVal.Columns.Add("EmailsDefinition", typeof(string));
            retVal.Columns.Add("PhoneNumbersDefinition", typeof(string));
            retVal.Columns.Add("RegistrationDateUTC", typeof(string));
            retVal.Columns.Add("RegistrationIP", typeof(string));
            retVal.Columns.Add("AccountClosureDate", typeof(string));
            retVal.Columns.Add("IsAccountStillActive", typeof(string));
            retVal.Columns.Add("AcctOwnerAddressStreet", typeof(string));
            retVal.Columns.Add("AcctOwnerCity", typeof(string));
            retVal.Columns.Add("AcctOwnerState", typeof(string));
            retVal.Columns.Add("AcctOwnerZip", typeof(string));
            retVal.Columns.Add("AcctOwnerCountry", typeof(string));
            retVal.Columns.Add("CurrentCity", typeof(string));
            retVal.Columns.Add("File", typeof(string));

            if (HasData)
            {
                DataRow row = retVal.NewRow();
                row["Service"] = !string.IsNullOrEmpty(Service) ? Service : null;
                row["TargetId"] = !string.IsNullOrEmpty(Target) ? Target : null;
                row["AccountIdentifier"] = !string.IsNullOrEmpty(AccountIdentifier) ? AccountIdentifier : null;
                row["AccountType"] = !string.IsNullOrEmpty(AccountType) ? AccountType : null;
                row["DateIndexGeneratedUTC"] = !string.IsNullOrEmpty(DateGeneratedUTC) ? DateGeneratedUTC : null;
                row["DateRangeUTC"] = !string.IsNullOrEmpty(DateRangeUTC) ? DateRangeUTC : null;
                row["AcctOwnerFirstName"] = !string.IsNullOrEmpty(FirstName) ? FirstName : null;
                row["AcctOwnerMidName"] = !string.IsNullOrEmpty(MiddleName) ? MiddleName : null;
                row["AcctOwnerLastName"] = !string.IsNullOrEmpty(LastName) ? LastName : null;
                row["VanityName"] = !string.IsNullOrEmpty(VanityName) ? VanityName : null;
                // new Phone Numbers Definition
                row["RegistrationDateDefinition"] = !string.IsNullOrEmpty(RegistrationDateDefinition) ? RegistrationDateDefinition : null;
                row["RegistrationIpDefinition"] = !string.IsNullOrEmpty(RegistrationIpDefinition) ? RegistrationIpDefinition : null;
                row["AccountEndDateDefinition"] = !string.IsNullOrEmpty(AccountEndDateDefinition) ? AccountEndDateDefinition : null;
                row["IpAddressesDefinition"] = !string.IsNullOrEmpty(IpAddressesDefinition) ? IpAddressesDefinition : null;
                row["VanityDefinition"] = !string.IsNullOrEmpty(VanityDefinition) ? VanityDefinition : null;
                // added
                row["VanityChangesDefinition"] = !string.IsNullOrEmpty(VanityChangesDefinition) ? VanityChangesDefinition : null;
                row["VanityChanges"] = !string.IsNullOrEmpty(VanityChanges) ? VanityChanges : null;
                row["Photos"] = !string.IsNullOrEmpty(Photos) ? Photos : null;
                row["PhotosDefinition"] = !string.IsNullOrEmpty(PhotosDefinition) ? PhotosDefinition : null;
                row["Unified_Messages"] = !string.IsNullOrEmpty(Unified_Messages) ? Unified_Messages : null;
                row["Videos"] = !string.IsNullOrEmpty(Videos) ? Videos : null;
                row["LiveVideos"] = !string.IsNullOrEmpty(LiveVideos) ? LiveVideos : null;
                // temp fix?
                row["Live Videos"] = !string.IsNullOrEmpty(LiveVideos) ? LiveVideos : null;
                row["VideosDefinition"] = !string.IsNullOrEmpty(VideosDefinition) ? VideosDefinition : null;

                row["EmailsDefinition"] = !string.IsNullOrEmpty(EmailsDefinition) ? EmailsDefinition : null;
                row["PhoneNumbersDefinition"] = !string.IsNullOrEmpty(PhoneNumbersDefinition) ? PhoneNumbersDefinition: null;
                row["RegistrationDateUTC"] = !string.IsNullOrEmpty(RegistrationDate) ? RegistrationDate : null;
                row["RegistrationIP"] = !string.IsNullOrEmpty(RegistrationIP) ? RegistrationIP : null;
                row["AccountClosureDate"] = !string.IsNullOrEmpty(AccountClosureDateUTC) ? AccountClosureDateUTC : null;
                row["IsAccountStillActive"] = !string.IsNullOrEmpty(IsAccountActive) ? IsAccountActive : null;
                row["AcctOwnerAddressStreet"] = !string.IsNullOrEmpty(Street) ? Street : null;
                row["AcctOwnerCity"] = !string.IsNullOrEmpty(City) ? City : null;
                row["AcctOwnerState"] = !string.IsNullOrEmpty(State) ? State : null;
                row["AcctOwnerZip"] = !string.IsNullOrEmpty(Zip) ? Zip : null;
                row["AcctOwnerCountry"] = !string.IsNullOrEmpty(Country) ? Country : null;
                row["CurrentCity"] = !string.IsNullOrEmpty(City) ? City : null;
                row["File"] = !string.IsNullOrEmpty(SourceFile) ? SourceFile : null;
                retVal.Rows.Add(row);
            }

            return retVal;
        }
        private DataTable GenerateRegisteredEmailAddressesTable()
        {
            DataTable retVal = new DataTable(MainTableName + "_RegisteredEmails");
            retVal.Columns.Add("TargetId");
            retVal.Columns.Add("Email");

            if (RegisteredEmailAddresses != null && RegisteredEmailAddresses.Any())
            {
                foreach (RegisterdEmail email in RegisteredEmailAddresses)
                {
                    if (email.HasData)
                    {
                        DataRow row = retVal.NewRow();
                        row["TargetId"] = !string.IsNullOrEmpty(Target) ? Target : null;
                        row["Email"] = !string.IsNullOrEmpty(email.Email) ? email.Email : null;
                        retVal.Rows.Add(row);
                    }
                }
            }
            return retVal;
        }
        private DataTable GeneratePhoneNumbersTable()
        {
            DataTable retVal = new DataTable(MainTableName + "_PhoneNumbers");
            retVal.Columns.Add("TargetId");
            retVal.Columns.Add("PhoneNumber");
            retVal.Columns.Add("Type");
            retVal.Columns.Add("Verified");
            retVal.Columns.Add("VerifiedOnUTC");

            if (PhoneNumbers != null && PhoneNumbers.Any())
            {
                foreach (Phone phone in PhoneNumbers)
                {
                    if (phone.HasData)
                    {
                        DataRow row = retVal.NewRow();
                        row["TargetId"] = !string.IsNullOrEmpty(Target) ? Target : null;
                        row["PhoneNumber"] = !string.IsNullOrEmpty(phone.PhoneNumber) ? phone.PhoneNumber : null;
                        row["Type"] = !string.IsNullOrEmpty(phone.Type) ? phone.Type : null;
                        row["Verified"] = !string.IsNullOrEmpty(phone.Verified) ? phone.Verified : null;
                        row["VerifiedOnUTC"] = !string.IsNullOrEmpty(phone.VerifiedOnUTC) ? phone.VerifiedOnUTC : null;
                        retVal.Rows.Add(row);
                    }
                }
            }
            return retVal;
        }
        private DataTable GenerateCreditCardsTable()
        {
            DataTable retVal = new DataTable(MainTableName + "_CreditCards");
            retVal.Columns.Add("TargetId");
            retVal.Columns.Add("CCGuid");
            retVal.Columns.Add("Identifier");
            retVal.Columns.Add("PaymentAccountID");
            retVal.Columns.Add("CCOwnerFirstName");
            retVal.Columns.Add("CCOwnerMiddleName");
            retVal.Columns.Add("CCOwnerLastName");
            retVal.Columns.Add("BillingAddressStreet");
            retVal.Columns.Add("BillingAddressStreet2");
            retVal.Columns.Add("BillingAddressCity");
            retVal.Columns.Add("BillingAddressState");
            retVal.Columns.Add("BillingAddressZip");
            retVal.Columns.Add("BillingAddressCountry");

            if (CreditCards != null && CreditCards.Any())
            {
                foreach (CreditCard cc in CreditCards)
                {
                    if (cc.HasData)
                    {
                        DataRow row = retVal.NewRow();

                        row["TargetId"] = !string.IsNullOrEmpty(Target) ? Target : null;
                        row["CCGuid"] = !string.IsNullOrEmpty(cc.GUID) ? cc.GUID : null;
                        row["Identifier"] = !string.IsNullOrEmpty(cc.ID) ? cc.ID : null;
                        row["PaymentAccountID"] = !string.IsNullOrEmpty(cc.PaymentAccountID) ? cc.PaymentAccountID : null;
                        row["CCOwnerFirstName"] = !string.IsNullOrEmpty(cc.First) ? cc.First : null;
                        row["CCOwnerMiddleName"] = !string.IsNullOrEmpty(cc.Middle) ? cc.Middle : null;
                        row["CCOwnerLastName"] = !string.IsNullOrEmpty(cc.Last) ? cc.Last : null;
                        row["BillingAddressStreet"] = !string.IsNullOrEmpty(cc.Street) ? cc.Street : null;
                        row["BillingAddressStreet2"] = !string.IsNullOrEmpty(cc.Street2) ? cc.Street2 : null;
                        row["BillingAddressCity"] = !string.IsNullOrEmpty(cc.City) ? cc.City : null;
                        row["BillingAddressState"] = !string.IsNullOrEmpty(cc.State) ? cc.State : null;
                        row["BillingAddressZip"] = !string.IsNullOrEmpty(cc.Zip) ? cc.Zip : null;
                        row["BillingAddressCountry"] = !string.IsNullOrEmpty(cc.Country) ? cc.Country : null;

                        retVal.Rows.Add(row);
                    }
                }
            }
            return retVal;
        }
        private DataTable GenerateCreditCardTransactionsTable()
        {
            DataTable retVal = new DataTable(MainTableName + "_CreditCard_Transactions");
            retVal.Columns.Add("CCGuid");
            retVal.Columns.Add("TimeUTC");
            retVal.Columns.Add("Type");
            retVal.Columns.Add("Amount");
            retVal.Columns.Add("Currency");
            retVal.Columns.Add("Status");
            retVal.Columns.Add("IPAddress");
            retVal.Columns.Add("ReferenceId");

            if (CreditCards != null && CreditCards.Any())
            {
                foreach (CreditCard cc in CreditCards.Where(x => x.HasTransactions))
                {
                    foreach (CreditCard.Transaction trans in cc.Transactions)
                    {
                        DataRow row = retVal.NewRow();

                        row["CCGuid"] = !string.IsNullOrEmpty(cc.GUID) ? cc.GUID : null;
                        row["TimeUTC"] = !string.IsNullOrEmpty(trans.TimeUTC) ? trans.TimeUTC : null;
                        row["Type"] = !string.IsNullOrEmpty(trans.Type) ? trans.Type : null;
                        row["Amount"] = !string.IsNullOrEmpty(trans.Amount) ? trans.Amount : null;
                        row["Currency"] = !string.IsNullOrEmpty(trans.Currency) ? trans.Currency : null;
                        row["Status"] = !string.IsNullOrEmpty(trans.Status) ? trans.Status : null;
                        row["IPAddress"] = !string.IsNullOrEmpty(trans.IPAddress) ? trans.IPAddress : null;
                        row["ReferenceId"] = !string.IsNullOrEmpty(trans.ReferenceId) ? trans.ReferenceId : null;

                        retVal.Rows.Add(row);
                    }
                }
            }
            return retVal;
        }
        private DataTable GenerateIPAddressesTable()
        {
            DataTable retVal = new DataTable(MainTableName + "_IPAddressess");
            retVal.Columns.Add("TargetId");
            retVal.Columns.Add("IPAddress");
            retVal.Columns.Add("TimeUTC");
            retVal.Columns.Add("Action");

            if (IPAddresses != null && IPAddresses.Any())
            {
                foreach (IPAddress ip in IPAddresses)
                {
                    if (ip.HasData)
                    {
                        DataRow row = retVal.NewRow();

                        row["TargetId"] = !string.IsNullOrEmpty(Target) ? Target : null;
                        row["IPAddress"] = !string.IsNullOrEmpty(ip.Address) ? ip.Address : null;
                        row["TimeUTC"] = !string.IsNullOrEmpty(ip.TimeUTC) ? ip.TimeUTC : null;
                        row["Action"] = !string.IsNullOrEmpty(ip.Action) ? ip.Action : null;

                        retVal.Rows.Add(row);
                    }
                }
            }
            return retVal;
        }
        #endregion

        #region Internal Classes
        internal class CreditCard
        {
            internal CreditCard(CommandLineLogger logger, string dataSectionName, string ccNumber, IEnumerable<ParseDataItem> dataItems)
            {
                GUID = Guid.NewGuid().ToString();
                ID = ccNumber;

                foreach (ParseDataItem dataItem in dataItems)
                {
                    switch (dataItem.Header.ToUpper().Trim())
                    {
                        case "PAYMENT ACCOUNT ID":
                            PaymentAccountID = dataItem.Value;
                            break;
                        case "FIRST":
                            First = dataItem.Value;
                            break;
                        case "MIDDLE":
                            Middle = dataItem.Value;
                            break;
                        case "LAST":
                            Last = dataItem.Value;
                            break;
                        case "STREET":
                            Street = dataItem.Value;
                            break;
                        case "STREET2":
                            Street2 = dataItem.Value;
                            break;
                        case "CITY":
                            City = dataItem.Value;
                            break;
                        case "STATE":
                            State = dataItem.Value;
                            break;
                        case "ZIP":
                            Zip = dataItem.Value;
                            break;
                        case "COUNTRY":
                            Country = dataItem.Value;
                            break;
                        case "TRANSACTIONS":
                            IEnumerable<Transaction> transactions = Transaction.BuildList(logger, dataSectionName, dataItem);
                            if (transactions.Count() > 0)
                                Transactions = transactions;
                            break;
                        case "TIME":
                        case "TYPE":
                        case "AMOUNT":
                        case "CURRENCY":
                        case "STATUS":
                        case "IP ADDRESS":
                        case "TRANSACTION REFERENCE ID":
                            break;
                        default:
                            logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + ":" + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                            throw new ApplicationException(dataItem.Header);
#endif
                            break;
                    }
                }
            }

            #region Properties
            internal string GUID { get; private set; }
            internal string ID { get; private set; }
            internal string PaymentAccountID { get; private set; }
            internal string First { get; private set; }
            internal string Middle { get; private set; }
            internal string Last { get; private set; }
            internal string Street { get; private set; }
            internal string Street2 { get; private set; }
            internal string City { get; private set; }
            internal string State { get; private set; }
            internal string Zip { get; private set; }
            internal string Country { get; private set; }
            internal IEnumerable<Transaction> Transactions { get; private set; }
            internal bool HasTransactions { get { return Transactions != null && Transactions.Any(x => x.HasData); } }
            internal bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(City) ||
                            !string.IsNullOrEmpty(Country) ||
                            !string.IsNullOrEmpty(First) ||
                            !string.IsNullOrEmpty(ID) ||
                            !string.IsNullOrEmpty(Last) ||
                            !string.IsNullOrEmpty(Middle) ||
                            !string.IsNullOrEmpty(PaymentAccountID) ||
                            !string.IsNullOrEmpty(State) ||
                            !string.IsNullOrEmpty(Street) ||
                            !string.IsNullOrEmpty(Street2) ||
                            !string.IsNullOrEmpty(Zip) ||
                           HasTransactions);
                }
            }
            #endregion

            public override string ToString()
            {
                List<string> x = new List<string>();

                x.Add(ID);
                x.Add("Payment Account ID: " + PaymentAccountID);
                x.Add("First: " + First);
                x.Add("Middle: " + Middle);
                x.Add("Last: " + Last);
                x.Add("Street: " + Street);
                x.Add("Street2:" + Street2);
                x.Add("City: " + City);
                x.Add("State: " + State);
                x.Add("Zip: " + Zip);
                x.Add("Country: " + Country);
                if (HasTransactions)
                {
                    List<string> t = new List<string>();
                    foreach (Transaction trans in Transactions.Where(y => y.HasData))
                    {
                        t.Add(trans.ToString());
                    }
                    if (t.Count() > 0)
                    {
                        x.Add("Transactions: {" + string.Join("; ", t) + "}");
                    }
                }

                return x.Any() ? string.Join("; ", x) : null;
            }

            internal class Transaction
            {
                #region Properties
                internal string TargetId { get; set; }
                internal string PaymentAccountID { get; set; }
                internal string TimeUTC { get; set; }
                internal string Type { get; set; }
                internal string Amount { get; set; }
                internal string Currency { get; set; }
                internal string Status { get; set; }
                internal string IPAddress { get; set; }
                internal string ReferenceId { get; set; }
                internal bool HasData
                {
                    get
                    {
                        return (
                            !string.IsNullOrEmpty(Amount) ||
                            !string.IsNullOrEmpty(Currency) ||
                            !string.IsNullOrEmpty(IPAddress) ||
                            !string.IsNullOrEmpty(PaymentAccountID) ||
                            !string.IsNullOrEmpty(ReferenceId) ||
                            !string.IsNullOrEmpty(Status) ||
                            !string.IsNullOrEmpty(TimeUTC) ||
                            !string.IsNullOrEmpty(Type));
                    }
                }
                #endregion

                public override string ToString()
                {
                    List<string> x = new List<string>();

                    x.Add("Time: " + TimeUTC);
                    x.Add("Type: " + Type);
                    x.Add("Amount: " + Amount);
                    x.Add("Currency: " + Currency);
                    x.Add("Status: " + Status);
                    x.Add("IP Address: " + IPAddress);
                    x.Add("Transaction Reference Id: " + ReferenceId);

                    return x.Any() ? string.Join("; ", x) : null;
                }

                internal void UpdateParentReferences(string target, string paymentAccountId)
                {
                    TargetId = target;
                    PaymentAccountID = paymentAccountId;
                }

                internal static IEnumerable<Transaction> BuildList(CommandLineLogger logger, string dataSectionName, ParseDataItem parentItem)
                {
                    List<Transaction> retVal = new List<Transaction>();
                    Transaction toAdd = null;
                    if (parentItem.HasChildren)
                    {
                        foreach (ParseDataItem dataItem in parentItem.Children)
                        {
                            switch (dataItem.Header.ToUpper().Trim())
                            {
                                case "TIME":
                                    if (toAdd != null && toAdd.HasData)
                                        retVal.Add(toAdd);
                                    toAdd = new Transaction
                                    {
                                        TimeUTC = dataItem.Value
                                    };
                                    break;
                                case "TYPE":
                                    toAdd.Type = dataItem.Value;
                                    break;
                                case "AMOUNT":
                                    toAdd.Amount = dataItem.Value;
                                    break;
                                case "CURRENCY":
                                    toAdd.Currency = dataItem.Value;
                                    break;
                                case "STATUS":
                                    toAdd.Status = dataItem.Value;
                                    break;
                                case "IP ADDRESS":
                                    toAdd.IPAddress = dataItem.Value;
                                    break;
                                case "TRANSACTION REFERENCE ID":
                                    toAdd.ReferenceId = dataItem.Value;
                                    break;
                                default:
                                    logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + ":" + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                    throw new ApplicationException(dataItem.Header);
#endif
                                    break;
                            }
                        }
                        if (toAdd != null && toAdd.HasData)
                            retVal.Add(toAdd);
                    }
                    return retVal;
                }
            }
        }
        internal class RegisterdEmail
        {
            #region Properties
            internal RegisterdEmail(string email)
            {
                Email = email;
            }
            internal string Email { get; private set; }
            internal bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(Email));
                }
            }
            #endregion
        }
        internal class Phone
        {
            internal Phone(string phoneString)
            {
                List<string> phoneData = phoneString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (phoneData.Count > 0)
                {
                    PhoneNumber = phoneData.ElementAt(0);
                    phoneData.RemoveAt(0);
                }

                if (phoneData.Count > 0)
                {
                    Type = phoneData.ElementAt(0);
                    phoneData.RemoveAt(0);
                }

                if (phoneData.Count > 0)
                {
                    Verified = phoneData.ElementAt(0);
                    phoneData.RemoveAt(0);
                }

                if (phoneData.Count > 0 && phoneData.ElementAt(0).ToUpper().Trim().Equals("ON"))
                {
                    phoneData.RemoveAt(0);
                }

                if (phoneData.Count > 0)
                {
                    VerifiedOnUTC = string.Join(" ", phoneData);
                }
            }
            #region Properties
            internal string PhoneNumber { get; private set; }
            internal string Type { get; private set; }
            internal string Verified { get; private set; }
            internal string VerifiedOnUTC { get; private set; }
            internal bool HasData
            {
                get
                {
                    return (!string.IsNullOrEmpty(PhoneNumber) ||
                            !string.IsNullOrEmpty(Type) ||
                            !string.IsNullOrEmpty(Verified) ||
                            !string.IsNullOrEmpty(VerifiedOnUTC));
                }
            }
            #endregion

            public override string ToString()
            {
                List<string> x = new List<string>();
                x.Add(PhoneNumber);
                x.Add(Type);
                x.Add(Verified);
                if (!string.IsNullOrEmpty(VerifiedOnUTC))
                    x.Add("on");
                x.Add(VerifiedOnUTC);
                return x.Any() ? string.Join(" ", x) : null;
            }
        }
        internal class IPAddress
        {
            #region Properties
            internal string Address { get; private set; }
            internal string TimeUTC { get; private set; }
            internal string Action { get; private set; }
            internal bool HasData
            {
                get
                {
                    return (
                        !string.IsNullOrEmpty(Action) ||
                        !string.IsNullOrEmpty(Address) ||
                        !string.IsNullOrEmpty(TimeUTC));
                }
            }
            #endregion
            internal static IEnumerable<IPAddress> BuildList(CommandLineLogger logger, string dataSectionName, ParseDataItem parentItem)
            {
                List<IPAddress> retVal = new List<IPAddress>();
                if (parentItem.HasChildren)
                {
                    IPAddress toAdd = null;
                    foreach (ParseDataItem dataItem in parentItem.Children)
                    {
                        switch (dataItem.Header.ToUpper().Trim())
                        {
                            case "IP ADDRESS":
                                if (toAdd != null && toAdd.HasData)
                                    retVal.Add(toAdd);
                                toAdd = new IPAddress
                                {
                                    Address = dataItem.Value
                                };
                                break;
                            case "TIME":
                                toAdd.TimeUTC = dataItem.Value;
                                break;
                            case "ACTION":
                                toAdd.Action = dataItem.Value;
                                break;
                            default:
                                logger.LogWarning("Unexpected Html Element - \"" + dataSectionName + ":" + dataItem.Header + "\".  Please contact NDCAC with section name and test data to improve parsing functionality");
#if DEBUG
                                throw new ApplicationException(dataItem.Header);
#endif
                                break;
                        }
                    }
                    if (toAdd != null && toAdd.HasData)
                        retVal.Add(toAdd);
                }
                return retVal;
            }
        }
        #endregion
    }
}

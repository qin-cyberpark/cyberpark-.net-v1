using System;
using System.Collections;
using System.Text;
using System.Configuration;
using System.Xml;

namespace CyberPark.Domain.Core
{
    public sealed class SysConfig : ConfigurationSection
    {
        private SysConfig()
        {

        }

        private static SysConfig _instance;
        public static SysConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (SysConfig)System.Configuration.ConfigurationManager.GetSection("cyberPark");
                }
                return _instance;
            }
        }

        [ConfigurationProperty("pxpay")]
        public AccessAccountElement PxPay
        {
            get
            {
                return (AccessAccountElement)this["pxpay"];
            }
        }

        [ConfigurationProperty("directories", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DirectoryElementCollection),
          AddItemName = "add",
          ClearItemsName = "clear",
          RemoveItemName = "remove")]
        public DirectoryElementCollection Directories
        {
            get
            {
                return (DirectoryElementCollection)base["directories"];
            }
        }

        [ConfigurationProperty("sysSettings", IsDefaultCollection = false)]
        private KeyValueConfigurationCollection SysSettings
        {
            get
            {
                return (KeyValueConfigurationCollection)base["sysSettings"];
            }
        }

        [ConfigurationProperty("mailSender", IsDefaultCollection = false)]
        public KeyValueConfigurationCollection MailSender
        {
            get
            {
                return (KeyValueConfigurationCollection)base["mailSender"];
            }
        }

        //
        public string TemporaryDirectory
        {
            get
            {
                return Directories["temp"];
            }
        }
        public string InvoicePdfDirectory
        {
            get
            {
                return Directories["invoicePDF"];
            }
        }
        public string ExternalBillDirectory
        {
            get
            {
                return Directories["externalBill"];
            }
        }
        public int ProductChargeAdvanceDays
        {
            get
            {
                return int.Parse(SysSettings["productChargeAdvanceDays"].Value);
            }
        }
        public int AutoOperatorId
        {
            get
            {
                return int.Parse(SysSettings["autoOperatorId"].Value);
            }
        }
        public double GST
        {
            get
            {
                return double.Parse(SysSettings["GST"].Value);
            }
        }
        public string DefaultBranchId
        {
            get
            {
                return SysSettings["defaultBranchId"].Value;
            }
        }
        public int BusinessInvoiceIssueDay
        {
            get
            {
                return int.Parse(SysSettings["businessInvoiceIssueDay"].Value);
            }
        }
        public int ExternalBillImportDay
        {
            get
            {
                return int.Parse(SysSettings["externalBillImportDay"].Value);
            }
        }
        public int InvoiceAutoDeliveryDelayHours
        {
            get
            {
                return int.Parse(SysSettings["invoiceAutoDeliveryDelayHours"].Value);
            }
        }

        //mail sender setting
        public string MailSenderServer
        {
            get
            {
                return MailSender["server"].Value;
            }
        }
        public int MailSenderPort
        {
            get
            {
                return int.Parse(MailSender["port"].Value);
            }
        }
        public bool MailSenderEnableSSL
        {
            get
            {
                return bool.Parse(MailSender["enableSsl"].Value);
            }
        }
        public string MailSenderUsername
        {
            get
            {
                return MailSender["username"].Value;
            }
        }
        public string MailSenderPassword
        {
            get
            {
                return MailSender["password"].Value;
            }
        }
        public string MailSenderAddress
        {
            get
            {
                return MailSender["senderAddress"].Value;
            }
        }
        public string MailSenderDisplayName
        {
            get
            {
                return MailSender["senderDisplayName"].Value;
            }
        }
        public int MailSenderTimeout
        {
            get
            {
                return int.Parse(MailSender["timeout"].Value);
            }
        }

    }

    /// <summary>
    /// access account element
    /// </summary>
    public class AccessAccountElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get
            {
                return this["id"] as string;
            }
        }
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get
            {
                return this["key"] as string;
            }
        }
    }

    /// <summary>
    /// directory element
    /// </summary>
    public class DirectoryElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        public string Type
        {
            get
            {
                return this["type"] as string;
            }
        }
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return this["path"] as string;
            }
        }
    }

    /// <summary>
    /// directory collection
    /// </summary>
    public class DirectoryElementCollection : ConfigurationElementCollection
    {

        new public string this[string directoryType]
        {
            get { return ((DirectoryElement)BaseGet(directoryType)).Path; }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new DirectoryElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DirectoryElement)element).Type;
        }
    }
}
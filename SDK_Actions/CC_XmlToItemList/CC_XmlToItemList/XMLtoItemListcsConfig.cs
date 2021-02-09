using System.Collections.Generic;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace CC_XmlToItemList
{
    public class XMLtoItemListcsConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "Source configuration", Order = 1)]
        public SourceConfiguration SourceConfiguration { get; set; }

        [ConfigGroupBox(DisplayName = "Target configuration", Order = 2)]
        public TargetConfiguration TargetConfiguration { get; set; }
    }
    public class SourceConfiguration
    {
        [ConfigEditableText(DisplayName = "Attachment picker", Description = "The id of the column which is used to pick an attachment.", Order = 1)]
        public string AttachmentPickerFieldId { get; set; }

        [ConfigEditableText(DisplayName = "XPath", Description = "The XPath which select all rows from which item list rows should be created. For example //row will return all nodes within the whole XML document.", Order = 2, DefaultText = "//row")]
        public string XPath { get; set; }

    }

    public class TargetConfiguration
    {
        [ConfigEditableText(DisplayName = "Target item list id")]
        public string TargetItemListId { get; set; }

        [ConfigEditableGrid(DisplayName = "Mapping",DescriptionAsHTML =true,Description = "The target type of the value.<br/><ul><li>Boolean = 0</li><li>Choose = 10</li><li>DateTime = 20</li><li>Decimal = 30</li><li>Text = 40</li><li>Picker = 50</li></ul>")]
        public List<Mapping> Mapping { get; set; }
    }

    public class Mapping
    {
        [ConfigEditableGridColumn(DisplayName = "Node name", Description = "The node name which value will be written to the target column.", IsRequired = true)]
        public string NodeName { get; set; }

        [ConfigEditableGridColumn(DisplayName = "Target column DB", Description = "DB name of target item list column.", IsRequired = true)]
        public string TargetColumnName { get; set; }

        [ConfigEditableGridColumn(DisplayName = "Target value type", Description = "The target type of the value. Boolean = 0, Choose = 10, DateTime = 20, Decimal = 30, Text = 40, Picker = 50", IsRequired = true)]
        public string Type { get; set; }


    }
    public enum TargetValueType
    {
        Boolean = 0,
        Choose = 10,
        DateTime = 20,
        Decimal = 30,
        Text = 40,
        Picker = 50,
    }
}
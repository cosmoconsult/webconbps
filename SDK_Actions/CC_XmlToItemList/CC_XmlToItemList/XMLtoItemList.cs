using System;
using System.Text;
using System.Xml;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents.Model;
using WebCon.WorkFlow.SDK.Documents.Model.ComplexValues;

namespace CC_XmlToItemList
{
    public class XMLtoItemList : CustomAction<XMLtoItemListcsConfig>
    {
        readonly IndentTextLogger logger = new IndentTextLogger();

        public override void Run(RunCustomActionParams args)
        {
            try
            {
                logger.Log("Extracting rows from xml");
                XmlNodeList rows = GetRowsFromXml(args.Context.CurrentDocument);

                logger.Log($"Getting item list '{Configuration.TargetConfiguration.TargetItemListId}'");
                var list = args.Context.CurrentDocument.ItemsLists.GetByID(int.Parse(Configuration.TargetConfiguration.TargetItemListId));

                logger.Log($"Populating item list");
                PopulateItemList(args.Context.CurrentDocument,rows, list);
            }
            catch (System.Exception ex)
            {
                logger.Indent();
                logger.Log($"Error executing {nameof(CC_XmlToItemList)}", ex);
                // HasErrors property is responsible for detection whether action has been executed properly or not. When set to "true"
                // whole path transition will be marked as faulted and all the actions on it will be rollbacked. User will be notified
                // about failure by display of error window.
                args.HasErrors = true;
                // Message property is responsible for error message content.
                args.Message = ex.Message;
            }
            finally
            {
                args.LogMessage = logger.ToString();
                logger.Dispose();
            }
        }

        private void PopulateItemList(CurrentDocumentData document, XmlNodeList rows, WebCon.WorkFlow.SDK.Documents.Model.ItemsLists.ItemsList list)
        {
            logger.Indent();
            for (int i = 0; i < rows.Count; i++)
            {
                logger.Log($"Creating a new row in item list for xml row number '{i}'");

                XmlNode row = rows[i];
                var newRow = list.Rows.AddNewRow();
                foreach (var mapping in Configuration.TargetConfiguration.Mapping)
                {
                    try
                    {
                        var column = list.Columns.GetByDbColumnName(mapping.TargetColumnName);
                        var node = row[mapping.NodeName];
                        object targetValue = null;
                        if (!string.IsNullOrEmpty(node.InnerText))
                        {                            
                            var type = (TargetValueType)int.Parse(mapping.Type);
                            switch (type)
                            {
                                case TargetValueType.Boolean:
                                    targetValue = bool.Parse(node.InnerText);
                                    break;
                                case TargetValueType.Choose:
                                    targetValue = new ChooseValue(node.InnerText, string.Empty);
                                    break;
                                case TargetValueType.DateTime:
                                    targetValue = DateTime.Parse(node.InnerText);
                                    break;
                                case TargetValueType.Decimal:
                                    targetValue = decimal.Parse(node.InnerText);
                                    break;
                                case TargetValueType.Text:
                                    targetValue = node.InnerText;
                                    break;
                                case TargetValueType.Picker:
                                    targetValue = new ChooseValue(node.InnerText, string.Empty);
                                    break;
                                default:
                                    throw new ApplicationException($"An unknown '{nameof(TargetValueType)}' with id '{mapping.Type}' has been defined.");
                            }
                        }

                        newRow.Cells.GetByDbColumnName(mapping.TargetColumnName).SetValue(targetValue);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException($"There was a problem parsing/setting value of node '{mapping.NodeName}' for xml row '{i}' to type '{mapping.Type}' for column '{mapping.TargetColumnName}' ", ex);
                    }
                }

            }
            logger.Outdent();
            
        }

        private XmlNodeList GetRowsFromXml(CurrentDocumentData document)
        {
            logger.Indent();
            logger.Log($"Fetching attachment from field id '{this.Configuration.SourceConfiguration.AttachmentPickerFieldId}'");
            var pickerFieldId = int.Parse(this.Configuration.SourceConfiguration.AttachmentPickerFieldId);
            var field = document.ChooseFields.GetByID(pickerFieldId);
            if (!int.TryParse(field.Value.ID, out int attachmentId))
            {
                throw new ApplicationException($"Could not parse Id to int of field value {field.Value}");
            }

            var attachment = document.Attachments.GetByID(attachmentId);

            logger.Log($"Creating XmlDocument from attachment '{attachmentId}'");
            XmlDocument xml = new XmlDocument();
            string attachmentContent = Encoding.UTF8.GetString(attachment.Content);
            xml.LoadXml(attachmentContent);
            logger.Log($"Selecting nodes using XPath '{this.Configuration.SourceConfiguration.XPath}'");
            var nodes = xml.SelectNodes(this.Configuration.SourceConfiguration.XPath);
            if (nodes.Count == 0)
            {
                throw new ApplicationException($"No rows were returned for attachment with id '{attachmentId}'");
            }
            else
            {
                logger.Log($"XPath returned {nodes.Count} nodes");
            }
            logger.Outdent();
            return nodes;
        }
    }
}
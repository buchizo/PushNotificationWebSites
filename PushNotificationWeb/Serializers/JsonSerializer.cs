namespace PushNotification.WebSites.Web.Serializers
{
    using System.Collections;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Xml;
    using PushNotification.WebSites.Web.Infrastructure;

    public class JsonSerializer : IFormatSerializer
    {
        public string SerializeReply(object originalReply, out string contentType)
        {
            if (HttpContext.Current == null)
            {
                throw new HttpException("Service Hosting ASP.NET Compatibility Mode must be enabled for JSON serialization.");
            }

            contentType = HttpConstants.MimeApplicationJson;
            return ConvertXmlToJson(originalReply as XmlDocument);
        }

        private static string ConvertXmlToJson(XmlDocument xmlDoc)
        {
            var builder = new StringBuilder();
            builder.Append("{ ");
            ConvertXmlToJsonNode(builder, xmlDoc.DocumentElement, true);
            builder.Append("}");

            return builder.ToString();
        }

        private static void ConvertXmlToJsonNode(StringBuilder jsonBuilder, XmlElement node, bool showNodeName)
        {
            var childAdded = false;
            if (showNodeName)
            {
                jsonBuilder.Append("\"" + SafeJsonString(node.Name) + "\": ");
            }

            jsonBuilder.Append("{");
            var childNodeNames = new SortedList();

            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    StoreChildNode(childNodeNames, attr.Name, attr.InnerText);
                }
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                childAdded = true;

                if (childNode is XmlText)
                {
                    StoreChildNode(childNodeNames, "value", childNode.InnerText);
                }
                else if (childNode is XmlElement)
                {
                    StoreChildNode(childNodeNames, childNode.Name, childNode);
                }
            }

            foreach (string childName in childNodeNames.Keys)
            {
                childAdded = true;

                var childNodeNamesList = (ArrayList)childNodeNames[childName];
                if (childNodeNamesList.Count == 1 && (childNodeNamesList[0] is string))
                {
                    OutputNode(childName, childNodeNamesList[0], jsonBuilder, true);
                }
                else
                {
                    jsonBuilder.Append(" \"" + SafeJsonString(childName) + "\": [ ");
                    foreach (object child in childNodeNamesList)
                    {
                        OutputNode(childName, child, jsonBuilder, false);
                    }

                    jsonBuilder.Remove(jsonBuilder.Length - 2, 2);
                    jsonBuilder.Append(" ], ");
                }
            }

            jsonBuilder.Remove(jsonBuilder.Length - 2, 2);
            jsonBuilder.Append(" }");

            jsonBuilder.Append(childAdded ? " }" : " null");
        }

        private static void StoreChildNode(SortedList childNodeNames, string nodeName, object nodeValue)
        {
            if (nodeValue is XmlElement)
            {
                var childNode = (XmlNode)nodeValue;
                if (childNode.Attributes.Count == 0)
                {
                    var children = childNode.ChildNodes;
                    if (children.Count == 0)
                    {
                        nodeValue = null;
                    }
                    else if (children.Count == 1 && (children[0] is XmlText))
                    {
                        nodeValue = ((XmlText)children[0]).InnerText;
                    }
                }
            }

            var tempNodeValueList = childNodeNames[nodeName];
            ArrayList nodeValueList;
            if (tempNodeValueList == null)
            {
                nodeValueList = new ArrayList();
                childNodeNames[nodeName] = nodeValueList;
            }
            else
            {
                nodeValueList = (ArrayList)tempNodeValueList;
            }

            nodeValueList.Add(nodeValue);
        }

        private static void OutputNode(string childname, object childNodeNamesList, StringBuilder jsonBuilder, bool showNodeName)
        {
            if (childNodeNamesList == null)
            {
                if (showNodeName)
                {
                    jsonBuilder.Append("\"" + SafeJsonString(childname) + "\": ");
                }

                jsonBuilder.Append("null");
            }
            else
            {
                var childNodeNamesListStr = childNodeNamesList as string;
                if (string.IsNullOrWhiteSpace(childNodeNamesListStr))
                {
                    if (showNodeName)
                    {
                        jsonBuilder.Append("\"" + SafeJsonString(childname) + "\": ");
                    }

                    childNodeNamesListStr = childNodeNamesListStr.Trim();
                    jsonBuilder.Append("\"" + SafeJsonString(childNodeNamesListStr) + "\"");
                }
                else
                {
                    ConvertXmlToJsonNode(jsonBuilder, (XmlElement)childNodeNamesList, showNodeName);
                }
            }

            jsonBuilder.Append(", ");
        }

        private static string SafeJsonString(string value)
        {
            var safeValue = new StringBuilder(value.Length);
            foreach (char ch in value)
            {
                if (char.IsControl(ch) || ch == '\'')
                {
                    var ich = (int)ch;
                    safeValue.Append(@"\u" + ich.ToString("x4", CultureInfo.InvariantCulture));
                    continue;
                }
                else if (ch == '\"' || ch == '\\' || ch == '/')
                {
                    safeValue.Append('\\');
                }

                safeValue.Append(ch);
            }

            return safeValue.ToString();
        }
    }
}
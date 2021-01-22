using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TechShare.Utility.Tools.TableDefinitions;

namespace TechShare.Utility.Tools.Conversations
{
    public static class ChatMessageHelper
    {
        #region Private Variables
        private static string _htmlDocTemplate_Google = "<!DOCTYPE html> <html lang='en' xmlns='http://www.w3.org/1999/xhtml'><head><meta charset='utf-8' /><title>{0}</title>{1}</head><body>{2}{3}</body>";
        private static string _styleString_Google = @"
                    <style>
                        body {
                            background-color: white;
                        }

                        ul {
                            list-style: none;
                            margin: 0;
                            padding: 0;
                        }

                        ul li {
                            display: inline-block;
                            clear: both;
                            padding: 10px;
                            border-radius: 30px;
                            margin-bottom: 2px;
                            font-family: Helvetica, Arial, sans-serif;
                        }

                        .outgoing {
                            background: #eee;
                            float: left;
                        }

                        .outgoingDeleted {
                            background: red;
                            float: left;
                        }

                        .outgoingCall {
                            background: #eee;
                            float: left;
                        }

                        .incoming {
                            float: right;
                            background: #0084ff;
                            color: #fff;
                        }

                        .incomingDeleted {
                            float: right;
                            background: red;
                            color: #fff;
                        }

                        .incomingCall {
                            float: right;
                            background: #0084ff;
                            color: #fff;
                        }

                        .outgoing+.incoming,
                        .outgoingDeleted+.incomingDeleted,
                        .outgoingCall+.incomingCall {
                            border-bottom-right-radius: 5px;
                        }

                        .incoming+.incoming,
                        .incomingDeleted+.incomingDeleted,
                        .incomingCall+.incomingCall {
                            border-top-right-radius: 5px;
                            border-bottom-right-radius: 5px;
                        }

                        .incoming:last-of-type,
                        incomingDeleted:last-of-type,
                        .incomingCall:last-of-type {
                            border-bottom-right-radius: 5px;
                        }

                        .outgoingTime {
                            font-size: 9px;
                            display: inline;
                            padding-left: 5px;
                            color: blue;
                        }

                        .outgoingTimeDeleted {
                            font-size: 9px;
                            display: inline;
                            padding-left: 5px;
                            color: white;
                        }

                        .outgoingTimeCall {
                            font-size: 9px;
                            display: inline;
                            padding-left: 5px;
                            color: blue;
                        }

                        .incomingTime {
                            font-size: 9px;
                            display: inline;
                            padding-left: 5px;
                            color: yellow;
                        }

                        .incomingTimeDeleted {
                            font-size: 9px;
                            display: inline;
                            padding-left: 5px;
                            color: white;
                        }

                        .incomingTimeCall {
                            font-size: 9px;
                            display: inline;
                            padding-left: 5px;
                            color: yellow;
                        }

                        .ShwVid {
                            display: block;
                        }

                        .outgoingAuthor {
                            display: block;
                            font-size: 11px;
                            color: blue;
                        }

                        .incomingAuthor {
                            display: block;
                            font-size: 11px;
                            color: yellow;
                        }
                    </style>";
        #endregion

        public static string GenerateGooglePreviewHTML(DataView messages, string targetColumn, string dateColumn, string fromColumn, string toColumn, string bodyColumn, string titleValue = null, List<string> headerValues = null)
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(targetColumn) && !string.IsNullOrEmpty(dateColumn) && !string.IsNullOrEmpty(fromColumn) && !string.IsNullOrEmpty(toColumn) && !string.IsNullOrEmpty(bodyColumn))
            {
                List<string> listItems = new List<string>();
                foreach (DataRowView messageRow in messages)
                {
                    DataRow row = messageRow.Row;
                    string targetValue = row[targetColumn].ToString();
                    DateTime? dateValue = DateTime.TryParse(row[dateColumn].ToString(), out DateTime tempDateValue) ? tempDateValue : (DateTime?)null;
                    string dateTextValue = dateValue.HasValue ? dateValue.Value.ToString("yyyy-MM-dd HH:mm:ss UTC") : string.Empty;
                    string fromValue = row[fromColumn].ToString();
                    string toValue = row[toColumn].ToString();
                    string bodyValue = row[bodyColumn].ToString();

                    bool sentByTarget = fromValue.ToUpper().Contains(targetValue.ToUpper());

                    listItems.Add(@"<li class='" + (sentByTarget ? "outgoing" : "incoming") + "'><span class='" + (sentByTarget ? "outgoingAuthor" : "incomingAuthor") + "'>" + fromValue +
                        "</span>" + bodyValue + "<p class='" + (sentByTarget ? "outgoingTime" : "incomingTime") + "'>" + dateTextValue + "</p></li>");
                }
                string htmlBodyText = string.Format("<ul>{0}</ul>", string.Join("", listItems));

                string titleText = !string.IsNullOrEmpty(titleValue) ? titleValue : string.Empty;
                string headerText = string.Empty;

                if (headerValues != null && headerValues.Any(x => x != null && !string.IsNullOrEmpty(x.Trim())))
                    headerText = "<p>" + string.Join("<br />", headerValues.Where(x => x != null && !string.IsNullOrEmpty(x.Trim()))) + "</p>";

                retVal = string.Format(_htmlDocTemplate_Google, titleText, _styleString_Google, headerText, htmlBodyText);
            }
            return retVal;
        }
        public static string GenerateGooglePreviewHTML(DataView messages, TableDefinition chatMessageTableDef, string titleValue = null, List<string> headerValues = null)
        {
            string retVal = string.Empty;
            if (chatMessageTableDef != null)
            {
                ColumnDefinition col = chatMessageTableDef.Columns.Where(x => x.ChatColumnType.HasValue && x.ChatColumnType.Value == ChatColumnTypeEnum.TARGET).FirstOrDefault();
                string targetColumn = col != null && !string.IsNullOrEmpty(col.Name) ? col.Name : null;

                col = chatMessageTableDef.Columns.Where(x => x.ChatColumnType.HasValue && x.ChatColumnType.Value == ChatColumnTypeEnum.DATE).FirstOrDefault();
                string dateColumn = col != null && !string.IsNullOrEmpty(col.Name) ? col.Name : null;

                col = chatMessageTableDef.Columns.Where(x => x.ChatColumnType.HasValue && x.ChatColumnType.Value == ChatColumnTypeEnum.FROM).FirstOrDefault();
                string fromColumn = col != null && !string.IsNullOrEmpty(col.Name) ? col.Name : null;

                col = chatMessageTableDef.Columns.Where(x => x.ChatColumnType.HasValue && x.ChatColumnType.Value == ChatColumnTypeEnum.TO).FirstOrDefault();
                string toColumn = col != null && !string.IsNullOrEmpty(col.Name) ? col.Name : null;

                col = chatMessageTableDef.Columns.Where(x => x.ChatColumnType.HasValue && x.ChatColumnType.Value == ChatColumnTypeEnum.BODY).FirstOrDefault();
                string bodyColumn = col != null && !string.IsNullOrEmpty(col.Name) ? col.Name : null;

                return GenerateGooglePreviewHTML(messages, targetColumn, dateColumn, fromColumn, toColumn, bodyColumn, titleValue: titleValue, headerValues: headerValues);
            }
            return retVal;
        }
    }
}

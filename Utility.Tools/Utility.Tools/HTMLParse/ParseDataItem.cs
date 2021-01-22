using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TechShare.Utility.Tools.Extensions;

namespace TechShare.Utility.Tools.HtmlParse
{
    public class ParseDataItem
    {
        private readonly List<string> reservedHeaders = new List<string>() { "LINK" };
        private readonly List<string> tagsToProcess = new List<string>() { TABLE_TAG, TABLEBODY_TAG, HEADER_TAG, DATA_TAG, ROW_TAG };
        private const string HEADER_TAG = "th";
        private const string ROW_TAG = "tr";
        private const string DATA_TAG = "td";
        private const string TABLE_TAG = "table";
        private const string TABLEBODY_TAG = "tbody";
        private const string TR_TAG = "tbody";
        public ParseDataItem(StreamReader sr, ParseDocument parentDocument, ParseDataItem parentDataItem)
        {
            ParentDocument = parentDocument;
            ParentDataItem = parentDataItem;

            string nextTag = string.Empty;
            int associatedChildCount = 0;
            Stack<string> tagStack = new Stack<string>();
            do
            {
                while (sr.Peek() > 0)
                {
                    char firstChar;
                    if (string.IsNullOrEmpty(nextTag))
                    {
                        firstChar = (char)sr.Read();
                        while (sr.Peek() > 0 && char.IsControl(firstChar))
                            firstChar = (char)sr.Read();
                    }
                    else
                        firstChar = nextTag[0];

                    string toTest = string.Empty;
                    string toTestNode = firstChar.Equals('/') ? nextTag : string.Empty;
                    if (!string.IsNullOrEmpty(toTestNode))
                    {
                        if (toTestNode.Contains(" ") || toTestNode.StartsWith("br"))
                            toTest = toTestNode.Substring(toTestNode.StartsWith("/") ? 1 : 0, toTestNode.StartsWith("/") ? toTestNode.IndexOf(" ") - 1 : toTestNode.IndexOf(" ")).Trim();
                        else
                            toTest = toTestNode.Substring(toTestNode.StartsWith("/") ? 1 : 0, toTestNode.StartsWith("/") ? toTestNode.IndexOf(">") - 1 : toTestNode.IndexOf(">")).Trim();
                    }
                    if (firstChar.Equals('<') ||
                        ((firstChar.Equals('/') && tagStack.Peek() != DATA_TAG) ||
                        (firstChar.Equals('/') && tagStack.Peek() == DATA_TAG && HasData && (!string.IsNullOrEmpty(toTest) && !tagsToProcess.Contains(toTest))) ||
                        (firstChar.Equals('/') && tagStack.Peek() == DATA_TAG && HasData && reservedHeaders.Contains(HTMLDecodedHeader.ToUpper().Trim())) ||
                        (firstChar.Equals('/') && tagStack.Peek() == DATA_TAG && !reservedHeaders.Contains(HTMLDecodedHeader.ToUpper().Trim()))))
                    {
                        string tagNode = string.IsNullOrEmpty(nextTag) ? StreamReaderExtensions.ReadUntil(sr, '>') : nextTag.StartsWith("<") ? nextTag.Substring(1) : nextTag;
                        nextTag = string.Empty;

                        bool isOpenNode = !(firstChar.Equals('/') || tagNode.StartsWith("/"));
                        string tag = string.Empty;
                        if (tagNode.Contains(" ") || tagNode.StartsWith("br"))
                            tag = tagNode.Substring(tagNode.StartsWith("/") ? 1 : 0, tagNode.StartsWith("/") ? tagNode.IndexOf(" ") - 1 : tagNode.IndexOf(" ")).Trim();
                        else
                            tag = tagNode.Substring(tagNode.StartsWith("/") ? 1 : 0, tagNode.StartsWith("/") ? tagNode.IndexOf(">") - 1 : tagNode.IndexOf(">")).Trim();

                        if (tagStack.Any() && tagStack.Peek().Trim().ToLower() == DATA_TAG && tag.Trim().ToLower() == TABLE_TAG)
                        {
                            if (isOpenNode)
                            {
                                ParseDataItem newChild = new ParseDataItem(sr, ParentDocument, this);
                                if (newChild.HasData)
                                {
                                    AddChild(newChild);
                                    associatedChildCount++;
                                }
                            }
                        }
                        else
                        {
                            if (tagsToProcess.Contains(tag))
                            {
                                if (isOpenNode)
                                {
                                    tagStack.Push(tag);
                                }
                                else
                                {
                                    if (tagStack.Peek().Equals(tag))
                                        tagStack.Pop();
                                    if (tag.Equals(DATA_TAG))
                                    {
                                        if (HTMLDecodedValues != null && HTMLDecodedValues.Any())
                                        {
                                            ValueCounts.Last().AssociatedChildCount = associatedChildCount;
                                        }
                                        associatedChildCount = 0;
                                    }
                                }
                            }
                            if (!tagStack.Any())
                                break;
                        }
                    }
                    else
                    {
                        if (tagStack != null && tagStack.Count > 0)
                        {
                            bool done = false;
                            int dataLoopCount = 0;
                            do
                            {
                                string control = Int32.TryParse(firstChar.ToString(), out int temp) || firstChar.ToString().ToUpper().Equals("X")
                                    || firstChar.ToString().ToUpper().Equals("Y") ? string.Empty : firstChar + "<";
                                string text = dataLoopCount == 0 ? firstChar + StreamReaderExtensions.ReadUntil(sr, '<') : StreamReaderExtensions.ReadUntil(sr, '<');
                                if (!text.Equals(control) && !text.StartsWith("br /") && !text.Equals("<"))
                                {
                                    text = new string(text.Substring(0, text.Length - 1).Where(c => !char.IsControl(c)).ToArray());
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        if (tagStack.Peek().Trim().ToLower() == HEADER_TAG)
                                            HTMLDecodedHeader = text;
                                        else if (tagStack.Peek().Trim().ToLower() == DATA_TAG)
                                        {
                                            if (HTMLDecodedValues != null && HTMLDecodedValues.Any())
                                            {
                                                ValueCounts.Last().AssociatedChildCount = associatedChildCount;
                                            }
                                            associatedChildCount = 0;
                                            AddValue(text);
                                        }
                                    }
                                    nextTag = StreamReaderExtensions.ReadUntil(sr, '>');
                                    if (!nextTag.StartsWith("br /") && !nextTag.StartsWith("br/"))
                                        done = true;
                                    dataLoopCount++;
                                }
                                else
                                {
                                    nextTag = StreamReaderExtensions.ReadUntil(sr, '>');

                                    string tempTag = nextTag;
                                    tempTag = tempTag.Replace(">", "");
                                    if (tagsToProcess.Contains(tempTag))
                                    {
                                        nextTag = "<" + nextTag;
                                    }
                                    done = true;
                                }
                            } while (!done);
                        }
                    }
                }
            } while (tagStack.Any() && sr.Peek() > 0);
        }
        public ParseDataItem(ref string fileContents, ParseDocument parentDocument, ParseDataItem parentDataItem)
        {
            ParentDocument = parentDocument;
            ParentDataItem = parentDataItem;

            Stack<string> tags = new Stack<string>();
            do
            {
                RemoveUnnecessaryTags(ref fileContents);
                int toRemoveLength = 0;
                if (fileContents.StartsWith("<"))
                {
                    string tagNode = fileContents.Substring(0, fileContents.IndexOf(">") + 1);
                    bool isOpenNode = !tagNode.StartsWith("</");
                    toRemoveLength = tagNode.Length;

                    string tag = string.Empty;
                    if (tagNode.Contains(" "))
                        tag = tagNode.Substring(1, tagNode.IndexOf(" ")).Trim();
                    else
                        tag = tagNode.Substring(1, tagNode.IndexOf(">") - 1).Trim();

                    if (tags.Any() && tags.Peek().Trim().ToUpper() == DATA_TAG && tag.Trim().ToUpper() == TABLE_TAG)
                    {
                        AddChild(new ParseDataItem(ref fileContents, ParentDocument, this));
                    }
                    else
                    {
                        if (isOpenNode)
                            tags.Push(tag);
                        else
                            tags.Pop();
                        fileContents = fileContents.Substring(toRemoveLength);
                    }
                }
                else
                {
                    do
                    {
                        RemoveUnnecessaryTags(ref fileContents);
                        string text = fileContents.Substring(0, fileContents.IndexOf("<"));
                        toRemoveLength = text.Length;
                        if (tags.Peek().Trim().ToUpper() == HEADER_TAG)
                            HTMLDecodedHeader = text;
                        else if (tags.Peek().Trim().ToUpper() == DATA_TAG)
                            AddValue(text);

                        fileContents = fileContents.Substring(toRemoveLength);
                        RemoveUnnecessaryTags(ref fileContents);
                    } while (!fileContents.StartsWith("<"));
                }
                RemoveUnnecessaryTags(ref fileContents);
            } while (tags.Any());
        }

        private void RemoveUnnecessaryTags(ref string fileContents)
        {
            do
            {
                if (fileContents.StartsWith("<br") ||
                    fileContents.StartsWith("<img") ||
                    fileContents.StartsWith("<video") ||
                    fileContents.StartsWith("<source"))

                    fileContents = fileContents.Substring(fileContents.IndexOf(">") + 1);

            } while (fileContents.StartsWith("<br") ||
                    fileContents.StartsWith("<img") ||
                    fileContents.StartsWith("<video") ||
                    fileContents.StartsWith("<source"));
        }
        private ParseDocument ParentDocument { get; set; }
        private ParseDataItem ParentDataItem { get; set; }
        public string HTMLDecodedHeader { get; private set; }
        public string Header { get { return WebUtility.HtmlDecode(HTMLDecodedHeader); } }

        private List<ValueCount> _values = null;
        public bool HasValues { get { return _values != null && _values.Any(x => !string.IsNullOrEmpty(x.Val)); } }
        public IEnumerable<string> HTMLDecodedValues { get { return HasValues ? _values.Where(x => !string.IsNullOrEmpty(x.Val)).Select(x => x.Val) : null; } }
        public IEnumerable<string> Values { get { return HasValues ? _values.Where(x => !string.IsNullOrEmpty(x.Val)).Select(x => WebUtility.HtmlDecode(x.Val)) : null; } }
        public IEnumerable<ValueCount> ValueCounts { get { return HasValues ? _values.Where(x => !string.IsNullOrEmpty(x.Val)) : null; } }
        public string Value { get { return HasValues && Values.Count() == 1 ? WebUtility.HtmlDecode(ValueCounts.First().Val) : null; } }
        public string HTMLDecodedValue { get { return HasValues && Values.Count() == 1 ? ValueCounts.First().Val : null; } }

        private List<ParseDataItem> _childItems = null;
        public bool HasChildren { get { return _childItems != null && _childItems.Any(x => x.HasData); } }
        public IEnumerable<ParseDataItem> Children { get { return HasChildren ? _childItems.Where(x => x.HasData) : null; } }

        public bool HasData { get { return !string.IsNullOrEmpty(Header) || HasValues || HasChildren; } }

        private void AddValue(string value)
        {
            if (_values == null)
                _values = new List<ValueCount>();
            _values.Add(new ValueCount() { Val = value });
        }

        private void AddChild(ParseDataItem child)
        {
            if (child != null)
            {
                if (_childItems == null)
                    _childItems = new List<ParseDataItem>();
                _childItems.Add(child);
            }
        }
        public ParseDataItem NextSibling()
        {
            if (ParentDataItem != null)
            {
                if (ParentDataItem._childItems != null)
                {
                    int myIndex = ParentDataItem._childItems.IndexOf(this);
                    if (ParentDataItem._childItems.Count > (myIndex + 1))
                        return ParentDataItem._childItems[(myIndex + 1)];
                }
            }
            return null;
        }

        public class ValueCount
        {
            public string Val { get; set; }
            public int AssociatedChildCount { get; set; }
            public override string ToString()
            {
                return Val.ToString();
            }
        }
    }
}

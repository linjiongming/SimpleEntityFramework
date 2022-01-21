using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Templates.Framework
{
    public class JsonHelperTemplate : ClassTemplate
    {
        public const string ClassName = "JsonHelper";

        public override string Name => ClassName;

        public override string FileContent => $@"{Profile}
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace {Namespace}
{{
    public class {Name}
    {{
        static {Name}()
        {{
            DefaultSettings = new Settings();
        }}

        public static Settings DefaultSettings {{ get; private set; }}

        public static string Serialize(object obj, Settings settings = null)
        {{
            if (settings == null)
            {{
                settings = DefaultSettings;
            }}
            var serializer = new JavaScriptSerializer() {{ RecursionLimit = settings.RecursionLimit }};
            var json = serializer.Serialize(obj);
            json = ReplaceDateTime(json, settings.DateTimeFormat);
            if (settings.Formatting == Formatting.Indented)
            {{
                json = new JsonFormatter(json).Format();
            }}
            return json;
        }}

        public static T Deserialize<T>(string json)
        {{
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }}

        private static string ReplaceDateTime(string json, string dateTimeFormat)
        {{
            return Regex.Replace(json, @""\\/Date\((\d+)\)\\/"", match =>
            {{
                var dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                dt = dt.ToLocalTime();
                return dt.ToString(dateTimeFormat);
            }});
        }}

        public class Settings
        {{
            public Settings()
            {{
                RecursionLimit = 100;
                DateTimeFormat = ""yyyy-MM-dd HH:mm:ss"";
                Formatting = Formatting.Indented;
            }}
            public int RecursionLimit {{ get; set; }}
            public string DateTimeFormat {{ get; set; }}
            public Formatting Formatting {{ get; set; }}
        }}

        public enum Formatting
        {{
            None = 0,
            Indented = 1
        }}

        public class StringWalker
        {{
            private readonly string _s;

            public int Index {{ get; private set; }}
            public bool IsEscaped {{ get; private set; }}
            public char CurrentChar {{ get; private set; }}

            public StringWalker(string s)
            {{
                _s = s;
                Index = -1;
            }}

            public bool MoveNext()
            {{
                if (Index == _s.Length - 1)
                    return false;

                if (IsEscaped == false)
                    IsEscaped = CurrentChar == '\\';
                else
                    IsEscaped = false;
                Index++;
                CurrentChar = _s[Index];
                return true;
            }}
        }}

        public class IndentWriter
        {{
            private readonly StringBuilder _result = new StringBuilder();
            private int _indentLevel;

            public void Indent()
            {{
                _indentLevel++;
            }}

            public void UnIndent()
            {{
                if (_indentLevel > 0)
                    _indentLevel--;
            }}

            public void WriteLine(string line)
            {{
                _result.AppendLine(CreateIndent() + line);
            }}

            private string CreateIndent()
            {{
                StringBuilder indent = new StringBuilder();
                for (int i = 0; i < _indentLevel; i++)
                    indent.Append(""  "");
                return indent.ToString();
            }}

            public override string ToString()
            {{
                return _result.ToString();
            }}
        }}

        public class JsonFormatter
        {{
            private readonly StringWalker _walker;
            private readonly IndentWriter _writer = new IndentWriter();
            private readonly StringBuilder _currentLine = new StringBuilder();
            private bool _quoted;

            public JsonFormatter(string json)
            {{
                _walker = new StringWalker(json);
                ResetLine();
            }}

            public void ResetLine()
            {{
                _currentLine.Length = 0;
            }}

            public string Format()
            {{
                while (MoveNextChar())
                {{
                    if (!_quoted && IsOpenBracket())
                    {{
                        WriteCurrentLine();
                        AddCharToLine();
                        WriteCurrentLine();
                        _writer.Indent();
                    }}
                    else if (!_quoted && IsCloseBracket())
                    {{
                        WriteCurrentLine();
                        _writer.UnIndent();
                        AddCharToLine();
                    }}
                    else if (!_quoted && IsColon())
                    {{
                        AddCharToLine();
                        WriteCurrentLine();
                    }}
                    else
                    {{
                        AddCharToLine();
                    }}
                }}
                WriteCurrentLine();
                return _writer.ToString();
            }}

            private bool MoveNextChar()
            {{
                bool success = _walker.MoveNext();
                if (IsApostrophe())
                {{
                    _quoted = !_quoted;
                }}
                return success;
            }}

            public bool IsApostrophe()
            {{
                return _walker.CurrentChar == '""' && !_walker.IsEscaped;
            }}

            public bool IsOpenBracket()
            {{
                return _walker.CurrentChar == '{{'
                    || _walker.CurrentChar == '[';
            }}

            public bool IsCloseBracket()
            {{
                return _walker.CurrentChar == '}}'
                    || _walker.CurrentChar == ']';
            }}

            public bool IsColon()
            {{
                return _walker.CurrentChar == ',';
            }}

            private void AddCharToLine()
            {{
                _currentLine.Append(_walker.CurrentChar);
            }}

            private void WriteCurrentLine()
            {{
                string line = _currentLine.ToString().Trim();
                if (line.Length > 0)
                {{
                    _writer.WriteLine(line);
                }}
                ResetLine();
            }}
        }}
    }}
}}";
    }
}

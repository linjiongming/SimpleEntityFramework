using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleEntityFramework.Domain.Roles.Templates;

namespace SimpleEntityFramework.Domain.Objects.Templates.Framework
{
    public class LoggerTemplate : ClassTemplate
    {
        public const string ClassName = "Logger";

        public LoggerTemplate(IProjectTemplate project) : base(project)
        {
        }

        public override string Name => ClassName;

        public override string FileContent => $@"{Profile}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace {Namespace}
{{
    public class {Name}
    {{
        private static readonly Lazy<{Name}> _lazy = new Lazy<{Name}>(() => new {Name}(), true);
        private static readonly LogLevel _logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), ConfigurationManager.AppSettings.Get(""LogLevel"") ?? ""All"", true);
        private static readonly Dictionary<int, string> _threadDirectory = new Dictionary<int, string>();

        public static {Name} Instance => _lazy.Value;

        public static void Info(object info, int stackTrace = -1)
        {{
            if (_logLevel < LogLevel.Info) return;
            Instance.WriteLog(info, ""[INFO]"", stackTrace + 1);
        }}

        public static void Error(object error)
        {{
            if (_logLevel < LogLevel.Error)
                Instance.WriteLog(error, ""[ERROR]"");
            else
                Instance.WriteLog(error is Exception ex ? ex.Message : error, ""[ERROR]"");
        }}

        private bool _needLogTime;
        private string _folder;

        public LogMode Mode {{ get; set; }}

        public string Folder
        {{
            get
            {{
                if (string.IsNullOrWhiteSpace(_folder))
                {{
                    _folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""log"");

                    if (!Directory.Exists(_folder))
                    {{
                        Directory.CreateDirectory(_folder);
                    }}
                }}

                return _folder;
            }}
        }}

        public string FileName => $""{{DateTime.Today:yyyy-MM-dd}}.log"";

        public string FilePath => Path.Combine(Folder, FileName);

        private {Name}()
        {{
            _needLogTime = true;
            Mode = LogMode.Console | LogMode.File;
        }}

        public string CurrentThreadTag
        {{
            get => _threadDirectory.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var value) ? value : null;
            set => _threadDirectory[Thread.CurrentThread.ManagedThreadId] = value;
        }}

        private static readonly object _locker = new object();
        public void WriteLog(object content, string prefix = null, int stackTrace = 0)
        {{
            lock (_locker)
            {{
                var formatted = string.Empty;
                if (File.Exists(FilePath) && new FileInfo(FilePath).Length > 1)
                {{
                    formatted += Environment.NewLine;
                }}
                if (!string.IsNullOrWhiteSpace(prefix))
                {{
                    formatted += prefix;
                }}
                if (_needLogTime)
                {{
                    formatted += $""[{{DateTime.Now:yyyy-MM-dd HH:mm:ss}}]"";
                }}
                formatted += string.IsNullOrWhiteSpace(formatted) ? content : ("": "" + content); // 日志主体
                if (stackTrace > 0)
                {{
                    var method = new StackTrace(true).GetFrame(stackTrace).GetMethod();
                    formatted += $""-[{{method.ReflectedType.Name}}.{{method.Name}}]"";
                }}
                if (!string.IsNullOrWhiteSpace(CurrentThreadTag))
                {{
                    formatted += $""-[{{CurrentThreadTag}}]"";
                }}
                if ((Mode & LogMode.Console) > 0)
                {{
                    Console.WriteLine(formatted);
                }}
                if ((Mode & LogMode.File) > 0)
                {{
                    File.AppendAllText(FilePath, formatted);
                }}
                if ((Mode & LogMode.Database) > 0)
                {{
                    // todo
                }}
            }}
        }}

        public enum LogMode : byte
        {{
            Console  /**/ = 0b00000001,
            File     /**/ = 0b00000010,
            Database /**/ = 0b00000100,
        }}

        public enum LogLevel : byte
        {{
            None  /**/ = 0,
            Fatal /**/ = 1,
            Error /**/ = 2,
            Warn  /**/ = 3,
            Info  /**/ = 4,
            Debug /**/ = 5,
            All   /**/ = 6,
        }}
    }}
}}";
    }
}

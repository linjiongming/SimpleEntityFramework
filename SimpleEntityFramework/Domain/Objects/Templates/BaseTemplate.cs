﻿using SimpleEntityFramework.Domain.Roles;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.IO;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public abstract class BaseTemplate : ITemplate
    {
        public static readonly string Profile = $"/* Generated by SimpleEntityFramework on {DateTime.Now:yyyy-MM-dd HH:mm:ss} */";

        public virtual ISefGenerator Generator => SefGenerator.Current;

        public abstract string Namespace { get; }

        public virtual string FolderPath => string.IsNullOrWhiteSpace(Namespace) ? Generator.OutputFolder : Path.Combine(Generator.OutputFolder, Namespace);

        public abstract string FileName { get; }

        public virtual string FilePath => Path.Combine(FolderPath, FileName);

        public abstract string FileContent { get; }

        public virtual void Generate()
        {
            Logger.Info($"{Namespace} > {FileName} is generating...");
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            File.WriteAllText(FilePath, FileContent);
        }
    }
}

﻿
using System;

namespace SimpleEntityFramework.Domain.Roles.Templates
{
    public interface ITemplate
    {
        ISefGenerator Generator { get; }
        string Namespace { get; }
        string FolderPath { get; }
        string FileName { get; }
        string FilePath { get; }
        string FileContent { get; }
        void Generate();
    }
}

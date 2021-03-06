﻿using System.Collections.Generic;

namespace ConfigSourceNupkgModPreserver.Contracts.WindowsFacade
{
    public interface IFileSystemFacade
    {
        string GetDirectoryName(string fileFullName);
        IEnumerable<string> GetSubDirectories(string dir);
        IEnumerable<string> GetFiles(string dir, string extension);
        string ReadAllText(string path);
        string CombinePath(string path1, string path2);
        string CombinePath(params string[] paths);
    }
}

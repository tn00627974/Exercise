using System;
using System.Collections.Generic;

namespace HelloWorld.MysqlModelsTest2;

public partial class NewsFiles
{
    public Guid? NewsFilesId { get; set; }

    public Guid? NewsId { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }

    public string Extentsion { get; set; }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Shared
{
    public class FileDescriptor
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string FileId { get; set; }
    }
}
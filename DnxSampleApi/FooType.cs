#nullable enable
using System;

namespace DnxSampleApi
{
    public class FooType
    {
        public System.IO.FileAttributes ReqEnum { get; set; }
        public System.IO.FileAccess? OptEnum { get; set; }
        public string ReqString { get; set; } = "-";
        public string? OptString { get; set; }
        public decimal ReqNumber { get; set; }
        public decimal? OptNumber { get; set; }
        public DateTimeOffset ReqDateTime { get; set; }
        public DateTimeOffset? OptDateTime { get; set; }
        public bool ReqBool { get; set; }
        public bool? OptBool { get; set; }
        public object ReqObject { get; set; } = new object();
        public object? OptObject { get; set; }
        public object[] ReqArray { get; set; } = { };
        public object[]? OptArray { get; set; }
    }
}

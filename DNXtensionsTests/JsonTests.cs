#nullable enable
using Xunit;
using System.Text.Json;
using System;

namespace DNXtensions.Tests
{
    public class JsonTests
    {
        [Theory()]
        [InlineData("true", true)]
        [InlineData("\"true\"", true)]
        [InlineData("1", true)]
        [InlineData("\"True\"", true)]
        [InlineData("\"TRUE\"", true)]
        [InlineData("false", false)]
        [InlineData("\"false\"", false)]
        [InlineData("0", false)]
        [InlineData("\"False\"", false)]
        [InlineData("\"FALSE\"", false)]
        public void Deserialize_VariousBoolValues_ShouldSucceed(string flagValue, bool expected)
        {
            var val = Json.Deserialize($"{{ \"flag\": {flagValue} }}", Json.DefaultOptions, new { flag = false });
            Assert.Equal(expected, val.flag);
        }

        [Theory]
        [InlineData("\"9\"", 9)]
        [InlineData("\"9.\"", 9)]
        [InlineData("\"9.0\"", 9)]
        [InlineData("\"9.5\"", 9.5)]
        [InlineData("9", 9)]
        [InlineData("9.0", 9)]
        [InlineData("9.5", 9.5)]
        public void Deserialize_VariousNumberValues_ShouldSucceed(string numberValue, decimal expected)
        {
            var val = Json.Deserialize($"{{ \"number\": {numberValue} }}", new { number = default(decimal?) });
            Assert.Equal(expected, val.number);
        }

        [Theory()]
        [InlineData("\"readOnly\"", System.IO.FileAttributes.ReadOnly)]
        [InlineData("\"ReadOnly\"", System.IO.FileAttributes.ReadOnly)]
        [InlineData("\"NORMAL\"", System.IO.FileAttributes.Normal)]
        [InlineData("null", default)]
        [InlineData("\"unknown\"", default)]
        public void Deserialize_VariousEnumValues_ShouldSucceed(string attrValue, System.IO.FileAttributes? expected)
        {
            var val = Json.Deserialize($"{{ \"attr\": {attrValue} }}", new { attr = default(System.IO.FileAttributes?) });
            Assert.Equal(expected, val.attr);
        }

        [Fact]
        public void Deserialize_MissingEnumValue_ShouldSucceed()
        {
            var val = Json.Deserialize("{ }", new { attr = default(System.IO.FileAccess) });
            Assert.Equal(default, val.attr);
        }       

        [Fact]
        public void Serialize_BoolAndEnumValue_ShouldSucceed()
        {
            var json = Json.Serialize(new { flag = true, attr = System.IO.FileAttributes.ReadOnly });
            Assert.Equal("{\"flag\":true,\"attr\":\"readOnly\"}", json);
        }

        [Fact]
        public void Serialize_NullEnumValue_ShouldSucceed()
        {
            var json = Json.Serialize(new { attr = default(System.IO.FileAttributes?) });
            Assert.Equal("{\"attr\":null}", json);
        }

        [Fact]
        public void Serialize_NullableEnumValue_ShouldSucceed()
        {
            var json = Json.Serialize(new { attr = (System.IO.FileAttributes?)System.IO.FileAttributes.Normal });
            Assert.Equal("{\"attr\":\"normal\"}", json);
        }

        class ComplexType
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
        };

        [Fact]
        public void Serialize_ComplexType_ShouldSucceed()
        {
            var json = Json.Serialize(new ComplexType());
            Assert.Equal("{\"reqEnum\":0,\"reqString\":\"-\",\"reqNumber\":0,\"reqDateTime\":\"0001-01-01T00:00:00+00:00\",\"reqBool\":false,\"reqObject\":{},\"reqArray\":[]}", json);

            json = Json.Serialize(new ComplexType
            {
                ReqEnum = System.IO.FileAttributes.Archive, ReqString = "", ReqNumber = -1.23m,
                ReqDateTime = new DateTimeOffset(2021, 2, 23, 4, 20, 3, TimeSpan.Zero),
                ReqBool = true, ReqArray = new[] { new object() }, ReqObject = new ComplexType()
            });
            Assert.Equal("{\"reqEnum\":\"archive\",\"reqString\":\"\",\"reqNumber\":-1.23,\"reqDateTime\":\"2021-02-23T04:20:03+00:00\",\"reqBool\":true," +
                "\"reqObject\":{\"reqEnum\":0,\"reqString\":\"-\",\"reqNumber\":0,\"reqDateTime\":\"0001-01-01T00:00:00+00:00\",\"reqBool\":false," +
                "\"reqObject\":{},\"reqArray\":[]},\"reqArray\":[{}]}", json);
        }

        [Fact]
        public void SerializeWithNulls_ComplexType_ShouldSucceed()
        {
            var json = Json.SerializeWithNulls(new ComplexType());

            Assert.Equal("{\"reqEnum\":0,\"optEnum\":null,\"reqString\":\"-\",\"optString\":null,\"reqNumber\":0,\"optNumber\":null,\"reqDateTime\":\"0001-01-01T00:00:00+00:00\",\"optDateTime\":null,\"reqBool\":false,\"optBool\":null,\"reqObject\":{},\"optObject\":null,\"reqArray\":[],\"optArray\":null}", json);

            json = Json.SerializeWithNulls(new ComplexType
            {
                ReqEnum = System.IO.FileAttributes.Archive, ReqString = "", ReqNumber = -1.23m,
                ReqDateTime = new DateTimeOffset(2021, 2, 23, 4, 20, 3, TimeSpan.Zero),
                ReqBool = true, ReqArray = new[] { new object() }, ReqObject = new ComplexType()
            });

            Assert.Equal("{\"reqEnum\":\"archive\",\"optEnum\":null,\"reqString\":\"\",\"optString\":null,\"reqNumber\":-1.23,\"optNumber\":null," +
                "\"reqDateTime\":\"2021-02-23T04:20:03+00:00\",\"optDateTime\":null,\"reqBool\":true,\"optBool\":null,\"reqObject\":" +
                "{\"reqEnum\":0,\"optEnum\":null,\"reqString\":\"-\",\"optString\":null,\"reqNumber\":0,\"optNumber\":null,\"reqDateTime\":\"0001-01-01T00:00:00+00:00\",\"optDateTime\":null,\"reqBool\":false,\"optBool\":null,\"reqObject\":{},\"optObject\":null,\"reqArray\":[],\"optArray\":null},\"optObject\":null,\"reqArray\":[{}],\"optArray\":null}", json);
        }

        [Fact]
        public void Deserialize_ComplexType_ShouldSucceed()
        {
            var json = "{\"reqEnum\":\"archive\",\"optEnum\":null,\"reqString\":\"\",\"optString\":null,\"reqNumber\":-1.23,\"optNumber\":null," +
                "\"reqDateTime\":\"2021-02-23T04:20:03+00:00\",\"optDateTime\":null,\"reqBool\":true,\"optBool\":null,\"reqObject\":" +
                "{\"reqEnum\":0,\"optEnum\":null,\"reqString\":\"-\",\"optString\":null,\"reqNumber\":0,\"optNumber\":null,\"reqDateTime\":" +
                "\"0001-01-01T00:00:00+00:00\",\"optDateTime\":null,\"reqBool\":false,\"optBool\":null,\"reqObject\":{},\"optObject\":null," +
                "\"reqArray\":[],\"optArray\":null},\"optObject\":null,\"reqArray\":[{}],\"optArray\":null}";

            var obj = Json.Deserialize<ComplexType>(json);

            Assert.Equal(System.IO.FileAttributes.Archive, obj.ReqEnum);
            Assert.Equal("", obj.ReqString);
            Assert.Equal(-1.23m, obj.ReqNumber);
            Assert.Equal(new DateTimeOffset(2021, 2, 23, 4, 20, 3, TimeSpan.Zero), obj.ReqDateTime);
            Assert.True(obj.ReqBool);
            Assert.IsType<JsonElement>(obj.ReqObject);
            Assert.IsType<object[]>(obj.ReqArray);
            Assert.Single(obj.ReqArray);
        }
    }
}
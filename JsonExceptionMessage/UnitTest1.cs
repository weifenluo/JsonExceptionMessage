using System;
using System.Text.Json;
using Xunit;

namespace JsonExceptionMessage
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var json = @"{""IntValue"":3}";
            var entity = EntitySerializer.Deserialize(json);
            Assert.Equal(3, entity.IntValue);
        }

        [Fact]
        public void Test2()
        {
            var json = @"{""Data"":{""IntValue"":3}}";
            var result = JsonSerializer.Deserialize<MyObject>(json);
            Assert.Equal(3, result!.Data.IntValue);
        }

        [Fact]
        public void Test3_failed_with_exception()
        {
            var json = @"{""IntValue"":3.5}";
            EntitySerializer.Deserialize(json);
        }

        [Fact]
        public void Test4_failed_with_exception()
        {
            var json = @"{""Data"":{""IntValue"":3.5}}";
            JsonSerializer.Deserialize<MyObject>(json);
        }
    }
}

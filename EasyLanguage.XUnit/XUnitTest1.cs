using Xunit;
using Xunit.Abstractions;
using Global;

public class XUnitTest1
{
    private readonly ITestOutputHelper Out;
    public XUnitTest1(ITestOutputHelper testOutputHelper)
    {
        Out = testOutputHelper;
        Print("Setup() called");
    }
    private void Print(object x, string title = null)
    {
        Out.WriteLine(ELang.ToPrintable(x, title));
    }
    [Fact]
    public void Test01()
    {
        var el1 = ELang.FromJson("""
            { "a": //line comment
              123
              b: '(777 888) }
            """);
        Print(el1, "el1");
        Assert.Equal("""
            {"a":123,"b":{"!":"quote","?":[777,888]}}
            """, ELang.ToJson(el1));
        var el2 = ELang.FromJson("""
            (add2 777 888) }
            """);
        Print(el2, "el2");
        Assert.Equal("""
            [{"!":"symbol","?":"add2"},777,888]
            """, ELang.ToJson(el2));
    }
}

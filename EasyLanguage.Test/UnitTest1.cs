using System;
using System.Collections.Generic;
using System.Linq;
using Global;
using static Global.ELang;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        Console.WriteLine("Setup() called");
    }

    [Test]
    public void Test01()
    {
        var el1 = ELang.FromJson("""
            { "a": //line comment
              123
              b: `(777 888) }
            """);
        Echo(el1, "el1");
        Assert.That(ELang.ToJson(el1), Is.EqualTo("""
            {"a":123,"b":{"!":"quasi-quote","?":[777,888]}}
            """));
        var el2 = ELang.FromJson("""
            (add2 777 888) }
            """);
        Echo(el2, "el2");
        Assert.That(ELang.ToJson(el2), Is.EqualTo("""
            [{"!":"symbol","?":"add2"},777,888]
            """));
        var el4 = ELang.FromJson("""
             { "a": //line comment
                123
                b: `(add2 777 888)
                x: (\1+ 123)
                y: \_!%&-=+*<>/ }
             """);
        Echo(ELang.ToJson(el4), "el4");
        //Echo(el4.ToJson(), "el4.ToJson()");
        Assert.That(ELang.ToJson(el4), Is.EqualTo("""
            {"a":123,"b":{"!":"quasi-quote","?":[{"!":"symbol","?":"add2"},777,888]},"x":[{"!":"symbol","?":"1+"},123],"y":{"!":"symbol","?":"_!%&-=+*<>/"}}
            """));
    }
}
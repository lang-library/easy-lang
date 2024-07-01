﻿using static Global.ELang;
using Xunit;
using Xunit.Abstractions;
using Global;
using System;
using System.Collections;

public class Elang1Test1
{
    private readonly ITestOutputHelper Out;
    public Elang1Test1(ITestOutputHelper testOutputHelper)
    {
        Out = testOutputHelper;
        //SharpJson.ClearAllSettings();
        Print("Setup() called");
    }
    private void Print(object x, string title = null)
    {
        Out.WriteLine(ToPrintable(x, title));
    }
    [Fact]
    public void Test01()
    {
        var o1 = Global.EasyLanguageParser.Parse("""
            (progn ([. console log] '"abc") true)
            """);
        string json1 = new ObjectParser().Stringify(o1, false);
        Print(json1, "json1");
        Assert.Equal("""
            [{"!":"symbol","?":"progn"},[[{"!":"dot"},{"!":"symbol","?":"console"},{"!":"symbol","?":"log"}],{"!":"quote","?":"abc"}],true]
            """, json1);
    }
    [Fact]
    public void Test02()
    {
        var o1 = Global.EasyLanguageParser.Parse("""
            (progn (setf (. ary[0]) '"abc"] true)
            """);
        string json1 = new ObjectParser().Stringify(o1, false);
        Print(json1, "json1");
        Assert.Equal("""
            [{"!":"symbol","?":"progn"},[{"!":"symbol","?":"setf"},[{"!":"dot"},{"!":"symbol","?":"ary"},[0]],{"!":"quote","?":"abc"}],true]
            """, json1);
    }
    [Fact]
    public void Test03()
    {
        var o1 = Global.EasyLanguageParser.Parse("""
            (progn (< 11 22) (> 11 22) (<= 11 22) (>= 11 22))
            """);
        string json1 = new ObjectParser().Stringify(o1, false);
        Print(json1, "json1");
        Assert.Equal("""
            [{"!":"symbol","?":"progn"},[{"!":"symbol","?":"<"},11,22],[{"!":"symbol","?":">"},11,22],[{"!":"symbol","?":"<="},11,22],[{"!":"symbol","?":">="},11,22]]
            """, json1);
    }
    [Fact]
    public void Test04()
    {
        var o1 = Global.EasyLanguageParser.Parse("""
            (progn (|| 11 22) (&& 11 22))
            """);
        string json1 = new ObjectParser().Stringify(o1, false);
        Print(json1, "json1");
        Assert.Equal("""
            [{"!":"symbol","?":"progn"},[{"!":"symbol","?":"||"},11,22],[{"!":"symbol","?":"&&"},11,22]]
            """, json1);
    }
    [Fact]
    public void Test05()
    {
        var o1 = Global.EasyLanguageParser.Parse("""
            `(progn (|| ~x 22) (&& ~@list))
            """);
        string json1 = new ObjectParser().Stringify(o1, false);
        Print(json1, "json1");
        Assert.Equal("""
            {"!":"quasi-quote","?":[{"!":"symbol","?":"progn"},[{"!":"symbol","?":"||"},{"!":"unquote","?":{"!":"symbol","?":"x"}},22],[{"!":"symbol","?":"&&"},{"!":"splice-unquote","?":{"!":"symbol","?":"list"}}]]}
            """, json1);
    }
    [Fact]
    public void Test06()
    {
        var o1 = Global.EasyLanguageParser.Parse("""
            $(11 22 33)
            """);
        string json1 = new ObjectParser().Stringify(o1, false);
        Print(json1, "json1");
        Assert.Equal("""
            {"!":"vector","?":[11,22,33]}
            """, json1);
    }
    [Fact]
    public void Test07()
    {
        // 𠀋 https://www.softel.co.jp/blogs/tech/archives/596 UTF-8で4バイトになる文字
        var o1 = Global.EasyLanguageParser.Parse("""
            ?𠀋
            """);
        string json1 = new ObjectParser().Stringify(o1, false);
        Print(json1, "json1");
        Assert.Equal("""
            {"!":"char","?":"𠀋"}
            """, json1);
    }
    [Fact]
    public void Test08()
    {
        var o1 = Global.EasyLanguageParser.Parse("""
            $'string
            """);
        string json1 = new ObjectParser().Stringify(o1, false);
        Print(json1, "json1");
        Assert.Equal("""
            {"!":"function-quote","?":{"!":"symbol","?":"string"}}
            """, json1);
    }
    [Fact]
    public void Test09()
    {
        var o1 = Global.EasyLanguageParser.Parse("""
            [^ lisp? true false null true1, false1, null1]
            """);
        string json1 = new ObjectParser().Stringify(o1, false);
        Print(json1, "json1");
        Assert.Equal("""
            [{"!":"symbol","?":"^"},{"!":"symbol","?":"lisp?"},true,false,null,{"!":"symbol","?":"true1"},{"!":"symbol","?":"false1"},{"!":"symbol","?":"null1"}]
            """, json1);
    }
}

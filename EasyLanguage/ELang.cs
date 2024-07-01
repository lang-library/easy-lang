using Esprima.Ast;
using Jint.Native.Function;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static Global.ELang;

namespace Global;

public class ELang
{
    public static bool DebugOutput = false;
    public static bool ShowDetail = false;
    //public static bool ForceASCII = false;
    public static object FromJson(string json)
    {
        return new CSharpEasyLanguageHandler(true, false).Parse(json);
    }
    public static object FromCode(string code)
    {
        object obj = FromJson(code);
        Echo(obj);
        return ToAST(obj);
    }
    public static object FromObject(object x)
    {
        return new ObjectParser(false).Parse(x);
    }
    public static string ToJson(object x, bool indent = false)
    {
        return new ObjectParser(false).Stringify(x, indent);
    }
    public static List<object> NewList(params object[] objects)
    {
        return new List<object>(objects);
    }
    public static string FullName(dynamic x)
    {
        if (x is null) return "null";
        string fullName = ((object)x).GetType().FullName;
        return fullName.Split('`')[0];
    }
    public static string ToPrintable(object x, string title = null)
    {
        return ObjectParser.ToPrintable(ShowDetail, x, title);
    }
    public static void Echo(object x, string title = null)
    {
        String s = ToPrintable(x, title);
        Console.WriteLine(s);
        System.Diagnostics.Debug.WriteLine(s);
    }
    public static void Log(object x, string? title = null)
    {
        String s = ToPrintable(x, title);
        Console.Error.WriteLine("[Log] " + s);
        System.Diagnostics.Debug.WriteLine("[Log] " + s);
    }
    public static void Debug(object x, string? title = null)
    {
        if (!DebugOutput) return;
        String s = ToPrintable(x, title);
        Console.Error.WriteLine("[Debug] " + s);
        System.Diagnostics.Debug.WriteLine("[Debug] " + s);
    }
    public static void Message(object x, string? title = null)
    {
        if (title == null) title = "Message";
        String s = ToPrintable(x, null);
        NativeMethods.MessageBoxW(IntPtr.Zero, s, title, 0);
    }
    internal static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int MessageBoxW(
            IntPtr hWnd, string lpText, string lpCaption, uint uType);
    }
    protected static object ToAST(object x)
    {
        if (x == null) return null;
        else if (x is decimal)
        {
            return x;
        }
        else if (x is string)
        {
            var result = new Dictionary<string, object>();
            result["!"] = "quote";
            result["?"] = x;
            return result;
        }
        else if (x is List<object> list)
        {
            var result = new List<object>();
            foreach (var e in list)
            {
                result.Add(ToAST(e));
            }
            return result;
        }
        else if (x is Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("!"))
            {
                var type = dict["!"];
                if (type is string)
                {
                    switch (type)
                    {
                        case "as-is":
                            return x;
                        case "quote":
                            return x;
                        case "dot":
                            return ".";
                        case "symbol":
                            return dict["?"];
                        default:
                            //throw new Exception($"{type} is not expected");
                            break;
                    }
                }
                else
                {
                    throw new Exception($"type is string");
                }
            }
            var result = new Dictionary<string, object>();
            foreach (var key in dict.Keys)
            {
                result[key] = ToAST(dict[key]);
            }
            return result;
        }
        else
        {
            throw new Exception($"{FullName(x)} is not supported");
        }
    }
}

public class ELang2Transform
{
    public static string gettype(object x)
    {
        string fullName = FullName(x);
        if (
            (x is System.Int32)
            ||
            (x is System.Double)
            )
        {
            return "number";
        }
        else if (x is System.String)
        {
            return "string";
        }
        else if (x is List<object> list)
        {
            return "list";
        }
        else if (x is Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("!"))
                return (string)dict["!"]; // gettype_for_special_bag(x);
            return "dict";
        }
        else
        {
            throw new Exception($"gettype(): {fullName} is not supported");
        }
    }
#if false
    static string gettype_for_special_bag(dynamic x)
    {
        return x["!"];
    }
#endif
    public static string transpile(dynamic ast)
    {
        ast = FromObject(ast);
        var sb = new StringBuilder();
        transpileBody(ast, sb);
        return sb.ToString();
    }

    static void transpileBody(dynamic ast, StringBuilder sb)
    {
        Echo(ast, "ast");
        string type = gettype(ast);
        Echo(type, "type");
        switch (type)
        {
            case "number":
                sb.Append(ast);
                return;
            case "string":
                sb.Append(ast);
                return;
            case "as-is":
                sb.Append(ast["?"].Trim());
                return;
            case "quote":
                sb.Append(new ObjectParser(false).Stringify(ast["?"], false));
                return;
#if true
            case "dict":
                transpileBag(ast, sb);
                return;
            case "list":
                transpileList(ast, sb);
                return;
#endif
            default:
                throw new Exception($"type:{type} is not supported");
        }
    }

    static void transpileBag(dynamic ast, StringBuilder sb)
    {
        sb.Append("{");
        int i = 0;
        foreach (var x in ast)
        {
            if (i > 0) sb.Append(",");
            string key = (string)x.Key;
            dynamic val = x.Value;
            sb.Append(new ObjectParser(false).Stringify(key, false));
            sb.Append(":");
            transpileBody(val, sb);
            i++;
        }
        sb.Append("}");
    }

    static void transpileList(dynamic ast, StringBuilder sb)
    {
        Echo(ast.Count);
        if (ast.Count == 0) throw new Exception("list length is 0");
        transpileFunCall(ast, sb);
    }

    static void transpileFunCall(dynamic ast, StringBuilder sb)
    {
        dynamic first = ast[0];
        Echo(first, "first");
        first = transpieFunName(first);
        Echo(first, "first");
        if (funcList.ContainsKey(first))
        {
            funcList[first](ast, sb);
            return;
        }
        sb.Append(first);
        sb.Append("(");
        for (int i = 1; i < ast.Count; i++)
        {
            if (i > 1) sb.Append(",");
            transpileBody(ast[i], sb);
        }
        sb.Append(")");
    }

    static string transpieFunName(dynamic ast)
    {
        string type = gettype(ast);
        if (type == "string") return ast;
        if (type == "list")
        {
            if (ast.Count == 0) throw new Exception("dot notation length is 0");
            string first = ast[0];
            Echo(first, "first(dot notation)");
            string result = "";
            for (int i = 1; i < ast.Count; i++)
            {
                if (i > 1) result += ".";
                result += ast[i];
            }
            return result;
        }
        throw new Exception($"transpieFunName(): type {type} not expected");
    }

    delegate dynamic TranspilerFunction(dynamic ast, StringBuilder sb);
    static Dictionary<string, TranspilerFunction> funcList = new Dictionary<string, TranspilerFunction>()
        {
        {"program", (dynamic ast, StringBuilder sb) => {
            for (int i = 1; i<ast.Count; i++) {
                transpileBody(ast[i], sb);
                sb.Append(";");
            }
            return null;
        }},
        {"define",  (dynamic ast, StringBuilder sb) => {
            sb.Append("var ");
            sb.Append(ast[1]);
            sb.Append("=");
            transpileBody(ast[2], sb);
            return null;
        }
        }
    };

};

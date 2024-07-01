using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using static Global.ELang;

namespace Global;

public class ELangScript
{
    public static V8ScriptEngine CreateEngine()
    {
        var engine = new V8ScriptEngine();
        engine.AddHostObject("v8", engine);
        engine.AddHostObject("host", new ExtendedHostFunctions());
        engine.AddHostObject("ELangAssembly", new HostTypeCollection(typeof(ELang).Assembly));
        engine.AddHostObject("Load", new Action<string>(path =>
        {
            string code = File.ReadAllText(path);
            engine.Execute(code);
        }));
        engine.Execute("""
            globalThis.FullName = ELangAssembly.Global.ELang.FullName;
            globalThis.Echo = ELangAssembly.Global.ELang.Echo;
            globalThis.Log = ELangAssembly.Global.ELang.Log;
            globalThis.Message = ELangAssembly.Global.ELang.Message;
            globalThis.FromJson = ELangAssembly.Global.ELang.FromJson;
            globalThis.FromObject = ELangAssembly.Global.ELang.FromObject;
            globalThis.ToJson = ELangAssembly.Global.ELang.ToJson;
            //Load("assets/test.js");
            var map = FromObject({ a: 123 });
            Echo(map, "map");
            """);
        var x = engine.Evaluate("11+22");
        Echo(x, "x");
        return engine;
    }
}

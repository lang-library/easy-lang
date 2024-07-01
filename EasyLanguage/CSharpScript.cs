//using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Global;
public class CSharpScript
{
    private ScriptOptions _opt;
    public CSharpScript(bool implicitUsings = false, string[] extraUsings = null, Assembly[] extraAssemblies = null)
    {
        this._opt = _CreateOptions(implicitUsings, extraUsings, extraAssemblies);
    }
    private ScriptOptions _CreateOptions(bool implicitUsings, string[] usingList, Assembly[] assemblyList = null)
    {
        List<Assembly> defaultAssemblyList = new List<Assembly>()
        {
            Assembly.GetAssembly(typeof(System.Dynamic.DynamicObject)), // System.Code
            Assembly.GetAssembly(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo)), // Microsoft.CSharp
            Assembly.GetAssembly(typeof(System.Dynamic.ExpandoObject)),
            Assembly.GetAssembly(typeof(System.Data.DataTable)),
        };
        if (assemblyList != null)
            foreach (var x in assemblyList)
            {
                defaultAssemblyList.Add(x);
            }
        List<string> defaultUsingList = !implicitUsings ? new List<string>() : new List<string>()
        {
            "System", "System.Dynamic", "System.Linq", "System.Text", "System.IO",
            "System.Collections.Generic", "System.Data",
            "System.Xml.Linq"
        };
        if (usingList != null)
            foreach (var x in usingList)
            {
                defaultUsingList.Add(x);
            }
        return ScriptOptions.Default
            .AddReferences(defaultAssemblyList)
            .AddImports(defaultUsingList)
            .WithLanguageVersion(LanguageVersion.Preview)
            ;
    }
    public async Task<object> EvaluateAsync(string source, object args = null)
    {
        try
        {
            object value = null;
            if (args == null)
            {
                var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(source, options: _opt);
                var result = script.RunAsync().Result;
                value = result.ReturnValue;
            }
            else
            {
                var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(source, options: _opt, globalsType: args.GetType());
                var result = await script.RunAsync(globals: args);
                value = result.ReturnValue;
            }
            return value;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }
        return null;
    }
    public object Evaluate(string source, object args = null)
    {
        var task = EvaluateAsync(source, args);
        task.Wait();
        return task.Result;
    }
    public async Task<bool> ExecuteAsync(string source, object args = null)
    {
        try
        {
            object value = null;
            if (args == null)
            {
                var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(source, options: _opt);
                var result = script.RunAsync();
                result.Wait();
                value = result.Result.ReturnValue;
            }
            else
            {
                var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(source, options: _opt, globalsType: args.GetType());
                var result = await script.RunAsync(globals: args);
                value = result.ReturnValue;
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }
        return false;
    }
    public bool Execute(string source, object args = null)
    {
        var task = ExecuteAsync(source, args);
        task.Wait();
        return task.Result;
    }
}

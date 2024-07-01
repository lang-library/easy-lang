using Global;
using System;
using System.IO;
using static Global.ELang;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using System.ComponentModel;
using System.Web.UI;
using System.Text;
using Antlr4.Runtime.Misc;

namespace Main;


static class Program
{
    [STAThread]
    static void Main(string[] originalArgs)
    {
        ELang.ShowDetail = true;

        var interp = NukataLisp.MakeInterp().Result;
        interp.Def("add2", 2, a => {
            var x = Convert.ToDecimal(a[0]);
            Echo(FullName(x), "FullName(x)");
            var y = Convert.ToDecimal(a[1]);
            Echo(FullName(y), "FullName(y)");
            return x + y;
        });
        StringReader sr = new StringReader("(print (add2 11 22))");
        var r = NukataLisp.Run(interp, sr).Result;
        Echo(r, "r");

        string code = File.ReadAllText("assets/test.el");
        var ast = ELang.FromCode(code);
        Echo(ast, "ast");

        using (var engine = ELangScript.CreateEngine())
        {
            engine.AddHostType(typeof(Console));
            engine.Execute("""
            Load("assets/test-funcs.js");
            """);
            string script = ELang2Transform.transpile(ast);
            Echo(script, "script");
            File.WriteAllText("tmp.js", script);
            script = Busybox.Output("js-beautify.cmd tmp.js");
            Echo(script, "script");
            var output = engine.Evaluate(script);
            Echo(output, "output");
            Echo(FromObject(output), "output");
        }


        MyJS myJS = new MyJS();
        myJS.Execute("""
            function add2(a, b) { return a + b }
            """);
        Echo(myJS.Evaluate("add2($1, $2)", 111, 222));

        System.Environment.Exit(0);

        var el1 = ELang.FromJson("""
            { "a": //line comment
              123
              b: `(777 888) }
            """);
        Echo(el1, "el1");
        var el2 = ELang.FromJson("""
            { "a": //line comment
              123
              b: `(add2 777 888) }
            """);
        Echo(el2, "el2");
        var el3 = Global.EasyLanguageParser.Parse("""
            { "a": //line comment
              123
              b: `(add2 777 888)
              x: (1+ 123)
              y: _!%&-=+*/?
              z: :keyword-_!%&-=+*/? }
            """);
        Echo(el3, "el3");
        var elang = new EasyLanguage();
#if false
        var ans01 = ELang2.EvalString("""
            (@+ 11 22)
            """);
        Echo(ans01, "ans01");
        var ans02 = ELang2.EvalFile("assets/test.elAng");
        Echo(ans02, "ans02");
        var csharp = new ELang2.CSharpScript();
        var answer = csharp.Evaluate("11+22");
        Echo(answer, "answer");
#endif
        var order = ELang.FromJson("{b: 123, a: 456, _:789}");
        Echo(order, "order");

        using (var engine = new V8ScriptEngine())

        {
            // expose a host type
            engine.AddHostType("Console", typeof(Console));
            engine.Execute("Console.WriteLine('{0} is an interesting number.', Math.PI)");

            // expose a host object
            engine.AddHostObject("random", new Random());
            engine.Execute("Console.WriteLine(random.NextDouble())");

            // expose entire assemblies
            engine.AddHostObject("lib", new HostTypeCollection("mscorlib", "System.Core"));
            engine.Execute("Console.WriteLine(lib.System.DateTime.Now)");

            // create a host object from script
            engine.Execute(@"
        birthday = new lib.System.DateTime(2007, 5, 22);
        Console.WriteLine(birthday.ToLongDateString());
    ");

            // use a generic class from script
            engine.Execute(@"
        Dictionary = lib.System.Collections.Generic.Dictionary;
        dict = new Dictionary(lib.System.String, lib.System.Int32);
        dict.Add('foo', 123);
    ");

            // call a host method with an output parameter
            engine.AddHostObject("host", new ExtendedHostFunctions());
            engine.Execute(@"
        intVar = host.newVar(lib.System.Int32);
        found = dict.TryGetValue('foo', intVar.out);
        Console.WriteLine('{0} {1}', found, intVar);
    ");

            // create and populate a host array
            engine.Execute(@"
        numbers = host.newArr(lib.System.Int32, 20);
        for (var i = 0; i < numbers.Length; i++) { numbers[i] = i; }
        Console.WriteLine(lib.System.String.Join(', ', numbers));
    ");

            // create a script delegate
            engine.Execute(@"
        Filter = lib.System.Func(lib.System.Int32, lib.System.Boolean);
        oddFilter = new Filter(function(value) {
            return (value & 1) ? true : false;
        });
    ");

            // use LINQ from script
            engine.Execute(@"
        oddNumbers = numbers.Where(oddFilter);
        Console.WriteLine(lib.System.String.Join(', ', oddNumbers));
    ");

            // use a dynamic host object
            engine.Execute(@"
        expando = new lib.System.Dynamic.ExpandoObject();
        expando.foo = 123;
        expando.bar = 'qux';
        delete expando.foo;
    ");

            // call a script function
            engine.Execute("function print(x) { Console.WriteLine(x); }");
            engine.Script.print(DateTime.Now.DayOfWeek);

            // examine a script object
            engine.Execute("person = { name: 'Fred', age: 5 }");
            Console.WriteLine(engine.Script.person.name);

            // read a JavaScript typed array
            engine.Execute("values = new Int32Array([1, 2, 3, 4, 5])");
            var values = (ITypedArray<int>)engine.Script.values;
            Console.WriteLine(string.Join(", ", values.ToArray()));

            // ELang
            //engine.AddHostType("ELang", typeof(ELang));
            //engine.AddHostType(typeof(ELang));
            engine.AddHostType(typeof(Global.ELang));
            var eo = Global.ELang.FromObject(new []  { 1, 2, 3 });
            engine.AddHostObject("eo", eo);
            engine.Execute("""
                function echo(msg, title = null) { ELang2.Echo(msg, title); }
                globalThis.echo2 = ELang2.Echo;
                ELang2.Echo(eo, "eo");
                ELang2.Echo(eo[1], "eo[1]");
                echo(eo[1]);
                var Int32T = host.type("System.Int32");
                var intValue = host.cast(Int32T, eo[1]);
                echo2(intValue, "intValue");
                var intValue2 = host.cast(lib.System.Int32, eo[2]);
                echo2(intValue2, "intValue2");
                for(var i=0; i<eo.Count; i++)
                {
                  echo2(eo[i], "eo[i]");
                }
                var list = ELang2.NewList(111, 222, 333);
                echo2(list, "list");
                echo2(ELang2.FullName(eo));
                echo2(ELang2.FullName(list));
                var map = { "a": 123, "b": "abc", c: [1, 2] };
                echo2(map, "map");
                //echo2(ELang2.FullName(map), "ELang2.FullName(map)");
                var map2 = ELang2.FromObject(map);
                echo2(map2, "map2");
                echo2(map2['c'], "map2['c']");
                echo2(ELang2.FullName(map2['c']), "ELang2.FullName(map2['c'])");
                                var ary = [1, 2, 3];
                echo2(ary, "ary");
                var ary2 = ELang2.FromObject(ary);
                echo2(ary2);
                echo2(ELang2.FullName(ary2[1]));
                """);
            // expose entire assemblies
            engine.AddHostObject("lib2", new HostTypeCollection(typeof(ELang).Assembly));
            engine.Execute("lib2.Global.ELang2.Echo('from lib2')");
            engine.AddHostObject("alert", new Action<object>(o =>
            {
                Console.WriteLine("alert:{0}", o);
            }));
            engine.Execute("alert('from action')");
            engine.AddHostObject("add2", new Func<int, int, int>((a, b) =>
            {
                return a + b;
            }));
            engine.Execute("echo2(add2(11, 22))");
            engine.Execute("map2.c[1] = 777");
            engine.Execute("echo2(map2)");
        }
    }
}
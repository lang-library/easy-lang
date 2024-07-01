using Jint;
using System.IO;
using System.Reflection;
using Global;
namespace Global;
public class JintScript
{
    public static Jint.Engine CreateEngine(params Assembly[] list)
    {
        var engine = new Jint.Engine(cfg =>
        {
            cfg.AllowClr();
            for (int i = 0; i < list.Length; i++)
            {
                cfg.AllowClr(list[i]);
            }
        });
        engine.SetValue("_globals", new JintScriptGlobal(engine));
        engine.Execute("""
            var echot = _globals.echo;
            var log = _globals.log;
            var getenv = _globals.getenv;
            var appFile = _globals.appFile;
            var appDir = _globals.appDir;
            var load = _globals.load;
            var $ns = importNamespace;
            """);
        return engine;
    }
}
internal class JintScriptGlobal
{
    Jint.Engine engine = null;
    public JintScriptGlobal(Jint.Engine engine)
    {

        this.engine = engine;
    }
    public void echo(dynamic x, string? title = null)
    {
        ELang.Echo(x, title);
    }
    public void log(dynamic x, string? title = null)
    {
        ELang.Log(x, title);
    }
    public object fromJson(string json)
    {
        return ELang.FromJson(json);
    }
    public string toJson(object x, bool indent = false)
    {
        return ELang.ToJson(x, indent);
    }
    public object fromObject(object x)
    {
        return ELang.FromObject(x);
    }
    public string getenv(string name)
    {
        return System.Environment.GetEnvironmentVariable(name);
    }
    public string appFile()
    {
        return Assembly.GetExecutingAssembly().Location;
    }
    public string appDir()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
    public void load(string path)
    {
        string code = File.ReadAllText(path);
        engine.Execute(code);
    }
}

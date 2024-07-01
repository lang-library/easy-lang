using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using static Global.EasyObject;
using static Global.ELang;

namespace Global;

public class EasyLanguage
{
    public EasyLanguage()
    {
    }
    public object Eval(object exp)
    {
        //Echo(exp.TypeValue, "exp.TypeValue");
        //Echo(exp.m_data.GetType().ToString());
        Echo(exp, "exp");
        if (exp == null)
        {
            return null;
        }
        else if (exp is List<object> list)
        {
            if (list.Count == 0)
            {
                return new List<object>();
            }
            var e0 = list[0];
            if (!(e0 is string))
            {
                return exp;
            }
            string funcName = (string)e0;
            var args = new List<object>();
            for (int i = 1; i<list.Count; i++)
            {
                args.Add(Eval(list[i]));
            }
            return EvalFunctionCall(funcName, args);
        }
        throw new Exception($"EasyObjectType.@{exp.GetType().ToString()} is not supported.");
    }
    public object EvalString(string expText)
    {
        Echo(expText, "expText");
        object exp = ELang.FromJson(expText);
        return Eval(exp);
    }
    public object EvalFile(string expPath)
    {
        string expTest = File.ReadAllText(expPath);
        return EvalString(expTest);
    }
    protected object EvalFunctionCall(string funcName, List<object> args)
    {
        switch (funcName)
        {
            case "+":
                return Convert.ToDecimal(args[0]) + Convert.ToDecimal(args[1]);
            default:
                return null;
        }
    }
}

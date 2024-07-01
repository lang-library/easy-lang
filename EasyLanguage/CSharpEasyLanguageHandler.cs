﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Global;

public class CSharpEasyLanguageHandler: IJsonHandler
{
    EasyLanguageParser jsonParser;
    ObjectParser objParser;
    public CSharpEasyLanguageHandler(bool NumberAsDecimal, bool ForceASCII)
    {
        this.jsonParser = new EasyLanguageParser(NumberAsDecimal);
        this.objParser = new ObjectParser(ForceASCII);
    }
    public object Parse(string json)
    {
        return this.jsonParser.ParseJson(json);
    }
    public string Stringify(object x, bool indent, bool sort_keys = false)
    {
        return this.objParser.Stringify(x, indent, sort_keys);
    }
}

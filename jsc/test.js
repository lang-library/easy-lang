import System;
Console.WriteLine("Hello JScript.NET World!");
Console.WriteLine(typeof "Hello JScript.NET World!");
Console.WriteLine(typeof []);
Console.WriteLine(typeof {});
//Console.WriteLine(Array.isArray([1, 2, 3]));
//Console.WriteLine(Array.isArray("a"));

var ary = [1, 2, 3];
Console.WriteLine(ary instanceof Array);
var n = 123;
Console.WriteLine(n.GetType().FullName);
var n = 123.45;
Console.WriteLine(n.GetType().FullName);
var n = [123.45];
Console.WriteLine(n.GetType().FullName);
var n = {"a":[123.45]};
Console.WriteLine(n.GetType().FullName);

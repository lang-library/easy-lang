Echo('this is gettype.js');

function gettype(x)
{
    let fullName = FullName(x);
    //Echo(fullName);
    switch (fullName) {
        case "System.Int32":
            return "number";
        case "System.String":
            return "string";
        case "Microsoft.ClearScript.V8.V8ScriptItem+V8Array":
            return "array";
        case "System.Collections.Generic.List":
            return "list";
        case "Microsoft.ClearScript.PropertyBag": {
            if ('!' in x) {
                //Echo("contains!");
                return gettype_for_special_bag(x);
            }
            return "bag";
        }
        default:
            throw new Error(`gettype(): ${fullName} is not supported`);
    }
}

function gettype_for_special_bag(x) {
    return x['!'];
    /*
    switch (x['!']) {
        default:
            throw new Error(`${x['!']} is not supported`);
    }
    */
}
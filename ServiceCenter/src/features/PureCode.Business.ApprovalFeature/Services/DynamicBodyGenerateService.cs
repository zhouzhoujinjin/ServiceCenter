using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PureCode.Business.ApprovalFeature.Models;

namespace PureCode.Business.ApprovalFeature.Services;

public class DynamicBodyGenerateService
{
  private static Dictionary<string, Type> _cacheTypes = [];

  public DynamicBodyGenerateService()
  {
  }

  public Type GetTypeByTemplateName(string templateName)
  {
    if (_cacheTypes.ContainsKey(templateName))
    {
      return _cacheTypes[templateName];
    }

    var list = new List<FormField>();

    foreach (var formField in list)
    {
      
    }
  }

  private void BuildField(TypeBuilder typeBuilder, string propertyName, Type propertyType)
  {
    var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);  
  
    var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);  
    var getPropBuilder = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);  
    ILGenerator getIl = getPropBuilder.GetILGenerator();  
  
    getIl.Emit(OpCodes.Ldarg_0);  
    getIl.Emit(OpCodes.Ldfld, fieldBuilder);  
    getIl.Emit(OpCodes.Ret);  
  
    var setPropBuilder =typeBuilder.DefineMethod("set_" + propertyName,  
      MethodAttributes.Public |  
      MethodAttributes.SpecialName |  
      MethodAttributes.HideBySig,  
      null, new[] { propertyType });  
  
    ILGenerator setIl = setPropBuilder.GetILGenerator();  
    Label modifyProperty = setIl.DefineLabel();  
    Label exitSet = setIl.DefineLabel();  
  
    setIl.MarkLabel(modifyProperty);  
    setIl.Emit(OpCodes.Ldarg_0);  
    setIl.Emit(OpCodes.Ldarg_1);  
    setIl.Emit(OpCodes.Stfld, fieldBuilder);  
  
    setIl.Emit(OpCodes.Nop);  
    setIl.MarkLabel(exitSet);  
    setIl.Emit(OpCodes.Ret);  
  
    propertyBuilder.SetGetMethod(getPropBuilder);  
    propertyBuilder.SetSetMethod(setPropBuilder);  
    
  }
}
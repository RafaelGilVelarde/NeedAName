using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using Godot;

public class ModuleInit
{
    [ModuleInitializer]
    public static void Initialize(){
        //AppDomain.CurrentDomain.FirstChanceException +=CurrentDomainOnFirstChanceException;
           System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(System.Reflection.Assembly.GetExecutingAssembly()).Unloading += alc =>
         {
                                 Debug.WriteLine("Unloading");

             {
                 ////complete godot async tasks, which may have been left hanging
                 var isDone = false;
                 while (isDone is false)
                 {
                     try
                     {
                         Dispatcher.SynchronizationContext.ExecutePendingContinuations();
                         isDone = true;
                         Debug.WriteLine("Initilaized");
                     }
                     catch (Exception ex)
                     {
                        Debug.WriteLine("Exception: "+ex);
                         /*if (ex._IsRoutineControlFlow() is false)
                         {
                             __.Assert(ex);
                         }*/
                     }
                 }
             }


             //need to detach the handler, or it will keep the assembly alive.
            //AppDomain.CurrentDomain.FirstChanceException -= CurrentDomainOnFirstChanceException;

             {
                 //unload STJ cached assemblies,
                 //see https://github.com/dotnet/runtime/issues/65323#issuecomment-1320949911
                 //and https://github.com/godotengine/godot/issues/78513#issuecomment-1624682104
                 var assembly = typeof(JsonSerializerOptions).Assembly;
                 var updateHandlerType = assembly.GetType("System.Text.Json.JsonSerializerOptionsUpdateHandler");
                 var clearCacheMethod = updateHandlerType?.GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public);
                 clearCacheMethod?.Invoke(null, new object?[] { null });

             }
         };
    }
     /*   [DebuggerNonUserCode]
	private static void CurrentDomainOnFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
	{
		var ex = e.Exception;
		var est = EnhancedStackTrace.Current();
		var frame = est.GetFrame(0);
		//GD.PushError($"{'='._Repeat(40)}\nFCE: {ex} \n{frame}\n{'='._Repeat(40)}");
     // __.Assert(ex.Message, memberName:frame.GetMethod()?.Name,sourceFilePath:frame.GetFileName(),sourceLineNumber:frame.GetFileLineNumber(),tags:["FCE"] );
	}*/
}

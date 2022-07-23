using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Reflection;
using Uno.Server.Annotation;
using System.Text.Json;
using System.Collections;
using System.Text;
using System.Net;

namespace Uno.Server.Middlewares;

/// <summary>
/// A middleware that utilizes the <see cref="DataStreamAttribute"/>.
/// Any action marked with <see cref="DataStreamAttribute"/> will be invoked by this middleware itself.
/// The action **must** have the return type of <see cref="IEnumerable"/> and parameter count of 0 or 1.
/// The connection will be kept open and the stream will run until the enumeration ends.
/// The protocol of the payloads is: <br/>
/// [amount of characters of the JSON payload] [the JSON] (repeat) <br/>
/// When the streaming is ended (either by the endpoint or the client aborted the connection) a function will be invoked
/// on the controller, whose name is [The original endpoint function name] + "Ended", e.g.: UpdateModelEnded().
/// </summary>
public class DataStreamMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider serviceProvider;

    public DataStreamMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        this.serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Handle(context, serviceProvider);

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }

    private void Handle(HttpContext context, IServiceProvider serviceProvider)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            return;
        }

        var metadata = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

        if (metadata == null)
        {
            return;
        }

        var attribute = endpoint.Metadata.GetMetadata<DataStreamAttribute>();

        if (attribute == null)
        {
            return;
        }

        var methodInfo = metadata.MethodInfo;
        var actionType = methodInfo.ReturnType;

        if (typeof(IEnumerable).IsAssignableFrom(actionType) == false)
        {
            throw new Exception($"Data stream endpoints must have a return type of {nameof(IEnumerable)}");
        }

        object? parameter;

        if (metadata.Parameters.Count == 0)
        {
            parameter = null;
        }
        else if (metadata.Parameters.Count == 1)
        {
            parameter = DeserializeParameter(context, metadata.Parameters[0]);
        }
        else
        {
            throw new Exception("Only endpoints with maximum of 1 parameters are allowed to be DataStreams");
        }

        var activator = serviceProvider.GetRequiredService<IControllerFactory>();

        var routeData = context.GetRouteData();
        var controllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext(new Microsoft.AspNetCore.Mvc.ActionContext(context, routeData, metadata));
        var controller = activator.CreateController(controllerContext);

        if (controller == null)
        {
            throw new Exception("Could not activate controller");
        }

        RunStream(context, controller, methodInfo, parameter)
            .Wait(); // TODO: Should run in background ?

        activator.ReleaseController(controllerContext, controller);

        // We don't call the rest of the middlewares, as this one keeps up a continuous stream of data.
    }

    private static async Task RunStream(HttpContext context, object controller, MethodInfo action, object? parameter)
    {
        object? methodReturn;

        if (parameter == null)
        {
            methodReturn = action.Invoke(controller, null);
        }
        else
        {
            methodReturn = action.Invoke(controller, new[] { parameter });
        }

        if (methodReturn == null)
        {
            throw new Exception("Data streams can not return null");
        }

        var enumerable = (IEnumerable)methodReturn;
        var enumerator = enumerable.GetEnumerator();

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.ContentType = "application/json";
        bool clientDisconnected = context.RequestAborted.IsCancellationRequested;

        while (enumerator.MoveNext() && clientDisconnected == false)
        {
            // Start only **after** the first MoveNext().
            // The endpoint is supposed to run validation and set up the HTTP status code
            // **before** the first yield return, and must also do a `yield break` in
            // case any of the validation fails.
            if (context.Response.HasStarted == false)
            {
                await context.Response.StartAsync();
            }

            var obj = enumerator.Current;
            var json = JsonSerializer.Serialize(obj);

            await context.Response.WriteAsync($"{json.Length}\r\n", Encoding.UTF8);
            await context.Response.WriteAsync(json, Encoding.UTF8);

            clientDisconnected = context.RequestAborted.IsCancellationRequested;
        }

        CallEnded(controller, action.Name);

        await context.Response.CompleteAsync();
    }

    private static object DeserializeParameter(HttpContext context, ParameterDescriptor parameterDescriptor)
    {
        var type = parameterDescriptor.ParameterType;
        var bodyStream = context.Request.Body;
        object? paramObject = null;
        try
        {
            paramObject = JsonSerializer.DeserializeAsync(bodyStream, type).Result; // Synchronous operations are not allowed HurrDurr
        }
        catch (Exception e)
        {
        }
        

        if (paramObject == null)
        {
            throw new Exception("Invalid request JSON");
        }

        return paramObject;
    }

    private static void CallEnded(object controller, string actionName)
    {
        var type = controller.GetType();

        var implicitMethod = type.GetMethod($"{actionName}Ended", BindingFlags.Public | BindingFlags.Instance);

        if (implicitMethod == null)
        {
            return;
        }

        if (implicitMethod.GetParameters().Length > 0)
        {
            throw new Exception("Stream end handlers must have no parameters");
        }

        implicitMethod.Invoke(controller, null);
    }
}

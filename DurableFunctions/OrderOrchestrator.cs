using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Services;

namespace DurableFunctions
{
    public static class ProcessOrderOrchestration
    {
        [Function(nameof(ProcessOrderOrchestration))]
        public static async Task<List<bool>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context
        )
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(ProcessOrderOrchestration));
            logger.LogInformation("Processing order.");

            var orderID = context.GetInput<int>();
            var tasks = new List<TaskName> { nameof(ProcessPayment), nameof(ProcessInventory) };
            var taskResults = new Dictionary<TaskName, bool>();

            foreach (var task in tasks)
                taskResults[task] = await context.CallActivityAsync<bool>(task, (orderID, false));

            // Check if any task result is unsuccessful
            if (taskResults.Any(t => !t.Value))
            {
                // Update order status to failed
                await context.CallActivityAsync<bool>(nameof(UpdateOrderStatus), (orderID, false));
                if (taskResults.Any(t => t.Value)) // Did any task succeed?
                    foreach (var result in taskResults) // Not all results are the same, so:
                        if (result.Value) // reverse/undo all that were successful
                            await context.CallActivityAsync<bool>(result.Key, (orderID, true));
            }
            else // Update order status to PendingShipping
                await context.CallActivityAsync<bool>(nameof(UpdateOrderStatus), (orderID, true));

            return [.. taskResults.Values];
        }

        private static IOrderService ScopedOrderService(FunctionContext executionContext) =>
            executionContext
                .InstanceServices.CreateScope()
                .ServiceProvider.GetRequiredService<IOrderService>();

        [Function(nameof(ProcessPayment))]
        public static bool ProcessPayment(
            [ActivityTrigger] (int orderID, bool reverse) input,
            FunctionContext executionContext
        )
        {
            ILogger logger = executionContext.GetLogger(nameof(ProcessPayment));
            logger.LogInformation($"Starting {nameof(ProcessPayment)} orderID: {input.orderID}.");
            var result = ScopedOrderService(executionContext)
                .ProcessPayment(input.orderID, input.reverse);
            logger.LogInformation(
                $"{nameof(ProcessPayment)} orderID: {input.orderID}, result: {result}."
            );
            return result;
        }

        [Function(nameof(ProcessInventory))]
        public static bool ProcessInventory(
            [ActivityTrigger] (int orderID, bool reverse) input,
            FunctionContext executionContext
        )
        {
            ILogger logger = executionContext.GetLogger(nameof(ProcessInventory));
            logger.LogInformation($"Starting {nameof(ProcessInventory)} orderID: {input.orderID}.");
            var result = ScopedOrderService(executionContext)
                .ProcessInventory(input.orderID, input.reverse);
            logger.LogInformation(
                $"{nameof(ProcessInventory)} orderID: {input.orderID}, result: {result}."
            );
            return result;
        }

        [Function(nameof(UpdateOrderStatus))]
        public static bool UpdateOrderStatus(
            [ActivityTrigger] (int orderID, bool success) input,
            FunctionContext executionContext
        )
        {
            ILogger logger = executionContext.GetLogger(nameof(UpdateOrderStatus));
            logger.LogInformation(
                $"Starting {nameof(UpdateOrderStatus)} orderID: {input.orderID}."
            );
            var result = ScopedOrderService(executionContext)
                .UpdateOrderStatus(
                    input.orderID,
                    input.success ? OrderStatus.PendingShipping : OrderStatus.Failed
                );
            logger.LogInformation(
                $"{nameof(ProcessInventory)} orderID: {input.orderID}, result: {result}."
            );
            return result;
        }

        [Function($"{nameof(ProcessOrderOrchestration)}_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext
        )
        {
            ILogger logger = executionContext.GetLogger("OrderOrchestrator_HttpStart");

            // Function input comes from the request content.
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            logger.LogInformation(requestBody);
            int orderID = JsonSerializer.Deserialize<int>(requestBody);
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(ProcessOrderOrchestration),
                orderID
            );

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }

    public static class ExampleOrchestrator
    {
        [Function(nameof(ExampleOrchestrator))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context
        )
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(ExampleOrchestrator));
            logger.LogInformation("Saying hello.");
            var outputs = new List<string>();

            // Replace name and input with values relevant for your Durable Functions Activity
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [Function(nameof(SayHello))]
        public static string SayHello(
            [ActivityTrigger] string name,
            FunctionContext executionContext
        )
        {
            ILogger logger = executionContext.GetLogger("SayHello");
            logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }

        [Function("OrderOrchestrator_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext
        )
        {
            ILogger logger = executionContext.GetLogger("OrderOrchestrator_HttpStart");

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(ExampleOrchestrator)
            );

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}

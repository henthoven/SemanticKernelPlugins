using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAIPlugins.Plugins;

var deploymentName = "<DEPLOYMENT_NAME>";
var endpoint = "https://<YOUR_OPENAI_RESOURCE>.openai.azure.com/";
var apiKey = "<YOUR_API_KEY>";

// Make the kernel builder
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
builder.Services.TryAddTransient<HttpClient>();

// build the kernel
Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Register the plugin
kernel.Plugins.AddFromType<WeatherPlugin>("WeatherPlugin", serviceProvider: builder.Services.BuildServiceProvider());

// Set the parameter so that functions are automatically called by SemanticKernel
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),

};
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var history = new ChatHistory();
string? userInput;
do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (!string.IsNullOrEmpty(userInput))
    {
        // Add the input to the history
        history.AddUserMessage(userInput!);

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel);

        Console.WriteLine("Assistant > " + result);

        // Add the answer to the history
        history.AddMessage(result.Role, $"Assistant response {result.Content}");
    }
} while (!string.IsNullOrEmpty(userInput));


// Below is for demo purposes so that you can see the chat history that includes the interaction
// by SemanticKernel for automatic function calling
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Exiting. Full chat history:");
var entryNumber = 0;
foreach (var item in history)
{
    entryNumber++;
    if (item.InnerContent != null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{entryNumber} - InnerContent: {item.InnerContent}");
    }

    if (!string.IsNullOrWhiteSpace(item.ToString()))
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{entryNumber} - Content: {item}");
    }
}
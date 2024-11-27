using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var deploymentName = "<DEPLOYMENT_NAME>";
var endpoint = "https://<YOUR_OPENAI_RESOURCE>.openai.azure.com/";
var apiKey = "<YOUR_API_KEY>";

// kernel builder aanmaken
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

// kernel bouwen
Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
string? userInput;
do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (!string.IsNullOrEmpty(userInput))
    {
        // Input aan de historie toevoegen
        history.AddUserMessage(userInput!);

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: null,
            kernel: kernel);

        Console.WriteLine("Assistant > " + result);

        // Antwoord aan de historie toevoegen
        history.AddMessage(result.Role, result.Content ?? string.Empty);
    }
} while (!String.IsNullOrEmpty(userInput));
using System.Text.Json;
using AIBridges.Models;
using Florence2;

namespace AIBridges.Services;

public class Florence2AI : IAIService
{
    private Florence2Model? _florence2Model;
    public async ValueTask InitializeAsync()
    {
        if (_florence2Model != null)
        {
            return;
        }

        var fmd = new FlorenceModelDownloader("./onnx_models/Florence2/");

        await fmd.DownloadModelsAsync(Console.WriteLine);

        _florence2Model = new Florence2Model(fmd);
    }

    public async Task<string> ProcessRequestAsync(AIBridgeRequest request, object requestBody)
    {
        if (_florence2Model == null)
        {
            throw new InvalidOperationException("Florence2 model is not initialized. Call InitializeAsync() first.");
        }

        List<TaskTypes> tasks = GetFlorence2TaskTypes(request);

        var results = new List<FlorenceResults>(15);

        int i = 1;
        foreach (var task in tasks)
        {
            Console.WriteLine($"Task {i++}: {task}");

            using var stream = new MemoryStream(requestBody as byte[] ?? throw new InvalidDataException("Request body is not a byte array."));

            var singleResults = await Task.Run(() => 
                _florence2Model.Run(task, [stream], textInput: "", CancellationToken.None));

            if (singleResults == null || singleResults.Length == 0)
            {
                Console.WriteLine($"No results produce for TaskTypes: {task}");
            }
            else
            {
                results.AddRange(singleResults);
            }
        }

        var jsonResult = JsonSerializer.Serialize(results);

        return jsonResult;
    }

    public static List<TaskTypes> GetFlorence2TaskTypes(AIBridgeRequest model)
    {
        var tasks = new List<TaskTypes>();

        try
        {
            var configTypes = model.Task?.Split(',');

            if (configTypes != null)
            {
                foreach (var t in configTypes)
                {
                    if (Enum.TryParse<TaskTypes>(t, out var result))
                    {
                        tasks.Add(result);
                    }
                }
            }
            else
            {
                tasks.AddRange(Enum.GetValues<TaskTypes>());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            tasks.AddRange(Enum.GetValues<TaskTypes>());
        }

        return tasks;
    }
}

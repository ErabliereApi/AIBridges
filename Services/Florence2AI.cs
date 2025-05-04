using System.ComponentModel;
using AIBridges.Attributes;
using AIBridges.Models;
using Florence2;

namespace AIBridges.Services;

[Version("1.0")]
[Description("Florence2 AI Service")]
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

        await fmd.DownloadModelsAsync(status => Console.WriteLine($"Download status: {status.Progress * 100} %"));

        _florence2Model = new Florence2Model(fmd);
    }

    public async Task<object> ProcessRequestAsync(AIBridgeRequest request, HttpRequest requestBody)
    {
        if (_florence2Model == null)
        {
            throw new InvalidOperationException("Florence2 model is not initialized. Call InitializeAsync() first.");
        }

        if (requestBody.Body == null)
        {
            throw new InvalidDataException("Request body content cannot be null.");
        }

        List<TaskTypes> tasks = GetFlorence2TaskTypes(request);

        var results = new List<FlorenceResults>(15);

        using var stream = new MemoryStream();

        await requestBody.Body.CopyToAsync(stream);

        int i = 1;
        foreach (var task in tasks)
        {
            Console.WriteLine($"Task {i++}: {task}");

            stream.Position = 0; // Reset the stream position for each task

            var singleResults = await Task.Run(() => _florence2Model.Run(task, [stream], textInput: "", CancellationToken.None));

            if (singleResults == null || singleResults.Length == 0)
            {
                Console.WriteLine($"No results produce for TaskTypes: {task}");
            }
            else
            {
                results.AddRange(singleResults);
            }
        }

        return results;
    }

    public static List<TaskTypes> GetFlorence2TaskTypes(AIBridgeRequest request)
    {
        var tasks = new List<TaskTypes>();

        try
        {
            var configTypes = request.Task?.Split(',');

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

namespace Keltusamor.PrivateAI;

public interface IPrivateAiClient
{
    Task<IList<ProcessTextResponse>> ProcessTextAsync(ProcessTextRequest request);

    Task<IList<string>> ReIdentifyTextAsync(ReIdentifyTextRequest request);
}

using System.Threading.Tasks;

public class EvaluateLikabilityUseCase
{
    private readonly ILikabilityEvaluator _likabilityEvaluator;

    public EvaluateLikabilityUseCase(ILikabilityEvaluator evaluator)
    {
        _likabilityEvaluator = evaluator;
    }

    public Task<int> ExecuteAsync(LikabilityEvaluationRequest request)
    {
        return _likabilityEvaluator.EvaluateAsync(request);
    }
}

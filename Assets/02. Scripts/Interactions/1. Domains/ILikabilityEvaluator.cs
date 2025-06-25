using System.Threading.Tasks;

public interface ILikabilityEvaluator
{
    Task<int> EvaluateAsync(LikabilityEvaluationRequest request);
}
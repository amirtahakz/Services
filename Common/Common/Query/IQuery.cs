using MediatR;
using Services.Common.Query.Filter;

namespace Services.Common.Query;

public interface IQuery<TResponse> : IRequest<TResponse> where TResponse : class?
{
}

public class QueryFilter<TResponse, TParam> : IQuery<TResponse>
    where TResponse : BaseFilter
    where TParam : BaseFilterParam
{
    public TParam FilterParams { get; set; }
    public QueryFilter(TParam filterParams)
    {
        FilterParams = filterParams;
    }
}
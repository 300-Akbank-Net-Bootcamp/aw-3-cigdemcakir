using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query;

public class EftTransactionQueryHandler:
    IRequestHandler<GetAllEftTransactionQuery, ApiResponse<List<EftTransactionResponse>>>,
    IRequestHandler<GetEftTransactionByIdQuery, ApiResponse<EftTransactionResponse>>,
    IRequestHandler<GetEftTransactionByParameterQuery, ApiResponse<List<EftTransactionResponse>>>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public EftTransactionQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetAllEftTransactionQuery request,
        CancellationToken cancellationToken)
    {
        var list = await dbContext.Set<EftTransaction>().ToListAsync(cancellationToken);
        
        var mappedList = mapper.Map<List<EftTransaction>, List<EftTransactionResponse>>(list);
         return new ApiResponse<List<EftTransactionResponse>>(mappedList);
    }

    public async Task<ApiResponse<EftTransactionResponse>> Handle(GetEftTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity =  await dbContext.Set<EftTransaction>()
            .FirstOrDefaultAsync(x => x.AccountId == request.Id, cancellationToken);

        if (entity == null)
        {
            return new ApiResponse<EftTransactionResponse>("Record not found");
        }
        
        var mapped = mapper.Map<EftTransaction, EftTransactionResponse>(entity);
        return new ApiResponse<EftTransactionResponse>(mapped);
    }

    public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetEftTransactionByParameterQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.EftTransactions.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(request.ReferenceNumber))
            query = query.Where(e => e.ReferenceNumber.Contains(request.ReferenceNumber));

        if (!string.IsNullOrWhiteSpace(request.Description))
            query = query.Where(e => e.Description.Contains(request.Description));

        if (!string.IsNullOrWhiteSpace(request.SenderAccount))
            query = query.Where(e => e.SenderAccount.Contains(request.SenderAccount));

        if (!string.IsNullOrWhiteSpace(request.SenderIban))
            query = query.Where(e => e.SenderIban.Contains(request.SenderIban));

        if (!string.IsNullOrWhiteSpace(request.SenderName))
            query = query.Where(e => e.SenderName.Contains(request.SenderName));

        var eftTransactions = await query.ToListAsync(cancellationToken);
        var response = mapper.Map<List<EftTransactionResponse>>(eftTransactions);
        return new ApiResponse<List<EftTransactionResponse>>(response);
    }
}
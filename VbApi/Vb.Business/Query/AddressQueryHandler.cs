using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query;

public class AddressQueryHandler :
    IRequestHandler<GetAllAddressQuery, ApiResponse<List<AddressResponse>>>,
    IRequestHandler<GetAddressByIdQuery, ApiResponse<AddressResponse>>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public AddressQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<AddressResponse>>> Handle(GetAllAddressQuery request,
        CancellationToken cancellationToken)
    {
        var list = await dbContext.Set<Address>()
            .Include(x => x.Customer).ToListAsync(cancellationToken);
        
        var mappedList = mapper.Map<List<Address>, List<AddressResponse>>(list);
         return new ApiResponse<List<AddressResponse>>(mappedList);
    }

    public async Task<ApiResponse<AddressResponse>> Handle(GetAddressByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity =  await dbContext.Set<Address>()
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return new ApiResponse<AddressResponse>("Record not found");
        }
        
        var mapped = mapper.Map<Address, AddressResponse>(entity);
        return new ApiResponse<AddressResponse>(mapped);
    }
    
    public async Task<ApiResponse<List<AddressResponse>>> Handle(GetAddressByParameterQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Addresses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Country))
            query = query.Where(a => a.Country.Contains(request.Country));

        if (!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(a => a.City.Contains(request.City));

        if (!string.IsNullOrWhiteSpace(request.County))
            query = query.Where(a => a.County.Contains(request.County));

        if (!string.IsNullOrWhiteSpace(request.PostalCode))
            query = query.Where(a => a.PostalCode == request.PostalCode);

        var addresses = await query.ToListAsync(cancellationToken);
        var response = mapper.Map<List<AddressResponse>>(addresses);
        return new ApiResponse<List<AddressResponse>>(response);
    }
}
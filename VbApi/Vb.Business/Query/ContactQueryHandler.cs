using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query;

public class ContactQueryHandler:
    IRequestHandler<GetAllContactQuery, ApiResponse<List<ContactResponse>>>,
    IRequestHandler<GetContactByIdQuery, ApiResponse<ContactResponse>>,
    IRequestHandler<GetContactByParameterQuery, ApiResponse<List<ContactResponse>>>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public ContactQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<ContactResponse>>> Handle(GetAllContactQuery request,
        CancellationToken cancellationToken)
    {
        var list = await dbContext.Set<Contact>().ToListAsync(cancellationToken);
        
        var mappedList = mapper.Map<List<Contact>, List<ContactResponse>>(list);
         return new ApiResponse<List<ContactResponse>>(mappedList);
    }

    public async Task<ApiResponse<ContactResponse>> Handle(GetContactByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity =  await dbContext.Set<Contact>()
            .FirstOrDefaultAsync(x => x.CustomerId == request.Id, cancellationToken);

        if (entity == null)
        {
            return new ApiResponse<ContactResponse>("Record not found");
        }
        
        var mapped = mapper.Map<Contact, ContactResponse>(entity);
        return new ApiResponse<ContactResponse>(mapped);
    }

    public async Task<ApiResponse<List<ContactResponse>>> Handle(GetContactByParameterQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Contacts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.ContactType))
            query = query.Where(c => c.ContactType.Contains(request.ContactType));

        if (!string.IsNullOrWhiteSpace(request.Information))
            query = query.Where(c => c.Information.Contains(request.Information));

        var contacts = await query.ToListAsync(cancellationToken);
        var response = mapper.Map<List<ContactResponse>>(contacts);
        return new ApiResponse<List<ContactResponse>>(response);
    }
}
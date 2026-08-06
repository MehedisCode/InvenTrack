namespace InvenTrack.Application.Features.Inventory.Queries.GetInventoryHistory;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Common.Models;
using InvenTrack.Application.Features.Inventory.DTOs;
using MediatR;

public class GetInventoryHistoryQuery : TransactionQueryParameters, IRequest<PaginatedList<StockTransactionDto>>
{
}

public class GetInventoryHistoryQueryHandler : IRequestHandler<GetInventoryHistoryQuery, PaginatedList<StockTransactionDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;

    public GetInventoryHistoryQueryHandler(IInventoryRepository inventoryRepository, IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<StockTransactionDto>> Handle(GetInventoryHistoryQuery request, CancellationToken cancellationToken)
    {
        var result = await _inventoryRepository.GetTransactionsAsync(request, cancellationToken);

        var dtos = _mapper.Map<List<StockTransactionDto>>(result.Items);

        return new PaginatedList<StockTransactionDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}

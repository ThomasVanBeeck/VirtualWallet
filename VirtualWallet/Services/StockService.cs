using VirtualWallet.Repositories;
using AutoMapper;
using VirtualWallet.DTOs;

namespace VirtualWallet.Services;

public class StockService
{
    private readonly StockRepository _stockRepository;
    private readonly IMapper _mapper;

    public StockService(StockRepository stockRepository, IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task<List<StockDTO>> GetAllStocks()
    {
        var stocks = await _stockRepository.GetAllAsync();
        return _mapper.Map<List<StockDTO>>(stocks);
    }
}
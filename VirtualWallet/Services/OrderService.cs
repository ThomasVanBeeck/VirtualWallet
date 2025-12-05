using AutoMapper;
using VirtualWallet.DTOs;
using VirtualWallet.Enums;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

public class OrderService
{
    private readonly OrderRepository _orderRepository;
    private readonly WalletService _walletService;
    private readonly HoldingService _holdingService;
    private readonly StockService _stockService;
    private readonly IMapper _mapper;

    public OrderService(OrderRepository orderRepository,
        StockService stockService,
        WalletService walletService,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _stockService = stockService;
        _walletService = walletService;
        _mapper = mapper;
    }

    public async Task AddOrderAsync(Guid userId, OrderPostDTO orderPostDTO)
    {
        var wallet = await _walletService.GetWalletByUserIdAsync(userId);
        
        if (wallet == null)
            throw new Exception("Wallet not found for user");
        
        var stock = await _stockService.GetStockByNameAsync(orderPostDTO.StockName);
        
        if (stock == null)
            throw new Exception("Stock not found");

        var holding = await _holdingService.GetHoldingByWalletIdAndStockIdAsync(stock.Id, wallet.Id);

        if (holding == null)
        {
            holding = await _holdingService.AddHoldingAsync(stock.Id, wallet.Id);
            if (holding == null)
                throw new Exception("Holding not found and unable to create holding.");
        }
        
        var entity = _mapper.Map<Order>(orderPostDTO);

        entity.Id = Guid.NewGuid();
        entity.HoldingId = holding.Id;
        
        await _orderRepository.AddAsync(entity);

        var total = orderPostDTO.Amount * orderPostDTO.Price;
        switch (orderPostDTO.Type)
        {
            case OrderType.Buy:
                wallet.TotalCash -= total;
                wallet.TotalInStocks += total;
                break;
            case OrderType.Sell:
                wallet.TotalCash += total;
                wallet.TotalInStocks -= total;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await _walletService.UpdateWalletAsync(wallet); 
    }
    
    
    
}
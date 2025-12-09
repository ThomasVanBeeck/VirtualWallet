using AutoMapper;
using VirtualWallet.Dtos;
using VirtualWallet.Enums;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;

namespace VirtualWallet.Services;

public class OrderService
{
    private readonly IHoldingRepository _holdingRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;

    public OrderService(IHoldingRepository holdingRepository, IOrderRepository orderRepository, IWalletRepository walletRepository,
        IStockRepository stockRepository,
        IMapper mapper)
    {
        _holdingRepository = holdingRepository;
        _orderRepository = orderRepository;
        _walletRepository = walletRepository;
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task AddOrderAsync(Guid userId, OrderPostDto orderPostDto)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        
        if (wallet == null)
            throw new Exception("Wallet not found for user");
        
        var stock = await _stockRepository.GetByNameAsync(orderPostDto.StockName);
        
        if (stock == null)
            throw new Exception("Stock not found");

        var holding = await _holdingRepository.GetByWalletAndStockAsync(stock.Id, wallet.Id);

        if (holding != null && orderPostDto.Type == OrderType.Sell)
        {
            var buys = holding.Orders.Where(o => o.Type == OrderType.Buy);
            var sells = holding.Orders.Where(o => o.Type == OrderType.Sell);
            var totalBought = buys.Sum(b => b.Amount);
            var totalSold = sells.Sum(s => s.Amount);
            var amount = totalBought - totalSold;
            if (amount < orderPostDto.Amount)
                throw new Exception("Insufficient stocks available to sell");
        }

        else if (holding == null)
        {
            if (holding == null && orderPostDto.Type == OrderType.Sell)
                throw new Exception("No stocks available to sell.");
            
            Holding newHolding = new Holding()
            {
                Id = Guid.NewGuid(),
                StockId = stock.Id,
                WalletId = wallet.Id
            };
            holding = await _holdingRepository.AddAsync(newHolding);
            if (holding == null)
                throw new Exception("Holding not found and unable to create holding.");
        }
        
        float totalValue = orderPostDto.Amount * orderPostDto.Price;
        if (orderPostDto.Type == OrderType.Buy)
        {
            if (totalValue > wallet.TotalCash)
                throw new Exception("Insufficient funds");
        }
        
        var entity = _mapper.Map<Order>(orderPostDto);

        entity.Id = Guid.NewGuid();
        entity.HoldingId = holding.Id;
        entity.WalletId = wallet.Id;
        var total = orderPostDto.Amount * orderPostDto.Price;
        
        entity.Total = total;
        await _orderRepository.AddAsync(entity);

        switch (orderPostDto.Type)
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

        await _walletRepository.UpdateAsync(wallet); 
    }

    public async Task<OrdersPaginatedDto> GetOrdersAsync(Guid userId, int page, int size)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
            throw new Exception("Wallet not found");
        
        var ordersPaginatedResult = await _orderRepository.GetOrdersByWalletIdAsync(wallet.Id, page, size);
        var itemsDto = _mapper.Map<List<OrderDto>>(ordersPaginatedResult.Items);
        var orderPage = new OrdersPaginatedDto
        {
            Orders = itemsDto,
            PageNumber = ordersPaginatedResult.CurrentPage,
            TotalPages = ordersPaginatedResult.TotalPages
        };
        return orderPage;
    }
}
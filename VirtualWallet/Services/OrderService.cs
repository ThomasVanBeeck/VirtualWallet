using AutoMapper;
using VirtualWallet.DTOs;
using VirtualWallet.Enums;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

public class OrderService
{
    private readonly HoldingRepository _holdingRepository;
    private readonly OrderRepository _orderRepository;
    private readonly WalletRepository _walletRepository;
    private readonly StockRepository _stockRepository;
    private readonly IMapper _mapper;

    public OrderService(HoldingRepository holdingRepository, OrderRepository orderRepository, WalletRepository walletRepository,
        StockRepository stockRepository,
        IMapper mapper)
    {
        _holdingRepository = holdingRepository;
        _orderRepository = orderRepository;
        _walletRepository = walletRepository;
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task AddOrderAsync(Guid userId, OrderPostDTO orderPostDTO)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        
        if (wallet == null)
            throw new Exception("Wallet not found for user");
        
        var stock = await _stockRepository.GetByNameAsync(orderPostDTO.StockName);
        
        if (stock == null)
            throw new Exception("Stock not found");

        var holding = await _holdingRepository.GetByWalletAndStockAsync(stock.Id, wallet.Id);

        if (holding != null && orderPostDTO.Type == OrderType.Sell)
        {
            var buys = holding.Orders.Where(o => o.Type == OrderType.Buy);
            var sells = holding.Orders.Where(o => o.Type == OrderType.Sell);
            var totalBought = buys.Sum(b => b.Amount);
            var totalSold = sells.Sum(s => s.Amount);
            var amount = totalBought - totalSold;
            if (amount < orderPostDTO.Amount)
                throw new Exception("Insufficient stocks available to sell");
        }

        else if (holding == null)
        {
            if (holding == null && orderPostDTO.Type == OrderType.Sell)
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
        
        float totalValue = orderPostDTO.Amount * orderPostDTO.Price;
        if (orderPostDTO.Type == OrderType.Buy)
        {
            if (totalValue > wallet.TotalCash)
                throw new Exception("Insufficient funds");
        }
        
        var entity = _mapper.Map<Order>(orderPostDTO);

        entity.Id = Guid.NewGuid();
        entity.HoldingId = holding.Id;
        entity.WalletId = wallet.Id;
        var total = orderPostDTO.Amount * orderPostDTO.Price;
        
        entity.Total = total;
        await _orderRepository.AddAsync(entity);

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

        await _walletRepository.UpdateAsync(wallet); 
    }

    public async Task<OrdersPaginatedDTO> GetOrdersAsync(Guid userId, int page, int size)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
            throw new Exception("Wallet not found");
        
        var ordersPaginatedResult = await _orderRepository.GetOrdersByWalletIdAsync(wallet.Id, page, size);
        var itemsDTO = _mapper.Map<List<OrderDTO>>(ordersPaginatedResult.Items);
        var orderPage = new OrdersPaginatedDTO
        {
            Orders = itemsDTO,
            PageNumber = ordersPaginatedResult.CurrentPage,
            TotalPages = ordersPaginatedResult.TotalPages
        };
        return orderPage;
    }
}
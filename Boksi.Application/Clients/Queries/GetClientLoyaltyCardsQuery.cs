using Boksi.Application.DTOs;
using Boksi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boksi.Application.Clients.Queries
{
    public class GetClientLoyaltyCardsQuery : IRequest<IEnumerable<ClientLoyaltyCardDto>>
    {
        public Guid ClientId { get; set; }
    }

    public class GetClientLoyaltyCardsQueryHandler : IRequestHandler<GetClientLoyaltyCardsQuery, IEnumerable<ClientLoyaltyCardDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetClientLoyaltyCardsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ClientLoyaltyCardDto>> Handle(GetClientLoyaltyCardsQuery request, CancellationToken cancellationToken)
        {
            var cards = await _dbContext.ClientLoyaltyCards
                .IgnoreQueryFilters()
                .Where(c => c.ClientId == request.ClientId)
                .ToListAsync(cancellationToken);

            if (!cards.Any()) return new List<ClientLoyaltyCardDto>();

            var salonIds = cards.Select(c => c.SalonId).Distinct().ToList();

            var salons = await _dbContext.Salons
                .IgnoreQueryFilters()
                .Where(s => salonIds.Contains(s.Identifier))
                .ToDictionaryAsync(s => s.Identifier, s => s.Name, cancellationToken);

            var result = cards.Select(c => new ClientLoyaltyCardDto
            {
                Id = c.Id,
                SalonId = c.SalonId,
                SalonName = salons.TryGetValue(c.SalonId, out var name) ? name : "Nieznany salon",
                CurrentPoints = c.CurrentPoints,
                TotalRewardsEarned = c.TotalRewardsEarned,
                LastVisitAt = c.LastVisitAt
            });

            return result;
        }
    }
}

using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using AkakceProject.Infrastructure.Contracts;
using MediatR;

namespace AkakceProject.Application.Mediators
{
    public class CampaignMediator
    {
        public class GetAllCampaignsQuery : IRequest<IEnumerable<Campaign>>
        {
        }

        public class GetCampaignQuery : IRequest<Campaign>
        {
            public int CampaignId { get; set; }
        }

        public class GetUserCampaignsQuery : IRequest<IEnumerable<Campaign>>
        {
            public int UserId { get; set; }
        }

        public class GetCampaignInfoQuery : IRequest<CampaignResponse>
        {
            public int UserId { get; set; }
            public int CampaignId { get; set; }
        }

        public class CreateCampaignQuery : IRequest<Campaign>
        {
            public CampaignForCreationDto Campaign { get; set; }
        }

        public class UpdateCampaignQuery : IRequest
        {
            public int CampaignId { get; set; }
            public CampaignForUpdateDto Campaign { get; set; }
        }

        public class DeleteCampaignQuery : IRequest
        {
            public int CampaignId { get; set; }
        }

        public class DeleteUserCampaignsQuery : IRequest
        {
            public int UserId { get; set; }
        }

        public class CampaignQueryHandler :
            IRequestHandler<GetAllCampaignsQuery, IEnumerable<Campaign>>,
            IRequestHandler<GetCampaignQuery, Campaign>,
            IRequestHandler<GetUserCampaignsQuery, IEnumerable<Campaign>>,
            IRequestHandler<GetCampaignInfoQuery, CampaignResponse>,
            IRequestHandler<CreateCampaignQuery, Campaign>,
            IRequestHandler<UpdateCampaignQuery>,
            IRequestHandler<DeleteCampaignQuery>,
            IRequestHandler<DeleteUserCampaignsQuery>
        {
            private readonly ICampaignRepository _campaignRepo;

            public CampaignQueryHandler(ICampaignRepository campaignRepo)
            {
                _campaignRepo = campaignRepo;
            }

            public async Task<IEnumerable<Campaign>> Handle(GetAllCampaignsQuery request, CancellationToken cancellationToken)
            {
                return await _campaignRepo.GetCampaigns();
            }

            public async Task<Campaign> Handle(GetCampaignQuery request, CancellationToken cancellationToken)
            {
                return await _campaignRepo.GetCampaign(request.CampaignId);
            }

            public async Task<IEnumerable<Campaign>> Handle(GetUserCampaignsQuery request, CancellationToken cancellationToken)
            {
                return await _campaignRepo.GetUserCampaigns(request.UserId);
            }

            public async Task<CampaignResponse> Handle(GetCampaignInfoQuery request, CancellationToken cancellationToken)
            {
                return await _campaignRepo.GetCampaignInfo(request.UserId, request.CampaignId);
            }

            public async Task<Campaign> Handle(CreateCampaignQuery request, CancellationToken cancellationToken)
            {
                return await _campaignRepo.CreateCampaign(request.Campaign);
            }

            public async Task Handle(UpdateCampaignQuery request, CancellationToken cancellationToken)
            {
                await _campaignRepo.UpdateCampaign(request.CampaignId, request.Campaign);
            }

            public async Task Handle(DeleteCampaignQuery request, CancellationToken cancellationToken)
            {
                await _campaignRepo.DeleteCampaign(request.CampaignId);
            }

            public async Task Handle(DeleteUserCampaignsQuery request, CancellationToken cancellationToken)
            {
                await _campaignRepo.DeleteUserCampaigns(request.UserId);
            }
        }
    }
}

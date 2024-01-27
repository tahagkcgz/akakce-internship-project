using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using AkakceProject.Infrastructure.Contracts;
using MediatR;

namespace AkakceProject.Application.Mediators
{
    public class ProductMediator
    {
        public class GetAllProductsQuery : IRequest<IEnumerable<Product>>
        {
        }

        public class GetProductQuery : IRequest<Product>
        {
            public int ProductId { get; set; }
        }

        public class GetUserProductsQuery : IRequest<IEnumerable<Product>>
        {
            public int UserId { get; set; }
        }

        public class GetCampaignProductsQuery : IRequest<IEnumerable<Product>>
        {
            public int CampaignId { get; set; }
        }

        public class GetProductInfoQuery : IRequest<ProductResponse>
        {
            public int UserId { get; set; }
            public int ProductId { get; set; }
        }

        public class CreateProductQuery : IRequest<Product>
        {
            public ProductForCreationDto Product { get; set; }
        }

        public class UpdateProductQuery : IRequest
        {
            public int ProductId { get; set; }
            public ProductForUpdateDto Product { get; set; }
        }

        public class DeleteProductQuery : IRequest
        {
            public int ProductId { get; set; }
        }

        public class DeleteUserProductsQuery : IRequest
        {
            public int UserId { get; set; }
        }

        public class DeleteCampaignProductsQuery : IRequest
        {
            public int CampaignId { get; set; }
        }

        public class ProductQueryHandler :
            IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>,
            IRequestHandler<GetProductQuery, Product>,
            IRequestHandler<GetUserProductsQuery, IEnumerable<Product>>,
            IRequestHandler<GetCampaignProductsQuery, IEnumerable<Product>>,
            IRequestHandler<GetProductInfoQuery, ProductResponse>,
            IRequestHandler<CreateProductQuery, Product>,
            IRequestHandler<UpdateProductQuery>,
            IRequestHandler<DeleteProductQuery>,
            IRequestHandler<DeleteUserProductsQuery>,
            IRequestHandler<DeleteCampaignProductsQuery>
        {
            private readonly IProductRepository _productRepo;

            public ProductQueryHandler(IProductRepository productRepo)
            {
                _productRepo = productRepo;
            }

            public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
            {
                return await _productRepo.GetProducts();
            }

            public async Task<Product> Handle(GetProductQuery request, CancellationToken cancellationToken)
            {
                return await _productRepo.GetProduct(request.ProductId);
            }

            public async Task<IEnumerable<Product>> Handle(GetUserProductsQuery request, CancellationToken cancellationToken)
            {
                return await _productRepo.GetUserProducts(request.UserId);
            }

            public async Task<IEnumerable<Product>> Handle(GetCampaignProductsQuery request, CancellationToken cancellationToken)
            {
                return await _productRepo.GetCampaignProducts(request.CampaignId);
            }

            public async Task<ProductResponse> Handle(GetProductInfoQuery request, CancellationToken cancellationToken)
            {
                return await _productRepo.GetProductInfo(request.UserId, request.ProductId);
            }

            public async Task<Product> Handle(CreateProductQuery request, CancellationToken cancellationToken)
            {
                return await _productRepo.CreateProduct(request.Product);
            }

            public async Task Handle(UpdateProductQuery request, CancellationToken cancellationToken)
            {
                await _productRepo.UpdateProduct(request.ProductId, request.Product);
            }

            public async Task Handle(DeleteProductQuery request, CancellationToken cancellationToken)
            {
                await _productRepo.DeleteProduct(request.ProductId);
            }

            public async Task Handle(DeleteUserProductsQuery request, CancellationToken cancellationToken)
            {
                await _productRepo.DeleteUserProducts(request.UserId);
            }

            public async Task Handle(DeleteCampaignProductsQuery request, CancellationToken cancellationToken)
            {
                await _productRepo.DeleteCampaignProducts(request.CampaignId);
            }
        }
    }
}

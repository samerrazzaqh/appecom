using AutoMapper;
using Core.Entities;
using Core.interfaces;
using Core.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Infrastructure.Repositries
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageManagementService _imageManagementService;
        private readonly IConnectionMultiplexer redis;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailService emailService;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IGenerateToken token;
        private readonly IConfiguration _configuration;
        public ICategoryRepositry CategoryRepositry { get; }

        public IPhotoRepositry PhotoRepositry { get; }

        public IProductRepositry productRepositry { get; }

        public ICustomerBasketRepositry CustomerBasket { get; }

        public IAuth Auth { get; }
        public IAdministration Administration { get; }

        public UnitOfWork(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService,

            IConnectionMultiplexer redis, UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken token,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _imageManagementService = imageManagementService;
            this.redis = redis;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.token = token;
            _configuration = configuration;
            CategoryRepositry = new CategoryRepositry(_context);
            PhotoRepositry = new PhotoRepositry(_context);
            productRepositry = new ProductRepositry(_context, _mapper, _imageManagementService);
            CustomerBasket = new CustomerBasketRepositry(redis);
            Auth = new AuthRepositry(userManager,emailService, signInManager, token, context,configuration);
            Administration = new AdministrationRepositry(userManager,roleManager, context);
        }
    }
}

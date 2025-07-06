using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using TP4SCS.API.Middleware;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Repositories;
using TP4SCS.Library.Services;
using TP4SCS.Library.Utils.Healpers;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Implements;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Implements;
using TP4SCS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.DefaultBufferSize = 32 * 1024;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();



#region Authentication UI For Swagger
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1"
    });

    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
#endregion

#region DBContext
builder.Services.AddDbContext<Tp4scsDevDatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
#endregion

//Word Blacklist Path
var wordFilePath = Path.Combine(builder.Environment.ContentRootPath, "WordBlacklist.json");


//Inject Repo
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IAssetUrlRepository, AssetUrlRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<ITicketCategoryRepository, TicketCategoryRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IPlatformPackRepository, PlatformPackRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IProcessRepository, ProcessRepository>();
builder.Services.AddScoped<IOrderNotificationRepository, OrderNotificationRepository>();
builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
builder.Services.AddScoped<IPackSubscriptionRepository, PackSubscriptionRepository>();
builder.Services.AddScoped<IBusinessStatisticRepository, BusinessStatisticRepository>();
builder.Services.AddScoped<IPlatformStatisticRepository, PlatformStatisticRepository>();

//Inject Service
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IBusinessBranchService, BusinessBranchService>();
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAssetUrlService, AssetUrlService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ITicketCategoryService, TicketCategoryService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<IPlatformPackService, PlatformPackService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IMoMoService, MoMoService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddScoped<IOrderNotificationService, OrderNotificationService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IBusinessStatisticService, BusinessStatisticService>();
builder.Services.AddScoped<IPlatformStatisticService, PlatformStatisticService>();
builder.Services.AddScoped<IWordBlacklistService>(provider => new WordBlacklistService(wordFilePath));
builder.Services.AddScoped<IChatService, ChatService>();

//Inject Util
builder.Services.AddTransient<Util>();

//Add HttpClient
builder.Services.AddHttpClient();

//Add MoMo HttpClient
builder.Services.AddHttpClient("MoMoClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MoMo:MomoApiUrl"]!);
    client.DefaultRequestHeaders.Add("Content-Type", "application/json; charset=UTF-8");
});

//Get EmailSettings
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailSettings"));

//Add Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Read File Config
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

#region OpenAI
builder.Services.AddHttpClient("ChatGPT", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
});
#endregion

#region Mapster
//Configure Mapster
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(AppDomain.CurrentDomain.GetAssemblies());

//Register Mapster Service 
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
#endregion

#region Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
#endregion

#region Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("ADMIN"));
    options.AddPolicy("Moderator", policy => policy.RequireRole("ADMIN", "MODERATOR"));
    options.AddPolicy("Customer", policy => policy.RequireRole("ADMIN", "CUSTOMER", "OWNER"));
    options.AddPolicy("Owner", policy => policy.RequireRole("ADMIN", "OWNER"));
    options.AddPolicy("Employee", policy => policy.RequireRole("ADMIN", "OWNER", "EMPLOYEE"));
});
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});
#endregion

#region Model State Error Response
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(apiBehaviorOptions =>
    {
        apiBehaviorOptions.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new
            {
                status = "error",
                statusCode = "400",
                message = "Input Validation Erorr!",
                errors = errors
            };

            var result = new ContentResult
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ContentType = "application/json",
                Content = JsonSerializer.Serialize(response)
            };

            return result;
        };
    });
#endregion

#region Rate Limiting
builder.Services.AddRateLimiter(options => options.AddFixedWindowLimiter(policyName: "BasePolicy", options =>
{
    options.PermitLimit = 20;
    options.Window = TimeSpan.FromSeconds(30);
    options.QueueLimit = 5;
    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
}));
#endregion

#region Logger
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TP4SCS"));
}

app.UseSwagger();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TP4SCS"));

app.UseMiddleware<ResponseMiddleware>();

app.UseHttpsRedirection();

app.UseCors("MyAllowSpecificOrigins");

app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



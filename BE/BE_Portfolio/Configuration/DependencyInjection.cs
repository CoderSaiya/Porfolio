using BE_Portfolio.Models.Commons;
using BE_Portfolio.Models.Specification;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using BE_Portfolio.Services;
using BE_Portfolio.Services.Auth;
using BE_Portfolio.Services.Background;
using BE_Portfolio.Services.Comment;
using BE_Portfolio.Services.Common;
using BE_Portfolio.Services.Interfaces;
using BE_Portfolio.Services.TwoFactor;

namespace BE_Portfolio.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MongoDbSettings>(config.GetSection("MongoDBSettings"));
        services.Configure<RabbitMqSettings>(config.GetSection("RabbitMqSettings"));
        services.Configure<MailSettings>(config.GetSection("MailSettings"));
        
        // Add JWT and Cookie settings as singletons
        var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
        var cookieSettings = config.GetSection("CookieSettings").Get<CookieSettings>();
        services.AddSingleton(jwtSettings!);
        services.AddSingleton(cookieSettings!);
        
        return services;
    }
    

    public static IServiceCollection AddMongoDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        services.AddScoped<IBlogCategoryRepository, BlogCategoryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        
        // Services
        services.AddScoped<PortfolioService>();
        services.AddScoped<ContactService>();
        
        // Authentication services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IDashboardService, DashboardService>();
        
        // Email services
        services.AddSingleton<IMailSender, EmailSender>();
        services.AddSingleton<IEmailQueue, RabbitMqEmailPublisher>();
        
        // Background services
        services.AddHostedService<RabbitMqListener>();
        
        // Domain Services
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ContactService>();
        
        return services;
    }
}
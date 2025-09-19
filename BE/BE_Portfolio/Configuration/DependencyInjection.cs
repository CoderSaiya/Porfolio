using BE_Portfolio.Models.Commons;
using BE_Portfolio.Models.Specification;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using BE_Portfolio.Services;
using BE_Portfolio.Services.Background;
using BE_Portfolio.Services.Interfaces;

namespace BE_Portfolio.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MongoDbSettings>(config.GetSection("MongoDBSettings"));
        services.Configure<RabbitMqSettings>(config.GetSection("RabbitMqSettings"));
        services.Configure<MailSettings>(config.GetSection("MailSettings"));
        
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

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        
        services.AddScoped<PortfolioService>();
        services.AddScoped<ContactService>();
        
        services.AddSingleton<IMailSender, EmailSender>();
        services.AddSingleton<IEmailQueue, RabbitMqEmailPublisher>();
        
        services.AddHostedService<RabbitMqListener>();
        
        return services;
    }
}
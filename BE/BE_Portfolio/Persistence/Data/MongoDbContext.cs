using BE_Portfolio.Models.Commons;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Data;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _db;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _db = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<Profile> Profiles
        => _db.GetCollection<Profile>(CollectionNames.Profile);

    public IMongoCollection<Project> Projects
        => _db.GetCollection<Project>(CollectionNames.Projects);

    public IMongoCollection<SkillCategory> SkillCategories
        => _db.GetCollection<SkillCategory>(CollectionNames.SkillCategories);

    public IMongoCollection<ContactMessage> ContactMessages
        => _db.GetCollection<ContactMessage>(CollectionNames.ContactMessages);

    public IMongoCollection<Image> Images 
        => _db.GetCollection<Image>(CollectionNames.Images);

    public IMongoCollection<User> Users
        => _db.GetCollection<User>(CollectionNames.Users);

    public async Task EnsureSeedAsync(CancellationToken ct = default)
    {
        // projects: slug unique, createdAt, highlight
        var projects = _db.GetCollection<Project>(CollectionNames.Projects);
        var projectIndexes = new List<CreateIndexModel<Project>>
        {
            new CreateIndexModel<Project>(
                Builders<Project>.IndexKeys.Ascending(x => x.Slug),
                new CreateIndexOptions { Unique = true, Name = "ux_slug" }),
            new CreateIndexModel<Project>(
                Builders<Project>.IndexKeys.Descending(x => x.CreatedAt),
                new CreateIndexOptions { Name = "ix_createdAt" }),
            new CreateIndexModel<Project>(
                Builders<Project>.IndexKeys.Ascending(x => x.Highlight),
                new CreateIndexOptions { Name = "ix_highlight" }),
        };
        await projects.Indexes.CreateManyAsync(projectIndexes, ct);

        // skill_categories: order
        var skills = _db.GetCollection<SkillCategory>(CollectionNames.SkillCategories);
        var skillIndexes = new List<CreateIndexModel<SkillCategory>>
        {
            
        };
        await skills.Indexes.CreateOneAsync(
            new CreateIndexModel<SkillCategory>(
                Builders<SkillCategory>.IndexKeys.Ascending(x => x.Order),
                new CreateIndexOptions { Name = "ix_order" }),
            cancellationToken: ct);

        // contact_messages: status, createdAt
        var msgs = _db.GetCollection<ContactMessage>(CollectionNames.ContactMessages);
        await msgs.Indexes.CreateManyAsync([
            new CreateIndexModel<ContactMessage>(
                Builders<ContactMessage>.IndexKeys.Descending(x => x.CreatedAt),
                new CreateIndexOptions { Name = "ix_createdAt" }),
            new CreateIndexModel<ContactMessage>(
                Builders<ContactMessage>.IndexKeys.Ascending(x => x.Status),
                new CreateIndexOptions { Name = "ix_status" })
        ], ct);
        
        // image: owner
        var imgs = _db.GetCollection<Image>(CollectionNames.Images);
        await imgs.Indexes.CreateOneAsync(
            new CreateIndexModel<Image>(
                Builders<Image>.IndexKeys
                    .Ascending(x => x.OwnerType)
                    .Ascending(x => x.OwnerId)
                    .Ascending(x => x.Variant),
                new CreateIndexOptions { Name = "ux_owner_variant", Unique = true }),
            cancellationToken: ct);
        
        var users = _db.GetCollection<User>(CollectionNames.Users);
        var userIndexes = new List<CreateIndexModel<User>>
        {
            
        };
    }
    
    [Obsolete("Obsolete")]
    public async Task EnsureIndexesAsync(CancellationToken ct = default)
    {
        var profiles = _db.GetCollection<Profile>(CollectionNames.Profile);
        if (await profiles.CountDocumentsAsync(_ => true, cancellationToken: ct) == 0)
        {
            await profiles.InsertOneAsync(new Profile
            {
                FullName = "Nhật Cường",
                Headline = ".NET Backend Developer",
                Location = "Hồ Chí Minh, VN",
                YearsExperience = 5,
                ProjectsCompleted = 50,
                Coffees = 2000,
                About = "Backend Developer đam mê hệ thống mạnh mẽ và mở rộng.",
                SocialLinks =
                [
                    new SocialLink { Platform = SocialPlatform.Github, Url = "https://github.com/CoderSaiya", Order = 1 },
                    new SocialLink { Platform = SocialPlatform.Linkedin, Url = "https://www.linkedin.com/in/nhat-cuong", Order = 2 },
                    new SocialLink
                        { Platform = SocialPlatform.Email, Url = "mailto:nhatcuongdev.contacts@gmail.com", Order = 3 }
                ]
            }, ct);
        }

        var projects = _db.GetCollection<Project>(CollectionNames.Projects);
        var p1Id = ObjectId.GenerateNewId();
        var p2Id = ObjectId.GenerateNewId();
        var p3Id = ObjectId.GenerateNewId();
        var p4Id = ObjectId.GenerateNewId();
        var p5Id = ObjectId.GenerateNewId();
        var p6Id = ObjectId.GenerateNewId();
        var p7Id = ObjectId.GenerateNewId();
        if (await projects.CountDocumentsAsync(_ => true, cancellationToken: ct) == 0)
        {
            await projects.InsertManyAsync([
                new Project 
                {
                    Id = p1Id,
                    Title = "Mobile Shop", Slug = "mobile-shop",
                    Description = "App winform dùng nội bộ để quản lý 1 store bán hàng về điện thoại",
                    Duration = 2, TeamSize = 3,
                    ImageUrl = ThumbRoute(p1Id), Github = "https://github.com/CoderSaiya/mobile-shop-winform", Demo = "#",
                    Technologies = ["Window Form", "ADO.NET", "SQL Server"],
                    Features = ["Quản lý nhập/xuất kho", "Tính tiền & in hoá đơn", "Báo cáo doanh thu"]
                },
                new Project 
                {
                    Id = p2Id,
                    Title = "Toy Shop", Slug = "toy-shop",
                    Description = "Nền tảng web thương mại điện tử về đồ chơi trẻ em.",
                    Duration = 2, TeamSize = 2,
                    ImageUrl = ThumbRoute(p2Id), Github = "https://github.com/CoderSaiya/ToyShop", Demo = "#",
                    Technologies = ["PHP", "XAMPP", "MySQL", "jQuery", "Bootstrap"],
                    Features = ["Giỏ hàng", "Đặt hàng & theo dõi đơn", "Quản trị sản phẩm/đơn hàng",
                        "Phân trang & tìm kiếm", "Thanh toán MoMo QR"]
                },
                new Project 
                {
                    Id = p3Id,
                    Title = "Finance Calculator Bot", Slug = "finance-calculator-bot",
                    Description = "Bot Telegram theo dõi chi tiêu và ghi lại vào file excel.",
                    Duration = 1, TeamSize = 1,
                    ImageUrl = ThumbRoute(p3Id), Github = "https://github.com/CoderSaiya/FinanceCaculationBot", Demo = "#",
                    Technologies = [".NET Worker Service", "Telegram.Bot", "EPPlus (Excel)"],
                    Features = ["Ghi chi tiêu theo tag", "Xuất Excel chia sẻ nhanh"]
                },
                new Project 
                {
                    Id = p4Id,
                    Title = "Freelance Marketplace", Slug = "freelance-marketplace",
                    Description = "Nền tảng kết nối freelancer và nhà tuyển dụng: đăng/nhận job, chat realtime," +
                                  " thanh toán, đánh giá/nhận xét sau dự án.",
                    Highlight = true, Duration = 3, TeamSize = 5,
                    ImageUrl = ThumbRoute(p4Id), Github = "https://github.com/CoderSaiya/Freelance-Marketplace_XDPMHDT", Demo = "#",
                    Technologies = ["ASP.NET Web API", "EF Core", "Identity Framework", "OAuth2.0", 
                        "JWT + Refresh Tokens", "SignalR", "Google Cloud (Storage/Messaging)",
                        "React.Js", "Redux Toolkit", "RTK Query", "Tailwind CSS"],
                    Features = ["Realtime chat & cập nhật wallet", "Đăng nhập bằng Oauth2.0", "Thanh toán Stripe",
                        "Quản lý milestone & escrow", "Đánh giá/nhận xét sau dự án", "Thông báo đẩy"],
                },
                new Project 
                {
                    Id = p5Id,
                    Title = "VibeTunes", Slug = "vibe-tunes",
                    Description = "Một ứng dụng mobile nghe nhạc.",
                    Highlight = true, Duration = 3, TeamSize = 1,
                    ImageUrl = ThumbRoute(p5Id), Github = "https://github.com/CoderSaiya/VibeTunes", Demo = "#",
                    Technologies = ["ASP.NET Core Web API", "EF Core", "SQL Server", "AWS", "React Native", "Tailwind CSS", "Redux"],
                    Features = ["Streaming HLS", "Playlist/Like/History", "Tìm kiếm full-text", "Gợi ý theo hành vi",
                        "Pre-signed URL bảo vệ nội dung"],
                },
                new Project 
                {
                    Id = p6Id,
                    Title = "MediSchedule", Slug = "medi-schedule",
                    Description = "Ứng dụng cho phép bệnh nhân có thể đặt lịch khám bệnh trực tuyến tại các bệnh viện liên kết.",
                    Highlight = true, Duration = 2, TeamSize = 5,
                    ImageUrl = ThumbRoute(p6Id), Github = "https://github.com/CoderSaiya/MediSchedule", Demo = "https://medischedule.nhatcuong.io.vn",
                    Technologies = ["ASP.NET Core Web API", "EF Core", "SQL Server", "MediatR", "JWT + Refresh Tokens", "MailKit",
                        "SignalR", "Azure Blob Storage", "Next.JS", "Ant Design", "shadcn/ui", "RTK Query", "Tailwind CSS"],
                    Features = ["Đặt lịch & thanh toán", "Quản lý hồ sơ bệnh nhân", "Phân quyền bác sĩ/admin",
                        "Quét mã QR để khám"],
                },
                new Project 
                {
                    Id = p7Id,
                    Title = "BookShop", Slug = "book-shop",
                    Description = "Một ứng dụng web hiện đại cho việc quản lý cửa hàng sách trực tuyến, " +
                                  "được xây dựng với kiến trúc Clean Architecture và công nghệ tiên tiến.",
                    Highlight = true, Duration = 2, TeamSize = 1,
                    ImageUrl = ThumbRoute(p7Id), Github = "https://github.com/CoderSaiya/book-shop", Demo = "https://bookshop.nhatcuong.io.vn",
                    Technologies = ["Angular + SCSS", ".NET 9 Web API", "EF Core", "SQL Server", "OAuth2.0", "JWT + Refresh Tokens",
                        "RabbitMQ", "MailKit", "SignalR", "Azure Translator", "Hugging Face Tranformers"],
                    Features = ["Giỏ hàng/Checkout", "Quản trị tồn kho/đơn hàng", "Bộ lọc & tìm kiếm", "Mã giảm giá",
                        "Thanh toán Momo & VNPay", "Gửi mail bất đồng bộ", "Cache bản dịch", "Hỗ trợ song ngữ Anh-Việt",
                        "Hỗ trợ theme sáng tối", "fine-tune model phân loại ý định người dùng"],
                },
            ], cancellationToken: ct);
        }

        var cats = _db.GetCollection<SkillCategory>(CollectionNames.SkillCategories);
        if (await cats.CountDocumentsAsync(_ => true, cancellationToken: ct) == 0)
        {
            await cats.InsertManyAsync([
                new SkillCategory
                {
                    Title = "Backend Development", Icon = "Server", Color = "#3b82f6", Order = 1,
                    Skills =
                    [
                        new SkillItem { Name = ".NET Core", Level = 95, Order = 1 },
                        new SkillItem { Name = "C#", Level = 90, Order = 2 },
                        new SkillItem { Name = "Web API", Level = 92, Order = 3 },
                        new SkillItem { Name = "SignalR", Level = 85, Order = 4 }
                    ]
                },
                new SkillCategory
                {
                    Title = "Database & ORM", Icon = "Database", Color = "#22c55e", Order = 2,
                    Skills =
                    [
                        new SkillItem { Name = "Entity Framework", Level = 90, Order = 1 },
                        new SkillItem { Name = "Microsoft SQL Server", Level = 82, Order = 2 },
                        new SkillItem { Name = "MySQL", Level = 75, Order = 3 },
                        new SkillItem { Name = "PostgreSQL", Level = 70, Order = 4 },
                        new SkillItem { Name = "Redis", Level = 78, Order = 5 }
                    ]
                },
                new SkillCategory
                {
                    Title = "Cloud & DevOps", Icon = "Cloud", Color = "#a855f7", Order = 3,
                    Skills =
                    [
                        new SkillItem { Name = "Azure", Level = 85, Order = 1 },
                        new SkillItem { Name = "GCP", Level = 72, Order = 2 },
                        new SkillItem { Name = "AWS", Level = 70, Order = 3 },
                        new SkillItem { Name = "Docker", Level = 80, Order = 4 },
                        new SkillItem { Name = "CI/CD", Level = 82, Order = 5 }
                    ]
                },
                new SkillCategory
                {
                    Title = "Architecture & Patterns", Icon = "Code", Color = "#fb923c", Order = 4,
                    Skills =
                    [
                        new SkillItem { Name = "Clean Architecture", Level = 88, Order = 1 },
                        new SkillItem { Name = "CQRS", Level = 85, Order = 2 },
                        new SkillItem { Name = "Microservices", Level = 60, Order = 3 }
                    ]
                },
                new SkillCategory
                {
                    Title = "Tools & Testing", Icon = "Wrench", Color = "#ef4444", Order = 5,
                    Skills =
                    [
                        new SkillItem { Name = "xUnit", Level = 65, Order = 1 },
                        new SkillItem { Name = "Postman", Level = 90, Order = 2 },
                        new SkillItem { Name = "Swagger", Level = 88, Order = 3 }
                    ]
                },
                new SkillCategory
                {
                    Title = "Version Control", Icon = "GitBranch", Color = "#ef4444", Order = 6,
                    Skills =
                    [
                        new SkillItem { Name = "Git", Level = 92, Order = 1 },
                        new SkillItem { Name = "GitHub", Level = 88, Order = 2 },
                        new SkillItem { Name = "Azure DevOps", Level = 85, Order = 3 }
                    ]
                },
            ], cancellationToken: ct);
        }

        var users = _db.GetCollection<User>(CollectionNames.Users);
        if (await users.CountDocumentsAsync(_ => true, cancellationToken: ct) == 0)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin", 12);
            
            await users.InsertOneAsync(new User
            {
                Username = "admin",
                Email = "admin@gmail.com",
                PasswordHash = hashedPassword,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                RefreshToken = null,
                RefreshTokenExpiryTime = null,
                TwoFactorEnabled = false,
                TwoFactorSecret = null,
                RecoveryCodes = new List<string>()
            }, cancellationToken: ct);
        }
    }

    private static string ThumbRoute(ObjectId id) => $"/api/portfolio/projects/{id}/image/thumb";
    private static string FullRoute (ObjectId id) => $"/api/portfolio/projects/{id}/image/full";
}
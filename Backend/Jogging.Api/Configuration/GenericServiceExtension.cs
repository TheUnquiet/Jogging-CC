using Jogging.Domain.DomainManagers;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Interfaces.ServiceInterfaces;
using Jogging.Domain.Models;
using Jogging.Domain.Services;
using Jogging.Infrastructure2.Repositories.MySqlRepositories;

namespace Jogging.Api.Configuration
{
    public static class GenericServiceExtension
    {
        public static void AddInterfaces(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationRepo, AuthRepo>();
            services.AddScoped<IPersonRepo, PersonRepo>();
            services.AddScoped<IProfileRepo, ProfileRepo>();
            services.AddScoped<ICompetitionRepo, CompetitionRepo>();
            services.AddScoped<IGenericRepo<AddressDom>, AddressRepo>();
            services.AddScoped<IGenericRepo<SchoolDom>, SchoolRepo>();
            services.AddScoped<IAgeCategoryRepo, AgeCategoryRepo>();
            services.AddScoped<ICompetitionPerCategoryRepo, CompetitionPerCategoryRepo>();
            services.AddScoped<IRegistrationRepo, RegistrationRepo>();
            services.AddScoped<IResultRepo, ResultRepo>();
            
            services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
            services.AddScoped<ICookieService, CookieService>();

        }

        public static void AddDomainManagerServices(this IServiceCollection services)
        {
            services.AddScoped<PersonManager>();
            services.AddScoped<AuthManager>();
            services.AddScoped<CompetitionManager>();
            services.AddScoped<RankingManager>();
            services.AddScoped<SchoolManager>();
            services.AddScoped<RegistrationManager>();
            services.AddScoped<ResultManager>();
            services.AddScoped<PaymentManager>();
            services.AddScoped<EmailManager>();
            services.AddScoped<ProfileManager>();
        }

        public static void AddHelperServices(this IServiceCollection services)
        {
            services.AddScoped<CachingHelper>();
            services.AddSingleton<CustomMemoryCache>();
        }
    }
}
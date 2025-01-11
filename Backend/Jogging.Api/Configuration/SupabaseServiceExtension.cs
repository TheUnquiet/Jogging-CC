//using Jogging.Domain.Configuration;
//using Supabase;

//namespace Jogging.Api.Configuration
//{
//    public static class SupabaseServiceExtension
//    {
//        public static void AddSupabase(this IServiceCollection services, IConfiguration configuration)
//        {
//            var supabaseConfiguration = configuration.GetSection("Supabase").Get<SupabaseConfiguration>();
//            if (supabaseConfiguration == null)
//            {
//                throw new InvalidOperationException("Supabase configuration is missing.");
//            }

//            services.AddScoped(_ =>
//            {
//                try
//                {
//                    return new Client(supabaseConfiguration.SupabaseUrl, supabaseConfiguration.SupabaseKey);
//                }
//                catch (Exception ex)
//                {
//                    var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
//                    var logger = loggerFactory.CreateLogger<Program>();
//                    logger.LogError(ex, "Failed to establish connection to Supabase.");
//                    throw;
//                }
//            });
//        }
//    }
//}
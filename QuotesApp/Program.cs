using Quotes.Repository;

namespace QuotesApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IQuotesRepository, QuotesRepository>();

            var app = builder.Build();
           
            var serviceProvider = new ServiceCollection()
                 .AddScoped<IQuotesRepository, QuotesRepository>()
                 .BuildServiceProvider();

            var s = serviceProvider.GetService<IQuotesRepository>();
            s.Init();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

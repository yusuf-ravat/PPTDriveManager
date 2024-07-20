using Microsoft.EntityFrameworkCore;
using PPTApi.Model;

namespace PPTApi
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

            builder.Services.AddDbContext<PptDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

            var app = builder.Build();
            app.UseCors(policy => policy.AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .SetIsOriginAllowed(origin => true)
                                        .AllowCredentials());


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

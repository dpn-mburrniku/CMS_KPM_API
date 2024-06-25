using Entities.Models;
using CMS.WebAPI;
using Contracts.IServices;
using Entities;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;
using System.Text.Json.Serialization;
using CMS.WebAPI.InternalServices;
using CMS.WebAPI.ExternalServices;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

if (Convert.ToBoolean(builder.Configuration["CorsPolicy:WithHeader"].ToString()))
{
    builder.Services.AddControllers(options =>
    options.Filters.Add<AutentifikimiApi>());
}
else
{
    builder.Services.AddControllers();
}
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IAirQualityServices, AirQualityServices>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMvc()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CMS_API",
        Version = "v2.0"
    });
});
string corsList = builder.Configuration["CorsPolicy:List"];
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins(corsList.Split(";")).AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});

builder.Services.AddDbContext<CmsContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"], x => x.UseNetTopologySuite());
});
// Add Identity Framework Core..
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (Convert.ToBoolean(builder.Configuration["CorsPolicy:ShowSwager"].ToString()))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();


//app.UseStaticFiles();
//app.UseRouting();
app.UseCors("corsapp");

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

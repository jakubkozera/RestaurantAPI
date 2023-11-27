using RestaurantAPI.Exceptions;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("RestaurantAPI.IntegrationTests")]
var builder = WebApplication.CreateBuilder(args);

#region

var authenticationSettings = new AuthenticationSettings();

builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
var services = builder.Services;

services.AddSingleton(authenticationSettings);
services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
    };
});
services.AddAuthorization(options =>
{
    options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish"));
    options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
    options.AddPolicy("CreatedAtleast2Restaurants",
        builder => builder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2)));
});

services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();
services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
services.AddControllers().AddFluentValidation();

services.AddScoped<RestaurantSeeder>();
services.AddAutoMapper(Assembly.GetExecutingAssembly());
services.AddScoped<IRestaurantService, RestaurantService>();
services.AddScoped<IDishService, DishService>();
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
services.AddScoped<RequestTimeMiddleware>();
services.AddScoped<IUserContextService, UserContextService>();
services.AddHttpContextAccessor();
services.AddSwaggerGen();
services.AddCors(options =>
{
    options.AddPolicy("FrontEndClient", corsBuilder =>

        corsBuilder.AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins(builder.Configuration["AllowedOrigins"])

        );
});

services.AddDbContext<RestaurantDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantDbConnection")));

#endregion

//services.AddScoped<ErrorHandlingMiddleware>();
services.AddExceptionHandler<GeneralExceptionHandler>();

services.AddExceptionHandler<AppExceptionHandler>();

var app = builder.Build();
//app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseExceptionHandler(_ => { });



app.MapGet("/throw", (_) => throw new Exception());
app.MapGet("/throwNotFound", (_) => throw new NotFoundException(".."));


#region

//var scope = app.Services.CreateScope();
//var seeder = scope.ServiceProvider.GetService<RestaurantSeeder>();

//app.UseResponseCaching();
//app.UseStaticFiles();
//app.UseCors("FrontEndClient");
//seeder.Seed();
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}


//app.UseMiddleware<RequestTimeMiddleware>();
//app.UseAuthentication();
//app.UseHttpsRedirection();

//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
//});

//app.UseRouting();
//app.UseAuthorization();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});
#endregion
app.Run();
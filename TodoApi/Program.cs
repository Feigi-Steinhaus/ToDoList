using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TodoApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//cors התקנת 
builder.Services.AddCors(option => option.AddPolicy("AllowAll",
    builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    }
    ));

//service-הזרקה של ה
builder.Services.AddDbContext<ToDoDbContext>();
//builder.Services.AddSingleton<ToDoDbContext>();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseCors("AllowAll");

//----myRoutes-----
//שליפת כל המשימות 
app.MapGet("/", async (ToDoDbContext context) =>
{
    var data = await context.Items.ToListAsync();
    return Results.Ok(data);
});

app.MapGet("/Hello", async () =>
{
    return Results.Ok("Hello world!");
});

//שליפת משימה מסוימת לפי קוד משימה
app.MapGet("/ToDo/{id}", async (ToDoDbContext context, int id) =>
{
    var item = await context.Items.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(item);
});

//שליפת משימה מסוימת לפי שם משימה
app.MapGet("/ToDoByName", async (ToDoDbContext context, string todo) =>
{
    var item = await context.Items.FirstOrDefaultAsync(x => x.Name == todo);
    if (item == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(item);
});

//שליפת כל המשימות שהושלמו או שלא הושלמו
app.MapGet("/ToDoByComplete", async (ToDoDbContext context, bool isComplete) =>
{
    var items = await context.Items.Where(x => x.IsComplete == isComplete).ToListAsync();

    if (items.Count == 0)
    {
        return Results.NotFound();
    }
    return Results.Ok(items);
});

//הוספת משימה חדשה
app.MapPost("/ToDo", async (ToDoDbContext context, Item i) =>
{
    var data = await context.Items.ToListAsync();
    context.Add(i);
    context.SaveChanges();
    return Results.Ok(data);
});

//הוספת משימה באמצעות שם משימה בלבד
app.MapPost("/", async (ToDoDbContext context, string todo) =>
{
    var data = await context.Items.ToListAsync();
    Item i = new Item();
    i.Name = todo;
    i.IsComplete = false;
    context.Add(i);
    context.SaveChanges();
    return Results.Ok(data);
});

// עדכון משימה
app.MapPut("/{id}", async (ToDoDbContext context, int id, Item updatedItem) =>
{
    var item = await context.Items.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound();
    }
    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await context.SaveChangesAsync();
    return Results.Ok("המשימה עודכנה בהצלחה👍");
});
//עדכון משימה שהיא הושלמה
app.MapPut("/ToDoComplete/{id}", async (ToDoDbContext db, int id, bool complete) =>
{
    var item = await db.Items.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound();
    }
    item.IsComplete = complete;
    await db.SaveChangesAsync();
    return Results.Ok("👍מצב המשימה עודכן בהצלחה");
});

//מחיקת משימה ספציפית
app.MapDelete("/{id}", async (ToDoDbContext context, int id) =>
{
    var item = await context.Items.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound();
    }
    context.Items.Remove(item);
    await context.SaveChangesAsync();
    return Results.Ok("המשימה נמחקה בהצלחה👍");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
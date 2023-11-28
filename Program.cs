using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTodo.Data;
using MiniTodo.model;

#region Services
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MinimalContextDb>(optioons =>
    optioons.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

#endregion


#region Configure Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/produto", async (MinimalContextDb context) => await context.Produtos.ToListAsync())
    .WithName("GetProduto")
    .WithTags("Produtos");

app.MapGet("/produto/{id}",
    async (int id, MinimalContextDb context) =>
        await context.Produtos.FindAsync(id) is Produto produto ? Results.Ok(produto) : Results.NotFound())
    .Produces<Produto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetProdutoPorId")
    .WithTags("Produtos");

app.MapPost("/produto",
    async (MinimalContextDb context, Produto produto) =>
    {
        context.Produtos.Add(produto);
        var result = await context.SaveChangesAsync();

        return result > 0 ? Results.CreatedAtRoute("GetProdutoPorId", new { id = produto.Id })
            : Results.BadRequest("Erro ao salvar dados");
    })
    .Produces<Produto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostProduto")
    .WithTags("Produtos");

app.MapPut("/produto/{id}",
    async (int id, MinimalContextDb context, Produto produto) =>
    {
        var produtoBannc = await context.Produtos.FindAsync(id);

        if (produtoBannc == null)
        {
            return Results.NotFound();
        }

        context.Produtos.Update(produto);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("Erro ao salvar dados"); ;
    }
    )
    .Produces<Produto>(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("EditandoProduto")
    .WithTags("Produtos");

app.MapDelete("/produto/{id}",
    async (int id, MinimalContextDb context) =>
    {
        var produto = await context.Produtos.FindAsync(id);

        if (produto == null)
        {
            return Results.NotFound();
        }

        context.Produtos.Remove(produto);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("Erro ao salvar dados"); ;
    }
    )
    .Produces<Produto>(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeletandoProduto")
    .WithTags("Produtos");

app.Run();
#endregion


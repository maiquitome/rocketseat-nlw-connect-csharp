using Microsoft.EntityFrameworkCore;
using TechLibrary.Api.Domain.Entities;
using TechLibrary.Api.Infrastructure.DataAccess;
using TechLibrary.Communication.Requests;
using TechLibrary.Communication.Responses;

namespace TechLibrary.Api.UseCases.Books.Filter;

public class FilterBookUseCase
{
    private const int BOOKS_PER_PAGE = 10;

    public ResponseBooksJson Execute(RequestFilterBooksJson request)
    {
        var dbContext = new TechLibraryDbContext();

        int booksToSkip = (request.PageNumber - 1) * BOOKS_PER_PAGE;
        // (1 - 1) * 10 = 0
        // (2 - 1) * 10 = 10
        // (3 - 1) * 10 = 20
        // (4 - 1) * 10 = 30

        IQueryable<Book> query = dbContext.Books.AsQueryable();

        bool isTitleFilledIn = string.IsNullOrWhiteSpace(request.Title) == false;

        if (isTitleFilledIn)
            query = query.Where(book => EF.Functions.Like(book.Title, $"%{request.Title!}%"));

        var books = query
            .OrderBy(book => book.Title).ThenBy(book => book.Author)
            .Skip(booksToSkip)
            .Take(BOOKS_PER_PAGE)
            .ToList(); // .ToList() is used to execute the query and get the results

        int totalCount = 0;
        if (isTitleFilledIn)
            totalCount = dbContext.Books.Count(book => EF.Functions.Like(book.Title, $"%{request.Title!}%"));
        else
            totalCount = dbContext.Books.Count();

        return new ResponseBooksJson
        {
            Pagination = new ResponsePaginationJson
            {
                PageNumber = request.PageNumber,
                TotalCount = totalCount,
            },
            Books = books.Select(book => new ResponseBookJson
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
            }).ToList()
        };
    }
}

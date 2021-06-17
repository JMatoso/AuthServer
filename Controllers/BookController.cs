using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using jwt_identity_api.Data.Repositories;
using jwt_identity_api.DTO;

namespace jwt_identity_api.Controllers
{
    /// <summary>
    /// Manage books.
    /// </summary>
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookApp _book;
        public BookController(IBookApp book)
        {
            _book = book;
        }

        /// <summary>
        /// List all books.
        /// </summary>
        /// <response code="200">Ok.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Any book found.</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Book>>> All() 
        {
            var books = await _book.Get();
            return books == null ? NotFound("Any book found.") : Ok(books);
        }

        /// <summary>
        /// Select a book.
        /// </summary>
        /// <response code="200">Ok.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        [HttpGet("{bookId}")]
        [Authorize]
        public async Task<ActionResult<Book>> Get(Guid bookId)
        {
            if(Guid.Empty == bookId)
            {
                return BadRequest("Insert a valid id.");
            }

            var book = await _book.Get(bookId);
            return book == null ? NotFound("Book not found.") : Ok(book);
        }

        /// <summary>
        /// Add a book.
        /// </summary>
        /// <response code="200">Ok.</response>
        /// <response code="400">Fill all required fields.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Add(Book model)
        {
            if(ModelState.IsValid)
            {
                return await _book.Add(model) == true ? Ok("Book added succesfully.") : BadRequest("Something went wrong, try again.");
            }

            return BadRequest("Fill all required fields.");
        }

        /// <summary>
        /// Update a book.
        /// </summary>
        /// <response code="200">Ok.</response>
        /// <response code="400">Fill all required fields.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(Book model)
        {
            if(ModelState.IsValid)
            {
                return await _book.Update(model) == true ? Ok("Book updated succesfully.") : BadRequest("Book wasn't found or something went wrong.");
            }

            return BadRequest("Fill all required fields.");
        }

        /// <summary>
        /// Remove a book.
        /// </summary>
        /// <param name="bookId">Book Id.</param>
        /// <response code="200">Ok.</response>
        /// <response code="400">Not Found or Something went wrong.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpDelete("{bookId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Remove(Guid bookId)
        {
            if(Guid.Empty == bookId)
            {
                return BadRequest("Insert a valid id.");
            }

            return await _book.Remove(bookId) == true ? Ok("Book removed succesfully.") : BadRequest("Book wasn't found or something went wrong.");
        }
    }
}
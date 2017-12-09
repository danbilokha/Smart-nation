﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookSender.Data.Models;
using BookSender.Data;
using Microsoft.EntityFrameworkCore;
using BookSender.Models.AccessoryModels;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;

namespace BookSender.Controllers
{
    [EnableCors("CorsPolicy")]
    public class PersonalCabinetController : Controller
    {
        private readonly ApplicationContext _context;

		public PersonalCabinetController(ApplicationContext context
			)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookModel incomingBook)
        {
            try
            {
                //Data.Models.User user =  await _context.Users.FirstOrDefaultAsync( u => u.Email == email || u.PhoneNumber == phone);

                //if (user != null)
                //{
                //incomingBook.ConributorId = user.Id;
                //incomingBook.CurrentUserId = user.Id;
                Book book = new Book
                {
                    Title = incomingBook.Title,
                    Author = incomingBook.Author,
                    Price = Convert.ToDecimal(incomingBook.Price)
                };

                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();

                    return Json("successful");
                //}
                //else
                //{
                //    throw new Exception("User was not found");
                //}
            }
            catch (Exception e)
            {
                return Json("Error: " + e.Message);
            }           
        }

		[HttpPost]
		public async Task<IActionResult> GetAllUserBooks()
		{

			try
			{
				var userId = User.Claims.FirstOrDefault(C => C.Type == ClaimTypes.NameIdentifier).Value;

				var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

				if (user != null)
				{
					List<Book> userBooks = await _context.Books.Where(
												b => b.ContributorId == user.Id).ToListAsync();
					return Json(userBooks);
				}
				else
				{
					throw new Exception("User was not found");
				}
			}
			catch (Exception e)
			{
				return Json("Error: " + e.Message);
			}
		}

		[HttpPost]
        public async Task<IActionResult> GetDetailedBookInfo([FromBody] Book incomingBook)
        {
            try
            {
                Data.Models.Book book = await _context.Books.FirstOrDefaultAsync(b => b.Id == incomingBook.Id);
                if (book != null)
                {
                    List<BookHistory> bookHistory = await _context.BookHistoryRecords
                                                        .Where(bh => bh.Id == incomingBook.Id).ToListAsync();

                    List<User> userList = new List<User>(); 

                    foreach (var bh in bookHistory)
                    {
                        userList.Add(
                           await _context.Users.Where(u => u.Id == bh.UserId).FirstOrDefaultAsync()); 
                    }

                    return Json("");
                }
                else
                {
                    throw new Exception("Book was not found");
                }
            }
            catch (Exception e)
            {
                return Json("Error: " + e.Message);
            }
        }
    }
}
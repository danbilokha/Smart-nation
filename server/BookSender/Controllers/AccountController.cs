﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookSender.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using GmailSender.Model;
using GmailSender;
using BookSender.Data;
using BookSender.Data.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using LoginData = BookSender.Models.AccessoryModels.LoginModel;
using RegisterData = BookSender.Models.AccessoryModels.RegisterModel;
using BookSender.Models.AccessoryModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using BookSender.Services.Interfaces;
using BookSender.Models.AccessoryModels.DealModels;

namespace BookSender.Controllers
{
	[EnableCors("CorsPolicy")]
	public class AccountController : Controller
	{
		private ApplicationContext _context;
		public AccountController(ApplicationContext context)
		{
			_context = context;
		}

		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<JsonResult> Register([FromBody] RegisterData user)
		{
			try
			{
				_context.Users.Add(new User { PhoneNumber = user.Phone, Password = user.Password, Email = "test@mail.ru", RatingStatusId = 1, RoleId = 2 });

				await _context.SaveChangesAsync();

				return Json($" 'Answer' : 'Successful user creation'");
			}
			catch (Exception ex)
			{
				return Json($" 'Answer' : ' Error = {ex.Message}' ");
			}
		}


		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login([FromBody] LoginData model)
		{
			try
			{
				if (model != null)
				{
					Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
					Regex regexPhone = new Regex(@"^\+\d{12}");
					Match matchEmail = (model.userLogInfo != null) ? regexEmail.Match(model.userLogInfo) : regexEmail.Match("");
					Match matchPhone = (model.userLogInfo != null) ? regexPhone.Match(model.userLogInfo) : regexPhone.Match("");


					if (matchEmail.Success)
					{
						BookSender.Data.Models.User user = await _context.Users
							.Include(u => u.Role)
							.FirstOrDefaultAsync(u => u.Email == model.userLogInfo && u.Password == model.Password);


						AccountLoginResponce acc = new AccountLoginResponce
						{
							Login = user.Email,
							Name = user.FirstName,
							Surname = user.LastName,
							Role = user.Role != null ? user.Role.Name : "Guest",
						};

						await Authenticate(user);

						return Json(acc);
					}
					else if (matchPhone.Success)
					{
						BookSender.Data.Models.User user = await _context.Users
							.Include(u => u.Role)
							.FirstOrDefaultAsync(u => u.PhoneNumber == model.userLogInfo && u.Password == model.Password);

						AccountLoginResponce acc = new AccountLoginResponce
						{
							Login = user.Email,
							Name = user.FirstName,
							Surname = user.LastName,
							Role = user.Role != null ? user.Role.Name : "Guest",
						};

						await Authenticate(user, isEmailAuth: false);

						return Json(acc);
					}
					else
					{
						return Json("'Answer': 'Wrong user credetials'");
					}
				}
				else
					return Json(new LoginData());
			}
			catch (Exception ex)
			{
				return Json($" 'Answer' : 'Error = {ex.Message}' ");
			}
		}

		[HttpPost]
		public async Task<IActionResult> LoginWithFacebook([FromBody] FaceBookLoginModel model)
		{
			try
			{
				if (model.email != null )
				{
                        string DefaultPassWord = SmtpClientLibrary.SendKey(model.email, "programmist5000@gmail.com", "Zaebalo45809");
                        string DefaultPhone = "9379992";
                    
					Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
					Match matchEmail = (model.email != null) ? regexEmail.Match(model.email) : regexEmail.Match("");


					if (matchEmail.Success)
					{
						AccountLoginResponce acc = new AccountLoginResponce();

						User user = await _context.Users
							.Include(u => u.Role)
							.FirstOrDefaultAsync(u => u.Email == model.email);

						if (user == null)
						{
							string[] names = model.name.Split(" ");

							_context.Users.Add(new User
							{
								Password = DefaultPassWord,
								PhoneNumber = DefaultPhone,
								Email = model.email,
								FirstName = names[0],
								LastName = names[1],
								RatingStatusId = 1,
								RoleId = 2
							});

							var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == 2);

							await _context.SaveChangesAsync();

							acc.Login = user.Email;
							acc.Name = names[0];
							acc.Surname = names[1];
							acc.Role = role.Name;


						}
						else
						{
							acc.Login = user.Email;
							acc.Name = user.FirstName;
							acc.Surname = user.LastName;
							acc.Role = user.Role != null ? user.Role.Name : "Guest";
						}

						await Authenticate(user);

						return Json(acc);
					}
					else
					{
						return Json($" 'Answer' : 'Error = BadEmail' ");
					}

				}
				else
				{
					return Json(new AccountLoginResponce());
				}
			}
			catch (Exception ex)
			{
				return Json($" 'Answer' : 'Error = {ex.Message}' ");
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Logout()
		{

			var userId = User.Claims.FirstOrDefault(C => C.Type == ClaimTypes.NameIdentifier).Value;

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));


			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return Json($" 'Answer' : 'Logedout = true' ");

		}

		private async Task Authenticate(Data.Models.User user, bool isEmailAuth = true)
		{
			var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

			var identifier = isEmailAuth ? user.Email : user.PhoneNumber;

			identity.AddClaim(new Claim(ClaimTypes.Name, identifier));
			identity.AddClaim(new Claim(ClaimTypes.Role, user.Role != null ? user.Role?.Name : "Guest"));
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

			// Authenticate using the identity
			var principal = new ClaimsPrincipal(identity);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });

		}
	}
}
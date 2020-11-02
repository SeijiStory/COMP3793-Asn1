using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using asn1.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace asn1.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly ILogger _logger;
        public BookController(ILogger<BookController> logger) {
            _logger = logger;
        }
        private const string BASE_URL = "https://www.googleapis.com/books/v1/volumes";
        private const string QUERY_STRING = "?q=harry+potter";
        private Book SeedBook(JToken token) {
            HtmlDocument htmlDoc = new HtmlDocument();
            var isbn = token["volumeInfo"]["industryIdentifiers"].Children().ToList().
                Where(i => i["type"].ToString().Equals("ISBN_10")).
                FirstOrDefault();
            string desc = "No description available";
            JToken tmp;
            if ((tmp = token["volumeInfo"]["description"]) != null) {
                desc = HttpUtility.HtmlDecode(tmp.ToString());
                /*
                htmlDoc.LoadHtml(HttpUtility.HtmlDecode(tmp.ToString()));
                desc = htmlDoc.DocumentNode.InnerText;
                /**/
            } else if ((tmp = token["searchInfo"]["textSnippet"]) != null) {
                desc = HttpUtility.HtmlDecode(tmp.ToString());
                /* Use search info if description not available; decode HTML to convert escapes to literals */
                /*
                htmlDoc.LoadHtml(HttpUtility.HtmlDecode(tmp.ToString()));
                desc = htmlDoc.DocumentNode.InnerText;
                /**/
            }
            string isbn_str = isbn["identifier"].ToString();
            if (isbn_str == null || isbn_str == "") isbn_str = "0";
            _logger.LogInformation(token["volumeInfo"]["title"].ToString() + ":");
            _logger.LogInformation(isbn["identifier"].ToString());
            Book b = new Book {
                BookID = (string)token["id"],
                Title = (string)token["volumeInfo"]["title"],
                Thumbnail = (string)token["volumeInfo"]["imageLinks"]["smallThumbnail"],
                Authors = token["volumeInfo"]["authors"].Select(t => t.ToString()).ToList(),
                Publisher = (string)token["volumeInfo"]["publisher"],
                PublishedDate = (string)token["volumeInfo"]["publishedDate"],
                Description = desc,
                ISBN_10 = Int32.Parse(isbn_str)
            };
            return b;
        }
        // GET: Book
        public async Task<ActionResult> IndexAsync()
        {
            List<Book> books = new List<Book>();
            string booksJsonString = null;
            using (var client = new WebClient()) {
                client.Headers.Set("Accept", "application/json");
                booksJsonString = await client.DownloadStringTaskAsync(new Uri($"{BASE_URL}{QUERY_STRING}"));
            }
            if (booksJsonString != null) {
                JObject booksJson = JObject.Parse(booksJsonString);
                JArray itemsJson = (JArray)booksJson["items"];
                var items = itemsJson.ToList();
                foreach (var i in items) {
                    books.Add(SeedBook(i));
                }
            }
            return View(books);
        }

        // GET: Book/Details/5
        public async Task<ActionResult> DetailsAsync(string id)
        {
            Book b = null;
            string booksJsonString = null;
            using (var client = new WebClient()) {
                client.Headers.Set("Accept", "application/json");
                booksJsonString = await client.DownloadStringTaskAsync(new Uri($"{BASE_URL}/{id}{QUERY_STRING}"));
            }
            if (booksJsonString != null) {
                JToken booksJson = JObject.Parse(booksJsonString);
                b = SeedBook(booksJson);
            }
            if (b == null) return NotFound();
            return View(b);
        }
    }
}
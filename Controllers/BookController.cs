using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using asn1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace asn1.Controllers
{
    public class BookController : Controller
    {
        private Book SeedBook(JToken token) {
            var isbn = token["volumeInfo"]["industryIdentifiers"].Children().ToList().
                Where(i => i["type"].ToString().Equals("ISBN_10")).
                FirstOrDefault();
            string desc = "No description available";
            JToken tmp;
            if ((tmp = token["volumeInfo"]["description"]) != null) {
                desc = tmp.ToString();
            } else if ((tmp = token["searchInfo"]["textSnippet"]) != null) {
                /* Use search info if description not available; decode HTML to convert escapes to literals */
                desc = HttpUtility.HtmlDecode(tmp.ToString()); 
            }
            Book b = new Book {
                BookID = (string)token["id"],
                Title = (string)token["volumeInfo"]["title"],
                Thumbnail = (string)token["volumeInfo"]["imageLinks"]["smallThumbnail"],
                Authors = token["volumeInfo"]["authors"].Select(t => t.ToString()).ToList(),
                Publisher = (string)token["volumeInfo"]["publisher"],
                PublishedDate = (string)token["volumeInfo"]["publishedDate"],
                Description = desc,
                ISBN_10 = Int32.Parse(isbn["identifier"].ToString())
            };
            return b;
        }
        // GET: Book
        public ActionResult Index()
        {
            List<Book> books = new List<Book>();
            WebClient client = new WebClient();
            string booksJsonString = client.DownloadString("https://www.googleapis.com/books/v1/volumes?q=harry+potter");
            if (booksJsonString != "") {
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
        [Authorize]
        public ActionResult Details(string id)
        {
            Book b = new Book();
            WebClient client = new WebClient();
            string booksJsonString = client.DownloadString("https://www.googleapis.com/books/v1/volumes?q=harry+potter");
            if (booksJsonString != "") {
                JObject booksJson = JObject.Parse(booksJsonString);
                JArray itemsJson = (JArray)booksJson["items"];
                b = SeedBook(itemsJson.Children().Where(i => i["id"].ToString().Equals(id)).FirstOrDefault());
            }
            return View(b);
        }
    }
}
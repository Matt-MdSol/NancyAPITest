using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;

namespace NancyAPITest
{
    public class Comic
    {
        public string title;
        public string publisher;
        public int issue;
    }

    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public long Permissions { get; set; }
        public Guid SecretKey { get; set; }
        public DateTime loginDate { get; set; }
    }

    public class ComicModule : NancyModule
    {
        private static DateTime startUp = DateTime.Now;

        private static List<Comic> comics = new List<Comic>()
        {
            new Comic() {
                title = "Avengers",
                publisher = "Marvel",
                issue = 132
            }
        };

        private string GenerateList(string name, List<string> elements)
        {
            string sHtml = "<select name='" + name + "'>\n";
            foreach ( string e in elements )
            {
                sHtml += "   <option value='" + e + "'" +"> " + e + "</option>\n";
            }
            sHtml += "</select><BR>\n";
            return sHtml;
        }

        public string GetLoginScreen()
        {
            string sHtml = "<html><body><form method='POST'>";
            sHtml += "User Name: " + "<input type='text' name='username'><br>";
            sHtml += "Password : " + "<input type='password' name='password'><br>";
            sHtml += "<input method=\"POST\" type=\"submit\" value=\"Submit\">";
            sHtml += "</form><body></html>";
            return sHtml;
        }

        public Response GetForm()
        {
            string sHtml = "<html><body><form method='POST'>";
            sHtml += "First Name: " + "<input type='text' name='firstname'><br>";
            sHtml += "Last Name: " + "<input type='text' name='lastname'><br>";
            sHtml += "<input type='date' name='appointmentdate' value='1960-01-10'><br>";
            // Add a radio button group
            sHtml += "<input type='radio' name='group1' value='1'>Option 1<br>";
            sHtml += "<input type='radio' name='group1' value='2'>Option 2<br>";
            sHtml += "<input type='radio' name='group1' value='2'>Option 2<br>";
            sHtml += "<input type='checkbox' name=red value='red'>Red<br>";
            sHtml += "<input type='checkbox' name=blue value='blue'>Blue<br>";
            sHtml += "<input type=image src='Content/6crob4Egi.png' width=32 height=32><BR>";
            List<string> options = new List<string>()
            {
                "Yes",
                "No",
                "Maybe"
            };
            sHtml += GenerateList("surety", options);
            sHtml += "<input method=\"POST\" type=\"submit\" value=\"Submit\">";
            sHtml += "</form><body></html>";
            Response resp = (Response)sHtml;
           return resp;
        }

        public ComicModule()
        {
            Get["/login"] = parameters =>
            {
                string sHtml = GetLoginScreen();
                return sHtml;

            };
            Post["/login"] = parameters =>
            {
                var form = this.Request.Form;
                string userName = form.username;
                string password = form.password;
                if (userName.Length == 0 || password.Length == 0)
                {
                    return GetLoginScreen();
                }
                Guid g = Guid.NewGuid();
                User user = new User();
                user.UserName = form.username;
                user.Password = form.password;
                user.SecretKey = g;
                user.loginDate = DateTime.Now;
                Response r = Response.AsRedirect("/form");
                string userString = JsonConvert.SerializeObject(user);
                r.WithCookie("X-Secret", userString);
                return r;
            };

            Get["/comics"] = parameters =>
            {
                return JsonConvert.SerializeObject(comics);
            };

            Get["/comic/{id}"] = parameters =>
            {
                return JsonConvert.SerializeObject(comics[parameters.id]);
            };

            Get["/"] = parameters =>
            {
                return Negotiate.WithHeader("X-Custom-Header", new Guid().ToString());
            };

            Post["/comics"] = parameters =>
            {
                var model = this.Bind<Comic>();
                comics.Add(model);
                return comics.Count.ToString();
            };

            Get["/form"] = parameters =>
            {
                var cookies = Request.Cookies;
                foreach ( string key in cookies.Keys )
                {
                    if ( key == "X-Secret")
                    {
                        // See if they date time is too old
                        User user = JsonConvert.DeserializeObject<User>(cookies[key]);
                        if (user.loginDate < startUp)
                            return Response.AsRedirect("/login");
                        return GetForm();
                    }
                }
                return Response.AsRedirect("/login");
            };
            Post["/form"] = parameters =>
            {
                var form =  this.Request.Form;
                string sREturn = "Hello " + form.firstname + " " + form.lastname;
                return sREturn;
            };
        }
    }
}

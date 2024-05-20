using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diplom
{
    internal class models
    {
        public class Suggestions
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("suggestion")]
            public string Suggestion { get; set; }
            [JsonProperty("right_word")]
            public string RightWord { get; set; }
        }

        public class Words
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("translate")]
            public string Translate { get; set; }
        }

        public class MyUser
        {
            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("first_name")]
            public string FirstName { get; set; }

            [JsonProperty("last_name")]
            public string LastName { get; set; }

            [JsonProperty("patronymic")]
            public string Patronymic { get; set; }

            [JsonProperty("anonymous")]
            public bool Anonymous { get; set; }

            [JsonProperty("use_english")]
            public bool UseEnglish { get; set; }

            [JsonProperty("right_answers")]
            public int RightAnswers { get; set; }

            [JsonProperty("wrong_answers")]
            public int WrongAnswers { get; set; }

            [JsonProperty("image")]
            public string Image { get; set; }
        }

    }
}

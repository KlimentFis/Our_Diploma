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
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("image")]
            public string Image { get; set; }
            [JsonProperty("last_login")]
            public DateTime LastLogin { get; set; }
            [JsonProperty("is_superuser")]
            public bool IsSuperuser { get; set; }
            [JsonProperty("username")]
            public string Username { get; set; }
            [JsonProperty("first_name")]
            public string FirstName { get; set; }
            [JsonProperty("last_name")]
            public string LastName { get; set; }
            [JsonProperty("email")]
            public string Email { get; set; }
            [JsonProperty("is_staff")]
            public bool IsStaff { get; set; }
            [JsonProperty("is_active")]
            public bool IsActive { get; set; }
            [JsonProperty("date_joined")]
            public DateTime DateJoined { get; set; }
            [JsonProperty("patronymic")]
            public string Patronymic { get; set; }
            [JsonProperty("use_english")]
            public bool UseEnglish { get; set; }
            [JsonProperty("anonymous")]
            public bool Anonymous { get; set; }
            [JsonProperty("right_answers")]
            public int RightAnswers { get; set; }
            [JsonProperty("wrong_answers")]
            public int WrongAnswers { get; set; }
            [JsonProperty("groups")]
            public List<object> Groups { get; set; }
            [JsonProperty("user_permissions")]
            public List<object> UserPermissions { get; set; }
        }
        public class User
        {
            [JsonProperty("username")]
            public string Username { get; set; }
        }
    }
}

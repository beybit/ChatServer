using ChatService.Integration.Users.Dtos;
using ChatService.Integration.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace ChatServer.ConsoleClient.Clients
{

    public class UsersClient(HttpClient http)
    {
        public async Task<List<UserDto>?> GetAll(GetUsersQuery query)
        {
            return await http.GetFromJsonAsync<List<UserDto>>($"/api/users?{query.ToQueryString()}");
        }
    }
}

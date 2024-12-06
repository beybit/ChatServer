using ChatService.Integration.Groups.Commands;
using ChatService.Integration.Groups.Dtos;
using ChatService.Integration.Groups.Queries;
using ChatService.Integration.Users.Commands;
using ChatService.Integration.Users.Dtos;
using ChatService.Integration.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public async Task<List<ChatGroupDto>?> GetGroups(GetChatGroupsQuery query)
        {
            return await http.GetFromJsonAsync<List<ChatGroupDto>>($"/api/chat-groups");
        }

        public async Task<ChatGroupDto?> CreateGroup(string name)
        {
            var response = await http.PostAsJsonAsync("/api/chat-groups", new CreateChatGroupCommand(name));

            return await response.ReadJson<ChatGroupDto>();
        }
    }
}

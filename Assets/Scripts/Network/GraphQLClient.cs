using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayMeow.Network
{
    public class GraphQLClient
    {
        // Update this to the real GraphQL endpoint before testing
        private const string DefaultEndpoint = "https://api.example.com/graphql";

        private readonly string _endpoint;

        public GraphQLClient(string endpoint = null)
        {
            _endpoint = endpoint ?? DefaultEndpoint;
        }

        public async Task<GraphQLResponse> QueryAsync(string query, object variables = null)
        {
            var body = new GraphQLRequest { query = query, variables = variables };
            string json = JsonUtility.ToJson(body);

            using var request = new UnityWebRequest(_endpoint, "POST");
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var op = request.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
                return new GraphQLResponse { error = request.error };

            string responseText = request.downloadHandler.text;
            return JsonUtility.FromJson<GraphQLResponse>(responseText)
                   ?? new GraphQLResponse { error = "Empty response" };
        }

        [Serializable]
        private class GraphQLRequest
        {
            public string query;
            public object variables;
        }
    }

    [Serializable]
    public class GraphQLResponse
    {
        public string error;
        public GraphQLData data;
    }

    [Serializable]
    public class GraphQLData
    {
        public LoginData login;
    }

    [Serializable]
    public class LoginData
    {
        public string token;
    }
}

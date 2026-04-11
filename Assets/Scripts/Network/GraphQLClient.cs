using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayMeow.Network
{
    /// <summary>
    /// Lightweight GraphQL HTTP client using UnityWebRequest.
    /// Handles POST requests, Authorization headers, and GraphQL error arrays.
    /// </summary>
    public class GraphQLClient
    {
        public const string DefaultEndpoint = "https://interview-api.join-playmeow.com/graphql";

        private readonly string _endpoint;

        public GraphQLClient(string endpoint = null)
        {
            _endpoint = endpoint ?? DefaultEndpoint;
        }

        /// <summary>
        /// Send a GraphQL query or mutation.
        /// </summary>
        /// <param name="query">GraphQL query/mutation string.</param>
        /// <param name="variables">String-keyed variables (string values only).</param>
        /// <param name="authToken">Optional Bearer token for the Authorization header.</param>
        public async Task<GraphQLResponse> QueryAsync(
            string query,
            Dictionary<string, string> variables = null,
            string authToken = null)
        {
            string requestJson = BuildRequestJson(query, variables);

            using var request = new UnityWebRequest(_endpoint, "POST");
            byte[] bytes = Encoding.UTF8.GetBytes(requestJson);
            request.uploadHandler   = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(authToken))
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");

            var op = request.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[GraphQLClient] Network error: {request.error}");
                return new GraphQLResponse { networkError = request.error };
            }

            string responseText = request.downloadHandler.text;
            Debug.Log($"[GraphQLClient] Response: {responseText}");

            var response = JsonUtility.FromJson<GraphQLResponse>(responseText);
            if (response == null)
                return new GraphQLResponse { networkError = "Failed to parse response" };

            return response;
        }

        // ---------------------------------------------------------------------------
        // JSON helpers (avoid Newtonsoft dependency)
        // ---------------------------------------------------------------------------

        private static string BuildRequestJson(string query, Dictionary<string, string> variables)
        {
            var sb = new StringBuilder();
            sb.Append("{\"query\":");
            sb.Append(JsonString(query));

            if (variables != null && variables.Count > 0)
            {
                sb.Append(",\"variables\":{");
                bool first = true;
                foreach (var kv in variables)
                {
                    if (!first) sb.Append(',');
                    sb.Append(JsonString(kv.Key));
                    sb.Append(':');
                    sb.Append(JsonString(kv.Value));
                    first = false;
                }
                sb.Append('}');
            }

            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>Wrap a string in double-quotes and escape special characters.</summary>
        internal static string JsonString(string s)
        {
            if (s == null) return "null";
            return "\"" + s
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                + "\"";
        }
    }

    // ---------------------------------------------------------------------------
    // Serializable response types
    // ---------------------------------------------------------------------------

    [Serializable]
    public class GraphQLResponse
    {
        /// <summary>Non-null when there is a transport/network-level error (not a GraphQL error).</summary>
        public string networkError;

        public GraphQLData data;
        public GraphQLError[] errors;

        /// <summary>True when the response contains at least one GraphQL-level error.</summary>
        public bool HasErrors => errors != null && errors.Length > 0;

        /// <summary>First GraphQL error message, or null.</summary>
        public string FirstError => HasErrors ? errors[0].message : null;
    }

    [Serializable]
    public class GraphQLError
    {
        public string message;
    }

    [Serializable]
    public class GraphQLData
    {
        public AuthPayload login;
        public GraphQLUser me;
    }

    [Serializable]
    public class AuthPayload
    {
        public string token;
        public GraphQLUser user;
    }

    [Serializable]
    public class GraphQLUser
    {
        public string id;
        public string username;
    }
}

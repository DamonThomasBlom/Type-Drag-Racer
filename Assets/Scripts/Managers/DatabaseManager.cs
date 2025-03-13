using UnityEngine;
using BestHTTP;
using System;
using Newtonsoft.Json;
using System.Text;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;
using Sirenix.OdinInspector;

public class DatabaseManager : MonoBehaviour
{
    private const string BASE_URL = "http://127.0.0.1:8090/api/collections/";

    //string jsonData = $"{{\"identity\": \"{email}\", \"password\": \"{password}\"}}";

    [Button]
    public void RegisterUser(string username, string email, string password)
    {
        HTTPRequest request = new HTTPRequest(new Uri($"{BASE_URL}users/records"), HTTPMethods.Post, OnRequestFinished);

        object jsonBody = new
        {
            password = password,
            passwordConfirm = password,
            email = email,
            emailVisibility = true,
            username = username
        };

        request.SetHeader("Content-Type", "application/json");
        request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonBody));
        //request.RawData

        request.Send();
    }

    private void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    // Everything went as expected!
                    Debug.LogFormat("{0} \n<color=green>response:</color> {1} \n<color=green>Absolute URL:</color> {2} ", 
                        req.GetFirstHeaderValue("curEvent"), 
                        resp.DataAsText, 
                        req.Uri.AbsoluteUri);
                }
                else
                {
                    Debug.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                    resp.StatusCode,
                                                    resp.Message,
                                                    resp.DataAsText));
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.Log(req.Uri.AbsoluteUri + " - Error");
                Debug.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.Log(req.Uri.AbsoluteUri + " - Abort");
                Debug.LogError("Request Aborted!");

                break;

            // Connecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.Log(req.Uri.AbsoluteUri + " - Connection Timed Out");
                Debug.LogError("Connection Timed Out!");

                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.Log(req.Uri.AbsoluteUri + " - Timed Out");
                Debug.LogError("Processing the request Timed Out!");

                break;
        }
    }
}

public partial class RegisterResponseContainer
{
    [JsonProperty("data")]
    public Data Data { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("status")]
    public long Status { get; set; }
}

public partial class Data
{
    public Id Id { get; set; }
}

public partial class Id
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}
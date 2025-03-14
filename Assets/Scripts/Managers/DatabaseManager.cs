using UnityEngine;
using BestHTTP;
using System;
using Newtonsoft.Json;
using System.Text;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;
using Sirenix.OdinInspector;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    #region SINGLETON

    public static DatabaseManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private const string BASE_URL = "http://127.0.0.1:8090/api/collections/";

    //string jsonData = $"{{\"identity\": \"{email}\", \"password\": \"{password}\"}}";

    [Button]
    public void RegisterUser(string username, string email, string password, Action<SimpleServerResponse> callback)
    {
        HTTPRequest request = new HTTPRequest(new Uri($"{BASE_URL}users/records"), HTTPMethods.Post, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull registration
                if (res.IsSuccess)
                {
                    callbackResponse.IsSuccess = true;
                    callback.Invoke(callbackResponse);

                    RegisterSuccessResponse userData = JsonConvert.DeserializeObject<RegisterSuccessResponse>(res.DataAsText);
                    // TODO: Send verification email here

                }
                else // Unsuccessful registration
                {
                    RegisterErrorResponse errorResponse = JsonConvert.DeserializeObject<RegisterErrorResponse>(res.DataAsText);

                    callbackResponse.IsSuccess = false;
                    switch (errorResponse.Data.Keys.First())
                    {
                        case "username":
                            callbackResponse.Message = "Username has already been taken.";
                            break;
                        default:
                            callbackResponse.Message = errorResponse.Data.Keys.First() + " - " + errorResponse.Data[errorResponse.Data.Keys.First()].Message;
                            break;
                    }

                    callback.Invoke(callbackResponse);
                }
            }
            else
            {
                callbackResponse.IsSuccess = false;
                callbackResponse.Message = "Register error, check your internet connection!";
                callback.Invoke(callbackResponse);
            }

        });

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

        request.Send();
    }

    private void OnRegisterFinished(HTTPRequest request, HTTPResponse response, Action<SimpleServerResponse> callback)
    {
        if (request.State == HTTPRequestStates.Finished)
        {
            //if (response.IsSuccess)
            //{

            //}
        }
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

public class SimpleServerResponse
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; }
}

public partial class RegisterSuccessResponse
{
    [JsonProperty("collectionId")]
    public string CollectionId { get; set; }

    [JsonProperty("collectionName")]
    public string CollectionName { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("emailVisibility")]
    public bool EmailVisibility { get; set; }

    [JsonProperty("verified")]
    public bool Verified { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("created")]
    public DateTimeOffset Created { get; set; }

    [JsonProperty("updated")]
    public DateTimeOffset Updated { get; set; }
}

public partial class RegisterErrorResponse
{
    public Dictionary<string, ValidationError> Data { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("status")]
    public long Status { get; set; }
}

public partial class ValidationError
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}
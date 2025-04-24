using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class User
{
    public string id;
}

public class UserMsg : User
{
    public string msg;
    
}

public class UnityClient : MonoBehaviour
{
    public InputField input_ID;
    public InputField input_SendMsg;
    public Text loginWhether;
    public Text sendText;


    public void LoginBtn() //로그인 버튼
    {

        User userid = new User
        {
            id =input_ID.text
        };

        string jsonid = JsonUtility.ToJson(userid);
        
        StartCoroutine(UserLogin("http://127.0.0.1:3000/fuckyou/", jsonid));
        


    }

    IEnumerator UserLogin(string URL,string json) 
    {
        using(UnityWebRequest request = UnityWebRequest.Post(URL, json))
        {
            byte[] buffer = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(buffer);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Context-Type", "");//, "application/json");

            yield return request.SendWebRequest();

            if(request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else{

                if(request.downloadHandler.text =="login fail")
                {
                    loginWhether.text = "로그인 여부 : 실패";
                }

                if (request.downloadHandler.text =="login success")
                {
                    loginWhether.text ="로그인 여부 : 성공";
                }

                Debug.Log(request.downloadHandler.text);
            }
        }
    }

    public void ButtonClickHandler()
    {
        UserMsg msg = new UserMsg
        {
            id=input_ID.text,
            msg = input_SendMsg.text
        };

        string jsonMsg = JsonUtility.ToJson(msg);

        StartCoroutine(RESTCall("http://127.0.0.1:3000/fuckyou/", jsonMsg));
        input_SendMsg.text = "";
    }

    IEnumerator RESTCall(string URL, string json) //restAPI를 호출하겠다.
    {
        using(UnityWebRequest www = UnityWebRequest.Post(URL, json))
        {
            byte [] buffer = new System.Text.UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(buffer);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Context-Type", "application/json");


            yield return www.SendWebRequest(); //a --> Love 메세지 보냄.

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                sendText.text = www.downloadHandler.text;
                //url에 담겨있는 내용을 downloadHandler에 넣고 내용을 text를 통해 출력함.
            }
        }

    }

    

    IEnumerator UserLogout(string URL, string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(URL, json))
        {
            byte[] buffer = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(buffer);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Context-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                if (request.downloadHandler.text =="logoutSuccess")
                {
                    loginWhether.text = "로그아웃하셨습니다.";
                    input_ID.text ="";
                }

                if(request.downloadHandler.text =="logoutFail")
                {
                    loginWhether.text = "해당 id가 없습니다.";
                    input_ID.text ="";
                }
                Debug.Log(request.downloadHandler.text);
            }
        }
    }


    public void LogoutBtn() //로그아웃 버튼
    {
        UserMsg useridOut = new UserMsg
        {
            id =input_ID.text,
            msg = "logout"
        };

        string jsonidOut = JsonUtility.ToJson(useridOut);

        StartCoroutine(UserLogout("http://127.0.0.1:3000/fuckyou/", jsonidOut));
    }
}

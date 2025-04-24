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


    public void LoginBtn() //�α��� ��ư
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
                    loginWhether.text = "�α��� ���� : ����";
                }

                if (request.downloadHandler.text =="login success")
                {
                    loginWhether.text ="�α��� ���� : ����";
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

    IEnumerator RESTCall(string URL, string json) //restAPI�� ȣ���ϰڴ�.
    {
        using(UnityWebRequest www = UnityWebRequest.Post(URL, json))
        {
            byte [] buffer = new System.Text.UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(buffer);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Context-Type", "application/json");


            yield return www.SendWebRequest(); //a --> Love �޼��� ����.

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                sendText.text = www.downloadHandler.text;
                //url�� ����ִ� ������ downloadHandler�� �ְ� ������ text�� ���� �����.
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
                    loginWhether.text = "�α׾ƿ��ϼ̽��ϴ�.";
                    input_ID.text ="";
                }

                if(request.downloadHandler.text =="logoutFail")
                {
                    loginWhether.text = "�ش� id�� �����ϴ�.";
                    input_ID.text ="";
                }
                Debug.Log(request.downloadHandler.text);
            }
        }
    }


    public void LogoutBtn() //�α׾ƿ� ��ư
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

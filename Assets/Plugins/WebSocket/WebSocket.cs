/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if UNITY_WEBGL || WEBSOCKET

using System;
using System.Text;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#else
using System.Collections.Generic;
//using System.Security.Authentication;
#endif


public class WebSocket
{
    private Uri mUrl;

    public WebSocket(Uri url)
    {
        mUrl = url;

        string protocol = mUrl.Scheme;
        if (!protocol.Equals("ws") && !protocol.Equals("wss"))
            throw new ArgumentException("Unsupported protocol: " + protocol);
    }

    public void SendString(string str)
    {
        Send(Encoding.UTF8.GetBytes (str));
    }

    public string RecvString()
    {
        byte[] retval = Recv();
        if (retval == null)
            return null;
        return Encoding.UTF8.GetString (retval);
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern int SocketCreate (string url);

    [DllImport("__Internal")]
    private static extern int SocketState (int socketInstance);

    [DllImport("__Internal")]
    private static extern void SocketSend (int socketInstance, byte[] ptr, int length);

    [DllImport("__Internal")]
    private static extern void SocketRecv (int socketInstance, byte[] ptr, int length);

    [DllImport("__Internal")]
    private static extern int SocketRecvLength (int socketInstance);

    [DllImport("__Internal")]
    private static extern void SocketClose (int socketInstance);

    [DllImport("__Internal")]
    private static extern int SocketError (int socketInstance, byte[] ptr, int length);

    int m_NativeRef = 0;

    public void Send(byte[] buffer)
    {
        SocketSend (m_NativeRef, buffer, buffer.Length);
    }

    public byte[] Recv()
    {
        int length = SocketRecvLength (m_NativeRef);
        if (length == 0)
            return null;
        byte[] buffer = new byte[length];
        SocketRecv (m_NativeRef, buffer, length);
        return buffer;
    }

    public void Connect()
    {
        m_NativeRef = SocketCreate (mUrl.ToString());

        //while (SocketState(m_NativeRef) == 0)
        //    yield return 0;
    }

    public void Close()
    {
        SocketClose(m_NativeRef);
    }

    public bool Connected
    {
        get { return SocketState(m_NativeRef) != 0; }
    }

    public string Error
    {
        get {
            const int bufsize = 1024;
            byte[] buffer = new byte[bufsize];
            int result = SocketError (m_NativeRef, buffer, bufsize);

            if (result == 0)
                return null;

            return Encoding.UTF8.GetString (buffer);
        }
    }
#else
    WebSocketSharp.WebSocket m_Socket;
    Queue<byte[]> m_Messages = new Queue<byte[]>();
    bool m_IsConnected = false;
    string m_Error = null;

    public void Connect()
    {
        m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString(), new string[] { "GpBinaryV16" });// modified by TS
        // m_Socket.SslConfiguration.EnabledSslProtocols = m_Socket.SslConfiguration.EnabledSslProtocols | (SslProtocols)(3072| 768);
        m_Socket.OnMessage += (sender, e) => m_Messages.Enqueue(e.RawData);
        m_Socket.OnOpen += (sender, e) => m_IsConnected = true;
        m_Socket.OnError += (sender, e) => m_Error = e.Message + (e.Exception == null ? "" : " / " + e.Exception);
        m_Socket.ConnectAsync();
    }

    public bool Connected { get { return m_IsConnected; } }// added by TS


    public void Send(byte[] buffer)
    {
        m_Socket.Send(buffer);
    }

    public byte[] Recv()
    {
        if (m_Messages.Count == 0)
            return null;
        return m_Messages.Dequeue();
    }

    public void Close()
    {
        m_Socket.Close();
    }

    public string Error
    {
        get
        {
            return m_Error;
        }
    }
#endif
}
#endif

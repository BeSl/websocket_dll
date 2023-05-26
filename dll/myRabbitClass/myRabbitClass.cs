using System;
using System.Runtime.InteropServices;
using System.Net.WebSockets;
using System.Threading;
using System.Text;

namespace bslRabbit
{
    [Guid("6844AACB-9194-46bf-81AF-9DA74EE687DC")]
    internal interface IEventWebSocket
    {
        [DispId(1)]
        bool UseWebSockets(string Host, int waits);
        string ReadMessage();
        bool SendCommand(string Message);
        bool CloseConnection();
    }

    [Guid("70DD7E62-7D82-4301-993C-B7D914330990"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IEventWS
    {
    }

    [Guid("69EE0677-884A-4eeb-A3BD-D407845C0C70"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IEventWS))]
    public class BslWSocket : IEventWebSocket
    {
        /// <summary>
        /// Адрес хоста
        /// </summary>
        public String Host { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// ИмяСервера
        /// </summary>
        public String HostName { get; set; }

        /// <summary>
        /// Порт.
        /// </summary>
        public int Port { get; set; }

        public int Waits { get; set; }

       public ClientWebSocket Ws { get; set; }

        public bool UseWebSockets(string Host, int waits)
        {
            //"wss://echo.websocket.org"
            this.Ws = new ClientWebSocket();

            this.Ws.ConnectAsync(new Uri(Host), CancellationToken.None);
            this.Waits = waits;

            return true;
        }

        public bool SendCommand(string messge) 
        {
            try
            {
               ArraySegment<byte> arraySegment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messge));
               var result = this.Ws.SendAsync(arraySegment, System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
               result.Wait(this.Waits*1000);
               return true;
            }
            catch (Exception e)
            {
                throw new COMException(e.ToString());
            }

        }

    
        public string ReadMessage()
        {
            try
            {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                var result = this.Ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                result.Wait(this.Waits * 1000);
                var response = Encoding.UTF8.GetString(bytesReceived.Array);

                return response;
            }
            catch (Exception e)
            {
                throw new COMException(e.ToString());
            }

        }

        public bool CloseConnection()
        {
            var result = this.Ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            result.Wait(this.Waits * 1000);
            
            return true;
        }
    } 
}

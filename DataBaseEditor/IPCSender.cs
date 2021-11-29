using InterProcessCommunication;
using MapInterfaceObjects;
using System.Collections.Generic;
using DatabaseInterface;

namespace DataBaseEditor
{
    public class IPCSender
    {
        private IPCHandler ipcHandler;
        private static IPCSender instance;
        private IPCSender()
        {
            ipcHandler = new IPCHandler(IPCUserType.SENDER, Program.port1);
        }

        public static IPCSender getInstance()
        {
            if (instance == null)
                instance = new IPCSender();
            return instance;
        }
        public static void setNull()
        {
            instance = null;
        }

        public bool sendIserializables(IEnumerable<ISerializable> serializables, SenderFunction senderFunction)
        {
            return ipcHandler.sendIserializables(serializables, IPCAppType.MAIN, senderFunction);
        }

        public bool sendElementyGraficzne(IEnumerable<IElementGraficzny> elementyGraficzne, string displayParameters, SenderFunction senderFunction)
        {
            return ipcHandler.sendElementyGraficzne(elementyGraficzne, displayParameters, IPCAppType.MAIN, senderFunction);
        }

        public bool sendConnectionData(DBConnectionData connectionData)
        {
            byte[] bytes = connectionData.toByteArray();
            return ipcHandler.sendBytes(bytes, IPCAppType.MAIN, SenderFunction.DBConnectionData);
        }

        public bool sendMessage(string message, SenderFunction senderFunction)
        {
            return ipcHandler.sendMessage(message, IPCAppType.MAIN, senderFunction);
        }
    }
}

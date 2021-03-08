
using InterProcessCommunication;

namespace DataBaseEditor
{
    public class IPCReceiver
    {
        private IPCHandler ipcHandler;
        private static IPCReceiver instance;

        private IPCReceiver()
        {
            ipcHandler = new IPCHandler(IPCUserType.RECEIVER, Program.port2);
            ipcHandler.dataReceivedEvent += IPCDataReceived;
            ipcHandler.clientDisconnectedEvent += IPCClientDisconnected;
        }

        public static IPCReceiver getInstance()
        {
            if (instance == null)
                instance = new IPCReceiver();
            return instance;
        }

        public int getDataReceivedEventInvocationList()
        {
            if(dataReceived != null)
                return dataReceived.GetInvocationList().Length;
            return 0;
        }

        protected virtual void onClientDisconnected()
        {
            ipcHandler.disconnectServer();
        }

        public delegate void dataReceiverEventHandler(object sender, IPCEventArgs args);
        public event dataReceiverEventHandler dataReceived;
        public event dataReceiverEventHandler clientDisconnected;

        private void IPCDataReceived(object sender, IPCEventArgs args)
        {
            IPCCodec codec = new IPCCodec();
            SenderFunction senderFunction;

            if (codec.getAppType(args.data, out senderFunction) == IPCAppType.MAIN)     //komunikacja jest na tym samym porcie, więc wysłana wiadomość wyzwala zdarzenie zarówno w addinie jak i programie głównym; jeżeli wysyła główny, ignoruję je
                return;
            if (dataReceived != null)
            {
                IPCEventArgs eventArgs = new IPCEventArgs() {senderFunc = senderFunction, data = args.data };
                dataReceived(this, eventArgs);   //przesyłam dalej, obsługa konkretnych działań w module graficznym
            }            
        }

        private void IPCClientDisconnected(object sender, IPCEventArgs args)
        {
            Program.isAddinConnected = false;
            IPCSender.setNull();
        }
    }
}

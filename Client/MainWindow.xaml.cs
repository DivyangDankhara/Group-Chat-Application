/*
* FILE          : mainWindows.xaml.cs
* PROJECT       : Assignment #4 Windows and mobile programming
* PROGRAMMER    : Divyangbhai Dankhara
* STUDENET NO.  : 8061566
* FIRST VERSION : 2018-11-19
* Description   : THis file contain the chat client
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker worker = new BackgroundWorker();
        NamedPipeClientStream clientOut;
        NamedPipeClientStream clientIn;

        StreamReader readServer;
        StreamWriter writeServer;

        String uName;


        /// --FUNCTION HADER COMMENT--
        //	Function Name	:	MainWindow
        //	Parameters		:	none
        //	Description		:   this dunction used to InitializeComponent for chat app
        //	Return Value	:   return nothing;
        public MainWindow()
        {
            InitializeComponent();
            worker.DoWork += Worker_DoWork;
        }




        /// --FUNCTION HADER COMMENT--
        //	Function Name	:	btnConnect_Click
        //	Parameters		:	object sender, RoutedEventArgs e: this function is event handler so it takes two perameter
        //	Description		:   this is a click event handeler function which use when some click on button
        //	Return Value	:   return nothing;
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (((tbServerAdd.Text) == "") && ((tbUserName.Text) == ""))
            {
                MessageBox.Show("User Name and server address are required field");
                return;
            }


            String serverAdd = tbServerAdd.Text;
            uName = tbUserName.Text;

            clientOut = new NamedPipeClientStream(serverAdd, "outGoingPipe", PipeDirection.Out);
            clientIn = new NamedPipeClientStream(serverAdd, "InComingPipe", PipeDirection.In);

            clientOut.Connect();
            clientIn.Connect();


            // create stream reader and writer which will help us to send and receive message from server
            readServer = new StreamReader(clientIn);
            writeServer = new StreamWriter(clientOut);
            writeServer.AutoFlush = true;


            // change the color of the logo so the user can see that ow we are connected
            color.Background = Brushes.Green;
            btnSend.IsEnabled = true;
            tbSend.IsEnabled = true;


            // start the worker in background

            btnConnect.IsEnabled = false;
            worker.RunWorkerAsync();
            writeServer.WriteLine(uName + " joint the group chat" );
        }






        /// --FUNCTION HADER COMMENT--
        //	Function Name	:	btnSend_Click
        //	Parameters		:	object sender, RoutedEventArgs e: this function is event handler so it takes two perameter
        //	Description		:   this is a click event handeler function which use when some click on button
        //	Return Value	:   return nothing;
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            String msg = tbSend.Text;
            if(msg == "")
            {
                MessageBox.Show("You can not send the empty massege");
            }
            writeServer.WriteLine(uName+"   >> " +msg.ToString());
            lvRecieve.Items.Add(uName + "   << " + msg.ToString());

            if(msg == "Shutdown")
            {
                clientIn.Close();
                clientOut.Close();
                System.Windows.Application.Current.Shutdown();
            }
            tbSend.Clear();
        }







        /// --FUNCTION HADER COMMENT--
        //	Function Name	:	Worker_DoWork
        //	Parameters		:	object sender, DoWorkEventArgs: default perameter
        //	Description		:   this is function will run in background and used constatnly will check for the incomming messages.
        //	Return Value	:   return nothing;
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 1;
            while (i != 0)
            {
                Thread.Sleep(50);
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                String msg = readServer.ReadLine();
                updateDelegate dele = new updateDelegate(displayReceive);
                lvRecieve.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, dele, msg);
            }
        }


        //to update the incoming message
        private delegate void updateDelegate(string msg);

        /// --FUNCTION HADER COMMENT--
        //	Function Name	:	displayReceive
        //	Parameters		:	string msg: contain the recieved message
        //	Description		:   this function will add the receive massage to the receive window
        //	Return Value	:   return nothing;
        private void displayReceive(string msg)
        {
            lvRecieve.Items.Add(msg);
        }
    }
}

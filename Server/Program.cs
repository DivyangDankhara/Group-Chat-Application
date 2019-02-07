/*
* FILE          : program.cs
* PROJECT       : Assignment #4 Windows and mobile programming
* PROGRAMMER    : Divyangbhai Dankhara
* STUDENET NO.  : 8061566
* FIRST VERSION : 2018-11-19
* Description   : THis file contain the chat server
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Threading;
namespace Server
{
    class Program
    {
        private static int maxCLientsAllowed = 10;
        private static int numberOfClientsAvilable;
        private static Dictionary<int, StreamWriter> clientWritters = clientWritters = new Dictionary<int, StreamWriter>();

        static void Main(string[] args)
        {

            ///initilize the golble variables;
            numberOfClientsAvilable = 0;

            /// creating thread which are able to handle the upto 10 clients
            Thread[] server = new Thread[maxCLientsAllowed];

            try
            {


                for (int i = 0; i < maxCLientsAllowed; i++)
                {
                    // strarting all the thread
                    server[i] = new Thread(ServerThread);
                    server[i].Start();
                }

                //sleep the current thread
                Thread.Sleep(500);


                for (int i = 0; i < maxCLientsAllowed; i++)
                {
                    /// waiting for the thread to complete
                    server[i].Join();
                }
            }
            catch (Exception e)
            {
                // this will print when any execption will accoure.
                Console.WriteLine(e);
            }
        }



        /// --FUNCTION HADER COMMENT--
        //	Function Name	:	ServerThread
        //	Parameters		:	object data : this is the default peremeter when we try to create a thread
        //	Description		:   this is a thread function which will run in background.
        //	Return Value	:   return nothing
        private static void ServerThread(object data)
        {
            bool done = false;
            NamedPipeServerStream serverIn = new NamedPipeServerStream("outGoingPipe", PipeDirection.In, maxCLientsAllowed);
            NamedPipeServerStream serverOut = new NamedPipeServerStream("InComingPipe", PipeDirection.Out, maxCLientsAllowed);

            serverIn.WaitForConnection();
            serverOut.WaitForConnection();

            //increase the number of clients
            numberOfClientsAvilable++;

            StreamReader readClient = new StreamReader(serverIn);
            StreamWriter writeClient = new StreamWriter(serverOut);
            writeClient.AutoFlush = true;



            int threadID = Thread.CurrentThread.ManagedThreadId;

            // add thread info to the globle dictionary so all thread can access that and send message to other clients.
            clientWritters.Add(threadID, writeClient);
            Console.WriteLine("Client {0} started", numberOfClientsAvilable);

            //writeClient.WriteLine("You are successfully connected");

            try
            {
                while (!done)
                {
                    String msg = readClient.ReadLine();
                    


                    foreach (var a in clientWritters)
                    {
                        if (a.Key != threadID)
                        {
                            StreamWriter client = a.Value;
                            client.WriteLine(msg);
                            client.Flush();
                        }
                    }

                    if (msg == "Shuotdown")
                    {
                        clientWritters.Remove(threadID);
                        numberOfClientsAvilable--;
                        done = true;
                        serverOut.Close();
                        serverIn.Close();
                    }
                }
                Console.WriteLine("user with treadId : " + threadID + " closed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Client with thread id :" + threadID + "had been closed or crashed");
            }
        }




























        /*
        private static int maxCLientsAllowed = 10;
        private static int numberOfClientsAvilable;
        private static Dictionary<int, StreamWriter> clientWritters = clientWritters = new Dictionary<int, StreamWriter>();

        static void Main(string[] args)
        {

            ///initilize the golble variables;
            ;
            numberOfClientsAvilable = 0;

            /// creating thread which are able to handle the upto 10 clients
            Thread[] server = new Thread[maxCLientsAllowed];

            try
            {


                for (int i = 0; i < maxCLientsAllowed; i++)
                {
                    // strarting all the thread
                    server[i] = new Thread(ServerThread);
                    server[i].Start();
                }

                //sleep the current thread
                Thread.Sleep(500);


                for (int i = 0; i < maxCLientsAllowed; i++)
                {
                    /// waiting for the thread to complete
                    server[i].Join();
                }
            }
            catch(Exception e)
            {
                // this will print when any execption will accoure.
                Console.WriteLine(e);
            }
        }



        private static void ServerThread(object data)
        {
            bool done = false;
            NamedPipeServerStream serverIn = new NamedPipeServerStream("outGoingPipe", PipeDirection.In, maxCLientsAllowed);
            NamedPipeServerStream serverOut = new NamedPipeServerStream("InComingPipe", PipeDirection.Out, maxCLientsAllowed);

            serverIn.WaitForConnection();
            serverOut.WaitForConnection();

            //increase the number of clients
            numberOfClientsAvilable++;

            StreamReader readClient = new StreamReader(serverIn);
            StreamWriter writeClient = new StreamWriter(serverOut);
            writeClient.AutoFlush = true;
            


            int threadID = Thread.CurrentThread.ManagedThreadId;

            // add thread info to the globle dictionary so all thread can access that and send message to other clients.
            clientWritters.Add(threadID,writeClient);
            Console.WriteLine("Client {0} started", numberOfClientsAvilable);

            //writeClient.WriteLine("You are successfully connected");

            try
            {
                while (!done)
                {
                    String msg = readClient.ReadLine();
                    Console.WriteLine(msg);
                    

                    foreach(var a in clientWritters)
                    {
                       if(a.Key != threadID)
                       {
                            StreamWriter client = a.Value;
                            client.WriteLine(msg);
                            client.Flush();
                       }
                    }

                    if(msg == "Shotdown")
                    {
                        clientWritters.Remove(threadID);
                        numberOfClientsAvilable--;
                        done = true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        /*static void Main(string[] args)
        {
            bool done = false;
            NamedPipeServerStream serverIn = new NamedPipeServerStream("outGoingPipe", PipeDirection.In);
            NamedPipeServerStream serverOut = new NamedPipeServerStream("InComingPipe", PipeDirection.Out);
            serverIn.WaitForConnection();
            Console.WriteLine("Server In connected");
            serverOut.WaitForConnection();
            Console.WriteLine("Server Out connected");
            StreamReader input = new StreamReader(serverIn);
            StreamWriter output = new StreamWriter(serverOut);
            output.AutoFlush = true;

            while (!done)
            {
                String inp = input.ReadLine();
                Console.WriteLine(inp);
                output.WriteLine("Read data: " + inp);
                if (inp == "Shutdown")
                {
                    done = true;
                }
            }
            Console.WriteLine("Program ended.");
        }*/
    }
}

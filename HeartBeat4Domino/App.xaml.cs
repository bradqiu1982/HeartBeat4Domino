using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HeartBeat4Domino
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string Unique = "Finisar_HeartBeat4Domino";
        private Mutex m;


        private void logmaininfo(string info)
        {
            var dominofolder= "d:\\HeartBeat4Domino";
            if (!Directory.Exists(dominofolder))
            {
                Directory.CreateDirectory(dominofolder);
            }

            var filename = dominofolder+"\\mainlog" + DateTime.Now.ToString("yyyy-MM-dd");

            if (File.Exists(filename))
            {
                var content = System.IO.File.ReadAllText(filename);
                content = content + info;
                System.IO.File.WriteAllText(filename, content);
            }
            else
            {
                System.IO.File.WriteAllText(filename, info);
            }
        }

        private void logthdinfo(string info)
        {
            var dominofolder = "d:\\HeartBeat4Domino";
            if (!Directory.Exists(dominofolder))
            {
                Directory.CreateDirectory(dominofolder);
            }

            var filename = dominofolder + "\\threadlog" + DateTime.Now.ToString("yyyy-MM-dd");
            if (File.Exists(filename))
            {
                var content = System.IO.File.ReadAllText(filename);
                content = content + info;
                System.IO.File.WriteAllText(filename, content);
            }
            else
            {
                System.IO.File.WriteAllText(filename, info);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {


            logmaininfo( "Start the HeartBeat4Domino\r\n");
            logthdinfo( "Start the HeartBeat4Domino\r\n");

            try
            {
                bool isNew;
                m = new Mutex(true, Unique, out isNew);
                if (!isNew)
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;

                
            }
            catch (Exception ex)
            {
            }


            try
            {
                
                while (true)
                {
                    var i = 0;
                    while (i < 2*30)
                    {
                        new System.Threading.ManualResetEvent(false).WaitOne(30000);
                        i = i + 1;
                    }

                    logmaininfo("wake up @" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");

                    var dominofolder = "d:\\HeartBeat4Domino"+"\\" + DateTime.Now.ToString("yyyy-MM-dd");
                    if (Directory.Exists(dominofolder))
                    {
                        continue;
                    }
                    else
                    {
                        i = 0;
                        while (i < 2 * 150)
                        {
                            new System.Threading.ManualResetEvent(false).WaitOne(30000);
                            i = i + 1;
                        }
                        Directory.CreateDirectory(dominofolder);
                        logmaininfo("wake up again@" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                    }

                    var thrd = new Thread(() => {
                        try
                        {
                            RealBeat();
                        }
                        catch (Exception ex)
                        {
                            logmaininfo("thread exception " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                        }
                    });

                    thrd.SetApartmentState(ApartmentState.STA);
                    thrd.Start();

                }
            }
            catch (Exception ex)
            {
                logmaininfo("app exit" + ex.Message + "\r\n");
                App.Current.Shutdown();
            }

        }

        private void RealBeat()
        {
            try
            {
                logthdinfo("enter real beat " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                
                var urltxt = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"heartbeat4domino.txt"));
                    var urls = urltxt.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var u in urls)
                    {
                        try
                        {
                            logthdinfo("run real beat " + u + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");

                            var browser = new WebBrowser();
                            browser.Navigate(u);
                        
                            new System.Threading.ManualResetEvent(false).WaitOne(25*60000);
                            browser.Navigate("about:blank");
                            new System.Threading.ManualResetEvent(false).WaitOne(20000);

                            browser.Dispose();
                            new System.Threading.ManualResetEvent(false).WaitOne(5000);
                            GC.Collect();
                            new System.Threading.ManualResetEvent(false).WaitOne(5000);
                            GC.Collect();
                            new System.Threading.ManualResetEvent(false).WaitOne(5000);
                            browser.Dispose();
                            new System.Threading.ManualResetEvent(false).WaitOne(5000);
                            GC.Collect();

                        logthdinfo("end real beat " + u + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                            
                        }
                        catch (Exception ex)
                        {
                            logthdinfo("nav exception " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
                            try
                            {
                                new System.Threading.ManualResetEvent(false).WaitOne(5000);
                                GC.Collect();
                            }
                            catch (Exception E)
                            { }
    
                        }
                    }


            }
            catch (Exception ex)
            {
                logthdinfo("nav exception " + ex.Message + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
            }

        }

}
}

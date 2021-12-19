using System;
using System.Diagnostics;
using System.Timers;

namespace ModelCeh
{
    class Program
    {
        public static int chug; // общее количества чугуна
        public static bool KReady; // готовность ковша принять сталь
        public static Timer kTimer; // время работы ковша
        public static bool KIsWorked; // отработал ли ковш
        public static bool UnrsActive; // есть ли свободная УНРС
        public static int slabs; // кол-во слябов

        static void Main(string[] args)
        {
            chug = 7600;
            slabs = 0;
            KReady = true;
            KIsWorked = false;
            UnrsActive = true;
            var rnd = new Random();
            
            var plavka = new[]
            {
                new PlavkaKon(370 + rnd.Next(-50, 50), 280),
                new PlavkaKon(340 + rnd.Next(-50, 50), 250),
                new PlavkaKon(430+ rnd.Next(-50, 50), 310)
            };

            var UNRSs = new[]
            {
                new UNRS(710 + rnd.Next(-100, 100) + 40 + rnd.Next(-10,10)),
                new UNRS(710 + rnd.Next(-100, 100) + 40 + rnd.Next(-10,10)),
                new UNRS(710 + rnd.Next(-100, 100) + 40 + rnd.Next(-10,10)),
                new UNRS(710 + rnd.Next(-100, 100) + 40 + rnd.Next(-10,10)),
                new UNRS(710 + rnd.Next(-100, 100) + 40 + rnd.Next(-10,10))
            };
            
            var watch = new Stopwatch();
            watch.Start();
            
            foreach (var kon in plavka)
            {
                if (kon.isReady)
                {
                    kon.isReady = false;
                    chug -= kon.Work();
                    Console.WriteLine("chug={0}, kon={1} ",chug, kon.countChug);
                }
            }
            
            
            while (chug > 0)
            {
                foreach (var kon in plavka)
                {
                    if (kon.isReady && KReady)
                    {
                        kon.isReady = false;
                        chug -= kon.Work();
                        KReady = false;
                        if (!KIsWorked)
                            SetTimer(kTimer, 50 + rnd.Next(-30, 30));
                        Console.WriteLine("chug={0}, kon={1} ",chug, kon.countChug);
                    }
                }

                if (KIsWorked)
                    UnrsActive = UnrsIsFree(UNRSs);
            }
            
            watch.Stop();
            Console.WriteLine("Time {0}, slabs {1}", watch.ElapsedMilliseconds, slabs);
        }
        
        private static void SetTimer(Timer timer, int timeWork)
        {
            timer = new Timer(timeWork);
            timer.Elapsed += OnTimerEvent;
            timer.AutoReset = false;
            timer.Enabled = true;
        }

        private static void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            KIsWorked = true;
            Console.WriteLine("Kowsh is worked");
        }

        private static bool UnrsIsFree(UNRS[] unrses)
        {
            var i = 0;
            foreach (var unrs in unrses)
            {
                i++;
                if (unrs.isReady)
                {
                    Console.WriteLine("UNRS {0} active", i);
                    KIsWorked = false;
                    KReady = true;
                    unrs.isReady = false;
                    slabs += unrs.Work();
                    return true;
                }
            }
            return false;
        }
    }

    public class PlavkaKon
    {
        public int countChug;
        public int timeWork;
        private Timer aTimer;
        public bool isReady;

        public PlavkaKon(int timeWork, int countChug)
        {
            this.countChug = countChug;
            this.timeWork = timeWork;
            isReady = true;
        }

        public int Work()
        {
            SetTimer();
            return countChug;
        }

        private void SetTimer()
        {
            aTimer = new Timer(timeWork);
            aTimer.Elapsed += OnTimerEvent;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
        }

        private void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            isReady = true;
        }
    }

    public class UNRS
    {
        public int timeWork;
        private Timer aTimer;
        public bool isReady;

        public UNRS(int timeWork)
        {
            this.timeWork = timeWork;
            isReady = true;
        }
        
        public int Work()
        {
            SetTimer();
            return 1;
        }

        private void SetTimer()
        {
            aTimer = new Timer(timeWork);
            aTimer.Elapsed += OnTimerEvent;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
        }

        private void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            isReady = true;
        }
    }
}
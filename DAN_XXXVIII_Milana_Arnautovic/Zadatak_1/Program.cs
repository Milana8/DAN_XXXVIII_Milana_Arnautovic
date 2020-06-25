using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak_1
{
    class Program
    {
        public static Semaphore Manager { get; set; }
        public static Random random = new Random();
        static EventWaitHandle waitHande = new AutoResetEvent(false);
        static Barrier barrier = new Barrier(10);
        static object l = new object();
        static int[] arrayRnd;
        static List<int> best = new List<int>();
        static List<int> best10 = new List<int>();
        static List<int> loadTimes = new List<int>();
        static int loadtime;



        public static void LoadingTrucks()

        {

            for (int i = 1; i <= 10; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Loading)); //creating threads
                t.Start(i); //starting threads
            }

        }

        public static void RoadTrucks()
        {

            waitHande.WaitOne();
            Thread.Sleep(500);
            for (int i = 1; i <= 10; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Road)); //creating threads
                t.Name = best10[i - 1].ToString();
                t.Start(i); //starting threads
            }
        }



        /// <summary>
        /// Random routes that are written to the file
        /// </summary>
        public static void Route()
        {

            arrayRnd = new int[1000];

            for (int i = 0; i < arrayRnd.Length; i++)
            {
                arrayRnd[i] = random.Next(1, 5000);
            }

            StreamWriter sw = new StreamWriter(@"../../Routes.txt");
            foreach (int number in arrayRnd)
            {
                sw.WriteLine(number);
            }
            sw.Close();
        }

        /// <summary>
        /// Method for filtering the best routes
        /// </summary>
        public static void BestRoute()
        {

            Thread.Sleep(random.Next(0, 3000));
            List<int> best = new List<int>();

            string[] red = File.ReadAllLines(@"../../Routes.txt");

            foreach (var item in red)
            {
                if (Int32.Parse(item) % 3 == 0)
                {

                    best.Add(Int32.Parse(item));
                }
            }

            best = best.OrderBy(x => x).Distinct().ToList();
            for (int i = 0; i < 10; i++)
            {
                best10.Add(best.ElementAt(i));
            }
            Console.WriteLine("The best routes are chosen.\nThe best routes are:");
            foreach (var item in best10)
            {
                Console.WriteLine(item);
            }
        }


        /// <summary>
        /// Truck loading method
        /// </summary>
        /// <param name="o"></param>
        public static void Loading(object o)
        {
            Console.WriteLine("The truck {0} is waiting to be loaded", o);
            Manager.WaitOne();
            Console.WriteLine("The truck {0} is loading", o);
            loadtime = random.Next(500, 5000);
            loadTimes.Add(loadtime);
            Console.WriteLine("The truck {0} finished loading. Loading time: {1} milliseconds", o, loadtime);
            Manager.Release(1);

            if ((int)o == 10)
            {

                Console.WriteLine("Loading is complete.");
                waitHande.Set();

            }


        }


        /// <summary>
        /// Method for a truck to travel to a specific route
        /// </summary>
        /// <param name="n"></param>
        public static void Road(object n)
        {

            Console.WriteLine("Truck {0} going to the route: {1}.", n, Thread.CurrentThread.Name);
            

            int rnd = random.Next(500, 5001);

            if (rnd > 3000)
            {
                Thread.Sleep(rnd);
                Console.WriteLine("Truck {0} did not cross the route: {1} on time. Order canceled.",
                    n, Thread.CurrentThread.Name);

                Thread.Sleep(rnd);
                Console.WriteLine("Truck {0} returning from failed road route: {2}. Road time: {1} milliseconds.",
                    n, rnd, Thread.CurrentThread.Name);


            }
            else
            {
                Thread.Sleep(rnd);
                Console.WriteLine("Truck {0} finishes road to route: {1}. Road time: {2} milliseconds.",
                    n, Thread.CurrentThread.Name, rnd);
                

            }
            barrier.SignalAndWait();
        }


        static void Main(string[] args)
        {
            Thread route = new Thread(Route);//Creatig thread
            route.Start();//thread start
            route.Join();


            Thread best = new Thread(BestRoute);//Creatig thread
            best.Start();//thread start
            best.Join();

            Manager = new Semaphore(2, 2);
            LoadingTrucks();

            RoadTrucks();

            Console.ReadLine();


        }

    }
}

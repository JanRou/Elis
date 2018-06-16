using Nancy.Hosting.Self;
using System;

namespace WebServiceStockModel {
    public class WebService : IWebServiceHost {
        private NancyHost _host;
        private bool _keepRunning = true;
        private IRegistry _registry;
        public WebService(IRegistry registry) {
            _registry = registry;
            _host = new NancyHost(new Uri(_registry.Url));
        }

        public void Start() {
            _host.Start();
        }

        public void Stop() {
            _host.Stop();
        }

        public void DebugStop() {
            if (_keepRunning) {
                _keepRunning = false;
            }
        }

        public void DebugRun() {
            Console.TreatControlCAsInput = true;
            do {
                Console.WriteLine("Debug menu:");
                Console.WriteLine("  S          - Statistics");
                Console.WriteLine("  Q / Ctrl+C - Exit");
                Console.WriteLine();
                var key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.S: {
                            Console.WriteLine($" Call counter is {_registry.CallCounter}");
                        }
                        break;
                    case ConsoleKey.Q: {
                            _keepRunning = false;
                        }
                        break;
                    case ConsoleKey.C:
                        if (key.Modifiers == ConsoleModifiers.Control) {
                            // Ctrl-c pressed, exit
                            _keepRunning = false;
                        }
                        break;
                    default: {
                            if (_keepRunning) {
                                Console.WriteLine("Type a menu letter");
                            }
                        }
                        break;
                }
            } while (_keepRunning);
        }
    }
}

using RomController;
using ShComp;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomUseAll
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("アイテムをクリックしてください。");
                var p = await GetPointAsync();
                var w = await RomWindow.FromPointAsync(p);
                if (w is null)
                {
                    Console.WriteLine($"({p.X}, {p.Y})にゲームウィンドウは見つかりませんでした。");
                    break;
                }

                Console.WriteLine($"(X:{w.Rectangle.X}, Y:{w.Rectangle.Y}, W:{w.Rectangle.Width}, H:{w.Rectangle.Height})にゲームウィンドウが見つかりました。");
                await Task.Delay(1000);

                w.ItemCountBox.Click();
                await Task.Delay(300);

                Keyboard.SendKey(0x2e);
                Keyboard.SendKey(25);
                await Task.Delay(300);

                Keyboard.SendKeys("9999");
                await Task.Delay(300);

                w.TextOkButton.Click();
                await Task.Delay(300);

                w.ItemUseButton.Click();
                await Task.Delay(1000);

                w.ItemUseButton.Click();

                Console.WriteLine();
            }
        }

        private static Task<Point> GetPointAsync()
        {
            var tcs = new TaskCompletionSource<Point>();
            var hooker = new MouseHooker();

            hooker.EventReceived += (status, point) =>
            {
                if (status == 514)
                {
                    tcs.TrySetResult(point);
                    Application.Exit();
                }
            };

            Task.Run(() =>
            {
                hooker.Start();
                Application.Run();
                hooker.Stop();
                Console.WriteLine("unhooked");
            });

            return tcs.Task;
        }
    }
}

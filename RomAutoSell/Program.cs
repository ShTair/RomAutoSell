using RomController;
using ShComp;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomAutoSell
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            Console.WriteLine("取引所のウィンドウを表示してください。");

            Console.WriteLine("アイテムの個数を入力してください。");
            var count = int.Parse(Console.ReadLine());
            Console.WriteLine($"{count}個を7回に分けて出品します。");

            Console.WriteLine($"アイテムの座標をクリックしてください。");
            var point = await GetPointAsync();
            var w = await RomWindow.FromPointAsync(point);
            Console.WriteLine($"(X:{w.Rectangle.X}, Y:{w.Rectangle.Y}, W:{w.Rectangle.Width}, H:{w.Rectangle.Height})にゲームウィンドウが見つかりました。");
            await Task.Delay(1000);

            for (int i = 0; i < 7; i++)
            {
                var c = (count - 1) / (7 - i) + 1;
                count -= c;
                Console.WriteLine($"{i + 1}回目の出品です。{c}個出品して、残り{count}個です。");

                w.SellCountBox.Click();
                await Task.Delay(300);

                Keyboard.SendKey(0x2e);
                Keyboard.SendKey(25);
                await Task.Delay(300);

                Keyboard.SendKeys(c.ToString());
                await Task.Delay(300);

                w.TextOkButton.Click();
                await Task.Delay(300);

                w.SellButton.Click();
                await Task.Delay(1000);

                if (i != 6)
                {
                    point.Click();
                    await Task.Delay(300);
                }
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

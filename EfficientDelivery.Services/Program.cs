using System;

namespace EfficientDelivery.Services
{
    class Program
    {
        static void Main(string[] args)
        {
            new LardiTransApiClient().CalculateRoute("Умань (Черкасская обл.)", "Днепр (Днепропетровская обл.)").GetAwaiter().GetResult();
        }
    }
}

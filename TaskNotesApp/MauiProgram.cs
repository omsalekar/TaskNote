using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace TaskNotesApp
{
    public static class MauiProgram
    {
      

    public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            Batteries.Init();   

            builder.UseMauiApp<App>();

            return builder.Build();
        }

}
}

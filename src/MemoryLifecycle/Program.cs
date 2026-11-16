using MemoryLifecycle.Demos;
using MemoryLifecycle.Gc;
using MemoryLifecycle.Resources;


public static class Program
{
    public static async Task Main()
    {
        //GcModeScenarios.RunParallelAllocation();
        //GcModeScenarios.MeasureForcedGen2Pause();


        //ResourceDemos.RunFinalizerDemo();
        //ResourceDemos.RunUsingDemo();
        //ResourceDemos.RunForgottenDisposeDemo();
        //ResourceDemos.RunLogFileDemo("memory-lifecycle.log");

       //1 вариант o
        using (var connection = new DatabaseConnection())
        {
            connection.Execute("SELECT * FROM Students");
        } 

        await using (var connection = new DatabaseConnection())
        {
            connection.Execute("SELECT * FROM Topics");
        } 

        GC.Collect();
        GC.WaitForPendingFinalizers();


        //2 вариант
        using (var buffer = new FileBuffer())
        {
            buffer.Write(new byte[] { 1, 2, 3, 4 });
        }

        await using (var buffer = new FileBuffer())
        {
            buffer.Write(new byte[] { 5, 6, 7 });
        }


    }
}

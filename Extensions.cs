using System.Threading.Tasks;

namespace Qonqr
{
    internal static class Extensions
    {
        public static T WaitGetResult<T>(this Task<T> task)
        {
            task.Wait();
            if (task.Exception != null)
                throw task.Exception;
            return task.Result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace jaytwo.NuGetCheck
{
    public static class TaskExtensions
    {
        public static T RunSync<T>(this Task<T> task)
        {
            return Task.Run(async () => { return await task; })
                .GetAwaiter()
                .GetResult();
        }
    }
}

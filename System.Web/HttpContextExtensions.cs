using Solid.Extensions.System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Web
{
    /// <summary>
    /// Extension methods on top of HttpContext
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Run an async function ensuring that HttpContext won't be null on the way
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        /// <param name="func">The function to run</param>
        /// <param name="preparing">A method to run while preparing to run the async function</param>
        /// <param name="running">A method to run while running the async function</param>
        public static void Run(this HttpContextBase context, Func<Task> func, Action<AsyncCallContext> preparing = null, Action<AsyncCallContext> running = null)
        {
            HttpContext.Current.Run(func, preparing, running);
        }

        /// <summary>
        /// Run an async function ensuring that HttpContext won't be null on the way
        /// </summary>
        /// <typeparam name="T">The type that the function returns</typeparam>
        /// <param name="context">The current HttpContext</param>
        /// <param name="func">The function to run</param>
        /// <param name="preparing">A method to run while preparing to run the async function</param>
        /// <param name="running">A method to run while running the async function</param>
        /// <returns>The response from the async function</returns>
        public static T Run<T>(this HttpContextBase context, Func<Task<T>> func, Action<AsyncCallContext> preparing = null, Action<AsyncCallContext> running = null)
        {
            return HttpContext.Current.Run<T>(func, preparing, running);
        }


        /// <summary>
        /// Run an async function ensuring that HttpContext won't be null on the way
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        /// <param name="func">The function to run</param>
        /// <param name="preparing">A method to run while preparing to run the async function</param>
        /// <param name="running">A method to run while running the async function</param>
        public static void Run(this HttpContext context, Func<Task> func, Action<AsyncCallContext> preparing = null, Action<AsyncCallContext> running = null)
        {
            context.Run<object>(async () =>
            {
                await func();
                return null;
            }, preparing, running);
        }

        /// <summary>
        /// Run an async function ensuring that HttpContext won't be null on the way
        /// </summary>
        /// <typeparam name="T">The type that the function returns</typeparam>
        /// <param name="context">The current HttpContext</param>
        /// <param name="func">The function to run</param>
        /// <param name="preparing">A method to run while preparing to run the async function</param>
        /// <param name="running">A method to run while running the async function</param>
        /// <returns>The response from the async function</returns>
        public static T Run<T>(this HttpContext context, Func<Task<T>> func, Action<AsyncCallContext> preparing = null, Action<AsyncCallContext> running = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), "HttpContext cannot be null");

            var callContext = AsyncCallContext.Current;

            try
            {
                callContext.Add("HttpContext", context);
                if (preparing != null)
                    preparing(callContext);
            }
            catch
            {
                callContext.Clear();
                throw;
            }

            var task = Task.Run<T>(async () =>
            {
                try
                {
                    if (running != null)
                        running(callContext);

                    var result = await func();
                    return result;
                }
                finally
                {
                    callContext.Clear();
                }
            });

            task.Wait();
            return task.Result;
        }
    }
}

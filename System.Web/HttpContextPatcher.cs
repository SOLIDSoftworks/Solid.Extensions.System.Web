using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Solid.Extensions.System.Web
{
    /// <summary>
    /// HttpContext patcher
    /// </summary>
    public static class HttpContextPatcher
    {
        /// <summary>
        /// Patches HttpContext.Current to check the AsyncCallContext whether HttpContext.Current originally returns null. This method is run automatically.
        /// </summary>
        public static void PatchHttpContext()
        {
            var harmony = HarmonyInstance.Create("System.Web.HttpContext");
            var method = typeof(HttpContext).GetMethod("get_Current", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public);
            if (harmony.GetPatchInfo(method) != null)
                throw new ApplicationException("HttpContext already patched");
            var postfix = typeof(HttpContextPatcher).GetMethod("GetHttpContextCurrent", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(method, postfix: new HarmonyMethod(postfix));
        }

        private static HttpContext GetHttpContextCurrent(HttpContext __result)
        {
            if (__result != null) return __result;
            return AsyncCallContext.Current.Get("HttpContext") as HttpContext;
        }
    }
}

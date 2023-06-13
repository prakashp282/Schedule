using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Scheduler.Extensions
{
    /// <summary>
    /// Enum extensions
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Convert Enum to Number
        /// </summary>
        /// <param name="self">Enum</param>
        /// <returns>Number represented by Enum</returns>
        public static int ToIntValue(this Enum self)
        {
            return Convert.ToInt16(self);
        }


        /// <summary>
        /// Get the description tag content of Enum(Description)
        /// </summary>
        /// <param name="self">Enum</param>
        /// <param name="defaultDesc">Default description</param>
        /// <returns> Enum description tag content </returns>
        public static string GetDescription(this Enum self, string defaultDesc = "")
        {
            var field = self.GetType().GetRuntimeField(self.ToString());
            var desc = defaultDesc;

            if (field != null)
            {
                desc = field.GetCustomAttributes<DescriptionAttribute>()
                    .FirstOrDefault()?.Description ?? defaultDesc;
            }

            return desc;
        }

    }
}

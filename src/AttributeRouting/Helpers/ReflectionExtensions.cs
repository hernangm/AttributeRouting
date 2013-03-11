﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AttributeRouting.Helpers
{
    public static class ReflectionExtensions
    {
        public static string GetConventionalAreaName(this Type type)
        {
            var typeNameSpace = type.Namespace;
            if (typeNameSpace == null)
                return null;

            return typeNameSpace
                .Split('.')
                .SkipWhile(s => !s.ValueEquals("Areas"))
                .Skip(1) // move past "Areas"
                .Take(1) // take the next
                .FirstOrDefault();
        }

        public static IEnumerable<MethodInfo> GetActionMethods(this Type type, bool inheritActionsFromBaseController)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;

            if (!inheritActionsFromBaseController)
                flags |= BindingFlags.DeclaredOnly;

            return type.GetMethods(flags);
        }

        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Type type, bool inherit)
        {
            return type.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Type type, bool inherit)
        {
            return type.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().FirstOrDefault();
        }

        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MethodInfo method, bool inherit)
        {
            return method.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
        }

        /// <summary>
        /// Get controllers in given assembly, depending on target framework (MVC, WebAPI)
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="controllerType"> </param>
        /// <returns></returns>
        public static IEnumerable<Type> GetControllerTypes(this Assembly assembly, Type controllerType)
        {
            return from type in assembly.GetTypes()
                   where !type.IsAbstract && controllerType.IsAssignableFrom(type)
                   select type;
        }

        public static string GetControllerName(this Type type)
        {
            return Regex.Replace(type.Name, "Controller$", "");
        }

		public static bool IsAsyncController(this Type type)
		{
			for (var t = type.BaseType; t != typeof(object); t = t.BaseType)
			{
				if (t.FullName.ValueEquals("System.Web.Mvc.AsyncController"))
					return true;
			}
			return false;
		}

    }
}
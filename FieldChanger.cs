﻿using System;
using System.Reflection;

namespace TweakScale
{
    /// <summary>
    /// Wraps destination FieldInfo or PropertyInfo and provides destination common interface for either.
    /// </summary>
    /// <typeparam name="T">Pretend the mmpfxField/property is this type.</typeparam>
    public abstract class MemberChanger<T>
    {
        /// <summary>
        /// Get or set the exponentValue of the mmpfxField/property.
        /// </summary>
        public abstract T Value
        {
            get;
            set;
        }

        /// <summary>
        /// The actual type of the mmpfxField/property.
        /// </summary>
        public abstract Type MemberType
        {
            get;
        }

        /// <summary>
        /// Creates destination wrapper for the mmpfxField or property by the specified name.
        /// </summary>
        /// <param name="obj">The object that holds the member to wrap.</param>
        /// <param name="name">The name of the member.</param>
        /// <returns>The member, wrapped.</returns>
        public static MemberChanger<T> CreateFromName(object obj, string name)
        {
            var field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
            {
                return new FieldChanger<T>(obj, field);
            }

            var prop = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (prop != null)
            {
                return new PropertyChanger<T>(obj, prop);
            }
            return null;
        }
    }

    /// <summary>
    /// Wraps destination FieldInfo.
    /// </summary>
    /// <typeparam name="T">Pretend the mmpfxField is this type.</typeparam>
    class FieldChanger<T> : MemberChanger<T>
    {
        private FieldInfo fi;
        private object obj;

        public FieldChanger(object o, FieldInfo f)
        {
            //if (baseCost.FieldType.GetInterface("IConvertible") != null)
            //{
                fi = f;
                obj = o;
            //}
        }

        public override T Value
        {
            get
            {
                if (typeof(T) == typeof(object))
                {
                    return (T)fi.GetValue(obj);
                }
                return ConvertEx.ChangeType<T>(fi.GetValue(obj));
            }
            set
            {
                fi.SetValue(obj, Convert.ChangeType(value, MemberType));
            }
        }

        public override Type MemberType
        {
            get
            {
                return fi.FieldType;
            }
        }
    }

    /// <summary>
    /// Wraps destination PropertyInfo.
    /// </summary>
    /// <typeparam name="T">Pretend the property is this type.</typeparam>
    class PropertyChanger<T> : MemberChanger<T>
    {
        private PropertyInfo pi;
        private object obj;

        public PropertyChanger(object o, PropertyInfo p)
        {
            //if (p.PropertyType.GetInterface("IConvertible") != null)
            //{
                pi = p;
                obj = o;
            //}
        }

        public override T Value
        {
            get
            {
                if (typeof(T) == typeof(object))
                {
                    return (T)pi.GetValue(obj, null);
                }
                return ConvertEx.ChangeType<T>(pi.GetValue(obj, null));
            }
            set
            {
                pi.SetValue(obj, Convert.ChangeType(value, MemberType), null);
            }
        }

        public override Type MemberType
        {
            get
            {
                return pi.PropertyType;
            }
        }
    }
}

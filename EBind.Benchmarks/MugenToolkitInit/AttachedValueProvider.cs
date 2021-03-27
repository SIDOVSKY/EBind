#nullable disable
#region Copyright

// ****************************************************************************
// <copyright file="AttachedValueProvider.cs">
// Copyright (c) 2012-2017 Vyacheslav Volkov
// Modifications Copyright (c) 2020 Vadim Sedov
// </copyright>
// ****************************************************************************
// <author>Vyacheslav Volkov</author>
// <email>vvs0205@outlook.com</email>
// <project>MugenMvvmToolkit</project>
// <web>https://github.com/MugenMvvmToolkit/MugenMvvmToolkit</web>
// <license>
// See license.txt in this solution or http://opensource.org/licenses/MS-PL
// </license>
// ****************************************************************************

#endregion Copyright

using System;
using System.Runtime.CompilerServices;
using MugenMvvmToolkit.Collections;
using MugenMvvmToolkit.Models;

namespace MugenMvvmToolkit.Infrastructure
{
    public class AttachedValueProvider : AttachedValueProviderBase
    {
        private static readonly ConditionalWeakTable<object, AttachedValueDictionary>.CreateValueCallback
            _createDictionaryDelegate = _ => new AttachedValueDictionary();

        private readonly ConditionalWeakTable<object, AttachedValueDictionary> _internalDictionary =
            new ConditionalWeakTable<object, AttachedValueDictionary>();

        public event Func<object, bool> ClearHandler;

        public event Func<object, bool, LightDictionaryBase<string, object>> GetOrAddAttachedDictionaryHandler;

        protected override bool ClearInternal(object item)
        {
            var handler = ClearHandler?.GetInvocationList();
            if (handler != null)
            {
                for (int i = 0; i < handler.Length; i++)
                {
                    if (((Func<object, bool>)handler[i]).Invoke(item))
                        return true;
                }
            }
            var model = item as NotifyPropertyChangedBase;
            if (model != null)
            {
                ClearAttachedValues(model);
                return true;
            }

            return _internalDictionary.Remove(item);
        }

        protected override LightDictionaryBase<string, object> GetOrAddAttachedDictionary(object item, bool addNew)
        {
            var handler = GetOrAddAttachedDictionaryHandler?.GetInvocationList();
            if (handler != null)
            {
                for (int i = 0; i < handler.Length; i++)
                {
                    var result = ((Func<object, bool, LightDictionaryBase<string, object>>)handler[i]).Invoke(item, addNew);
                    if (result != null)
                        return result;
                }
            }
            var model = item as NotifyPropertyChangedBase;
            if (model != null)
                return GetOrAddAttachedValues(model, true);

            if (addNew)
                return _internalDictionary.GetValue(item, _createDictionaryDelegate);

            _internalDictionary.TryGetValue(item, out var value);
            return value;
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using EBind.PropertyAccessors;

namespace EBind.Platform
{
    /// <summary>
    /// A facade for direct indication of value-type parameters for internal generics instantiated with reflection.
    /// See a <see href="https://stackoverflow.com/a/12840169">detailed description</see>.
    /// </summary>
    [Linker.Preserve(AllMembers = true), ExcludeFromCodeCoverage]
    public static class AotCompilerHints
    {
        internal static readonly bool @true = true;

        private static void Include()
        {
            Include<int>();
            Include<bool>();
            Include<char>();
            Include<sbyte>();
            Include<byte>();
            Include<short>();
            Include<ushort>();
            Include<uint>();
            Include<long>();
            Include<ulong>();
            Include<float>();
            Include<double>();
            Include<nint>();
            Include<nuint>();
            Include<nfloat>();
            Include<IntPtr>();
            Include<System.Drawing.SizeF>();
            Include<System.Drawing.RectangleF>();
            Include<System.Drawing.PointF>();
            Include<CoreGraphics.CGSize>();
            Include<CoreGraphics.CGRect>();
            Include<CoreGraphics.CGPoint>();
            Include<CoreGraphics.CGAffineTransform>();
            Include<UIKit.UIEdgeInsets>();
            Include<CoreAnimation.CATransform3D>();
        }

        /// <summary>
        /// Points a value-type out as being used by the <see cref="EBind"/> for the AOT compiler.
        /// </summary>
        /// <typeparam name="TMember">Type of a non-standard struct to pre-seed in the AOT compiler.</typeparam>
        public static void Include<TMember>() where TMember : struct
        {
            if (@true) return;

            default(RefTargetPropertyAccessor<TMember>)!.Get(default);
            default(RefTargetPropertyAccessor<TMember>)!.Set(default, default);
        }

        /// <summary>
        /// Points a type set out as being used by the <see cref="EBind"/> for the AOT compiler.<br/>
        /// Useful when <typeparamref name="TTarget"/> is a struct or <typeparamref name="TMember"/>
        /// is a platform- or architecture-varying value-type (native e.g <see cref="nint"/>)
        /// </summary>
        /// <typeparam name="TTarget">Type of a member owner.</typeparam>
        /// <typeparam name="TMember">Type of a member.</typeparam>
        public static void Include<TTarget, TMember>()
        {
            if (@true) return;

            default(PropertyAccessor<TTarget, TMember>)!.Get(default);
            default(PropertyAccessor<TTarget, TMember>)!.Set(default, default);

            default(RefTargetPropertyAccessor<TMember>)!.Get(default);
            default(RefTargetPropertyAccessor<TMember>)!.Set(default, default);
        }
    }
}

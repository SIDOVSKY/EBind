using System;

namespace EBind
{
    /// <summary>
    /// Options that alter bindings' behavior
    /// </summary>
    [Flags]
    public enum BindFlag
    {
        /// <summary>
        /// Default binding mode without any additional specifications.
        /// One-Way, Source-to-Target (right to left), invoke after construction.
        /// </summary>
        None = 0b0,

        /// <summary>
        /// States that bindings should be created to work in a Two-Way mode.
        /// A warning is printed to the console if the binding does not have a trigger for one side.
        /// </summary>
        TwoWay = 0b1,

        /// <summary>
        /// States that the binding should fire only once.
        /// It will be disposed after the first triggering (the initial one if <see cref="NoInitialTrigger"/> is not specified).
        /// </summary>
        OneTime = 0b10,

        /// <summary>
        /// States that the binding should not trigger just after the construction but only according to the triggers (on value changes).
        /// </summary>
        NoInitialTrigger = 0b1000
    }
}

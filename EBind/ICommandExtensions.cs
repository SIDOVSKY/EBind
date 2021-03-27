using System.Windows.Input;

namespace EBind
{
    /// <summary>
    /// Extension methods for an <see cref="ICommand"/>.
    /// </summary>
    public static class ICommandExtensions
    {
        /// <summary>
        /// Execute the <paramref name="command"/> with <c>null</c> parameter
        /// only if it <see cref="ICommand.CanExecute(object)"/>.
        /// </summary>
        public static void TryExecute(this ICommand command) => command.TryExecute<object?>(null);

        /// <summary>
        /// Execute the <paramref name="command"/> with <paramref name="parameter"/>
        /// only if it <see cref="ICommand.CanExecute(object)"/>.
        /// </summary>
        public static void TryExecute<T>(this ICommand command, T parameter)
        {
            if (!command.CanExecute(parameter))
                return;

            command.Execute(parameter);
        }
    }
}

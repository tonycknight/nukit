namespace Nukit.Console
{
    internal interface IConsoleWriterFactory
    {
        IConsoleWriter CreateWriter(BaseCommandSettings settings);
    }

    internal class ConsoleWriterFactory : IConsoleWriterFactory
    {
        public IConsoleWriter CreateWriter(BaseCommandSettings settings) => new ConsoleWriter(settings);
    }
}

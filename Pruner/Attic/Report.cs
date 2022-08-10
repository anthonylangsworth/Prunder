namespace Pruner.Attic
{
    internal abstract class Report
    {
        public Report()
        {

        }

        public abstract void Generate(TextWriter textWriter);
    }
}

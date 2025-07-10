using JobShared.Jobs;

namespace JobShared.Registries
{
    public static class JobTypeRegistry
    {
        private static readonly Dictionary<string, Type> _types = new()
    {
        { "PrintJob", typeof(PrintJob) },
        { "EmailJob", typeof(EmailJob) },
        { "MathJob", typeof(MathJob) },
    };

        public static Type? Resolve(string typeName)
        {
            return _types.TryGetValue(typeName, out var type) ? type : null;
        }
    }
}

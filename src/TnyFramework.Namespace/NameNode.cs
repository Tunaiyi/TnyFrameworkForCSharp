namespace TnyFramework.Namespace
{

    public class NameNode<TValue>
    {
        public string Name { get; }

        public long Id { get; }

        public TValue Value { get; }

        public long Version { get; }

        public long Revision { get; }

        public bool Delete { get; }

        public NameNode(string name, long id, TValue value, long version, long revision, bool delete)
        {
            Name = name;
            Id = id;
            Value = value;
            Version = version;
            Revision = revision;
            Delete = delete;
        }

        public override string ToString()
        {
            return
                $"{nameof(Name)}: {Name}, {nameof(Id)}: {Id}, {nameof(Value)}: {Value}, {nameof(Version)}: {Version}, {nameof(Revision)}: {Revision}, {nameof(Delete)}: {Delete}";
        }
    }

}

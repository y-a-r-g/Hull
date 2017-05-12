namespace Hull.GameServer.Interfaces {
    public interface IIndexedAccess<out TValue, in TIndex> {
        TValue this[TIndex index] { get; }
    }
}

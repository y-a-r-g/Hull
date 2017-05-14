namespace Hull.GameServer.Interfaces {
    public interface IIndexedAccess<TValue, in TIndex> {
        bool TryGetValue(TIndex index, out TValue value);
    }
}
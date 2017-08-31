namespace Hull.Unity.Pooling {
    public interface IPoolable {
        void Instantiated();
        void Pooled();
    }
}
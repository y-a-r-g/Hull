namespace Hull.Types {
    public struct ImmutableStrinig {
        public ImmutableStrinig(string value) {
            Value = value;
        }

        public readonly string Value;

        public override string ToString() {
            return Value;
        }
    }
}

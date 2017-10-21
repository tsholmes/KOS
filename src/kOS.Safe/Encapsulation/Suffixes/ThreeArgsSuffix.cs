namespace kOS.Safe.Encapsulation.Suffixes
{
    public class ThreeArgsSuffix<T, TReturn, TParam, TParam2, TParam3> : SuffixBase<T, ThreeArgsSuffix<T, TReturn, TParam, TParam2, TParam3>.Del>
        where T : Structure where TReturn : Structure where TParam : Structure where TParam2 : Structure where TParam3 : Structure
    {
        public delegate TReturn Del(TParam one, TParam2 two, TParam3 three);

        public ThreeArgsSuffix(GetDel get, string description = "") : base(get, description) { }

        protected override object Call(Del del, object[] args)
        {
            return del((TParam)args[0], (TParam2)args[1], (TParam3)args[2]);
        }
    }

    public class ThreeArgsVoidSuffix<T, TParam, TParam2, TParam3> : SuffixBase<T, ThreeArgsVoidSuffix<T, TParam, TParam2, TParam3>.Del>
        where T : Structure where TParam : Structure where TParam2 : Structure where TParam3: Structure
    {
        public delegate void Del(TParam one, TParam2 two, TParam3 three);

        public ThreeArgsVoidSuffix(GetDel get, string description = "") : base(get, description) { }

        protected override object Call(Del del, object[] args)
        {
            del((TParam)args[0], (TParam2)args[1], (TParam3)args[2]);
            return null;
        }
    }
}
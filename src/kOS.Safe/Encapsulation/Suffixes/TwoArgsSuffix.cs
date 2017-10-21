namespace kOS.Safe.Encapsulation.Suffixes
{
    public class TwoArgsSuffix<T, TReturn, TParam, TParam2> : SuffixBase<T, TwoArgsSuffix<T, TReturn, TParam, TParam2>.Del>
        where T : Structure where TReturn : Structure where TParam : Structure where TParam2 : Structure
    {
        public delegate TReturn Del(TParam one, TParam2 two);

        public TwoArgsSuffix(GetDel get, string description = "") : base(get, description) { }

        protected override object Call(Del del, object[] args)
        {
            return del((TParam)args[0], (TParam2)args[1]);
        }
    }

    public class TwoArgsVoidSuffix<T, TParam, TParam2> : SuffixBase<T, TwoArgsVoidSuffix<T, TParam, TParam2>.Del>
        where T : Structure where TParam : Structure where TParam2 : Structure
    {
        public delegate void Del(TParam one, TParam2 two);

        public TwoArgsVoidSuffix(GetDel get, string description = "") : base(get, description) { }

        protected override object Call(Del del, object[] args)
        {
            del((TParam)args[0], (TParam2)args[1]);
            return null;
        }
    }
}
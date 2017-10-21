namespace kOS.Safe.Encapsulation.Suffixes
{
    public class VarArgsSuffix<T, TReturn, TParam> : SuffixBase<T, VarArgsSuffix<T, TReturn, TParam>.Del>
        where T : Structure where TReturn : Structure where TParam : Structure
    {
        public delegate TReturn Del(params TParam[] arguments);

        public VarArgsSuffix(GetDel get, string description = "") : base(get, description) { }

        protected override object Call(Del del, object[] args)
        {
            del((TParam[])args[0]);
            return null;
        }
    }
}
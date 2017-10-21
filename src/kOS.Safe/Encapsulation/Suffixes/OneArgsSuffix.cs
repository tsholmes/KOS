namespace kOS.Safe.Encapsulation.Suffixes
{
    public class OneArgsSuffix<T,TReturn,TParam> : SuffixBase<T, OneArgsSuffix<T,TReturn,TParam>.Del> where T : Structure where TReturn : Structure where TParam : Structure
    {
        public delegate TReturn Del(TParam one);

        public OneArgsSuffix(GetDel get, string description = "") :base(get, description) {}

        protected override object Call(Del del, object[] args)
        {
            return del((TParam)args[0]);
        }
    }
}
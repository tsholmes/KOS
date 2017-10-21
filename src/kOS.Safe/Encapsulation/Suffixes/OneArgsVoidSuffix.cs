namespace kOS.Safe.Encapsulation.Suffixes
{
    public class OneArgsVoidSuffix<T,TParam> : SuffixBase<T,OneArgsVoidSuffix<T,TParam>.Del> where T : Structure where TParam : Structure
    {
        public delegate void Del(TParam argOne);

        public OneArgsVoidSuffix(GetDel get, string description = ""):base(get, description) {}

        protected override object Call(Del del, object[] args)
        {
            del((TParam)args[0]);
            return null;
        }
    }
}
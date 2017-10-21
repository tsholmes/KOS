namespace kOS.Safe.Encapsulation.Suffixes
{
    public class NoArgsSuffix<T, TReturn> : SuffixBase<T, NoArgsSuffix<T,TReturn>.Del> where T : Structure where TReturn : Structure
    {
        public delegate TReturn Del();

        public NoArgsSuffix(GetDel get, string description = ""):base(get, description) {}

        protected override object Call(Del del, object [] args)
        {
            return del();
        }
    }

}
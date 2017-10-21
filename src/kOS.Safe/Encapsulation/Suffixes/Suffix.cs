namespace kOS.Safe.Encapsulation.Suffixes
{
    public class Suffix<T, TReturn> : SuffixBase<T, SuffixGetDlg<TReturn>> where T : Structure where TReturn : Structure
    {
        public Suffix(GetDel get, string description = ""):base(get, description)
        {
        }

        protected override object Call(SuffixGetDlg<TReturn> del, object[] args)
        {
            return del();
        }
    }
}

namespace kOS.Safe.Encapsulation.Suffixes
{
    /// <summary>
    /// Although we always have a dummy return from every call in the VM,
    /// in the underlying C# a suffix might be backed by a Delegate that
    /// returns void.  Use this construct for suffixes that take no args
    /// and return nothing.  (that are only called for their effect).
    /// </summary>
    public class NoArgsVoidSuffix<T> : SuffixBase<T, NoArgsVoidSuffix<T>.Del> where T : Structure
    {
        public delegate void Del();

        public NoArgsVoidSuffix(GetDel get, string description = ""):base(get, description) {}

        protected override object Call(Del del, object[] args)
        {
            del();
            return null;
        }
    }
}
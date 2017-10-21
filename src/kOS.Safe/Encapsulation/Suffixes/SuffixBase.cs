using System;

namespace kOS.Safe.Encapsulation.Suffixes
{
    public abstract class SuffixBase<T, Del> : ISuffix where T : Structure
    {
        private GetDel get;

        public delegate Del GetDel(T structure);

        protected SuffixBase(GetDel get, string description)
        {
            this.get = get;
            Description = description;
        }

        public ISuffixResult Get(Structure structure)
        {
            Del del = get((T)structure);
            return new DelegateSuffixResult(del as Delegate, (args) => Call(del, args));
        }

        protected abstract object Call(Del del, object[] args);

        public string Description { get; private set; }
    }
}

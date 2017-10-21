using System;

namespace kOS.Safe.Encapsulation.Suffixes
{
    public class SetSuffix<T, TValue> : Suffix<T, TValue>, ISetSuffix where T : Structure where TValue : Structure
    {
        private readonly SetDel set;

        public delegate SuffixSetDlg<TValue> SetDel(T structure);

        public SetSuffix(GetDel get, SetDel set, string description = "")
            : base(get, description)
        {
            this.set = set;
        }

        public virtual void Set(Structure structure, object value)
        {
            SuffixSetDlg<TValue> setter = set((T)structure);
            TValue toSet;
            if (value is TValue)
            {
                toSet = (TValue) value;
            }
            else
            {
                Structure newValue = Structure.FromPrimitiveWithAssert(value);  // Handles converting built in types to Structures that Convert.ChangeType() can't.
                toSet = (TValue)Convert.ChangeType(newValue, typeof(TValue));
            }
            setter(toSet);
        }
    }
}
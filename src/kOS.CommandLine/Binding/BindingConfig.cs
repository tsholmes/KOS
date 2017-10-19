using kOS.Safe.Binding;
using kOS.Safe.Utilities;
using kOS.Safe;

namespace kOS.CommandLine.Binding
{
    [Binding("ksp")]
    public class BindingConfig : kOS.Safe.Binding.SafeBinding
    {
        public override void AddTo(SafeSharedObjects shared)
        {
            shared.BindingMgr.AddGetter("CONFIG", () => SafeHouse.Config);
        }
    }
}

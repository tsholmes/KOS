using kOS.Safe.Binding;
using kOS.Suffixed;
using kOS.Suffixed.Part;
using kOS.Utilities;
using kOS.Module;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.Binding
{
    [Binding("ksp")]
    public class MissionSettings : Binding
    {
        private VesselTarget ship;
        private SharedObjects sharedObj;

        public override void AddTo(SharedObjects shared)
        {
            sharedObj = shared;

            shared.BindingMgr.AddGetter("CORE", () => new Core((kOSProcessor)shared.Processor, shared));
            shared.BindingMgr.AddGetter("SHIP", () => ship ?? (ship = VesselTarget.CreateOrGetExisting(shared)));
            // These are now considered shortcuts to SHIP:suffix
            foreach (var scName in VesselTarget.ShortCuttableShipSuffixes)
            {
                shared.BindingMgr.AddGetter(scName, () => VesselShortcutGetter(scName));
            }

            shared.BindingMgr.AddSetter("TARGET", val =>
            {
                if (shared.Vessel != FlightGlobals.ActiveVessel)
                {
                    throw new kOS.Safe.Exceptions.KOSSituationallyInvalidException("TARGET can only be set for the Active Vessel");
                }
                var targetable = val as IKOSTargetable;
                if (targetable != null)
                {
                    VesselUtils.SetTarget(targetable, shared.Vessel);
                    return;
                }

                if (!string.IsNullOrEmpty(val.ToString().Trim()))
                {
                    var body = VesselUtils.GetBodyByName(val.ToString());
                    if (body != null)
                    {
                        VesselUtils.SetTarget(body, shared.Vessel);
                        return;
                    }

                    var vessel = VesselUtils.GetVesselByName(val.ToString(), shared.Vessel);
                    if (vessel != null)
                    {
                        VesselUtils.SetTarget(vessel, shared.Vessel);
                        return;
                    }
                }
                //Target not found, if we have a target we clear it
                VesselUtils.UnsetTarget();
            });

            shared.BindingMgr.AddGetter("TARGET", () =>
            {
                if (shared.Vessel != FlightGlobals.ActiveVessel)
                {
                    throw new kOS.Safe.Exceptions.KOSSituationallyInvalidException("TARGET can only be returned for the Active Vessel");
                }
                var currentTarget = FlightGlobals.fetch.VesselTarget;

                var vessel = currentTarget as Vessel;
                if (vessel != null)
                {
                    return VesselTarget.CreateOrGetExisting(vessel, shared);
                }
                var body = currentTarget as CelestialBody;
                if (body != null)
                {
                    return BodyTarget.CreateOrGetExisting(body, shared);
                }
                var dockingNode = currentTarget as ModuleDockingNode;
                if (dockingNode != null)
                {
                    return new DockingPortValue(dockingNode, shared);
                }

                throw new kOS.Safe.Exceptions.KOSSituationallyInvalidException("No TARGET is selected");
            });

            shared.BindingMgr.AddGetter("HASTARGET", () =>
            {
                if (shared.Vessel != FlightGlobals.ActiveVessel) return false;
                // the ship has a target if the object does not equal null.
                return FlightGlobals.fetch.VesselTarget != null;
            });

        }

        public object VesselShortcutGetter(string name)
        {
            ISuffixResult suffix =ship.GetSuffix(name);
            if (!suffix.HasValue)
                suffix.Invoke(sharedObj.Cpu);
            return suffix.Value;
        }

        public override void Update()
        {
            base.Update();
            if (ship == null)
            {
                ship = VesselTarget.CreateOrGetExisting(sharedObj);
                ship.LinkCount++;
            }
            else if (ship.Vessel == null)
            {
                ship.LinkCount--;
                ship = VesselTarget.CreateOrGetExisting(sharedObj);
                ship.LinkCount++;
            }
            else if (!ship.Vessel.id.Equals(sharedObj.Vessel.id))
            {
                ship.LinkCount--;
                ship = VesselTarget.CreateOrGetExisting(sharedObj);
                ship.LinkCount++;
            }
        }
    }
}
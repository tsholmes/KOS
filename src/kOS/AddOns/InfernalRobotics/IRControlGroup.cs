﻿using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Suffixed;
using kOS.Safe.Exceptions;
using System.Collections.Generic;

namespace kOS.AddOns.InfernalRobotics
{
    [kOS.Safe.Utilities.KOSNomenclature("IRControlGroup")]
    public class IRControlGroupWrapper : Structure
    {
        private readonly IRWrapper.IControlGroup cg;
        private readonly SharedObjects shared;

        public IRControlGroupWrapper(IRWrapper.IControlGroup init, SharedObjects shared)
        {
            cg = init;
            this.shared = shared;
            InitializeSuffixes();
        }

        private void InitializeSuffixes()
        {
            AddSuffix("NAME", new SetSuffix<StringValue>(() => cg.Name, value => cg.Name = value));
            AddSuffix("SPEED", new SetSuffix<ScalarValue>(() => cg.Speed, value => cg.Speed = value));
            AddSuffix("EXPANDED", new SetSuffix<BooleanValue>(() => cg.Expanded, value => cg.Expanded = value));
            AddSuffix("FORWARDKEY", new SetSuffix<StringValue>(() => cg.ForwardKey, value => cg.ForwardKey = value));
            AddSuffix("REVERSEKEY", new SetSuffix<StringValue>(() => cg.ReverseKey, value => cg.ReverseKey = value));

            AddSuffix("SERVOS", new NoArgsSuffix<ListValue> (GetServos));

            AddSuffix("MOVERIGHT", new NoArgsVoidSuffix(MoveRight));
            AddSuffix("MOVELEFT", new NoArgsVoidSuffix(MoveLeft));
            AddSuffix("MOVECENTER", new NoArgsVoidSuffix(MoveCenter));
            AddSuffix("MOVENEXTPRESET", new NoArgsVoidSuffix(MoveNextPreset));
            AddSuffix("MOVEPREVPRESET", new NoArgsVoidSuffix(MovePrevPreset));
            AddSuffix("STOP", new NoArgsVoidSuffix(Stop));

            AddSuffix("VESSEL", new Suffix<VesselTarget>(GetVessel));
        }

        public ListValue GetServos()
        {
            var list = new List <IRServoWrapper> ();

            if(IRWrapper.APIReady)
            {
                foreach(IRWrapper.IServo s in cg.Servos)
                {
                    list.Add(new IRServoWrapper(s, shared));
                }
            }
            
            return ListValue.CreateList(list);
        }

        public VesselTarget GetVessel()
        {
            if (IRWrapper.APIReady) 
            {
                //IF IR version is 0.21.4 or below IR API may return null, but it also means that IR API only returns groups for ActiveVessel
                //so returning the ActiveVessel should work
                return cg.Vessel != null ? VesselTarget.CreateOrGetExisting(cg.Vessel, shared) : VesselTarget.CreateOrGetExisting(FlightGlobals.ActiveVessel, shared);
            } 
            else
                return VesselTarget.CreateOrGetExisting(shared.Vessel, shared); //user should not be able to get here anyway, but to avoid null will return shared.Vessel
        }

        public void ThrowIfNotCPUVessel()
        {
            VesselTarget vt = GetVessel();
            if (vt.Vessel.id != shared.Vessel.id)
                throw new KOSWrongCPUVesselException();
        }

        public void MoveRight()
        {
            ThrowIfNotCPUVessel();
            cg.MoveRight();
        }

        public void MoveLeft()
        {
            ThrowIfNotCPUVessel();
            cg.MoveLeft();
        }

        public void MoveCenter()
        {
            ThrowIfNotCPUVessel();
            cg.MoveCenter();
        }

        public void MoveNextPreset()
        {
            ThrowIfNotCPUVessel();
            cg.MoveNextPreset();
        }

        public void MovePrevPreset()
        {
            ThrowIfNotCPUVessel();
            cg.MovePrevPreset();
        }

        public void Stop()
        {
            ThrowIfNotCPUVessel();
            cg.Stop();
        }
    }
}
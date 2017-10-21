using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using kOS.Safe.Serialization;

namespace kOS.Safe.Encapsulation
{
    [KOSNomenclature("Structure")]
    public abstract class Structure : ISuffixed 
    {
        private SuffixMap instanceSuffixes;

        protected Structure(SuffixMap instanceSuffixes)
        {
            this.instanceSuffixes = instanceSuffixes;
        }
        
        public string KOSName { get { return KOSNomenclature.GetKOSName(GetType()); } }

        protected static SuffixMap StructureSuffixes<T>() where T : Structure
        {
            SuffixMap suffixes = new SuffixMap();
            
            suffixes.AddSuffix("TOSTRING", new NoArgsSuffix<T, StringValue>((structure) => () => structure.ToString()));
            suffixes.AddSuffix("HASSUFFIX", new OneArgsSuffix<T, BooleanValue, StringValue>((structure) => structure.HasSuffix));
            suffixes.AddSuffix("SUFFIXNAMES", new NoArgsSuffix<T, ListValue>((structure) => structure.GetSuffixNames));
            suffixes.AddSuffix("ISSERIALIZABLE", new NoArgsSuffix<T, BooleanValue>((structure) => () => structure is SerializableStructure));
            suffixes.AddSuffix("TYPENAME", new NoArgsSuffix<T, StringValue>((structure) => () => new StringValue(structure.KOSName)));
            suffixes.AddSuffix("ISTYPE", new OneArgsSuffix<T, BooleanValue,StringValue>((structure) => structure.GetKOSIsType));
            suffixes.AddSuffix("INHERITANCE", new NoArgsSuffix<T, StringValue>((structure) => structure.GetKOSInheritance));

            return suffixes;
        }

        public virtual bool SetSuffix(string suffixName, object value)
        {
            return instanceSuffixes.SetSuffix(suffixName, this, value);
        }

        public virtual ISuffixResult GetSuffix(string suffixName)
        {
            return instanceSuffixes.GetSuffix(suffixName, this);
        }
        
        public virtual BooleanValue HasSuffix(StringValue suffixName)
        {
            return instanceSuffixes.HasSuffix(suffixName);
        }
        
        public virtual ListValue GetSuffixNames()
        {
            return instanceSuffixes.GetSuffixNames();
        }
        
        public virtual BooleanValue GetKOSIsType(StringValue queryTypeName)
        {
            // We can't use Reflection's IsAssignableFrom because of the annoying way Generics work under Reflection.
            
            for (Type t = GetType() ; t != null ; t = t.BaseType)
            {
                // Our KOSNomenclature mapping can't store a Dictionary mapping for all
                // the new generics types that get made on the fly and weren't present when the static constructor was made.
                // So instead we ask Reflection to get the base from which it came so we can look that up instead.
                if (t.IsGenericType)
                    t = t.GetGenericTypeDefinition();
                
                if (KOSNomenclature.HasKOSName(t))
                {
                    string kOSname = KOSNomenclature.GetKOSName(t);
                    if (kOSname == queryTypeName)
                        return true;
                    if (t == typeof(Structure))
                        break; // don't bother walking further up - there won't be any more KOS types above this.
                }
            }
            return false;
        }
        
        public virtual StringValue GetKOSInheritance()
        {
            StringBuilder sb = new StringBuilder();
            
            string prevKosName = "";
            
            for (Type t = GetType() ; t != null ; t = t.BaseType)
            {
                // Our KOSNomenclature mapping can't store a Dictionary mapping for all
                // the new generics types that get made on the fly and weren't present when the static constructor was made.
                // So instead we ask Reflection to get the base from which it came so we can look that up instead.
                if (t.IsGenericType)
                    t = t.GetGenericTypeDefinition();
                
                if (KOSNomenclature.HasKOSName(t))
                {
                    string kOSname = KOSNomenclature.GetKOSName(t);
                    if (kOSname != prevKosName) // skip extra iterations where we mash parent C# types and child C# types into the same KOS type.
                    {
                        if (prevKosName != "")
                            sb.Append(" derived from ");
                        sb.Append(kOSname);
                    }
                    prevKosName = kOSname;
                    if (t == typeof(Structure))
                        break; // don't bother walking further up - there won't be any more KOS types above this.
                }
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return KOSNomenclature.GetKOSName(GetType()) + ": \"\""; // print as the KOSNomenclature string name, will look like: Structure: ""
        }

        /// <summary>
        /// Attempt to convert the given object into a kOS encapsulation type (something
        /// derived from kOS.Safe.Encapsulation.Structure), returning that instead.
        /// This never throws exception or complains in any way if the conversion cannot happen.
        /// Insted in that case it just silently ignores the request and returns the original object
        /// reference unchanged.  Thus it is safe to call it "just in case", even in places where it won't
        /// always be necessary, or have an effect at all.  You should use in anywhere you need to
        /// ensure that a value a user's script might see on the stack or in a script variable is properly
        /// wrapped in a kOS Structure, and not just a raw primitive like int or double.
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <returns>new converted value, or original value if conversion couldn't happen or was unnecesary</returns>
        public static object FromPrimitive(object value)
        {
            if (value == null)
                return value; // If a null exists, let it pass through so it will bomb elsewhere, not here in FromPrimitive() where the exception message would be obtuse.

            if (value is Structure)
                return value; // Conversion is unnecessary - it's already a Structure.

            var convert = value as IConvertible;
            if (convert == null)
                return value; // Conversion isn't even theoretically possible.

            TypeCode code = convert.GetTypeCode();
            switch (code)
            {
                case TypeCode.Boolean:
                    return new BooleanValue(Convert.ToBoolean(convert));
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return ScalarValue.Create(Convert.ToDouble(convert));
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return ScalarValue.Create(Convert.ToInt32(convert));
                case TypeCode.String:
                    return new StringValue(Convert.ToString(convert, CultureInfo.CurrentCulture));
                default:
                    break;
            }
            return value; // Conversion is one this method didn't implement.
        }

        /// <summary>
        /// This is identical to FromPrimitive, except that it WILL throw an exception
        /// if it was unable to guarantee that the result became (or already was) a kOS Structure.
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <returns>value after conversion, or original value if conversion unnecessary</returns>
        public static Structure FromPrimitiveWithAssert(object value)
        {
            object convertedVal = FromPrimitive(value);
            Structure returnValue = convertedVal as Structure;
            if (returnValue == null)
                throw new KOSException(
                    string.Format("Internal Error.  Contact the kOS developers with the phrase 'impossible FromPrimitiveWithAssert({0}) was attempted'.\nAlso include the output log if you can.",
                                  value == null ? "<null>" : value.GetType().ToString()));
            return returnValue;
        }

        public static object ToPrimitive(object value)
        {
            var primitive = value as PrimitiveStructure;
            if (primitive != null)
            {
                return primitive.ToPrimitive();
            }

            return value;
        }
    }
}

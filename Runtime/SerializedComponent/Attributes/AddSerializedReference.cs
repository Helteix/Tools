using System;

namespace LTX.Tools.SerializedComponent
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AddSerializedReference : Attribute
    {
        public AddSerializedReference() : this(string.Empty)
        {

        }
        public AddSerializedReference(string pathConstraint)
        {

        }
    }
}
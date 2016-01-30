using System;
using System.Collections;
using System.ComponentModel;

namespace Xania.Calculation.Designer.Components
{
    public class CollectionPropertyDescriptor : PropertyDescriptor
    {
        private readonly IList _collection;
        private readonly int _index;

        public CollectionPropertyDescriptor(IList expandableDesignerCollection,
            int idx)
            : base("#" + idx, null)
        {
            _collection = expandableDesignerCollection;
            _index = idx;
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(null);
            }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get
            {
                return _collection.GetType();
            }
        }

        public override string DisplayName
        {
            get
            {
                return _collection[_index].ToString();
            }
        }

        public override string Description
        {
            get
            {
                return _collection[_index].ToString();
            }
        }

        public override object GetValue(object component)
        {
            return _collection[_index];
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "#" + _index; }
        }

        public override Type PropertyType
        {
            get { return _collection[_index].GetType(); }
        }

        public override void ResetValue(object component) { }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            // this.collection[index] = value;
        }
    }
}
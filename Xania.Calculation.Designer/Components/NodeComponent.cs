using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Xania.Calculation.Designer.Components
{
    public class NodeComponent : TreeComponent
    {
        public NodeComponent()
        {
            Branches = new ExpandableDesignerCollection<BranchComponent>();
        }

        [Category("Functional")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual ExpandableDesignerCollection<BranchComponent> Branches { get; private set; }
    }

    public class TreeRefComponent : TreeComponent
    {
        public string Name { get; set; }
    }

    public class ExpandableDesignerCollection<T> : Collection<T>, ICustomTypeDescriptor
    {
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            // Create a new collection object PropertyDescriptorCollection
            var pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            for (int i = 0; i < Count; i++)
            {
                // For each employee create a property descriptor 
                // and add it to the 
                // PropertyDescriptorCollection instance
                CollectionPropertyDescriptor pd = new
                    CollectionPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            return pds;
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }

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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xania.Calculation.Designer.Components
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TreeArgument
    {
        public string Name { get; set; }

        [Browsable(false)]
        public ITreeComponent Tree { get; set; }
    }

    public class Formula : TreeArgument
    {
    }

    public class Branch : TreeArgument
    {
        public bool Many { get; set; }
        public string Path { get; set; }
    }

    public class Repository : TreeArgument
    {
        private IDictionary<string, string> _where;

        public Repository()
        {
        }

        [TypeConverter(typeof (DictionaryTypeConverter))]
        public IDictionary<string, string> Where
        {
            get
            {
                if (_where == null)
                {
                    var component = Tree as CsvRepositoryComponent;
                    if (component != null)
                    {
                        _where = new Dictionary<string, string>();
                        
                        foreach (var field in component.Fields)
                            _where.Add(new KeyValuePair<string, string>(field, string.Empty));
                    }
                }
                return _where;
            }
        }

        public override string ToString()
        {
            return Tree.ToString();
        }
    }

    [TypeConverter(typeof(DictionaryTypeConverter))]
    public class DesignerDictionary : Dictionary<string, string>
    {
    }

    public class DictionaryTypeConverter: TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var dictionary = value as IDictionary<string, string>;
            if (dictionary != null)
            {
                var arr =
                    dictionary.Select(kvp => new TestPropertyDescriptor(kvp.Key, new Attribute[0]))
                        .OfType<PropertyDescriptor>()
                        .ToArray();

                return new PropertyDescriptorCollection(arr);
            }
            return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            if (context.PropertyDescriptor != null)
            {
                return typeof (IDictionary<string, string>).IsAssignableFrom(context.PropertyDescriptor.PropertyType);
            }
            return false;
        }

        class TestPropertyDescriptor : PropertyDescriptor
        {
            public TestPropertyDescriptor(string name, Attribute[] attrs) : base(name, attrs)
            {
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                var dictionary = component as IDictionary<string, string>;
                if (dictionary != null)
                {
                    return dictionary[this.Name];
                }
                return null;
            }

            public override void ResetValue(object component)
            {
            }

            public override void SetValue(object component, object value)
            {
                var dictionary = component as IDictionary<string, string>;
                if (dictionary != null && value is string)
                {
                    dictionary[this.Name] = (string) value;
                }
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get { return typeof(IDictionary<string, string>); }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return typeof(string); }
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiive.Owl.Core.XOML;
using Fiive.Owl.Core.Keywords;
using Fiive.Owl.Core.Exceptions;
using Fiive.Owl.Formats.Output.Auxiliar;
using Fiive.Owl.Core.Extensions;

namespace Fiive.Owl.Formats.Output.XOML
{
    /// <summary>
    /// Seccion de Salida
    /// </summary>
    public class ElementOutput : IXOMLObject
    {
        #region Constructor

        public ElementOutput() { DataType = ElementDataType.Alphanumeric; }

        #endregion

        #region Properties

        public string Name { get; set; }
        public FieldLength Length { get; set; }
        public ElementDataType DataType { get; set; }
        public bool Mandatory { get; set; }
        public string Format { get; set; }
        public string NewVariable { get; set; }
        public string Counter { get; set; }
        public bool? Event { get; set; }
        public bool? EventSection { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }
        public char InputDecimalSeparator { get; set; }
        public char OutputDecimalSeparator { get; set; }
        public bool Overwrite { get; set; }
        public string Id { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string Node { get; set; }

        /// <summary>
        /// Obtiene / Establece el valor del Elemento
        /// </summary>
        public string Value { get; set; }

        public IKeyword Keyword { get; set; }

        /// <summary>
        /// Obtiene / Establece un valor que indica si el elemento tiene un error
        /// </summary>
        public bool ElementWithError { get; set; }

        #endregion

        #region Properties Read Only

        /// <summary>
        /// Obtiene la linea de donde se obtiene el elemento
        /// </summary>
        public string Line { get; internal set; }

        /// <summary>
        /// Obtiene el numero de la linea de donde se obtiene el elemento
        /// </summary>
        public string NumberLine { get; internal set; }

        #endregion

        #region IXOMLObject

        public virtual XOMLSigning GetSigning()
        {
            return new XOMLSigning
            {
                Restrictions = new List<XOMLSigning.XOMLRestriction>()
                {
                    new XOMLSigning.XOMLRestriction { TagName = "name", PropertyName = "Name", Attribute = true, Tag = false, Mandatory = true, PropertyType = XOMLPropertyType.String },
                    new XOMLSigning.XOMLRestriction { TagName = "new-variable", PropertyName = "NewVariable", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.String },
                    new XOMLSigning.XOMLRestriction { TagName = "event", PropertyName = "Event", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.Boolean },
                    new XOMLSigning.XOMLRestriction { TagName = "event-section", PropertyName = "EventSection", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.Boolean },
                    new XOMLSigning.XOMLRestriction { TagName = "description", PropertyName = "Description", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.String },
                    new XOMLSigning.XOMLRestriction { TagName = "hidden", PropertyName = "Hidden", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.Boolean },
                    new XOMLSigning.XOMLRestriction { TagName = "input-decimal-separator", PropertyName = "InputDecimalSeparator", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.Char },
                    new XOMLSigning.XOMLRestriction { TagName = "output-decimal-separator", PropertyName = "OutputDecimalSeparator", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.Char },
                    new XOMLSigning.XOMLRestriction { TagName = "overwrite", PropertyName = "Overwrite", Attribute = true, Tag = false, Mandatory = false, PropertyType = XOMLPropertyType.Boolean },
                    new XOMLSigning.XOMLRestriction { TagName = "id", PropertyName = "Id", Attribute = true, Tag = false, Mandatory = false, PropertyType = XOMLPropertyType.String },
                    new XOMLSigning.XOMLRestriction { TagName = "before", PropertyName = "Before", Attribute = true, Tag = false, Mandatory = false, PropertyType = XOMLPropertyType.String },
                    new XOMLSigning.XOMLRestriction { TagName = "after", PropertyName = "After", Attribute = true, Tag = false, Mandatory = false, PropertyType = XOMLPropertyType.String },
                    new XOMLSigning.XOMLRestriction { TagName = "node", PropertyName = "Node", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.String },
                    new XOMLSigning.XOMLRestriction { TagName = "data-type", PropertyName = "DataType", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.Enum },
                    new XOMLSigning.XOMLRestriction { TagName = "mandatory", PropertyName = "Mandatory", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.Boolean },
                    new XOMLSigning.XOMLRestriction { TagName = "length", PropertyName = "Length", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.Object },
                    new XOMLSigning.XOMLRestriction { TagName = "format", PropertyName = "Format", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.String },
                    new XOMLSigning.XOMLRestriction { TagName = "counter", PropertyName = "Counter", Attribute = true, Tag = true, Mandatory = false, PropertyType = XOMLPropertyType.String }
                }
            };
        }

        public virtual void SetPropertyValue(string property, string value)
        {
            if (property == "DataType") { DataType = (ElementDataType)Enum.Parse(typeof(ElementDataType), value.XOMLName()); }
            else if (property == "Length") { Length = new FieldLength(value); }
            else { throw new OwlException(string.Format(ETexts.GT(ErrorType.XOMLEnumInvalid), property)); }
        }

        #endregion
    }
}

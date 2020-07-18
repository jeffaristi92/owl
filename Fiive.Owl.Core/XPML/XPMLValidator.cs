﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using Fiive.Owl.Core.Input;
using System.Data;
using Fiive.Owl.Core.Keywords;
using System.Xml.XPath;
using Fiive.Owl.Core.Exceptions;

namespace Fiive.Owl.Core.XPML
{
    /// <summary>
    /// Validador XPML
    /// </summary>
    public class XPMLValidator
    {
        #region Publics

        /// <summary>
        /// Valida y obtiene el objeto
        /// </summary>
        /// <param name="obj">Objeto a validar y obtener</param>
        /// <param name="node">Nodo con la configuracion</param>
        /// <param name="handler">Orquestador</param>
        /// <returns>Objeto con los datos</returns>
        public IXPMLObject GetXPMLObject(IXPMLObject obj, XmlNode node, OwlHandler handler)
        {
            XPMLSigning xs = obj.GetSigning();
            Type type = obj.GetType();
            List<XmlNode> valuesConf = new List<XmlNode>();
            bool hasXPMLConfiguration = ValidateXPMLCount(node);

            #region Valida propiedades

            foreach (XPMLSigning.XPMLRestriction xr in xs.Restrictions)
            {
                bool hasMandatoryValidation = false;

                #region Attribute

                XmlAttribute nAttribute = null;
                if (xr.Attribute) { nAttribute = GetXPMLAttribute(node, xr.TagName); } // Validate if the field can be an attribute

                if (nAttribute != null)
                {
                    string value = nAttribute.Value;
                    hasMandatoryValidation = true;

                    if (xr.PropertyType != XPMLPropertyType.List)
                    {
                        value = ValidateInlineXPML(value, handler);

                        if (xr.PropertyType == XPMLPropertyType.String) { SetProperty(xr.PropertyName, obj, value); }
                        else if (xr.PropertyType == XPMLPropertyType.Char && value.Length > 0) { SetProperty(xr.PropertyName, obj, value[0]); }
                        else if (xr.PropertyType == XPMLPropertyType.Boolean) { SetBooleanProperty(xr.PropertyName, obj, value); }
                        else if (xr.PropertyType == XPMLPropertyType.Int) { SetIntProperty(xr.PropertyName, obj, value); }
                        else if (xr.PropertyType == XPMLPropertyType.Enum || xr.PropertyType == XPMLPropertyType.Object) { SetPropertyValue(xr.PropertyName, obj, value); }
                    }
                    else
                    {
                        List<string> values = new List<string>();
                        foreach (string v in value.Split(',')) { string val = ValidateInlineXPML(v, handler); values.Add(val); }

                        SetProperty(xr.PropertyName, obj, values);
                    }
                }

                #endregion

                #region Tag

                else if (xr.Tag && hasXPMLConfiguration) // Validate if the field can be an tag
                {
                    XmlNode nProperty = GetXPMLProperty(node, xr.TagName);
                    if (nProperty != null)
                    {
                        string value = string.Empty;
                        hasMandatoryValidation = true;
                        if (xr.PropertyType != XPMLPropertyType.List) { value = GetKeywordValue(nProperty, handler); }

                        if (xr.PropertyType == XPMLPropertyType.String) { SetProperty(xr.PropertyName, obj, value); }
                        else if (xr.PropertyType == XPMLPropertyType.Char && value.Length > 0) { SetProperty(xr.PropertyName, obj, value[0]); }
                        else if (xr.PropertyType == XPMLPropertyType.Boolean) { SetBooleanProperty(xr.PropertyName, obj, value); }
                        else if (xr.PropertyType == XPMLPropertyType.Int) { SetIntProperty(xr.PropertyName, obj, value); }
                        else if (xr.PropertyType == XPMLPropertyType.Enum || xr.PropertyType == XPMLPropertyType.Object) { SetPropertyValue(xr.PropertyName, obj, value); }
                        else if (xr.PropertyType == XPMLPropertyType.List)
                        {
                            List<string> values = new List<string>();
                            foreach (XmlNode valueNode in nProperty.SelectNodes(string.Concat(nProperty.Name, ".", "Valor"))) { values.Add(GetKeywordValue(valueNode, handler)); }
                            SetProperty(xr.PropertyName, obj, values);
                        }
                    }
                }

                #endregion

                #region Final Validation

                // Validate the field if doesn't exist in the configuration
                if (!hasMandatoryValidation && xr.Mandatory) { throw new OwlException(string.Format(ETexts.GT(ErrorType.XPMLPropertyDoesNotExist), xr.PropertyName)); }

                #endregion
            }

            #endregion

            return obj;
        }

        /// <summary>
        /// Obtiene un valor de una palabra clave
        /// </summary>
        /// <param name="nProperty">Nodo con la propiedad</param>
        /// <param name="handler">Orquestador</param>
        /// <returns>Valor</returns>
        public string GetKeywordValue(XmlNode nProperty, OwlHandler handler)
        {
            try
            {
                if (nProperty.FirstChild != null && nProperty.FirstChild.NodeType == XmlNodeType.Element)
                {
                    return handler.KeywordsManager.GetXPMLKeyword(nProperty.FirstChild, handler).GetValue(handler);
                }
                else { return nProperty.InnerText; }
            }
            catch (TargetInvocationException e) { throw e.InnerException; }
        }

        #endregion

        #region Protected

        /// <summary>
        /// Set the property value
        /// </summary>
        /// <param name="property">Property Name</param>
        /// <param name="obj">Object</param>
        /// <param name="value">Value</param>
        protected void SetProperty(string property, object obj, object value)
        {
            PropertyInfo prop = obj.GetType().GetProperty(property);
            prop.SetValue(obj, value, null);
        }

        /// <summary>
        /// Set the value in Boolean properties
        /// </summary>
        /// <param name="property">Property Name</param>
        /// <param name="obj">Object</param>
        /// <param name="value">Value</param>
        protected void SetBooleanProperty(string property, object obj, string value)
        {
            bool boolValue;
            if (value == "true") { boolValue = true; }
            else if (value == "false") { boolValue = false; }
            else { throw new OwlException(string.Format(ETexts.GT(ErrorType.XPMLPropertyInvalidValue), value, property)); }

            SetProperty(property, obj, boolValue);
        }

        /// <summary>
        /// Set the value in Int properties
        /// </summary>
        /// <param name="property">Property Name</param>
        /// <param name="obj">Object</param>
        /// <param name="value">Value</param>
        protected void SetIntProperty(string property, object obj, string value)
        {
            int number;
            if (int.TryParse(value, out number)) { SetProperty(property, obj, number); }
            else { throw new OwlException(string.Format(ETexts.GT(ErrorType.XPMLPropertyInvalidValue), value, property)); }
        }

        /// <summary>
        /// Set the value in Enum properties
        /// </summary>
        /// <param name="property">Property</param>
        /// <param name="obj">Object</param>
        /// <param name="value">Value</param>
        protected void SetPropertyValue(string property, object obj, string value)
        {
            try
            {
                MethodInfo m = obj.GetType().GetMethod("SetPropertyValue");
                m.Invoke(obj, new object[] { property, value });
            }
            catch (TargetInvocationException e) { throw e.InnerException; }
        }

        protected string ValidateInlineXPML(string value, OwlHandler handler)
        {
            string[] parts = value.Split(new char[] { ':' }, 2);
            if (parts.Length == 1) { return value; }
            else
            {
                IKeyword keyword = handler.KeywordsManager.GetInlineKeyword(parts, handler);
                if (keyword != null) { return keyword.GetValue(handler); }
            }

            return value;
        }

        /// <summary>
        /// Obtiene el XmlNode que representa la propiedad
        /// </summary>
        /// <param name="node">Nodo con la configuracion</param>
        /// <param name="property">Propiedad a obtener</param>
        /// <returns>XmlNodo con la propiedad</returns>
        protected XmlNode GetXPMLProperty(XmlNode node, string property)
        {
            return node.SelectSingleNode(string.Concat(node.Name, '.', property));
        }

        /// <summary>
        /// Obtiene el XmlAttribute que representa la propiedad
        /// </summary>
        /// <param name="node">Nodo con la configuracion</param>
        /// <param name="property">Propiedad a obtener</param>
        /// <returns>XmlAttribute con la propiedad</returns>
        protected XmlAttribute GetXPMLAttribute(XmlNode node, string property)
        {
            return node.Attributes[property];
        }

        /// <summary>
        /// Validate if the node have XPML configuration
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>true if have, otherwise false</returns>
        protected bool ValidateXPMLCount(XmlNode node)
        {
            if (node.HasChildNodes)
            {
                var a = from source in node.ChildNodes.Cast<XmlNode>() where source.Name.StartsWith(string.Concat(node.Name, ".")) select source;
                if (a.Count<XmlNode>() == 0) { return false; }
                return true;
            }
            else { return false; }
        }

        #endregion
    }
}

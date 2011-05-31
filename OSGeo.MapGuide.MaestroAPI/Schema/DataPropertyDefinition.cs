﻿#region Disclaimer / License
// Copyright (C) 2010, Jackie Ng
// http://trac.osgeo.org/mapguide/wiki/maestro, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml;

namespace OSGeo.MapGuide.MaestroAPI.Schema
{
    /// <summary>
    /// Derives from <see cref="T:OSGeo.MapGuide.MaestroAPI.Schema.PropertyDefinition"/> and represents simple values or 
    /// collections of simple values. This can take on any of the data types listed in the 
    /// <see cref="T:OSGeo.MapGuide.MaestroAPI.Schema.DataPropertyType"/> enumeration. 
    /// </summary>
    public class DataPropertyDefinition : PropertyDefinition
    {
        private DataPropertyDefinition() { this.DataType = DataPropertyType.String; }

        public DataPropertyDefinition(string name, string description) 
            : this()
        {
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// Gets the data type of this property
        /// </summary>
        public DataPropertyType DataType { get; set; }

        /// <summary>
        /// Gets or sets the default value. Applies only to string data types
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the length of this property. Applies only to string and blob/clob data types
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets whether this property accepts null values
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the precision of this property. Applies only to decimal data types
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// Gets or sets whether this property is read-only
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the scale of this property. Applies only to decimal data types.
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// Gets or sets whether this property automatically generates a value on insert.
        /// </summary>
        public bool IsAutoGenerated { get; set; }

        /// <summary>
        /// Gets the type of Property Definition
        /// </summary>
        public override PropertyDefinitionType Type
        {
            get { return PropertyDefinitionType.Data; }
        }

        private static string GetXmlType(DataPropertyType dataPropertyType)
        {
            switch (dataPropertyType)
            {
                case DataPropertyType.Blob:
                    return "xs:base64Binary";
                case DataPropertyType.Boolean:
                    return "xs:boolean";
                case DataPropertyType.Byte:
                    return "fdo:byte";
                case DataPropertyType.DateTime:
                    return "xs:dateTime";
                case DataPropertyType.Double:
                    return "xs:double";
                case DataPropertyType.Int16:
                    return "xs:int16";
                case DataPropertyType.Int32:
                    return "fdo:int32";
                case DataPropertyType.Int64:
                    return "xs:int64";
                case DataPropertyType.Single:
                    return "xs:single";
                case DataPropertyType.String:
                    return "xs:string";
                case DataPropertyType.Clob:
                    return "xs:string";
                default:
                    throw new ArgumentException();
            }
        }

        public static DataPropertyType GetDataType(string xmlType)
        {
            switch (xmlType)
            {
                case "xs:hexbinary":
                case "xs:base64Binary":
                    return DataPropertyType.Blob;
                case "xs:boolean":
                    return DataPropertyType.Boolean;
                case "fdo:byte":
                    return DataPropertyType.Byte;
                case "xs:datetime":
                    return DataPropertyType.DateTime;
                case "fdo:double":
                case "fdo:decimal":
                case "xs:decimal":
                case "xs:double":
                    return DataPropertyType.Double;
                case "fdo:int16":
                case "xs:int16":
                    return DataPropertyType.Int16;
                case "fdo:int32":
                case "xs:int32":
                case "xs:int":
                    return DataPropertyType.Int32;
                case "fdo:int64":
                case "xs:int64":
                    return DataPropertyType.Int64;
                case "xs:float":
                case "xs:single":
                case "fdo:single":
                    return DataPropertyType.Single;
                case "xs:string":
                    return DataPropertyType.String;
                //case "xs:string":
                //    return DataPropertyType.Clob;
                default:
                    throw new ArgumentException();
            }
        }

        public override void WriteXml(System.Xml.XmlDocument doc, System.Xml.XmlNode currentNode)
        {
            var prop = doc.CreateElement("xs", "element", XmlNamespaces.XS);
            prop.SetAttribute("name", this.Name); //TODO: This may have been decoded. Should it be re-encoded?
            prop.SetAttribute("minOccurs", this.IsNullable ? "0" : "1");
            if (this.IsReadOnly)
                prop.SetAttribute("readOnly", XmlNamespaces.FDO, this.IsReadOnly.ToString().ToLower());
            if (this.IsAutoGenerated)
                prop.SetAttribute("autogenerated", XmlNamespaces.FDO, this.IsAutoGenerated.ToString().ToLower());

            var simp = doc.CreateElement("xs", "simpleType", XmlNamespaces.XS);
            prop.AppendChild(simp);

            var rest = doc.CreateElement("xs", "restriction", XmlNamespaces.XS);
            simp.AppendChild(rest);

            rest.SetAttribute("base", GetXmlType(this.DataType));
            if (this.DataType == DataPropertyType.String)
            {
                var max = doc.CreateElement("xs", "maxLength", XmlNamespaces.XS);
                max.SetAttribute("value", this.Length.ToString(CultureInfo.InvariantCulture));
            }
            
            currentNode.AppendChild(prop);
        }

        public override void ReadXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr)
        {
            var ro = Utility.GetFdoAttribute(node, "readOnly");
            var autogen = Utility.GetFdoAttribute(node, "autogenerated");

            this.DataType = GetDataType(node["xs:simpleType"]["xs:restriction"].Attributes["base"].Value.ToLower());
            this.IsNullable = (node.Attributes["minOccurs"] != null && node.Attributes["minOccurs"].Value == "0");
            this.IsReadOnly = (ro != null && ro.Value == "true");
            this.IsAutoGenerated = (autogen != null && autogen.Value == "true");
            this.DefaultValue = (node.Attributes["default"] != null ? node.Attributes["default"].Value : string.Empty);
        }

        /// <summary>
        /// Convenience method to get whether this data property is numeric
        /// </summary>
        /// <returns></returns>
        public bool IsNumericType()
        {
            return this.DataType == DataPropertyType.Byte ||
                   this.DataType == DataPropertyType.Double ||
                   this.DataType == DataPropertyType.Int16 ||
                   this.DataType == DataPropertyType.Int32 ||
                   this.DataType == DataPropertyType.Int64 ||
                   this.DataType == DataPropertyType.Single;
        }
    }
}

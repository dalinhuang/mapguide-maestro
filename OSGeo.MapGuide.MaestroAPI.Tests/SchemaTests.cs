﻿#region Disclaimer / License

// Copyright (C) 2014, Jackie Ng
// https://github.com/jumpinjackie/mapguide-maestro
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

#endregion Disclaimer / License
using OSGeo.MapGuide.MaestroAPI.Schema;
using System;
using System.IO;
using System.Xml;
using Xunit;

namespace OSGeo.MapGuide.MaestroAPI.Tests
{
    public class SchemaTests
    {
        //These tests exercise various parts of the schema part of the Maestro API

        [Fact]
        public void TestFeatureSchemaRoundtrip()
        {
            FeatureSchema schema = new FeatureSchema("Default", "Default Schema");
            ClassDefinition cls = new ClassDefinition("Class1", "Test Class");

            cls.AddProperty(new DataPropertyDefinition("ID", "ID Property")
            {
                IsAutoGenerated = true,
                DataType = DataPropertyType.Int64,
                IsNullable = false,
            }, true);

            var prop = cls.FindProperty("ID") as DataPropertyDefinition;

            Assert.Single(cls.IdentityProperties);
            Assert.Single(cls.Properties);
            Assert.NotNull(prop);
            Assert.Equal(DataPropertyType.Int64, prop.DataType);
            Assert.True(prop.IsAutoGenerated);
            Assert.False(prop.IsReadOnly);
            Assert.False(prop.IsNullable);

            cls.AddProperty(new DataPropertyDefinition("Name", "The name")
            {
                DataType = DataPropertyType.String,
                Length = 255
            });

            prop = cls.FindProperty("Name") as DataPropertyDefinition;

            Assert.Single(cls.IdentityProperties);
            Assert.Equal(2, cls.Properties.Count);
            Assert.NotNull(prop);
            Assert.Equal(DataPropertyType.String, prop.DataType);
            Assert.False(prop.IsAutoGenerated);
            Assert.False(prop.IsReadOnly);
            Assert.False(prop.IsNullable);

            cls.AddProperty(new DataPropertyDefinition("Date", "The date")
            {
                DataType = DataPropertyType.DateTime,
                IsNullable = true
            });

            prop = cls.FindProperty("Date") as DataPropertyDefinition;

            Assert.Single(cls.IdentityProperties);
            Assert.Equal(3, cls.Properties.Count);
            Assert.NotNull(prop);
            Assert.Equal(DataPropertyType.DateTime, prop.DataType);
            Assert.False(prop.IsAutoGenerated);
            Assert.False(prop.IsReadOnly);
            Assert.True(prop.IsNullable);

            schema.AddClass(cls);
            Assert.Single(schema.Classes);

            XmlDocument doc = new XmlDocument();
            schema.WriteXml(doc, doc);

            string path = Path.GetTempFileName();
            doc.Save(path);

            FeatureSourceDescription fsd = new FeatureSourceDescription(Utils.OpenFile(path));
            Assert.Single(fsd.Schemas);

            schema = fsd.Schemas[0];
            Assert.NotNull(schema);

            cls = schema.GetClass("Class1");
            Assert.NotNull(cls);

            prop = cls.FindProperty("ID") as DataPropertyDefinition;

            Assert.Single(cls.IdentityProperties);
            Assert.Equal(3, cls.Properties.Count);
            Assert.NotNull(prop);
            Assert.Equal(DataPropertyType.Int64, prop.DataType);
            Assert.True(prop.IsAutoGenerated);
            Assert.False(prop.IsReadOnly);
            Assert.False(prop.IsNullable);

            prop = cls.FindProperty("Name") as DataPropertyDefinition;

            Assert.Single(cls.IdentityProperties);
            Assert.Equal(3, cls.Properties.Count);
            Assert.NotNull(prop);
            Assert.Equal(DataPropertyType.String, prop.DataType);
            Assert.False(prop.IsAutoGenerated);
            Assert.False(prop.IsReadOnly);
            Assert.False(prop.IsNullable);

            prop = cls.FindProperty("Date") as DataPropertyDefinition;

            Assert.Single(cls.IdentityProperties);
            Assert.Equal(3, cls.Properties.Count);
            Assert.NotNull(prop);
            Assert.Equal(DataPropertyType.DateTime, prop.DataType);
            Assert.False(prop.IsAutoGenerated);
            Assert.False(prop.IsReadOnly);
            Assert.True(prop.IsNullable);
        }

        [Fact]
        public void TestCreate()
        {
            var schema = new FeatureSchema("Default", "Default Schema");
            Assert.Equal("Default", schema.Name);
            Assert.Equal("Default Schema", schema.Description);

            var cls = new ClassDefinition("Class1", "My Class");
            Assert.Equal("Class1", cls.Name);
            Assert.Equal("My Class", cls.Description);
            Assert.True(string.IsNullOrEmpty(cls.DefaultGeometryPropertyName));
            Assert.Empty(cls.Properties);
            Assert.Empty(cls.IdentityProperties);

            var prop = new DataPropertyDefinition("ID", "identity");
            Assert.Equal("ID", prop.Name);
            Assert.Equal("identity", prop.Description);
            Assert.False(prop.IsAutoGenerated);
            Assert.False(prop.IsReadOnly);
            Assert.True(string.IsNullOrEmpty(prop.DefaultValue));

            prop.DataType = DataPropertyType.Int32;
            Assert.Equal(DataPropertyType.Int32, prop.DataType);

            prop.IsAutoGenerated = true;
            Assert.True(prop.IsAutoGenerated);

            prop.IsReadOnly = true;
            Assert.True(prop.IsReadOnly);

            cls.AddProperty(prop, true);
            Assert.Single(cls.Properties);
            Assert.Single(cls.IdentityProperties);
            Assert.Equal(cls, prop.Parent);
            Assert.NotNull(cls.FindProperty(prop.Name));

            cls.RemoveProperty(prop);
            Assert.Empty(cls.Properties);
            Assert.Empty(cls.Properties);
            Assert.Null(prop.Parent);
            Assert.Null(cls.FindProperty(prop.Name));

            cls.AddProperty(prop, true);
            Assert.Single(cls.Properties);
            Assert.Single(cls.IdentityProperties);
            Assert.Equal(cls, prop.Parent);
            Assert.NotNull(cls.FindProperty(prop.Name));

            cls.AddProperty(new DataPropertyDefinition("Name", "")
            {
                DataType = DataPropertyType.String,
                Length = 255,
                IsNullable = true
            });

            Assert.Equal(2, cls.Properties.Count);
            Assert.Single(cls.IdentityProperties);

            cls.AddProperty(new GeometricPropertyDefinition("Geom", "")
            {
                HasMeasure = false,
                HasElevation = false,
                GeometricTypes = FeatureGeometricType.All,
                SpecificGeometryTypes = (SpecificGeometryType[])Enum.GetValues(typeof(SpecificGeometryType))
            });
            Assert.Equal(3, cls.Properties.Count);
            Assert.Single(cls.IdentityProperties);
            Assert.True(string.IsNullOrEmpty(cls.DefaultGeometryPropertyName));

            var geom = cls.FindProperty("Geom");
            Assert.NotNull(geom);

            cls.DefaultGeometryPropertyName = geom.Name;
            Assert.False(String.IsNullOrEmpty(cls.DefaultGeometryPropertyName));

            schema.AddClass(cls);
            Assert.Equal(schema, cls.Parent);
        }

        [Fact]
        public void TestClassNameEncoding()
        {
            // Round-trip various invalid XML names. Copied from FDO test suite
            string name1 = "Abc def";
            string name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("Abc-x20-def", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = " Abc#defg$$";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x20-Abc-x23-defg-x24--x24-", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = " Abc#defg hij";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x20-Abc-x23-defg-x20-hij", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "--abc-def---ghi--";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x2d--abc-def---ghi--", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "--abc-x20-def-x23--x24-ghi--";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x2d--abc-x2d-x20-def-x2d-x23--x2d-x24-ghi--", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "-xab";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x2d-xab", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "&Entity";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x26-Entity", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "11ab";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x31-1ab", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "2_Class";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x32-_Class", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "2%Class";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x32--x25-Class", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "2-Class";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x32--Class", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "2-x2f-Class";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x32--x2d-x2f-Class", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "_x2d-";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x00-_x2d-", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "-x3d-";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x2d-x3d-", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "_x2d-_x3f-";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x00-_x2d-_x3f-", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "__x2d-_x3f-";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("__x2d-_x3f-", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "_Class";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_Class", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "_5Class";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_5Class", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "_-5Class";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_-5Class", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "-_x2f-Class";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("_x2d-_x2f-Class", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            name1 = "Foo/Bar - snafu";
            name2 = Utility.EncodeFDOName(name1);
            Assert.Equal("Foo-x2f-Bar-x20---x20-snafu", name2);
            Assert.Equal(name1, Utility.DecodeFDOName(name2));

            // Backward compatibility check. Make sure old-style 1st character encodings get decoded.
            name2 = "-x40-A";
            Assert.Equal("@A", Utility.DecodeFDOName(name2));
        }
    }
}

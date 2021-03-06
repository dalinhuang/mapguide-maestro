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
using System;
using System.Xml;
using Xunit;

namespace OSGeo.MapGuide.MaestroAPI.Schema.Tests
{
    public class FeatureSchemaTests
    {
        [Fact]
        public void FeatureSchemaTest()
        {
            var fs = new FeatureSchema("Foo", "");
            Assert.Equal("Foo", fs.Name);
            Assert.True(String.IsNullOrEmpty(fs.Description));
            Assert.Empty(fs.Classes);

            fs = new FeatureSchema("Foo", "Bar");
            Assert.Equal("Foo", fs.Name);
            Assert.Equal("Bar", fs.Description);
            Assert.Empty(fs.Classes);
        }

        [Fact]
        public void AddClassTest()
        {
            var fs = new FeatureSchema("Foo", "Bar");
            Assert.Equal("Foo", fs.Name);
            Assert.Equal("Bar", fs.Description);
            Assert.Empty(fs.Classes);

            var cls = new ClassDefinition("Class1", "Test Class");
            fs.AddClass(cls);
            Assert.Single(fs.Classes);
        }

        [Fact]
        public void RemoveClassTest()
        {
            var fs = new FeatureSchema("Foo", "Bar");
            Assert.Equal("Foo", fs.Name);
            Assert.Equal("Bar", fs.Description);
            Assert.Empty(fs.Classes);

            var cls = new ClassDefinition("Class1", "Test Class");
            fs.AddClass(cls);
            Assert.Single(fs.Classes);

            fs.RemoveClass("asdgsdf");
            Assert.Single(fs.Classes);

            Assert.True(fs.RemoveClass(cls));
            Assert.Empty(fs.Classes);

            fs.AddClass(cls);
            Assert.Single(fs.Classes);
            fs.RemoveClass("Class1");
            Assert.Empty(fs.Classes);
        }

        [Fact]
        public void GetClassTest()
        {
            var fs = new FeatureSchema("Foo", "Bar");
            Assert.Equal("Foo", fs.Name);
            Assert.Equal("Bar", fs.Description);
            Assert.Empty(fs.Classes);

            var cls = new ClassDefinition("Class1", "Test Class");
            fs.AddClass(cls);

            Assert.NotNull(fs.GetClass("Class1"));
        }

        [Fact]
        public void IndexOfTest()
        {
            var fs = new FeatureSchema("Foo", "Bar");
            Assert.Equal("Foo", fs.Name);
            Assert.Equal("Bar", fs.Description);
            Assert.Empty(fs.Classes);

            var cls = new ClassDefinition("Class1", "Test Class");
            fs.AddClass(cls);

            Assert.True(fs.IndexOf(cls) >= 0);
            Assert.True(fs.RemoveClass(cls));
            Assert.True(fs.IndexOf(cls) < 0);
        }

        [Fact]
        public void GetItemTest()
        {
            var fs = new FeatureSchema("Foo", "Bar");
            Assert.Equal("Foo", fs.Name);
            Assert.Equal("Bar", fs.Description);
            Assert.Empty(fs.Classes);

            var cls = new ClassDefinition("Class1", "Test Class");
            fs.AddClass(cls);

            Assert.Throws<ArgumentOutOfRangeException>(() => fs.GetItem(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => fs.GetItem(1));
            Assert.NotNull(fs.GetItem(0));
            Assert.True(fs.RemoveClass(cls));
            Assert.Throws<ArgumentOutOfRangeException>(() => fs.GetItem(0));
        }

        [Fact]
        public void WriteXmlTest()
        {
            var fs = new FeatureSchema("Foo", "Bar");
            Assert.Equal("Foo", fs.Name);
            Assert.Equal("Bar", fs.Description);
            Assert.Empty(fs.Classes);

            var cls = new ClassDefinition("Class1", "Test Class");
            var id = new DataPropertyDefinition("ID", "");
            id.DataType = DataPropertyType.Int32;
            id.IsAutoGenerated = true;
            var name = new DataPropertyDefinition("Name", "");
            cls.AddProperty(id, true);
            cls.AddProperty(name);
            fs.AddClass(cls);

            var doc = new XmlDocument();
            fs.WriteXml(doc, doc);

            string xml = doc.ToXmlString();
            Assert.False(String.IsNullOrEmpty(xml));
        }

        [Fact]
        public void CloneTest()
        {
            var fs = new FeatureSchema("Foo", "Bar");
            Assert.Equal("Foo", fs.Name);
            Assert.Equal("Bar", fs.Description);
            Assert.Empty(fs.Classes);

            var cls = new ClassDefinition("Class1", "Test Class");
            var id = new DataPropertyDefinition("ID", "");
            id.DataType = DataPropertyType.Int32;
            id.IsAutoGenerated = true;
            var name = new DataPropertyDefinition("Name", "");
            cls.AddProperty(id, true);
            cls.AddProperty(name);
            fs.AddClass(cls);

            var fs2 = FeatureSchema.Clone(fs);
            Assert.Equal(fs.Name, fs2.Name);
            Assert.Equal(fs.Description, fs2.Description);
            Assert.Equal(fs.Classes.Count, fs2.Classes.Count);
            Assert.NotEqual(fs, fs2);
        }
    }
}

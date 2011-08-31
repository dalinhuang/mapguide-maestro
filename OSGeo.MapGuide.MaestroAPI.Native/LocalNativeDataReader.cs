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
using OSGeo.MapGuide.MaestroAPI.Feature;
using OSGeo.MapGuide.MaestroAPI.Schema;
using GisSharpBlog.NetTopologySuite.IO;

namespace OSGeo.MapGuide.MaestroAPI.Native
{
    public class LocalNativeDataReader : ReaderBase
    {
        private MgDataReader _reader;
        private WKTReader _mgReader;
        private MgAgfReaderWriter _agfRw;
        private MgWktReaderWriter _wktRw;

        public LocalNativeDataReader(MgDataReader reader) 
        {
            _reader = reader;
            _mgReader = new WKTReader();
            _agfRw = new MgAgfReaderWriter();
            _wktRw = new MgWktReaderWriter();
        }

        protected override IRecord ReadNextRecord()
        {
            if (_reader.ReadNext())
                return new LocalNativeRecord(_reader, _mgReader, _agfRw, _wktRw);

            return null;
        }

        public override void Close()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader.Dispose();
                _reader = null;
            }
        }

        public override void Dispose()
        {
            Close();
            if (_agfRw != null)
            {
                _agfRw.Dispose();
                _agfRw = null;
            }
            if (_wktRw != null)
            {
                _wktRw.Dispose();
                _wktRw = null;
            }
            base.Dispose();
        }

        public override ReaderType ReaderType
        {
            get { return ReaderType.Data; }
        }

        public override string GetName(int index)
        {
            return _reader.GetPropertyName(index);
        }

        public override Type GetFieldType(int i)
        {
            string name = GetName(i);
            //The enum uses the same values as MgPropertyType
            var type = (PropertyValueType)_reader.GetPropertyType(name);
            return ClrFdoTypeMap.GetClrType(type);
        }
    }
}
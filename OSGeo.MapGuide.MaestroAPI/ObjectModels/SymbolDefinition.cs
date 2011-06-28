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
using OSGeo.MapGuide.MaestroAPI.Resource;
using System.Xml.Serialization;
using OSGeo.MapGuide.MaestroAPI;
using OSGeo.MapGuide.ObjectModels.SymbolDefinition;
using System.Globalization;
using OSGeo.MapGuide.MaestroAPI.Exceptions;
using System.Xml;
using System.IO;
using OSGeo.MapGuide.ObjectModels.Common;

#pragma warning disable 1591, 0114, 0108

#if SYM_DEF_110
namespace OSGeo.MapGuide.ObjectModels.SymbolDefinition_1_1_0
#else
namespace OSGeo.MapGuide.ObjectModels.SymbolDefinition_1_0_0
#endif
{
    abstract partial class SymbolDefinitionBase : ISymbolDefinitionBase
    {
#if SYM_DEF_110
        private static readonly Version RES_VERSION = new Version(1, 1, 0);
#else
        private static readonly Version RES_VERSION = new Version(1, 0, 0);
#endif

        [XmlIgnore]
        public OSGeo.MapGuide.MaestroAPI.IServerConnection CurrentConnection
        {
            get;
            set;
        }

        private string _resId;

        [XmlIgnore]
        public string ResourceID
        {
            get
            {
                return _resId;
            }
            set
            {
                if (!ResourceIdentifier.Validate(value))
                    throw new InvalidOperationException("Not a valid resource identifier"); //LOCALIZE

                var res = new ResourceIdentifier(value);
                if (res.Extension != ResourceTypes.SymbolDefinition.ToString())
                    throw new InvalidOperationException("Invalid resource identifier for this type of object: " + res.Extension); //LOCALIZE

                _resId = value;
                this.OnPropertyChanged("ResourceID");
            }
        }

        [XmlIgnore]
        public ResourceTypes ResourceType
        {
            get
            {
                return ResourceTypes.SymbolDefinition;
            }
        }

        [XmlIgnore]
        public Version ResourceVersion
        {
            get
            {
                return RES_VERSION;
            }
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        [XmlAttribute("noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string ValidatingSchema
        {
#if SYM_DEF_110
            get { return "SymbolDefinition-1.1.0.xsd"; }
#else
            get { return "SymbolDefinition-1.0.0.xsd"; }
#endif
            set { }
        }

        [XmlIgnore]
        public bool IsStronglyTyped
        {
            get { return true; }
        }

        [XmlIgnore]
        public abstract SymbolDefinitionType Type { get; }
    }

    partial class SimpleSymbolDefinition : ISimpleSymbolDefinition
    {
        public static SimpleSymbolDefinition CreateDefault()
        {
            var simpleSym = new SimpleSymbolDefinition()
            {
                Graphics = new System.ComponentModel.BindingList<GraphicBase>(),
                ParameterDefinition = new ParameterDefinition()
                {
                    Parameter = new System.ComponentModel.BindingList<Parameter>()
                }
            };
            return simpleSym;
        }

        [XmlIgnore]
        IEnumerable<IGraphicBase> ISimpleSymbolDefinition.Graphics
        {
            get 
            {
                foreach (var g in this.Graphics)
                {
                    yield return g;
                }
            }
        }

        void ISimpleSymbolDefinition.AddGraphics(IGraphicBase graphics)
        {
            var g = graphics as GraphicBase;
            if (g != null)
            {
                this.Graphics.Add(g);
            }
        }

        void ISimpleSymbolDefinition.RemoveGraphics(IGraphicBase graphics)
        {
            var g = graphics as GraphicBase;
            if (g != null)
            {
                this.Graphics.Remove(g);
            }
        }

        [XmlIgnore]
        IResizeBox ISimpleSymbolDefinition.ResizeBox
        {
            get
            {
                return resizeBoxField;
            }
            set
            {
                resizeBoxField = (ResizeBox)value;
            }
        }

        [XmlIgnore]
        IPointUsage ISimpleSymbolDefinition.PointUsage
        {
            get
            {
                return pointUsageField;
            }
            set
            {
                pointUsageField = (PointUsage)value;
            }
        }

        [XmlIgnore]
        ILineUsage ISimpleSymbolDefinition.LineUsage
        {
            get
            {
                return lineUsageField;
            }
            set
            {
                lineUsageField = (LineUsage)value;
            }
        }

        [XmlIgnore]
        IAreaUsage ISimpleSymbolDefinition.AreaUsage
        {
            get
            {
                return areaUsageField;
            }
            set
            {
                areaUsageField = (AreaUsage)value;
            }
        }

        [XmlIgnore]
        IParameterDefinition ISimpleSymbolDefinition.ParameterDefinition
        {
            get { return parameterDefinitionField; }
        }


        public IImageReference CreateImageReference(string resourceId, string dataName)
        {
            return new ImageReference()
            {
                ResourceId = resourceId,
                LibraryItemName = dataName
            };
        }

        public IInlineImage CreateInlineImage(byte[] content)
        {
            return new InlineImage() { Content = content };
        }

        public IPointUsage CreatePointUsage()
        {
            return new PointUsage()
            {
                //Angle = "0",
                //AngleControl = "'FromAngle'",
                //OriginOffsetX = "0.0",
                //OriginOffsetY = "0.0"
            };
        }

        public ILineUsage CreateLineUsage()
        {
            return new LineUsage()
            {
                //Angle = "0",
                //AngleControl = "'FromGeometry'",
                /*
                DefaultPath = new Path()
                {
                    
                },*/
                //EndOffset = "0",
                //Repeat = "0",
                //StartOffset = "0",
                //UnitsControl = "'Absolute'",
                //VertexAngleLimit = "0",
                //VertexControl = "'OverlapNone'",
                //VertexJoin = "'Round'",
                //VertexMiterLimit = "5"
            };
        }

        public IAreaUsage CreateAreaUsage()
        {
            return new AreaUsage()
            {
                //Angle = "0",
                //AngleControl = "'FromAngle'",
                //BufferWidth = "0",
                //ClippingControl = "'Clip'",
                //OriginControl = "'Global'",
                //OriginX = "0",
                //OriginY = "0",
                //RepeatX = "0",
                //RepeatY = "0"
            };
        }

        public IResizeBox CreateResizeBox()
        {
            return new ResizeBox()
            {
                SizeX = "1.0",
                SizeY = "1.0",
                PositionX = "0.0",
                PositionY = "0.0",
                GrowControl = "\'GrowInXYMaintainAspect\'"
            };
        }


        public ITextFrame CreateFrame()
        {
            return new TextFrame() { };
        }

        public ITextGraphic CreateTextGraphics()
        {
            return new Text() { };
        }

        public IPathGraphic CreatePathGraphics()
        {
            return new Path() { };
        }

        public IImageGraphic CreateImageGraphics()
        {
            return new Image() { };
        }

        public IParameter CreateParameter()
        {
            return new Parameter() { };
        }

        [XmlIgnore]
        public override SymbolDefinitionType Type
        {
            get { return SymbolDefinitionType.Simple; }
        }
    }

    public class InlineImage : IInlineImage
    {
        public byte[] Content
        {
            get;
            set;
        }

        public ImageType Type
        {
            get { return ImageType.Inline; }
        }
    }

#if SYM_DEF_110
    partial class Text : ITextGraphic2
#else
    partial class Text : ITextGraphic
#endif
    {
        [XmlIgnore]
        ITextFrame ITextGraphic.Frame
        {
            get
            {
                return this.Frame;
            }
            set
            {
                this.Frame = (TextFrame)value;
            }
        }
    }

    partial class TextFrame : ITextFrame
    {
        [XmlIgnore]
        double? ITextFrame.OffsetX
        {
            get
            {
                double d;
                if (!double.TryParse(this.OffsetX, out d))
                    return d;
                return null;
            }
            set
            {
                if (value.HasValue)
                    this.OffsetX = value.Value.ToString(CultureInfo.InvariantCulture);
                else
                    this.OffsetX = null;
            }
        }

        [XmlIgnore]
        double? ITextFrame.OffsetY
        {
            get
            {
                double d;
                if (!double.TryParse(this.OffsetY, out d))
                    return d;
                return null;
            }
            set
            {
                if (value.HasValue)
                    this.OffsetY = value.Value.ToString(CultureInfo.InvariantCulture);
                else
                    this.OffsetY = null;
            }
        }
    }

    partial class ResizeBox : IResizeBox
    {
    }

    partial class Graphics : IGraphics
    {
        [XmlIgnore]
        public IEnumerable<IGraphicBase> Elements
        {
            get 
            {
                foreach (var g in this.Items)
                {
                    yield return g;
                }
            }
        }

        public void AddGraphicElement(IGraphicBase graphics)
        {
            var g = graphics as GraphicBase;
            if (g != null)
            {
                this.Items.Add(g);
            }
        }

        public void RemoveGraphicElement(IGraphicBase graphics)
        {
            var g = graphics as GraphicBase;
            if (g != null)
            {
                this.Items.Remove(g);
            }
        }
    }

    partial class GraphicBase : IGraphicBase
    {
        
    }

    partial class Parameter : IParameter
    {
        [XmlIgnore]
        string IParameter.DataType
        {
            get
            {
                return dataTypeField.ToString();
            }
            set
            {
#if SYM_DEF_110
                dataTypeField = (DataType2)Enum.Parse(typeof(DataType2), value);
#else
                dataTypeField = (DataType)Enum.Parse(typeof(DataType), value);
#endif
            }
        }

        string IExpressionPropertySource.Name
        {
            get { return this.Identifier; }
        }

        string IExpressionPropertySource.Description
        {
            get { return this.Description; }
        }

#if SYM_DEF_110
        static ExpressionDataType GetExpressionType(DataType2 dt)
        {
            switch (dt)
            {
                case DataType2.Angle:
                    return ExpressionDataType.Sym_Angle;
                case DataType2.Bold:
                    return ExpressionDataType.Sym_Bold;
                case DataType2.Boolean:
                    return ExpressionDataType.Sym_Boolean;
                case DataType2.Color:
                    return ExpressionDataType.Sym_Color;
                case DataType2.Content:
                    return ExpressionDataType.Sym_Content; 
                case DataType2.EndOffset:
                    return ExpressionDataType.Sym_EndOffset;
                case DataType2.FillColor:
                    return ExpressionDataType.Sym_FillColor;
                case DataType2.FontHeight:
                    return ExpressionDataType.Sym_FontHeight;
                case DataType2.FontName:
                    return ExpressionDataType.Sym_FontName;
                case DataType2.FrameFillColor:
                    return ExpressionDataType.Sym_FrameFillColor;
                case DataType2.FrameLineColor:
                    return ExpressionDataType.Sym_FrameLineColor;
                case DataType2.GhostColor:
                    return ExpressionDataType.Sym_GhostColor;
                case DataType2.HorizontalAlignment:
                    return ExpressionDataType.Sym_HorizontalAlignment;
                case DataType2.Integer:
                    return ExpressionDataType.Sym_Integer;
                case DataType2.Italic:
                    return ExpressionDataType.Sym_Italic;
                case DataType2.Justification:
                    return ExpressionDataType.Sym_Justification;
                case DataType2.LineColor:
                    return ExpressionDataType.Sym_LineColor;
                case DataType2.LineSpacing:
                    return ExpressionDataType.Sym_LineSpacing;
                case DataType2.LineWeight:
                    return ExpressionDataType.Sym_LineWeight;
                case DataType2.Markup:
                    return ExpressionDataType.Sym_Markup;
                case DataType2.ObliqueAngle:
                    return ExpressionDataType.Sym_ObliqueAngle;
                case DataType2.Overlined:
                    return ExpressionDataType.Sym_Overlined;
                case DataType2.Real:
                    return ExpressionDataType.Sym_Real;
                case DataType2.RepeatX:
                    return ExpressionDataType.Sym_RepeatX;
                case DataType2.RepeatY:
                    return ExpressionDataType.Sym_RepeatY;
                case DataType2.StartOffset:
                    return ExpressionDataType.Sym_StartOffset;
                case DataType2.String:
                    return ExpressionDataType.Sym_String;
                case DataType2 .TextColor:
                    return ExpressionDataType.Sym_TextColor;
                case DataType2.TrackSpacing:
                    return ExpressionDataType.Sym_TrackSpacing;
                case DataType2.Underlined:
                    return ExpressionDataType.Sym_Underlined;
                case DataType2.VerticalAlignment:
                    return ExpressionDataType.Sym_VerticalAlignment;
            }
            throw new ArgumentException();
        }
#else
        static ExpressionDataType GetExpressionType(DataType dt)
        {
            switch (dt)
            {
                case DataType.Boolean:
                    return ExpressionDataType.Sym_Boolean;
                case DataType.Color:
                    return ExpressionDataType.Sym_Color;
                case DataType.Integer:
                    return ExpressionDataType.Sym_Integer;
                case DataType.Real:
                    return ExpressionDataType.Sym_Real;
                case DataType.String:
                    return ExpressionDataType.Sym_String;
            }
            throw new ArgumentException();
        }
#endif


        ExpressionDataType IExpressionPropertySource.ExpressionType
        {
            get { return GetExpressionType(this.DataType); }
        }
    }
 

    partial class ParameterDefinition : IParameterDefinition
    {
        [XmlIgnore]
        IEnumerable<IParameter> IParameterDefinition.Parameter
        {
            get 
            {
                foreach (var p in this.Parameter)
                {
                    yield return p;
                }
            }
        }

        public void AddParameter(IParameter param)
        {
            var p = param as Parameter;
            if (p != null)
            {
                this.Parameter.Add(p);
            }
        }

        public void RemoveParameter(IParameter param)
        {
            var p = param as Parameter;
            if (p != null)
            {
                this.Parameter.Remove(p);
            }
        }
    }

    partial class LineUsage : ILineUsage
    {
        [XmlIgnore]
        IPathGraphic ILineUsage.DefaultPath
        {
            get
            {
                return this.DefaultPath;
            }
            set
            {
                this.DefaultPath = (Path)value;
            }
        }
    }

    partial class PointUsage : IPointUsage
    {
        
    }

    partial class AreaUsage : IAreaUsage
    {
        
    }

    partial class Path : IPathGraphic
    {
        
    }

    partial class CompoundSymbolDefinition : ICompoundSymbolDefinition
    {
        public static CompoundSymbolDefinition CreateDefault()
        {
            var sym = new CompoundSymbolDefinition()
            {
                SimpleSymbol = new System.ComponentModel.BindingList<SimpleSymbol>(),
            };
            return sym;
        }

        [XmlIgnore]
        IEnumerable<ISimpleSymbolReferenceBase> ICompoundSymbolDefinition.SimpleSymbol
        {
            get 
            {
                foreach (var sym in this.SimpleSymbol)
                {
                    yield return sym;
                }
            }
        }

        void ICompoundSymbolDefinition.AddSimpleSymbol(ISimpleSymbolReferenceBase sym)
        {
            var symb = sym as SimpleSymbol;
            if (symb != null)
            {
                this.SimpleSymbol.Add(symb);
            }
        }

        void ICompoundSymbolDefinition.RemoveSimpleSymbol(ISimpleSymbolReferenceBase sym)
        {
            var symb = sym as SimpleSymbol;
            if (symb != null)
            {
                this.SimpleSymbol.Remove(symb);
            }
        }

        public ISimpleSymbolReferenceBase CreateSimpleSymbol(ISimpleSymbolDefinition sym)
        {
            return new SimpleSymbol() { Item = (SimpleSymbolDefinition)sym };
        }

        public ISimpleSymbolReferenceBase CreateSymbolReference(string resourceId)
        {
            return new SimpleSymbol() { Item = resourceId };
        }

        [XmlIgnore]
        public override SymbolDefinitionType Type
        {
            get { return SymbolDefinitionType.Compound; }
        }
    }

    partial class SimpleSymbol : ISimpleSymbolInlineReference, ISimpleSymbolLibraryReference
    {
        [XmlIgnore]
        ISimpleSymbolDefinition ISimpleSymbolInlineReference.SimpleSymbolDefinition
        {
            get
            {
                return (ISimpleSymbolDefinition)this.Item;
            }
            set
            {
                this.Item = value;
            }
        }

        [XmlIgnore]
        public SimpleSymbolReferenceType Type
        {
            get 
            {
                if (this.Item != null)
                {
                    if (typeof(ISimpleSymbolDefinition).IsAssignableFrom(this.Item.GetType()))
                        return SimpleSymbolReferenceType.Inline;
                    else if (typeof(string) == this.Item.GetType())
                        return SimpleSymbolReferenceType.Library;
                }
                return SimpleSymbolReferenceType.Undefined;
            }
        }

        [XmlIgnore]
        string IResourceIdReference.ResourceId
        {
            get
            {
                return (string)this.Item;
            }
            set
            {
                if (!ResourceIdentifier.Validate(value))
                    throw new InvalidOperationException("Not a valid resource identifier"); //LOCALIZE

                var res = new ResourceIdentifier(value);
                if (res.Extension != ResourceTypes.SymbolDefinition.ToString())
                    throw new InvalidOperationException("Invalid resource identifier for this type of object: " + res.Extension); //LOCALIZE

                this.Item = value;
                OnPropertyChanged("ResourceId");
            }
        }
    }

    partial class Image : IImageGraphic
    {
        IImageBase IImageGraphic.Item
        {
            get
            {
                byte[] content = this.Item as byte[];
                ImageReference imageRef = this.Item as ImageReference;
                if (content != null)
                    return new InlineImage() { Content = content };
                else if (imageRef != null)
                    return imageRef;
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    var inline = value as IInlineImage;
                    var imageRef = value as IImageReference;
                    if (inline != null)
                        this.Item = inline.Content;
                    else if (imageRef != null)
                        this.Item = (ImageReference)imageRef;
                }
                else
                {
                    this.Item = null;
                }
            }
        }
    }
 
    partial class ImageReference : ISymbolLibraryReference, IImageReference
    {
        [XmlIgnore]
        SymbolInstanceType ISymbolInstanceReference.Type
        {
            get { return SymbolInstanceType.Reference; }
        }

        ImageType IImageBase.Type
        {
            get { return ImageType.Reference; }
        }
    }
}

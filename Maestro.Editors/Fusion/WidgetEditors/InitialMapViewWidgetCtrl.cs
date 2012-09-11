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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.MapGuide.ObjectModels.ApplicationDefinition;
using Maestro.Editors.Common;

namespace Maestro.Editors.Fusion.WidgetEditors
{
    [ToolboxItem(false)]
    internal partial class InitialMapViewWidgetCtrl : UserControl, IWidgetEditor
    {
        public InitialMapViewWidgetCtrl()
        {
            InitializeComponent();
        }

        private IWidget _widget;

        public void Setup(IWidget widget, FlexibleLayoutEditorContext context, IEditorService edsvc)
        {
            _widget = widget;
            baseEditor.Setup(_widget, context, edsvc);
            txtViewType.Text = _widget.GetValue("ViewType"); //NOXLATE
        }

        public Control Content
        {
            get { return this; }
        }

        private string[] _values = { "full", "initial" }; //NOXLATE

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var item = GenericItemSelectionDialog.SelectItem(Strings.TitleViewType, Strings.PromptViewType, _values);
            if (item != null)
            {
                txtViewType.Text = item;
            }
        }

        private void txtViewType_TextChanged(object sender, EventArgs e)
        {
            _widget.SetValue("ViewType", txtViewType.Text); //NOXLATE
        }
    }
}

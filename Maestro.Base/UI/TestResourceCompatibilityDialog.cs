﻿#region Disclaimer / License

// Copyright (C) 2012, Jackie Ng
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

using OSGeo.MapGuide.MaestroAPI;
using System;
using System.Windows.Forms;

namespace Maestro.Base.UI
{
    internal partial class TestResourceCompatibilityDialog : Form
    {
        public TestResourceCompatibilityDialog()
        {
            InitializeComponent();
            cmbSiteVersion.DataSource = Enum.GetValues(typeof(KnownSiteVersions));
            cmbSiteVersion.SelectedIndex = 0;
        }

        public Version SelectedVersion
        {
            get { return SiteVersions.GetVersion((KnownSiteVersions)cmbSiteVersion.SelectedItem); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void cmbSiteVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblVersion.Text = this.SelectedVersion.ToString();
        }
    }
}
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

using OSGeo.MapGuide.MaestroAPI.Services;
using OSGeo.MapGuide.ObjectModels;
using OSGeo.MapGuide.ObjectModels.Common;
using System;
using System.Windows.Forms;

namespace Maestro.Editors.Common
{
    /// <summary>
    /// A control that provides a tree-based view of the repository
    /// </summary>
    public partial class RepositoryView : UserControl
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public RepositoryView()
        {
            InitializeComponent();
            this.SelectOnDrag = false;
        }

        /// <summary>
        /// Determines whether a node that is dragged causes it to be selected
        /// </summary>
        public bool SelectOnDrag { get; set; }

        /// <summary>
        /// Raises the Load event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RepositoryIcons.PopulateImageList(resImageList);
        }

        /// <summary>
        /// Navigates to the given folder, expanding any parent nodes along the way
        /// </summary>
        /// <param name="folderId"></param>
        public void NavigateTo(string folderId)
        {
            if (_model != null)
                _model.NavigateTo(folderId);
        }

        /// <summary>
        /// Initializes this view
        /// </summary>
        /// <param name="resSvc"></param>
        /// <param name="bFoldersOnly"></param>
        /// <param name="bOmitEmptyFolders"></param>
        public void Init(IResourceService resSvc, bool bFoldersOnly, bool bOmitEmptyFolders)
        {
            if (_model != null)
            {
                _model.ItemSelected -= OnItemSelectedInternal;
            }

            _model = new RepositoryFolderTreeModel(resSvc, trvRepository, bFoldersOnly, bOmitEmptyFolders);
            _model.ItemSelected += OnItemSelectedInternal;
        }

        private void OnItemSelectedInternal(object sender, EventArgs e)
        {
            this.ItemSelected?.Invoke(this, EventArgs.Empty);
        }

        private RepositoryFolderTreeModel _model;

        /// <summary>
        /// Gets the currently selected item
        /// </summary>
        public IRepositoryItem SelectedItem
        {
            get
            {
                if (_model != null)
                {
                    var it = _model.SelectedItem;
                    if (it != null)
                        return it.Instance;
                }
                return null;
            }
        }

        /// <summary>
        /// Raised when a repository item is selected
        /// </summary>
        public event EventHandler ItemSelected;

        /// <summary>
        /// Adds a resource type to filter on
        /// </summary>
        /// <param name="rt"></param>
        public void AddResourceTypeFilter(string rt)
        {
            if (_model != null) _model.AddResourceTypeFilter(rt);
        }

        /// <summary>
        /// Clears all applied resource type filters
        /// </summary>
        public void ClearResourceTypeFilters()
        {
            if (_model != null) _model.ClearResourceTypeFilters();
        }

        /// <summary>
        /// Gets whether this view has resource type filters applied
        /// </summary>
        /// <returns></returns>
        public bool HasFilteredTypes()
        {
            if (_model != null)
                return _model.HasFilteredTypes();
            else
                return false;
        }

        /// <summary>
        /// Refreshes the model of the repostiory from the specified folder id
        /// </summary>
        /// <param name="folderId"></param>
        public void RefreshModel(string folderId)
        {
            if (_model != null)
            {
                if (string.IsNullOrEmpty(folderId))
                {
                    _model.Refresh(null);
                }
                else
                {
                    if (!ResourceIdentifier.IsFolderResource(folderId))
                        throw new ArgumentException(Strings.ErrNotAFolder);

                    _model.Refresh(folderId);
                }
            }
        }

        /// <summary>
        /// Raised when an item is dragged
        /// </summary>
        public event ItemDragEventHandler ItemDrag;

        private void trvRepository_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var node = e.Item as TreeNode;
            if (node != null)
            {
                if (node != trvRepository.SelectedNode && this.SelectOnDrag)
                {
                    trvRepository.SelectedNode = node;
                }
            }

            this.ItemDrag?.Invoke(this, e);
        }
    }
}
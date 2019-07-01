﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util;

namespace Codartis.SoftVis.UI.Wpf
{
    /// <summary>
    /// Implements a WPF UI service. 
    ///  </summary>
    public class WpfUiService : IWpfUiService
    {
        private ResourceDictionary _resourceDictionary;
        private IDiagramStyleProvider _diagramStyleProvider;

        public DiagramViewModel DiagramViewModel { get; }

        public WpfUiService(DiagramViewModel diagramViewModel)
        {
            DiagramViewModel = diagramViewModel;
        }

        public void Initialize(ResourceDictionary resourceDictionary, IDiagramStyleProvider diagramStyleProvider)
        {
            _resourceDictionary = resourceDictionary;
            _diagramStyleProvider = diagramStyleProvider;
        }

        public async Task<BitmapSource> CreateDiagramImageAsync(double dpi, double margin,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null)
        {
            try
            {
                // The image creator must be created on the UI thread so it can read the necessary view and view model data.
                var diagramImageCreator = new DataCloningDiagramImageCreator(DiagramViewModel, _diagramStyleProvider, _resourceDictionary);

                return await Task.Factory.StartSTA(() =>
                    diagramImageCreator.CreateImage(dpi, margin, cancellationToken, progress, maxProgress), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public void ZoomToDiagram() => DiagramViewModel.ZoomToContent();
        public void FollowDiagramNode(IDiagramNode diagramNode) => DiagramViewModel.FollowDiagramNodes(new [] {diagramNode});
        public void FollowDiagramNodes(IReadOnlyList<IDiagramNode> diagramNodes) => DiagramViewModel.FollowDiagramNodes(diagramNodes);
        public void KeepDiagramCentered() => DiagramViewModel.KeepDiagramCentered();

        public event ShowModelItemsEventHandler ShowModelItemsRequested
        {
            add => DiagramViewModel.ShowModelItemsRequested += value;
            remove => DiagramViewModel.ShowModelItemsRequested -= value;
        }

        public event Action<IDiagramNode, Size2D> DiagramNodeSizeChanged
        {
            add => DiagramViewModel.DiagramNodeSizeChanged += value;
            remove => DiagramViewModel.DiagramNodeSizeChanged -= value;
        }

        public event Action<IDiagramNode> DiagramNodeInvoked
        {
            add => DiagramViewModel.DiagramNodeInvoked += value;
            remove => DiagramViewModel.DiagramNodeInvoked -= value;
        }

        public event Action<IDiagramNode> RemoveDiagramNodeRequested
        {
            add => DiagramViewModel.RemoveDiagramNodeRequested += value;
            remove => DiagramViewModel.RemoveDiagramNodeRequested -= value;
        }
    }
}

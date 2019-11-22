﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.UI.Wpf
{
    /// <summary>
    /// Implements a WPF UI service. 
    ///  </summary>
    public class WpfDiagramUiService : IWpfDiagramUiService
    {
        public DiagramViewModel DiagramViewModel { get; }
        public DiagramControl DiagramControl { get; }

        public WpfDiagramUiService(
            [NotNull] IDiagramUi diagramUi,
            [NotNull] DiagramControl diagramControl)
        {
            DiagramViewModel = (DiagramViewModel)diagramUi;

            DiagramControl = diagramControl;
            DiagramControl.DataContext = DiagramViewModel;
        }

        public IDiagramUi DiagramUi => DiagramViewModel;

        public async Task<BitmapSource> CreateDiagramImageAsync(
            double dpi,
            double margin,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null,
            IProgress<int> maxProgress = null)
        {
            try
            {
                // The image creator must be created on the UI thread so it can read the necessary view and view model data.
                var diagramImageCreator = new DataCloningDiagramImageCreator(DiagramViewModel, DiagramControl);

                return await Task.Factory.StartSTA(
                    () =>
                        diagramImageCreator.CreateImage(dpi, margin, cancellationToken, progress, maxProgress),
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public void ZoomToDiagram() => DiagramViewModel.ZoomToContent();
        public void FollowDiagramNode(ModelNodeId nodeId) => DiagramViewModel.FollowDiagramNodes(new[] { nodeId });
        public void FollowDiagramNodes(IReadOnlyList<ModelNodeId> nodeIds) => DiagramViewModel.FollowDiagramNodes(nodeIds);
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

        public event Action<IDiagramNode, Size2D> DiagramNodePayloadAreaSizeChanged
        {
            add => DiagramViewModel.DiagramNodePayloadAreaSizeChanged += value;
            remove => DiagramViewModel.DiagramNodePayloadAreaSizeChanged -= value;
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
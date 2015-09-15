﻿using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands.ShellTriggered
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddToDiagramCommand : ShellTriggeredCommandBase
    {
        public AddToDiagramCommand(IPackageServices packageServices)
            :base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.AddToDiagramCommand, packageServices)
        {
        }

        public override async void Execute(object sender, EventArgs e)
        {
            var workspaceServices = PackageServices.GetWorkspaceServices();
            var symbol = await workspaceServices.GetCurrentSymbol();

            var modelBuilder = PackageServices.GetModelServices();
            var modelEntity = modelBuilder.GetModelEntity(symbol);

            var diagramServices = PackageServices.GetDiagramServices();
            diagramServices.ShowModelEntity(modelEntity);
            diagramServices.ShowDiagram();
            diagramServices.FitDiagramToView();
        }
    }
}
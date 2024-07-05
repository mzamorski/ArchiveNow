using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ArchiveNow.Utils.VsSolution;
using Microsoft.Build.Evaluation;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace ArchiveNow.Integration
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".sln")]
    public class VsSolutionShellContextMenu : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            var menuItem = new ToolStripMenuItem
            {
                Text = "Archive solution..."
            };

            menuItem.Click += (sender, args) => ParseSolution();
            menu.Items.Add(menuItem);

            return menu;
        }

        private void ParseSolution()
        {
            foreach (var filePath in SelectedItemPaths)
            {
                var directory = Path.GetDirectoryName(filePath);

                var solution = new Solution(filePath);
                foreach (var solutionProject in solution.Projects)
                {
                    string fullpath = Path.Combine(directory, solutionProject.RelativePath);
                    MessageBox.Show(fullpath);

                    var project = new Project(fullpath);
                    foreach (var definition in project.ItemDefinitions)
                    {
                        MessageBox.Show(definition.Key + "/" + definition.Value);
                    }
                }
            }
            
        }
    }
}
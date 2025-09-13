using System.Windows.Forms;
using System.Collections.Generic;


namespace SimpleJSONTreeViewer
{
    internal class MatchResult
    {
        private int index;
        private List<TreeNode> nodes = new List<TreeNode>();
        private TreeView ownerTreeView;

        public MatchResult(List<TreeNode> nodes, TreeView ownerTreeView)
        {
            this.index = 0;
            this.nodes = nodes;
            this.ownerTreeView = ownerTreeView;

            this.RefreshTreeView();
        }
        public void Last()
        {
            index--;

            if (index < 0)
                index = nodes.Count - 1;
            
            RefreshTreeView();
        }
        public void Next()
        {
            index++;

            if (index == nodes.Count)
                index = 0;

            RefreshTreeView();
        }
        public void RefreshTreeView()
        {
            ownerTreeView.SelectedNode = nodes[index];
            ownerTreeView.SelectedNode.EnsureVisible();
            ownerTreeView.Focus();
        }
        public string MatchStr()
        {
            return (index + 1) + "/" + nodes.Count;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace SimpleJSONTreeViewer
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        /// 
        public const string LeafNode = "LeafNode";
        public const int MaxSize = 1 * 1024 * 1024;

        private System.ComponentModel.IContainer components = null;
        private int dataSize; // 载入的json数据的大小
        private TreeView treeview;
        private Panel topPanel; // 顶部输入框、按钮等控件的容器
        private MenuStrip menuStrip;
        private TextBox textBox; // 查找关键字的文本框
        private Label labelMatchResult; // 显示查找结果
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem toolStripMenuItemCopyKey;
        private ToolStripMenuItem toolStripMenuItemCopyValue;
        private ToolStripMenuItem toolStripMenuItemCopyObj;
        private Dictionary<string, MatchResult> matchResultCache = new Dictionary<string, MatchResult>();

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Text = "SimpleJSONTreeViewer";

            this.menuStrip = new MenuStrip();
            ToolStripMenuItem importJSONDataMenu = new ToolStripMenuItem("导入JSON数据");
            ToolStripMenuItem menuItemFromFile = new ToolStripMenuItem("从文件");
            menuItemFromFile.Click += new EventHandler(OnImportFromFileClick);
            ToolStripMenuItem menuItemFormClipboard = new ToolStripMenuItem("从剪贴板");
            menuItemFormClipboard.Click += new EventHandler(OnFromClipboardClick);

            importJSONDataMenu.DropDownItems.Add(menuItemFromFile);
            importJSONDataMenu.DropDownItems.Add(menuItemFormClipboard);

            ToolStripMenuItem helpMenu = new ToolStripMenuItem("帮助(&H)");
            helpMenu.DropDownItems.Add("关于(&A)");

            this.menuStrip.Items.Add(importJSONDataMenu);
            this.menuStrip.Items.Add(helpMenu);
            this.Controls.Add(this.menuStrip);

            this.topPanel = new Panel();
            this.topPanel.Location = new Point(5, 26);
            this.topPanel.Size = new Size(this.ClientSize.Width - 10, 21);

            this.textBox = new TextBox();
            this.textBox.BorderStyle = BorderStyle.FixedSingle;
            this.textBox.Left = 0;
            this.textBox.Top = 0;
            this.textBox.Size = new Size(300, 80);
            this.textBox.KeyDown += new KeyEventHandler(OnTextBoxKeyDown);
            this.topPanel.Controls.Add(this.textBox);

            Button searchButton = new Button();
            searchButton.Left = 305;
            searchButton.Text = "查找";
            searchButton.Size = new Size(50, 21);
            searchButton.Click += new EventHandler(OnSearchBtnClick);
            this.topPanel.Controls.Add(searchButton);

            Button clearButton = new Button();
            clearButton.Left = 360;
            clearButton.Text = "清除";
            clearButton.Size = new Size(50, 21);
            clearButton.Click += new EventHandler(OnClearBtnClick);
            this.topPanel.Controls.Add(clearButton);

            Button lastButton = new Button();
            lastButton.Left = 415;
            lastButton.Text = "上一个";
            lastButton.Size = new Size(50, 21);
            lastButton.Click += new EventHandler(OnLastBtnClick);
            this.topPanel.Controls.Add(lastButton);

            Button nextButton = new Button();
            nextButton.Left = 470;
            nextButton.Text = "下一个";
            nextButton.Size = new Size(50, 21);
            nextButton.Click += new EventHandler(OnNextBtnClick);
            this.topPanel.Controls.Add(nextButton);

            Button expandAllButton = new Button();
            expandAllButton.Left = 525;
            expandAllButton.Text = "展开所有节点";
            expandAllButton.Size = new Size(90, 21);
            expandAllButton.Click += new EventHandler(OnExpandAllBtnClick);
            this.topPanel.Controls.Add(expandAllButton);

            Button collapseAllButton = new Button();
            collapseAllButton.Left = 620;
            collapseAllButton.Text = "收起所有节点";
            collapseAllButton.Size = new Size(90, 21);
            collapseAllButton.Click += new EventHandler(OnCollapseAllBtnClick);
            this.topPanel.Controls.Add(collapseAllButton);

            Label label = new Label();
            label.Text = "查找结果";
            label.Left = 720;
            label.Top = 4;
            label.Size = new Size(55, 21);
            topPanel.Controls.Add(label);

            this.labelMatchResult = new Label();
            this.labelMatchResult.Left = 775;
            this.labelMatchResult.Top = 4;
            this.labelMatchResult.Size = new Size(50, 21);
            this.topPanel.Controls.Add(label);
            this.topPanel.Controls.Add(this.labelMatchResult);
            this.Controls.Add(this.topPanel);

            this.contextMenuStrip = new ContextMenuStrip();

            this.toolStripMenuItemCopyKey = new ToolStripMenuItem("复制Key");
            this.toolStripMenuItemCopyKey.Click += new EventHandler(OnCopyKey);

            this.toolStripMenuItemCopyValue = new ToolStripMenuItem("复制Value");
            this.toolStripMenuItemCopyValue.Click += new EventHandler(OnCopyValue);

            this.toolStripMenuItemCopyObj = new ToolStripMenuItem("复制对象");
            this.toolStripMenuItemCopyObj.Click += new EventHandler(OnCopyObj);

            this.contextMenuStrip.Items.AddRange(new ToolStripItem[] { this.toolStripMenuItemCopyKey, this.toolStripMenuItemCopyValue, this.toolStripMenuItemCopyObj });

            this.treeview = new TreeView();
            this.treeview.Font = new Font("Microsoft Sans Serif", 10);
            this.treeview.ShowLines = true;
            this.treeview.Width = this.ClientSize.Width - 10;
            this.treeview.Height = this.ClientSize.Height - this.topPanel.Height - this.menuStrip.Height - 10;
            this.treeview.Left = this.ClientSize.Width / 2 - this.treeview.Width / 2;
            this.treeview.Top = (this.ClientSize.Height - this.topPanel.Height - this.menuStrip.Height) / 2 - this.treeview.Height / 2 + this.topPanel.Height + this.menuStrip.Height;
            this.treeview.BeforeExpand += new TreeViewCancelEventHandler(OnBeforTreeNodeExpand);
            this.treeview.MouseClick += new MouseEventHandler(OnMouseClick);
            Controls.Add(this.treeview);
        }
        #endregion

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (this.treeview != null)
            {
                this.treeview.Width = this.ClientSize.Width - 10;
                this.treeview.Height = this.ClientSize.Height - this.topPanel.Height - this.menuStrip.Height - 10;
            }

            if (this.topPanel != null)
            {
                this.topPanel.Size = new Size(this.ClientSize.Width - 10, 21);
            }
        }
        private void Copy(bool isKey)
        {
            if (this.treeview.Nodes.Count > 0)
            {
                TreeNode treeNodeSelected = this.treeview.SelectedNode;

                if (treeNodeSelected != null && treeNodeSelected.Name == LeafNode)
                {
                    int index = treeNodeSelected.Text.IndexOf(":");

                    if (index != -1)
                    {
                        string data = isKey ? treeNodeSelected.Text.Substring(0, index).Trim() : treeNodeSelected.Text.Substring(index + 1).Trim();
                        Clipboard.SetText(data);
                    }
                }
            }
        }
        private void OnCopyKey(object sender, EventArgs e)
        {
            this.Copy(true);
        }
        private void OnCopyValue(object sender, EventArgs e)
        {
            this.Copy(false);
        }
        private void OnCopyObj(object sender, EventArgs e)
        {
            TreeNode treeNodeSelected = this.treeview.SelectedNode;

            if (treeNodeSelected != null && treeNodeSelected.Name != LeafNode)
            {
                Clipboard.SetText((treeNodeSelected.Tag as TreeNodeContext).Value.ToString());
            }
        }
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.treeview.SelectedNode != null)
            {
                if (this.treeview.SelectedNode.Name == LeafNode)
                {
                    this.toolStripMenuItemCopyObj.Enabled = false;
                    this.toolStripMenuItemCopyKey.Enabled = true;
                    this.toolStripMenuItemCopyValue.Enabled = true;
                }
                else
                {
                    this.toolStripMenuItemCopyObj.Enabled = true;
                    this.toolStripMenuItemCopyKey.Enabled = false;
                    this.toolStripMenuItemCopyValue.Enabled = false;
                }

                this.contextMenuStrip.Show(this.treeview, e.Location);
            }
        }
        /// <summary>
        /// 处理Enter键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(this.textBox.Text) && this.treeview.Nodes.Count > 0)
                {
                    OnSearchBtnClick(null, new EventArgs());
                    e.Handled = true;
                }
            }
        }
        /// <summary>
        /// 清除按钮点击时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClearBtnClick(object sender, EventArgs e)
        {
            this.textBox.Text = string.Empty;
            this.matchResultCache.Clear();
            this.labelMatchResult.Text = string.Empty;
        }
        /// <summary>
        /// 展开所有节点按钮点击时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExpandAllBtnClick(object sender, EventArgs e)
        {
            if (treeview.Nodes.Count == 0)
                return;

            if (this.dataSize <= MaxSize || MessageBox.Show("当前加载的JSON数据大于1M，可能耗时较久，是否继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                this.treeview.BeginUpdate();
                this.treeview.ExpandAll();
                this.treeview.EndUpdate();
                this.treeview.Nodes[0].EnsureVisible(); // 全部展开时默认会到最底部，此时设置确保根节点可见，使得不滚动到底部
            }
        }
        /// <summary>
        /// 收起所有按钮点击时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollapseAllBtnClick(object sender, EventArgs e)
        {
            if (treeview.Nodes.Count == 0)
                return;

            this.treeview.CollapseAll();
        }
        /// <summary>
        /// 从文件导入json数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImportFromFileClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件|*.*|json文件|*.json";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.treeview.Nodes.Clear();
                string res = File.ReadAllText(openFileDialog.FileName);
                this.dataSize = res.Length;
                JToken root = null;

                try
                {
                    root = JToken.Parse(res);
                }
                catch 
                {
                    MessageBox.Show("从文件中解析JSON数据异常，请检查文件内容是否符合JSON数据格式", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 只生成一层节点，其他下层节点在展开当前选中节点时再生成对应的孩子节点，根节点名称默认为Root
                TreeNode rootNode = GenerateChildNodeLazy(root, "Root"); //GenerateTreeNode(root, "Root");

                this.treeview.BeginUpdate();
                this.treeview.Nodes.Add(rootNode);
                this.treeview.EndUpdate();
            }
        }
        /// <summary>
        /// 从剪贴板导入json数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFromClipboardClick(object sender, EventArgs e)
        {
            string res = Clipboard.GetText();

            if (!string.IsNullOrEmpty(res))
            {
                this.treeview.Nodes.Clear();
                this.dataSize = res.Length;
                JToken root = null;

                try
                {
                    root = JToken.Parse(res);
                }
                catch
                {
                    MessageBox.Show(this, "从剪贴板中解析JSON数据异常，请检查剪贴板内容是否符合JSON数据格式", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                TreeNode rootNode = GenerateChildNodeLazy(root, "Root");

                this.treeview.BeginUpdate();
                this.treeview.Nodes.Add(rootNode);
                this.treeview.EndUpdate();
            }
        }
        /// <summary>
        /// 上一个 按钮点击时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLastBtnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBox.Text) && this.matchResultCache.ContainsKey(this.textBox.Text))
            {
                MatchResult matchResult = this.matchResultCache[this.textBox.Text];
                matchResult.Last();
                this.labelMatchResult.Text = matchResult.MatchStr();
            }
        }
        /// <summary>
        /// 下一个 按钮点击时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNextBtnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBox.Text) && this.matchResultCache.ContainsKey(this.textBox.Text))
            {
                MatchResult matchResult = this.matchResultCache[this.textBox.Text];
                matchResult.Next();
                this.labelMatchResult.Text = matchResult.MatchStr();
            }
        }
        /// <summary>
        /// 搜索 按钮点击时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchBtnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.textBox.Text) && this.treeview.Nodes.Count > 0)
            {
                if (this.matchResultCache.ContainsKey(this.textBox.Text))
                {
                    MatchResult matchResult = this.matchResultCache[this.textBox.Text];
                    matchResult.RefreshTreeView();
                    this.labelMatchResult.Text = matchResult.MatchStr();
                    return;
                }

                TreeNode root = this.treeview.Nodes[0];
                List<JToken> result = new List<JToken>();
                TraverseSearch(this.textBox.Text, result, (root.Tag as TreeNodeContext).Value);
                List<List<JToken>> allMatchObjectPath = new List<List<JToken>>(result.Count);

                for (int i = 0; i < result.Count; i++)
                {
                    List<JToken> list = new List<JToken>();

                    if (!(result[i] is JProperty)) // 忽略JProperty，因为TreeNode的Tag中保存的是JArray、JObject、JString等类型，不会保存JProperty
                    {
                        list.Insert(0, result[i]);
                    }

                    JToken parent = result[i].Parent;

                    while (parent != null)
                    {
                        if (!(parent is JProperty))
                        {
                            list.Insert(0, parent);
                        }

                        parent = parent.Parent;
                    }

                    allMatchObjectPath.Add(list);
                }

                List<TreeNode> allMatchTreeNode = new List<TreeNode>();

                foreach (List<JToken> path in allMatchObjectPath)
                {
                    TreeNode treeNode = FindTreeNode(this.treeview.Nodes[0], path, 0);

                    if (treeNode != null)
                        allMatchTreeNode.Add(treeNode);
                }

                if (allMatchTreeNode.Count > 0)
                {
                    MatchResult matchResult = new MatchResult(allMatchTreeNode, this.treeview);
                    this.matchResultCache[this.textBox.Text] = matchResult;
                    this.labelMatchResult.Text = matchResult.MatchStr();
                }
                else
                {
                    this.labelMatchResult.Text = "0";
                }
            }
        }
        /// <summary>
        /// 从树根节点，根据path保存的对象路径，找到目标树节点
        /// </summary>
        /// <param name="treeNode">当前处理的树节点</param>
        /// <param name="path">一些列对象组成的路径集合，第1个元素是根节点对应的JToken对象，第2是第二层，以此类推</param>
        /// <param name="index">path的索引位置</param>
        /// <returns>符合此路径的树节点</returns>
        private TreeNode FindTreeNode(TreeNode treeNode, List<JToken> path, int index)
        {
            if (index < path.Count && (treeNode.Tag as TreeNodeContext).Value == path[index])
            {
                if (index == path.Count - 1)
                    return treeNode;

                GenerateChildNodeLazy(treeNode);

                for (int i = 0; i < treeNode.Nodes.Count; i++)
                {
                    TreeNode res = FindTreeNode(treeNode.Nodes[i], path, index + 1);

                    if (res != null)
                        return res;
                }
            }

            return null;
        }
        /// <summary>
        /// 根据搜索关键字，递归遍历加载的json数据，找到所有包含关键字的JToken（此方法的前置条件是TreeNode的Tag存储了它所对应的JToken对象，目的是根据关键字找到与之对应的JToken对象，以便获得从它到根JToken的对象组成的路径）
        /// </summary>
        /// <param name="keyword">搜索关键字</param>
        /// <param name="result">存储所有包含搜索关键字的JToken</param>
        /// <param name="jToken">当前要读取的JToken数据</param>
        private void TraverseSearch(string keyword, List<JToken> result, JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Array:
                    var array = (JArray)jToken;
                    for (int i = 0; i < array.Count; i++)
                    {
                        TraverseSearch(keyword, result, array[i]);
                    }
                    break;

                case JTokenType.Object:
                    var obj = (JObject)jToken;
                    foreach (var property in obj.Properties())
                    {
                        if (!string.IsNullOrEmpty(property.Name) && property.Name.Contains(keyword)) // 对象的某个属性名包含关键字，
                            result.Add(property.Value);

                        TraverseSearch(keyword, result, property.Value);
                    }
                    break;

                default:
                    if (jToken.ToString().Contains(keyword))
                    {
                        result.Add(jToken);
                    }
                    break;
            }
        }
        /// <summary>
        /// 一次性创建所有节点
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="text"></param>
        /// <returns>返回根节点</returns>
        private TreeNode GenerateTreeNode(JToken jToken, string text)
        {
            TreeNode treeNode = new TreeNode();

            switch (jToken.Type)
            {
                case JTokenType.Array:
                    treeNode.Text = "[] " + text;
                    var array = (JArray)jToken;

                    for (int i = 0; i < array.Count; i++)
                    {
                        treeNode.Nodes.Add(GenerateTreeNode(array[i], i.ToString()));
                    }
                    break;

                case JTokenType.Object:
                    treeNode.Text = "{} " + text;
                    var obj = (JObject)jToken;

                    foreach (var property in obj.Properties())
                    {
                        treeNode.Nodes.Add(GenerateTreeNode(property.Value, property.Name));
                    }
                    break;

                default:
                    treeNode.Text = text + " : " + jToken.ToString();
                    treeNode.Name = LeafNode;
                    break;
            }

            return treeNode;
        }
        /// <summary>
        /// 根据json数据是对象或是数组、或是基本属性，创建树节点，不递归创建，只创建下一层子节点
        /// </summary>
        /// <param name="jToken">当前要处理的数据</param>
        /// <param name="text">生成的节点设置的text值</param>
        /// <returns>根据jToken参数创建的树节点</returns>
        private TreeNode GenerateChildNodeLazy(JToken jToken, string text)
        {
            TreeNode treeNode = new TreeNode();

            switch (jToken.Type)
            {
                case JTokenType.Array:
                    treeNode.Nodes.Add(new TreeNode()); // 用来占位的节点，使得父节点是对象或者数组类型且还没有加载没有孩子节点时，可以显示展开图标，以便在点击展开图标时可以触发OnBeforTreeNodeExpand来加载下一层孩子节点
                    treeNode.Text = "[] " + text;
                    break;

                case JTokenType.Object:
                    treeNode.Nodes.Add(new TreeNode());
                    treeNode.Text = "{} " + text;
                    break;

                default:
                    treeNode.Text = text + " : " + jToken.ToString();
                    treeNode.Name = LeafNode;
                    break;
            }

            treeNode.Tag = new TreeNodeContext(jToken, false);
            return treeNode;
        }
        /// <summary>
        /// 用于展开树节点时，根据节点绑定的tag是对象还是数组，调用GenerateChildNodeLazy方法创建它的下一层节点
        /// </summary>
        /// <param name="parent">父节点</param>
        private void GenerateChildNodeLazy(TreeNode parent)
        {
            TreeNodeContext treeNodeContext = parent.Tag as TreeNodeContext;
            JToken jToken = treeNodeContext.Value;

            if (!treeNodeContext.IsChildLoaded) // 孩子节点没创建则走创建孩子节点的逻辑
            {
                treeNodeContext.IsChildLoaded = true;
                this.treeview.BeginUpdate();
                parent.Nodes.Clear(); // 清除用于占位的孩子节点

                switch (jToken.Type)
                {
                    case JTokenType.Array:
                        var array = (JArray)jToken;

                        for (int i = 0; i < array.Count; i++)
                        {
                            parent.Nodes.Add(GenerateChildNodeLazy(array[i], i.ToString()));
                        }
                        break;

                    case JTokenType.Object:
                        var obj = (JObject)jToken;

                        foreach (var property in obj.Properties())
                        {
                            parent.Nodes.Add(GenerateChildNodeLazy(property.Value, property.Name));
                        }
                        break;
                }

                this.treeview.EndUpdate();
            }
        }
        private void OnBeforTreeNodeExpand(object sender, TreeViewCancelEventArgs e)
        {
            this.GenerateChildNodeLazy(e.Node);
        }
    }
}


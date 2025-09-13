using Newtonsoft.Json.Linq;

namespace SimpleJSONTreeViewer
{
    internal class TreeNodeContext
    {
        private JToken value;
        private bool isChildLoaded;
        public TreeNodeContext(JToken value, bool isChildLoaded)
        {
            this.value = value;
            this.isChildLoaded = isChildLoaded;
        }

        public JToken Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public bool IsChildLoaded
        {
            get { return isChildLoaded; }
            set { isChildLoaded = value; }
        }
    }
}

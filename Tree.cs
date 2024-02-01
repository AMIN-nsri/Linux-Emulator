using System;
namespace Linux
{
    class TreeNode
    {
        public string Name { get; }
        public bool IsDirectory { get; }
        public List<TreeNode> Children { get; } = new List<TreeNode>();
        public TreeNode Parent { get; set; }

        public TreeNode(string name, bool isDirectory)
        {
            Name = name;
            IsDirectory = isDirectory;
        }
    }
}


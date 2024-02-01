using System;
namespace Linux
{
	//public class FileSystem
	//{
	//	public FileSystem()
	//	{
	//	}
        class FileSystem
        {
            public TreeNode Root { get; } = new TreeNode("/", true);
            public TreeNode CurrentDirectory { get; set; }

            public FileSystem()
            {
                CurrentDirectory = Root;
            }

            public TreeNode GetNodeByPath(string path)
            {
                TreeNode currentNode = CurrentDirectory;

                foreach (var segment in path.Split('/').Where(s => !string.IsNullOrEmpty(s)))
                {
                    currentNode = currentNode.Children.FirstOrDefault(c => c.Name == segment);

                    if (currentNode == null)
                        return null;
                }

                return currentNode;
            }
        }
    //}
}


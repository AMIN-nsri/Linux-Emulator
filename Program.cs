using System;
using System.Collections.Generic;
using System.Linq;

namespace Linux 
{    
    

    

    class Program
    {
        public static FileSystem fileSystem = new FileSystem();

        static void Main()
        {
            
            Console.WriteLine("Linux Terminal Emulator with Tree Data Structure");

            while (true)
            {
                Console.Write($"{fileSystem.CurrentDirectory.Name}> ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                string output = ExecuteCommand(input);
                Console.WriteLine(output);
            }
        }

        static string ExecuteCommand(string command)
        {
            string[] args = command.Split(' ');

            switch (args[0].ToLower())
            {
                case "su":
                    if (args.Length >= 2)
                    {
                        string username = args[1];
                        if (username == "AMIN" || username == "guest")
                            return $"Logged in as {username}";
                        else return $"Invalid username: {username}";
                    }
                    else
                    {
                        return "Usage: su Username";
                    }

                case "pwd":
                    return $"Current Directory: {GetPath(fileSystem.CurrentDirectory)}";

                case "mkdir":
                    if (args.Length >= 2)
                    {
                        string name = args[1];
                        TreeNode newDirectory = new TreeNode(name, true);
                        newDirectory.Parent = fileSystem.CurrentDirectory;
                        fileSystem.CurrentDirectory.Children.Add(newDirectory);
                        return $"Directory '{name}' created in {GetPath(fileSystem.CurrentDirectory)}";
                    }
                    else
                    {
                        return "Usage: mkdir Name";
                    }

                case "touch":
                    if (args.Length >= 2)
                    {
                        string name = args[1];
                        TreeNode newFile = new TreeNode(name, false);
                        newFile.Parent = fileSystem.CurrentDirectory;
                        fileSystem.CurrentDirectory.Children.Add(newFile);
                        return $"File '{name}' created in {GetPath(fileSystem.CurrentDirectory)}";
                    }
                    else
                    {
                        return "Usage: touch Name";
                    }

                case "rmdir":
                    if (args.Length >= 2)
                    {
                        string name = args[1];
                        TreeNode directoryToRemove = fileSystem.CurrentDirectory.Children.FirstOrDefault(c => c.Name == name && c.IsDirectory);

                        if (directoryToRemove != null)
                        {
                            fileSystem.CurrentDirectory.Children.Remove(directoryToRemove);
                            return $"Directory '{name}' removed from {GetPath(fileSystem.CurrentDirectory)}";
                        }
                        else
                        {
                            return $"Directory '{name}' not found in {GetPath(fileSystem.CurrentDirectory)}";
                        }
                    }
                    else
                    {
                        return "Usage: rmdir Name";
                    }

                case "cp":
                    if (args.Length >= 3)
                    {
                        string sourcePath = args[1];
                        string destinationPath = args[2];

                        TreeNode sourceNode = fileSystem.GetNodeByPath(sourcePath);
                        TreeNode destinationNode = fileSystem.GetNodeByPath(destinationPath);

                        if (sourceNode != null && destinationNode != null && !sourceNode.IsDirectory)
                        {
                            TreeNode copiedFile = new TreeNode(sourceNode.Name, false);
                            copiedFile.Parent = destinationNode;
                            destinationNode.Children.Add(copiedFile);
                            return $"File '{sourceNode.Name}' copied to {GetPath(destinationNode)}";
                        }
                        else
                        {
                            return "Invalid source or destination path";
                        }
                    }
                    else
                    {
                        return "Usage: cp /Absolute_Path1 /Absolute_Path2";
                    }

                case "mv":
                    if (args.Length >= 3)
                    {
                        string sourcePath = args[1];
                        string destinationPath = args[2];

                        TreeNode sourceNode = fileSystem.GetNodeByPath(sourcePath);
                        TreeNode destinationNode = fileSystem.GetNodeByPath(destinationPath);

                        if (sourceNode != null && destinationNode != null && !sourceNode.IsDirectory)
                        {
                            sourceNode.Parent.Children.Remove(sourceNode);
                            sourceNode.Parent = destinationNode;
                            destinationNode.Children.Add(sourceNode);
                            return $"File '{sourceNode.Name}' moved to {GetPath(destinationNode)}";
                        }
                        else
                        {
                            return "Invalid source or destination path";
                        }
                    }
                    else
                    {
                        return "Usage: mv /Absolute_Path1 /Absolute_Path2";
                    }

                case "ls":
                    string fileList = string.Join("  ", fileSystem.CurrentDirectory.Children.Select(c => c.Name));
                    return fileList;

                case "cd":
                    if (args.Length >= 2)
                    {
                        string name = args[1];

                        if (name == "..")
                        {
                            if (fileSystem.CurrentDirectory.Parent != null)
                                fileSystem.CurrentDirectory = fileSystem.CurrentDirectory.Parent;
                            return $"Moved to parent directory: {GetPath(fileSystem.CurrentDirectory)}";
                        }
                        else
                        {
                            TreeNode directoryToEnter = fileSystem.CurrentDirectory.Children.FirstOrDefault(c => c.Name == name && c.IsDirectory);

                            if (directoryToEnter != null)
                            {
                                fileSystem.CurrentDirectory = directoryToEnter;
                                return $"Entered directory '{name}': {GetPath(fileSystem.CurrentDirectory)}";
                            }
                            else
                            {
                                return $"Directory '{name}' not found in {GetPath(fileSystem.CurrentDirectory)}";
                            }
                        }
                    }
                    else
                    {
                        return "Usage: cd Name (use '..' to move to the parent directory)";
                    }

                case "find":
                    if (args.Length >= 5)
                    {
                        string startingPath = args[1];
                        string type = args[2].ToLower();
                        string searchName = args[3];
                        string searchType = args[4].ToLower();

                        TreeNode startingNode = fileSystem.GetNodeByPath(startingPath);

                        if (startingNode != null && startingNode.IsDirectory)
                        {
                            List<TreeNode> searchResults;

                            if (searchType == "bfs")
                                searchResults = BFS(startingNode, type, searchName);
                            else if (searchType == "dfs")
                                searchResults = DFS(startingNode, type, searchName);
                            else
                                return "Invalid search type. Use 'bfs' or 'dfs'.";

                            string resultOutput = string.Join(", ", searchResults.Select(n => GetPath(n)));
                            return $"Search results: {resultOutput}";
                        }
                        else
                        {
                            return "Invalid starting path";
                        }
                    }
                    else
                    {
                        return "Usage: find Absolute_Path Type Name Search (Search: bfs or dfs)";
                    }

                case "sp":
                    if (args.Length >= 3)
                    {
                        string path1 = args[1];
                        string path2 = args[2];

                        TreeNode commonFolder = FindCommonFolder(path1, path2);

                        if (commonFolder != null)
                            return $"Common Folder: {GetPath(commonFolder)}";
                        else
                            return "No common folder found";
                    }
                    else
                    {
                        return "Usage: sp /Absolute_Path1 /Absolute_Path2";
                    }

                case "sz":
                    if (args.Length >= 2)
                    {
                        string path = args[1];
                        TreeNode node = fileSystem.GetNodeByPath(path);

                        if (node != null && node.IsDirectory)
                        {
                            int size = CalculateDirectorySize(node);
                            return $"Size of {GetPath(node)}: {size} bytes";
                        }
                        else
                        {
                            return "Invalid directory path";
                        }
                    }
                    else
                    {
                        return "Usage: sz /Absolute_Path1";
                    }

                default:
                    return $"Command not found: {args[0]}";
            }
        }

        static string GetPath(TreeNode node)
        {
            if (node == fileSystem.Root)
                return "/";

            return GetPath(node.Parent) + "/" + node.Name;
        }

        static TreeNode FindCommonFolder(string path1, string path2)
        {
            TreeNode node1 = fileSystem.GetNodeByPath(path1);
            TreeNode node2 = fileSystem.GetNodeByPath(path2);

            if (node1 != null && node2 != null)
            {
                List<TreeNode> ancestors1 = GetAncestors(node1);
                List<TreeNode> ancestors2 = GetAncestors(node2);

                TreeNode commonFolder = ancestors1.Intersect(ancestors2).FirstOrDefault();

                return commonFolder;
            }

            return null;
        }

        static List<TreeNode> GetAncestors(TreeNode node)
        {
            List<TreeNode> ancestors = new List<TreeNode>();

            while (node != null)
            {
                ancestors.Insert(0, node);
                node = node.Parent;
            }

            return ancestors;
        }

        static int CalculateDirectorySize(TreeNode directory)
        {
            int size = 0;

            foreach (var child in directory.Children)
            {
                if (child.IsDirectory)
                {
                    size += CalculateDirectorySize(child);
                }
                else
                {
                    // Assuming each file has a size of 1 for simplicity
                    size++;
                }
            }

            return size;
        }

        static List<TreeNode> BFS(TreeNode startingNode, string type, string searchName)
        {
            List<TreeNode> results = new List<TreeNode>();
            Queue<TreeNode> queue = new Queue<TreeNode>();

            queue.Enqueue(startingNode);

            while (queue.Count > 0)
            {
                TreeNode currentNode = queue.Dequeue();

                if (currentNode.Name.ToLower().Contains(searchName.ToLower()))
                {
                    if (type == "f" && !currentNode.IsDirectory)
                        results.Add(currentNode);
                    else if (type == "d" && currentNode.IsDirectory)
                        results.Add(currentNode);
                }

                foreach (var child in currentNode.Children)
                {
                    queue.Enqueue(child);
                }
            }

            return results;
        }

        static List<TreeNode> DFS(TreeNode startingNode, string type, string searchName)
        {
            List<TreeNode> results = new List<TreeNode>();
            Stack<TreeNode> stack = new Stack<TreeNode>();

            stack.Push(startingNode);

            while (stack.Count > 0)
            {
                TreeNode currentNode = stack.Pop();

                if (currentNode.Name.ToLower().Contains(searchName.ToLower()))
                {
                    if (type == "f" && !currentNode.IsDirectory)
                        results.Add(currentNode);
                    else if (type == "d" && currentNode.IsDirectory)
                        results.Add(currentNode);
                }

                foreach (var child in currentNode.Children)
                {
                    stack.Push(child);
                }
            }

            return results;
        }
    }


}
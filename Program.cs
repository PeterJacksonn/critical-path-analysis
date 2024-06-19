using System;

namespace CriticalPath
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome");
            Console.WriteLine("How many Nodes are in your graph/project");
            int totalNodes = Convert.ToInt32(Console.ReadLine());
            int[][][] nodes = new int[totalNodes][][];

            int currNode = 0;

            while (currNode < totalNodes-1)
            {
                Console.WriteLine($"(N {currNode}) enter time,end node"); 
                string temp = Console.ReadLine();
                string[] temp2 = temp.Split(',');

                // Convert the string values to integers and create a new array
                int[] task = { Convert.ToInt32(temp2[0]), currNode , Convert.ToInt32(temp2[1]) };

                // If nodes[currNode] is null, create a new array with the current task
                if (nodes[currNode] == null)
                {
                    nodes[currNode] = new int[][] { task };
                }
                else
                {
                    // If nodes[currNode] is not null, append the new task to the existing array
                    int[][] tempArray = new int[nodes[currNode].Length + 1][];
                    Array.Copy(nodes[currNode], tempArray, nodes[currNode].Length);
                    tempArray[tempArray.Length - 1] = task;
                    nodes[currNode] = tempArray;
                }

                bool choice = false;
                while (choice == false)
                {
                    Console.WriteLine("More tasks from this node? y/n");
                    string moreTasks = Console.ReadLine();
                    if (moreTasks == "y")
                    {
                        Console.WriteLine($"(N {currNode}) enter time,end node");
                        temp = Console.ReadLine();
                        temp2 = temp.Split(',');

                        // Convert the string values to integers and create a new array
                        task = new int[] { Convert.ToInt32(temp2[0]), currNode, Convert.ToInt32(temp2[1]) };

                        // Append the new task to the existing array
                        int[][] tempArray = new int[nodes[currNode].Length + 1][];
                        Array.Copy(nodes[currNode], tempArray, nodes[currNode].Length);
                        tempArray[tempArray.Length - 1] = task;
                        nodes[currNode] = tempArray; 
                    }
                    else if (moreTasks == "n")
                    {
                        currNode++;
                        choice = true;
                    }
                }
            }

            // Output the arrays in nodes
            for (int i = 0; i < totalNodes-1; i++)
            {
                Console.WriteLine($"Node {i}:");
                if (nodes[i] != null)
                {
                    foreach (var task in nodes[i])
                    {
                        Console.WriteLine($"Time: {task[0]}, End Node: {task[2]}");
                    }
                }
            }

            // Find all paths
            Console.WriteLine("\nAll Paths:");
            FindAllPaths(nodes, totalNodes);
        }

        static void FindAllPaths(int[][][] nodes, int totalNodes)
        {
            List<List<int>> allPaths = new List<List<int>>(); //list of lists, to hold each path
            List<int> currentPath = new List<int>();

            DFS(nodes, 0, totalNodes - 1, currentPath, allPaths);

            // Output all paths and their total time
            for (int i = 0; i < allPaths.Count; i++)
            {
                Console.Write($"Path {i + 1}: ");
                foreach (var node in allPaths[i])
                {
                    Console.Write(node + " ");
                }
                Console.WriteLine($"- Total Time: {CalculateTotalTime(nodes, allPaths[i])}");
            }

            // Find and display the critical path(s)
            Console.WriteLine("\nCritical Path(s):");
            var criticalPaths = findCriticalPaths(nodes, allPaths);
            foreach (var path in criticalPaths)
            {
                Console.Write("Critical Path: ");
                foreach (var node in path)
                {
                    Console.Write(node + " ");
                }
                Console.WriteLine($"- Total Time: {CalculateTotalTime(nodes, path)}");
            }


            // Calculate and display EST, LST, and slack time for each task
            calculateESTLST(allPaths ,nodes, totalNodes, criticalPaths);

        }

        static void DFS(int[][][] nodes, int currentNode, int targetNode, List<int> currentPath, List<List<int>> allPaths)
        {
            currentPath.Add(currentNode);

            // If the target node is reached, add the current path to the list of all paths
            if (currentNode == targetNode)
            {
                allPaths.Add(new List<int>(currentPath));
            }
            else
            {
                // Continue DFS for each adjacent node
                if (nodes[currentNode] != null)
                {
                    foreach (var task in nodes[currentNode])
                    {
                        DFS(nodes, task[2], targetNode, currentPath, allPaths);
                    }
                }
            }

            // Backtrack
            currentPath.RemoveAt(currentPath.Count - 1);
        }

        static List<List<int>> findCriticalPaths(int[][][] nodes, List<List<int>> allPaths)
        {
            List<List<int>> criticalPaths = new List<List<int>>();
            int maxTime = 0;

            foreach (var path in allPaths)
            {
                int pathTime = CalculateTotalTime(nodes, path);
                if (pathTime > maxTime)
                {
                    maxTime = pathTime;
                    criticalPaths.Clear();
                    criticalPaths.Add(path);
                }
                else if (pathTime == maxTime)
                {
                    criticalPaths.Add(path);
                }
            }

            return criticalPaths;
        }

        static int CalculateTotalTime(int[][][] nodes, List<int> path)
        {
            int totalTime = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                int currentNode = path[i];
                int nextNode = path[i + 1];

                // Find the task that connects the current node to the next node
                foreach (var task in nodes[currentNode])
                {
                    if (task[2] == nextNode)
                    {
                        totalTime += task[0];
                        break;
                    }
                }
            }

            return totalTime;
        }


        static void calculateESTLST(List<List<int>> allPaths, int[][][] nodes, int totalNodes, List<List<int>> criticalPaths)
        {
            List<List<int>> foundESTsLSTs = new List<List<int>>(); // start node, end node, time
            int critPathTime = CalculateTotalTime(nodes, criticalPaths[0]);

            foreach (var path in allPaths)
            {
                int time = 0;
                int currPathTime = CalculateTotalTime(nodes, path);
                int LST = critPathTime - currPathTime;

                for (int i = 0; i < path.Count - 1; i++)
                {
                    int currNode = path[i];
                    int nextNodeInPath = path[i + 1];

                    //check if the nodes exist in any critical path
                    bool existsInCriticalPath = false;
                    foreach (var critPath in criticalPaths)
                    {
                        for (int z = 0; z < critPath.Count - 1; z++)
                        {
                            if (critPath[z] == currNode && critPath[z + 1] == nextNodeInPath)
                            {
                                existsInCriticalPath = true;
                                break;
                            }
                        }
                    }

                    int[][] nodeSearch = nodes[currNode];

                    foreach (int[] next in nodeSearch)
                    {
                        int nextNodeInNodes = next[2];
                        int timeToAdd = next[0];
                        if (nextNodeInNodes == nextNodeInPath)
                        {
                            // Check if the EST entry already exists in foundESTs
                            bool entryExists = false;
                            foreach (List<int> estLst in foundESTsLSTs)
                            {

                                // If the entry already exists
                                if (estLst[0] == currNode && estLst[1] == nextNodeInPath)
                                {
                                    // Update existing entry if slower time found (slowest time = est)
                                    // next project can only start when all tasks leading into node are finished
                                    if (time > estLst[2])
                                    {
                                        estLst[2] = time;
                                    }
                                    
                                    // If the pair is found in any position within a critical path
                                    if (existsInCriticalPath)
                                    {
                                        estLst[3] = estLst[2];
                                        
                                    }

                                    // If the pair is not found in any critical path
                                    else if (!existsInCriticalPath && ((estLst[2] + LST) < estLst[3]))
                                    {
                                        estLst[3] = (estLst[2] + LST);
                                    }
                                                                 
                                    estLst[4] = estLst[3] - estLst[2];   
                                    
                                    entryExists = true;
                                    break;
                                }
                            }

                            // If the entry doesn't exist, add it to foundESTs
                            if (!entryExists)
                            {
                                List<int> currST = new List<int>();
                                currST.Add(currNode);
                                currST.Add(nextNodeInPath);
                                currST.Add(time);  //EST

                                // If the pair is found in any position within a critical path
                                if (existsInCriticalPath)
                                {
                                    currST.Add(time); //LST
                                }

                                // If the pair is not found in any critical path
                                else if (!existsInCriticalPath)
                                {
                                    currST.Add(time + LST); // LST
                                }
                                int slack = currST[3] - currST[2];
                                currST.Add(slack);  //slack
                                foundESTsLSTs.Add(currST);
                            }
                            time += timeToAdd;
                            break;
                        }
                    }
                }
            }

            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("| Activity   | EST      | LST      | Slack    |");
            Console.WriteLine("-----------------------------------------------");

            foreach (List<int> estLst in foundESTsLSTs)
            {
                string activityString = estLst[0] + " to " + estLst[1];

                Console.WriteLine($"| {activityString,-8}   | {estLst[2],-8} | {estLst[3],-8} | {estLst[4],-8} |");
            }

            Console.WriteLine("-----------------------------------------------");
        }

    }
}

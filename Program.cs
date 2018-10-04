//Robert Kowalchuk
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DTree
{
    class DTree
    {
        static void Main(string[] args)
        {
            //create my file reader
            XMLReader Reader = new XMLReader();
            //create a D tree from XML file
            Reader.CreateTreeFromFile();
            //Print the created tree
            Reader.PrintTree(Reader.ParentNode);
            //loop input for searching the tree
            Reader.BeginSearching(Reader.ParentNode);
        }
    }

    class Node
    {
        public string Behavior;
        public string Response;
        public int Depth;

        public Node Parent;
        public List<Node> Children;

        public Node(string Behavior, string Response, int Depth, Node Parent)
        {
            this.Behavior = Behavior;
            this.Response = Response;
            this.Depth = Depth;
            this.Parent = Parent;
            Children = new List<Node>();
        }

        public void AddChild(Node Child)
        {
            Children.Add(Child);
        }
    }

    class XMLReader
    {
        string ROOT = "<root>";
        string CloseNode = "</node>";
        string Behavior = "behavior=";
        string Response = "response=";
        //XML File neame in here
        string XML_File = "XML_TEST.xml";
        //For finding random response
        Random rand = new Random();
        
        //Create root of D tree
        public Node ParentNode = new Node("Root", "", 0, null);
        Node XML_Tree_Node;


        public void CreateTreeFromFile()
        {
            string temp;
            string[] upto;
            string behavior = "";
            string response = "";

            Node NewRoot = null;
            Node NewLeaf = null;

            XML_Tree_Node = ParentNode;

            try
            {
                
                using (StreamReader sr = new StreamReader(XML_File))
                {
                    //travers the entire xml file
                    while (!sr.EndOfStream)
                    {
                        temp = sr.ReadLine();
                        if (!temp.Contains(ROOT))
                        {
                            //look to see if beahvior and if it has a string to it
                            if (temp.Contains(Behavior))
                            {
                                upto = temp.Split('"');
                                Console.Write(upto[1] + "\t");
                                behavior = upto[1];
                                if (behavior != "")
                                {
                                    //create a parent node
                                    NewRoot = new Node(behavior, "", XML_Tree_Node.Depth + 1, XML_Tree_Node);
                                    XML_Tree_Node.AddChild(NewRoot);
                                    XML_Tree_Node = NewRoot;
                                    NewRoot = null;
                                }
                            }
                            //look to see if response and if it has a string to it
                            if (temp.Contains(Response))
                            {
                                upto = temp.Split('"');
                                Console.Write(upto[3] + "\t");
                                response = upto[3];
                                if (response != "")
                                {
                                    //create a child node
                                    NewLeaf = new Node("", response, XML_Tree_Node.Depth + 1, XML_Tree_Node);
                                    XML_Tree_Node.AddChild(NewLeaf);
                                    NewLeaf = null;
                                }
                            }
                        }
                        //go up the tree if this parent node is complete
                        if (temp.Contains(CloseNode))
                        {
                            XML_Tree_Node = XML_Tree_Node.Parent;
                        }
                        Console.WriteLine();
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception("File didn't read?", e);
            }
        }

        public void PrintTree(Node tempNode)
        {
            //traverse ebtire tree printing the right response or behavior as needed
            string tabString = "";
            if (tempNode == null)
            {
                return;
            }
            //Node Depth is used to get the tabs needed to format the printed tree
            for (int i = 0; i < tempNode.Depth; i++)
            {
                tabString += "\t";
            }
            //No response print behavior or no behavior preint response
            if (tempNode.Response == "")
            {
                Console.WriteLine(tabString + Behavior + " " + tempNode.Behavior);
            }
            else
            {
                Console.WriteLine(tabString + Response + " " + tempNode.Response);
            }
            foreach (Node n in tempNode.Children)
            {
                PrintTree(n);
            }

        }

        public void BeginSearching(Node tempNode)
        {
            string input = "";
            int turns = 1;
            //Loop waits for input and exits if 'quit' is typed
            //else it searches for the input among the trees behaviors
            while (true)
            {
                Console.Write("Event ('quit' to exit) : ");
                input = Console.ReadLine();
                if (input == "quit")
                {
                    break;
                }
                //start searching
                //Since I don't count the attempts at a random response I don't see 
                //the harm in the searches printing different responses since they 
                //both search for the same behavior
                BreadthFirstSearch(tempNode, input, ref turns);
                turns = 1;
                DepthFirstSearch(tempNode, input, ref turns);
                turns = 1;
            }
        }

        public void DepthFirstSearch(Node tempNode, string target, ref int turns)
        {
            //children first
            //traverse to the lowest child node first
            turns++;
            foreach (Node N in tempNode.Children)
            {
                DepthFirstSearch(N, target, ref turns);
            }
            
            //Now see if this node is the target behavior
            if(tempNode.Behavior == target)
            {
                Console.WriteLine(Response + " " + ReturnResponse(tempNode, ref turns) + "\n\t DepthFirstSearch in " + turns + " turns");
                return;
            }

            if (tempNode == null)
            {
                return;
            }

        }

        public void BreadthFirstSearch(Node tempNode, string target, ref int turns)
        {
            //Node by Node
            //check the current node before moving on to the other nodes
            if(tempNode == null)
            {
                return;
            }

            if (tempNode.Behavior == target)
            {
                Console.WriteLine(Response + " " + ReturnResponse(tempNode, ref turns) + "\n\t BreadthFirstSearch in " + turns + " turns");
                return;
            }

            foreach (Node n in tempNode.Children)
            {
                turns++;
                BreadthFirstSearch(n, target, ref turns);
            }
        }

        string ReturnResponse(Node tempNode, ref int turns)
        {
            string re = "";
            //Pick a random child from the Parents Children
            re = tempNode.Children[rand.Next(tempNode.Children.Count)].Response;

            //If random child is beahvior then pick a random child from it and retry
            if (re == "")
            {
                re = ReturnResponse(tempNode.Children[rand.Next(tempNode.Children.Count)], ref turns);
            }

            //I wasn't sure about counting the turns it took to find a random response because 
            //when searching it's only for that target behavior and not these response
            //turns++;
            return re;
        }
    }
}

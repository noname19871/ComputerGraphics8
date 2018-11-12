using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;


namespace ComputerGraphics8
{
    public class Primitive
    {
        public Vector[] Vertices { get; set; }
        public int[][] Verges { get; set; }

        public Vector Center
        {
            get
            {
                Vector center = new Vector();
                foreach (var v in Vertices)
                {
                    center.X += v.X;
                    center.Y += v.Y;
                    center.Z += v.Z;
                }
                center.X /= Vertices.Length;
                center.Y /= Vertices.Length;
                center.Z /= Vertices.Length;
                return center;
            }
        }

        public Primitive(Tuple<Vector[], int[][]> data)
            : this(data.Item1, data.Item2)
        {
        }

        public Primitive(Vector[] vertices, int[][] verges)
        {
            Vertices = vertices;
            Verges = verges;
        }

        public Primitive(string path)
        {
            var vertices = new List<Vector>();
            var verges = new List<List<int>>();
            var info = File.ReadAllLines(path);
            int index = 0;
            while (info[index].Equals("") || !info[index][0].Equals('v'))
                index++;
            while (info[index].Equals("") || info[index][0].Equals('v'))
            {
                var infoPoint = info[index].Split(' ');
                double x = double.Parse(infoPoint[1]);
                double y = double.Parse(infoPoint[2]);
                double z = double.Parse(infoPoint[3]);
                vertices.Add(new Vector(x, y, z));
                index++;
            }
            while (info[index].Equals("") || !info[index][0].Equals('f'))
                index++;
            int indexPointSeq = 0;
            while (info[index].Equals("") || info[index][0].Equals('f'))
            {
                var infoPointSeq = info[index].Split(' ');
                var listPoints = new List<int>();
                for (int i = 1; i < infoPointSeq.Length; ++i)
                {
                    int elem;
                    if (int.TryParse(infoPointSeq[i], out elem))
                        listPoints.Add(elem - 1);
                }
                verges.Add(listPoints);
                index++;
                indexPointSeq++;
            }
            Vertices = vertices.ToArray();
            Verges = verges.Select(x => x.ToArray()).ToArray();
        }

        public void Apply(Matrix transformation)
        {
            for (int i = 0; i < Vertices.Length; ++i)
                Vertices[i] *= transformation;
        }

        public virtual void Draw(Graphics3D graphics)
        {

            Random r = new Random(256);

            foreach (var verge in Verges)
            {                
                int k = r.Next(0, 256);
                int k2 = r.Next(0, 256);
                int k3 = r.Next(0, 256);

                for (int i = 1; i < verge.Length - 1; ++i)
                {

                    var a = new Vertex(Vertices[verge[0]], new Vector(), Color.FromArgb(k2, k, k3));
                    var b = new Vertex(Vertices[verge[i]], new Vector(), Color.FromArgb(k2, k, k3));
                    var c = new Vertex(Vertices[verge[i + 1]], new Vector(), Color.FromArgb(k2, k, k3));
                    graphics.DrawTriangle(a, b, c);
                }
            }
        }

        public void Save(string path)
        {
            string info = "# File Created: " + DateTime.Now.ToString() + "\r\n";
            foreach (var v in Vertices)
                info += "v " + v.X + " " + v.Y + " " + v.Z + "\r\n";
            info += "# " + Vertices.Length + " vertices\r\n";
            foreach (var verge in Verges)
            {
                info += "f ";
                for (int i = 0; i < verge.Length; ++i)
                    info += (verge[i] + 1) + " ";
                info += "\r\n";
            }
            info += "# " + Verges.Length + " polygons\r\n";
            File.WriteAllText(path, info);
        }
    }
}

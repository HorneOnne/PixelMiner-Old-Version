using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PixelMiner.WorldBuilding
{
    public static class LogUtils
    {
        public static void WriteMeshToFile(Mesh mesh, string filename)
        {
            string directoryPath = @"C:\Users\anhla\Desktop\PixelMinerLog\";
            string filePath = Path.Combine(directoryPath, filename);

            // Check if the directory exists; if not, create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write vertices
                Color32[] colors = mesh.colors32; // Assuming colors are assigned to the mesh
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    Vector3 vertex = mesh.vertices[i];
                    Color32 color = (i < colors.Length) ? colors[i] : new Color32(255, 255, 255, 255);

                    writer.WriteLine($"Vertex: {vertex.x}, {vertex.y}, {vertex.z}, Color: {color.r}, {color.g}, {color.b}, {color.a}");
                }

                // Write normals
                writer.WriteLine("\nNormals:");
                foreach (Vector3 normal in mesh.normals)
                {
                    writer.WriteLine($"{normal.x}, {normal.y}, {normal.z}");
                }

                // Write UV coordinates
                writer.WriteLine("\nUVs:");
                List<Vector3> uvs = new List<Vector3>();
                mesh.GetUVs(0, uvs);
                foreach (Vector3 uv in uvs)
                {
                    writer.WriteLine($"{uv.x}, {uv.y}, {uv.z}");
                }

               
                List<Vector2> uv2s = new List<Vector2>();
                mesh.GetUVs(1, uv2s);
                if(uv2s.Count > 0)
                {
                    writer.WriteLine("\nUV2s:");
                    foreach (Vector3 uv in uv2s)
                    {
                        writer.WriteLine($"{uv.x}, {uv.y}, {uv.z}");
                    }
                }


                List<Vector2> uv3s = new List<Vector2>();
                mesh.GetUVs(2, uv3s);
                if (uv3s.Count > 0)
                {
                    writer.WriteLine("\nUV3s:");
                    foreach (Vector3 uv in uv3s)
                    {
                        writer.WriteLine($"{uv.x}, {uv.y}, {uv.z}");
                    }
                }



                // Write triangles
                writer.WriteLine("\nTriangles:");
                for (int i = 0; i < mesh.triangles.Length; i += 3)
                {
                    int index1 = mesh.triangles[i];
                    int index2 = mesh.triangles[i + 1];
                    int index3 = mesh.triangles[i + 2];

                    writer.WriteLine($"{index1}, {index2}, {index3}");
                }

                OpenFileWithDefaultApplication(filePath);
                Debug.Log($"Mesh data written to file: {filePath}");
            }
        }


        public static void Log(List<Vector3[]> list, string filename)
        {
            string directoryPath = @"C:\Users\anhla\Desktop\PixelMinerLog\";
            string filePath = Path.Combine(directoryPath, filename);

            // Check if the directory exists; if not, create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write List
                writer.WriteLine("List:");

                foreach (var quad in list)
                {
                    // Write each Vector3 array in the list
                    writer.WriteLine("Quad:");
                    foreach (var vertex in quad)
                    {
                        writer.WriteLine($"    {vertex.x}, {vertex.y}, {vertex.z}");
                    }
                }

                Debug.Log($"Mesh data written to file: {filePath}");
            }
        }
        public static void Log(bool[,] list, string filename)
        {
            string directoryPath = @"C:\Users\anhla\Desktop\PixelMinerLog\";
            string filePath = Path.Combine(directoryPath, filename);

            // Check if the directory exists; if not, create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                // Write List
                writer.WriteLine();
                for (int i = 0; i < list.GetLength(0); i++)
                {
                    for (int j = 0; j < list.GetLength(1); j++)
                    {
                        writer.Write(list[i, j] ? "1 " : "0 ");
                    }
                    writer.WriteLine(); // Move to the next row
                }


               
                Debug.Log($"Mesh data written to file: {filePath}");
            }
            OpenFileWithDefaultApplication(filePath);
        }
        public static void Log(float[] list, string filename)
        {
            string directoryPath = @"C:\Users\anhla\Desktop\PixelMinerLog\";
            string filePath = Path.Combine(directoryPath, filename);

            // Check if the directory exists; if not, create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write List
                writer.WriteLine();
                for (int i = 0; i < list.Length; i++)
                {
                    writer.WriteLine(list[i]);
                }



                Debug.Log($"Data written to file: {filePath}");
            }
            OpenFileWithDefaultApplication(filePath);
        }

        private static void OpenFileWithDefaultApplication(string filePath)
        {
            try
            {
                System.Diagnostics.Process.Start(filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error opening file: {e.Message}");
            }
        }
    }
}

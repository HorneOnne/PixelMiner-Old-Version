using UnityEngine;
using System.IO;
using System.Collections.Generic;

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
                writer.WriteLine("Vertices:");
                foreach (Vector3 vertex in mesh.vertices)
                {
                    writer.WriteLine($"{vertex.x}, {vertex.y}, {vertex.z}");
                }

                // Write normals
                writer.WriteLine("\nNormals:");
                foreach (Vector3 normal in mesh.normals)
                {
                    writer.WriteLine($"{normal.x}, {normal.y}, {normal.z}");
                }

                // Write UV coordinates
                writer.WriteLine("\nUVs:");
                foreach (Vector2 uv in mesh.uv)
                {
                    writer.WriteLine($"{uv.x}, {uv.y}");
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

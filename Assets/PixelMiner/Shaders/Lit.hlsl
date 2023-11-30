// This file contains the vertex and fragment functions for the forward lit pass
// This is the shader pass that computers visible colors for a material
// by reading material, light, shadow, etc. data


// Pull in URP library functions and our own common functions
#include "Library\PackageCache\com.unity.render-pipelines.universal@14.0.9\ShaderLibrary\Lighting.hlsl"

// This attribute struct receives data about the mesh we're currently rendering
// Data is automatically placed in fileds according to their semantic
struct Attributes
{
    float3 position : POSITION; // Position in object space
};

void Vertex(Attributes input)
{
    VertexPositionInputs posInputs = GetVertexPositionInput(input.position);
}
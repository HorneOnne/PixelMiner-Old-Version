Shader "PixelMiner/LitBlock"
{
    // Subshader allow for different behaviour options for different pipelines and platforms
    SubShader
    {
        // These tags are shared by all passes in this sub shader
        Tags { "RenderPipeline" = "UniversalPipeline" } 

        // Shaders can have several passes which are used to render different data about the Material
        // Each pass has it's own vertex and fragment function and shader variant keywords'

        Pass
        {
            Name "ForwardLit"   // For debugging
            Tags {"LightMode" = "UniversalForward"}  // Pass specific tags.
            // "UniversalForward" tells Unity this  is the main lighting pass of this shader

            HLSLPROGRAM


            ENDHLSL
        }

    }
}

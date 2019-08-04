// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Glitch"
{
    Properties
    {
		_Noise_Tiling("Noise_Tiling", Float) = 0.07
		[HDR]_BaseColor("BaseColor", Color) = (1,1,1,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.7
		_Level("Level", Float) = -0.85
		_Mask_Smoothness("Mask_Smoothness", Range( 0 , 4)) = 0.35
		_Metallic("Metallic", Float) = 0
		_Glow("Glow", Float) = 0
		[HDR]_GlowColor("GlowColor", Color) = (0,0,0,0)
    }


    SubShader
    {
		
        Tags { "RenderPipeline"="LightweightPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 3.0
		ENDHLSL
		
        Pass
        {
			
        	Tags { "LightMode"="LightweightForward" }

        	Name "Base"
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
            
        	HLSLPROGRAM
            #define _RECEIVE_SHADOWS_OFF 1

            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            

        	// -------------------------------------
            // Lightweight Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            
        	// -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex vert
        	#pragma fragment frag

        	#define ASE_SRP_VERSION 50702
        	#define _AlphaClip 1


        	#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
        	#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
        	#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"

			float4 _BaseColor;
			float4 _GlowColor;
			float _Glow;
			float _Noise_Tiling;
			float _Metallic;
			float _Smoothness;
			float _Level;
			float _Mask_Smoothness;

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;
                float4 ase_tangent : TANGENT;
                float4 texcoord1 : TEXCOORD1;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        	struct GraphVertexOutput
            {
                float4 clipPos                : SV_POSITION;
                float4 lightmapUVOrVertexSH	  : TEXCOORD0;
        		half4 fogFactorAndVertexLight : TEXCOORD1; // x: fogFactor, yzw: vertex light
            	float4 shadowCoord            : TEXCOORD2;
				float4 tSpace0					: TEXCOORD3;
				float4 tSpace1					: TEXCOORD4;
				float4 tSpace2					: TEXCOORD5;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            	UNITY_VERTEX_OUTPUT_STEREO
            };

			
            GraphVertexOutput vert (GraphVertexInput v  )
        	{
        		GraphVertexOutput o = (GraphVertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
            	UNITY_TRANSFER_INSTANCE_ID(v, o);
        		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				float3 vertexValue =  float3( 0, 0, 0 ) ;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal =  v.ase_normal ;

        		// Vertex shader outputs defined by graph
                float3 lwWNormal = TransformObjectToWorldNormal(v.ase_normal);
				float3 lwWorldPos = TransformObjectToWorld(v.vertex.xyz);
				float3 lwWTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				float3 lwWBinormal = normalize(cross(lwWNormal, lwWTangent) * v.ase_tangent.w);
				o.tSpace0 = float4(lwWTangent.x, lwWBinormal.x, lwWNormal.x, lwWorldPos.x);
				o.tSpace1 = float4(lwWTangent.y, lwWBinormal.y, lwWNormal.y, lwWorldPos.y);
				o.tSpace2 = float4(lwWTangent.z, lwWBinormal.z, lwWNormal.z, lwWorldPos.z);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                
         		// We either sample GI from lightmap or SH.
        	    // Lightmap UV and vertex SH coefficients use the same interpolator ("float2 lightmapUV" for lightmap or "half3 vertexSH" for SH)
                // see DECLARE_LIGHTMAP_OR_SH macro.
        	    // The following funcions initialize the correct variable with correct data
        	    OUTPUT_LIGHTMAP_UV(v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy);
        	    OUTPUT_SH(lwWNormal, o.lightmapUVOrVertexSH.xyz);

        	    half3 vertexLight = VertexLighting(vertexInput.positionWS, lwWNormal);
        	    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
        	    o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
        	    o.clipPos = vertexInput.positionCS;

        	#ifdef _MAIN_LIGHT_SHADOWS
        		o.shadowCoord = GetShadowCoord(vertexInput);
        	#endif
        		return o;
        	}

        	half4 frag (GraphVertexOutput IN  ) : SV_Target
            {
            	UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

        		float3 WorldSpaceNormal = normalize(float3(IN.tSpace0.z,IN.tSpace1.z,IN.tSpace2.z));
				float3 WorldSpaceTangent = float3(IN.tSpace0.x,IN.tSpace1.x,IN.tSpace2.x);
				float3 WorldSpaceBiTangent = float3(IN.tSpace0.y,IN.tSpace1.y,IN.tSpace2.y);
				float3 WorldSpacePosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldSpaceViewDirection = SafeNormalize( _WorldSpaceCameraPos.xyz  - WorldSpacePosition );
    
				float4 Albedo15 = _BaseColor;
				
				float3 temp_output_11_0_g54 = ceil( ( WorldSpacePosition / _Noise_Tiling ) );
				float dotResult2_g54 = dot( temp_output_11_0_g54 , float3(15400,28700,38700) );
				float dotResult5_g54 = dot( temp_output_11_0_g54 , float3(35300,51700,79500) );
				float dotResult6_g54 = dot( temp_output_11_0_g54 , float3(49700,20800,73000) );
				float3 appendResult9_g54 = (float3(dotResult2_g54 , dotResult5_g54 , dotResult6_g54));
				float3 temp_output_5_0 = ( ( ( ( frac( ( sin( appendResult9_g54 ) * ( 42940.0 + _Time.y ) ) ) * float3( 2,2,2 ) ) - float3( 1,1,1 ) ) + 1.0 ) * 0.5 );
				float TiledNoise_B11 = (temp_output_5_0).y;
				float clampResult110 = clamp( ceil( ( _Glow - TiledNoise_B11 ) ) , 0.0 , 1.0 );
				float4 lerpResult114 = lerp( _GlowColor , float4( 0,0,0,0 ) , clampResult110);
				float4 Emission115 = lerpResult114;
				
				float Metallic104 = _Metallic;
				
				float Smoothness17 = _Smoothness;
				
				float3 objToWorld23 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float temp_output_30_0 = ( ((( WorldSpacePosition - ( _Level + objToWorld23 ) )).xyz).y * _Mask_Smoothness );
				float clampResult32 = clamp( ( 1.0 - temp_output_30_0 ) , 0.0 , 1.0 );
				float Mask_A_Output_A33 = clampResult32;
				float TiledNoise_A10 = (temp_output_5_0).x;
				float clampResult42 = clamp( ceil( ( Mask_A_Output_A33 - TiledNoise_A10 ) ) , 0.0 , 1.0 );
				float Opacity43 = clampResult42;
				
				
		        float3 Albedo = Albedo15.rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = Emission115.rgb;
				float3 Specular = float3(0.5, 0.5, 0.5);
				float Metallic = Metallic104;
				float Smoothness = Smoothness17;
				float Occlusion = 1;
				float Alpha = floor( Opacity43 );
				float AlphaClipThreshold = 1.0;

        		InputData inputData;
        		inputData.positionWS = WorldSpacePosition;

        #ifdef _NORMALMAP
        	    inputData.normalWS = normalize(TransformTangentToWorld(Normal, half3x3(WorldSpaceTangent, WorldSpaceBiTangent, WorldSpaceNormal)));
        #else
            #if !SHADER_HINT_NICE_QUALITY
                inputData.normalWS = WorldSpaceNormal;
            #else
        	    inputData.normalWS = normalize(WorldSpaceNormal);
            #endif
        #endif

        #if !SHADER_HINT_NICE_QUALITY
        	    // viewDirection should be normalized here, but we avoid doing it as it's close enough and we save some ALU.
        	    inputData.viewDirectionWS = WorldSpaceViewDirection;
        #else
        	    inputData.viewDirectionWS = normalize(WorldSpaceViewDirection);
        #endif

        	    inputData.shadowCoord = IN.shadowCoord;

        	    inputData.fogCoord = IN.fogFactorAndVertexLight.x;
        	    inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
        	    inputData.bakedGI = SAMPLE_GI(IN.lightmapUVOrVertexSH.xy, IN.lightmapUVOrVertexSH.xyz, inputData.normalWS);

        		half4 color = LightweightFragmentPBR(
        			inputData, 
        			Albedo, 
        			Metallic, 
        			Specular, 
        			Smoothness, 
        			Occlusion, 
        			Emission, 
        			Alpha);

			#ifdef TERRAIN_SPLAT_ADDPASS
				color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
			#else
				color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
			#endif

        #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif

		#if ASE_LW_FINAL_COLOR_ALPHA_MULTIPLY
				color.rgb *= color.a;
		#endif
        		return color;
            }

        	ENDHLSL
        }

		
        Pass
        {
			
        	Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }

            ZWrite On
			ColorMask 0

            HLSLPROGRAM
            #define _RECEIVE_SHADOWS_OFF 1

            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag

            #define ASE_SRP_VERSION 50702
            #define _AlphaClip 1


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			float _Level;
			float _Mask_Smoothness;
			float _Noise_Tiling;

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        	struct VertexOutput
        	{
        	    float4 clipPos      : SV_POSITION;
                float4 ase_texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
        	};

			           

            VertexOutput vert(GraphVertexInput v  )
            {
                VertexOutput o = (VertexOutput)0;
        	    UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord.xyz = ase_worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				float3 vertexValue =  float3(0,0,0) ;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal =  v.ase_normal ;

        	    o.clipPos = TransformObjectToHClip(v.vertex.xyz);
        	    return o;
            }

            half4 frag(VertexOutput IN  ) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);

				float3 ase_worldPos = IN.ase_texcoord.xyz;
				float3 objToWorld23 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float temp_output_30_0 = ( ((( ase_worldPos - ( _Level + objToWorld23 ) )).xyz).y * _Mask_Smoothness );
				float clampResult32 = clamp( ( 1.0 - temp_output_30_0 ) , 0.0 , 1.0 );
				float Mask_A_Output_A33 = clampResult32;
				float3 temp_output_11_0_g54 = ceil( ( ase_worldPos / _Noise_Tiling ) );
				float dotResult2_g54 = dot( temp_output_11_0_g54 , float3(15400,28700,38700) );
				float dotResult5_g54 = dot( temp_output_11_0_g54 , float3(35300,51700,79500) );
				float dotResult6_g54 = dot( temp_output_11_0_g54 , float3(49700,20800,73000) );
				float3 appendResult9_g54 = (float3(dotResult2_g54 , dotResult5_g54 , dotResult6_g54));
				float3 temp_output_5_0 = ( ( ( ( frac( ( sin( appendResult9_g54 ) * ( 42940.0 + _Time.y ) ) ) * float3( 2,2,2 ) ) - float3( 1,1,1 ) ) + 1.0 ) * 0.5 );
				float TiledNoise_A10 = (temp_output_5_0).x;
				float clampResult42 = clamp( ceil( ( Mask_A_Output_A33 - TiledNoise_A10 ) ) , 0.0 , 1.0 );
				float Opacity43 = clampResult42;
				

				float Alpha = floor( Opacity43 );
				float AlphaClipThreshold = 1.0;

         #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif
                return 0;
            }
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
		
        Pass
        {
			
        	Name "Meta"
            Tags { "LightMode"="Meta" }

            Cull Off

            HLSLPROGRAM
            #define _RECEIVE_SHADOWS_OFF 1

            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag

            #define ASE_SRP_VERSION 50702
            #define _AlphaClip 1


			uniform float4 _MainTex_ST;
			
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/MetaInput.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			float4 _BaseColor;
			float4 _GlowColor;
			float _Glow;
			float _Noise_Tiling;
			float _Level;
			float _Mask_Smoothness;

            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature EDITOR_VISUALIZATION


            struct GraphVertexInput
            {
                float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        	struct VertexOutput
        	{
        	    float4 clipPos      : SV_POSITION;
                float4 ase_texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
        	};

			
            VertexOutput vert(GraphVertexInput v  )
            {
                VertexOutput o = (VertexOutput)0;
        	    UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				o.ase_texcoord.xyz = ase_worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;

				float3 vertexValue =  float3(0,0,0) ;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal =  v.ase_normal ;
#if !defined( ASE_SRP_VERSION ) || ASE_SRP_VERSION  > 51300				
                o.clipPos = MetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST);
#else
				o.clipPos = MetaVertexPosition (v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST);
#endif
        	    return o;
            }

            half4 frag(VertexOutput IN  ) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);

           		float4 Albedo15 = _BaseColor;
           		
           		float3 ase_worldPos = IN.ase_texcoord.xyz;
           		float3 temp_output_11_0_g54 = ceil( ( ase_worldPos / _Noise_Tiling ) );
           		float dotResult2_g54 = dot( temp_output_11_0_g54 , float3(15400,28700,38700) );
           		float dotResult5_g54 = dot( temp_output_11_0_g54 , float3(35300,51700,79500) );
           		float dotResult6_g54 = dot( temp_output_11_0_g54 , float3(49700,20800,73000) );
           		float3 appendResult9_g54 = (float3(dotResult2_g54 , dotResult5_g54 , dotResult6_g54));
           		float3 temp_output_5_0 = ( ( ( ( frac( ( sin( appendResult9_g54 ) * ( 42940.0 + _Time.y ) ) ) * float3( 2,2,2 ) ) - float3( 1,1,1 ) ) + 1.0 ) * 0.5 );
           		float TiledNoise_B11 = (temp_output_5_0).y;
           		float clampResult110 = clamp( ceil( ( _Glow - TiledNoise_B11 ) ) , 0.0 , 1.0 );
           		float4 lerpResult114 = lerp( _GlowColor , float4( 0,0,0,0 ) , clampResult110);
           		float4 Emission115 = lerpResult114;
           		
           		float3 objToWorld23 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
           		float temp_output_30_0 = ( ((( ase_worldPos - ( _Level + objToWorld23 ) )).xyz).y * _Mask_Smoothness );
           		float clampResult32 = clamp( ( 1.0 - temp_output_30_0 ) , 0.0 , 1.0 );
           		float Mask_A_Output_A33 = clampResult32;
           		float TiledNoise_A10 = (temp_output_5_0).x;
           		float clampResult42 = clamp( ceil( ( Mask_A_Output_A33 - TiledNoise_A10 ) ) , 0.0 , 1.0 );
           		float Opacity43 = clampResult42;
           		
				
		        float3 Albedo = Albedo15.rgb;
				float3 Emission = Emission115.rgb;
				float Alpha = floor( Opacity43 );
				float AlphaClipThreshold = 1.0;

         #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif

                MetaInput metaInput = (MetaInput)0;
                metaInput.Albedo = Albedo;
                metaInput.Emission = Emission;
                
                return MetaFragment(metaInput);
            }
            ENDHLSL
        }
		
    }
    Fallback "Hidden/InternalErrorShader"
	CustomEditor "ASEMaterialInspector"
	
}
/*ASEBEGIN
Version=16900
47;24;1210;776;360.5591;1178.302;2.215841;True;False
Node;AmplifyShaderEditor.TransformPositionNode;23;928.6512,-637.6843;Float;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;25;556.1978,-702.9784;Float;False;Property;_Level;Level;3;0;Create;True;0;0;False;0;-0.85;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;1279.138,-704.2815;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;22;979.3962,-910.9999;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;1;949.9247,-348.654;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;26;1457.433,-825.7286;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;3;1030.195,-44.18703;Float;False;Property;_Noise_Tiling;Noise_Tiling;0;0;Create;True;0;0;False;0;0.07;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;2;1259.928,-290.5285;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;27;1706.003,-825.6013;Float;False;FLOAT3;0;1;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;28;1927.717,-817.9767;Float;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;7;1586.039,54.64319;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;4;1524.663,-288.4358;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;29;1839.861,-670.6897;Float;False;Property;_Mask_Smoothness;Mask_Smoothness;4;0;Create;True;0;0;False;0;0.35;1.55;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;6;1767.021,-283.7144;Float;True;Noise3D;-1;;54;d764e00d3db3c4fbf9040cfc2b5cc731;0;2;11;FLOAT3;0,0,0;False;13;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;2191.283,-794.7208;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;5;2113.246,-285.2883;Float;True;ConstantBiasScale;-1;;55;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT3;0,0,0;False;1;FLOAT;1;False;2;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;31;2379.913,-823.1445;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;8;2425.76,-290.0092;Float;False;True;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;32;2571.128,-846.4005;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;9;2426.021,-185.087;Float;False;False;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;2674.803,-182.4588;Float;False;TiledNoise_B;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;2780.43,-864.4882;Float;True;Mask_A_Output_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;2674.413,-291.4531;Float;False;TiledNoise_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;1165.51,906.3787;Float;False;11;TiledNoise_B;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;712.2343,420.4361;Float;False;10;TiledNoise_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;1203.19,797.9143;Float;False;Property;_Glow;Glow;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;698.8296,272.9856;Float;False;33;Mask_A_Output_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;38;1100.967,278.3477;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;109;1422.871,865.5126;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;161;1496.815,322.1904;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;160;1605.908,866.4734;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;166;1704,641.1071;Float;False;Property;_GlowColor;GlowColor;7;1;[HDR];Create;True;0;0;False;0;0,0,0,0;11.98431,11.98431,11.98431,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;110;1732.796,886.4132;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;42;1828.441,292.1085;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;114;1981.222,714.6432;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;12;803.8831,-1194.917;Float;False;Property;_BaseColor;BaseColor;1;1;[HDR];Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;2155.254,288.1552;Float;True;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;2801.429,342.9726;Float;True;43;Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;115;2205.275,699.1604;Float;True;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;1176.863,-1199.638;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;104;2500.061,-1270.627;Float;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;2200.803,-1267.426;Float;False;Property;_Metallic;Metallic;5;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;35;2781.602,-632.5533;Float;True;Mask_A_Output_B;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;151;3075.109,296.4444;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;2926.478,-198.0474;Float;True;115;Emission;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;34;2449.681,-629.3461;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;3099.659,133.3469;Float;False;17;Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;1453.844,-1190.196;Float;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;False;0;0.7;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;3021.781,599.6631;Float;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;3006.862,45.34795;Float;False;104;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;1829.972,-1187.048;Float;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;3060.481,-244.0782;Float;False;15;Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;146;3429.815,-82.0116;Float;False;False;2;Float;ASEMaterialInspector;0;1;Hidden/Templates/LightWeightSRPPBR;1976390536c6c564abb90fe41f6ee334;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;145;3375.699,-75.41209;Float;False;True;2;Float;ASEMaterialInspector;0;2;Glitch;1976390536c6c564abb90fe41f6ee334;True;Base;0;0;Base;11;False;False;False;True;2;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;True;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=LightweightForward;False;0;Hidden/InternalErrorShader;0;0;Standard;2;Vertex Position,InvertActionOnDeselection;1;Receive Shadows;0;1;_FinalColorxAlpha;0;4;True;False;True;True;False;11;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;9;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT3;0,0,0;False;10;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;147;3429.815,-82.0116;Float;False;False;2;Float;ASEMaterialInspector;0;1;Hidden/Templates/LightWeightSRPPBR;1976390536c6c564abb90fe41f6ee334;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;148;3429.815,-82.0116;Float;False;False;2;Float;ASEMaterialInspector;0;1;Hidden/Templates/LightWeightSRPPBR;1976390536c6c564abb90fe41f6ee334;True;Meta;0;3;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;0
WireConnection;24;0;25;0
WireConnection;24;1;23;0
WireConnection;26;0;22;0
WireConnection;26;1;24;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;27;0;26;0
WireConnection;28;0;27;0
WireConnection;4;0;2;0
WireConnection;6;11;4;0
WireConnection;6;13;7;0
WireConnection;30;0;28;0
WireConnection;30;1;29;0
WireConnection;5;3;6;0
WireConnection;31;0;30;0
WireConnection;8;0;5;0
WireConnection;32;0;31;0
WireConnection;9;0;5;0
WireConnection;11;0;9;0
WireConnection;33;0;32;0
WireConnection;10;0;8;0
WireConnection;38;0;37;0
WireConnection;38;1;36;0
WireConnection;109;0;162;0
WireConnection;109;1;108;0
WireConnection;161;0;38;0
WireConnection;160;0;109;0
WireConnection;110;0;160;0
WireConnection;42;0;161;0
WireConnection;114;0;166;0
WireConnection;114;2;110;0
WireConnection;43;0;42;0
WireConnection;115;0;114;0
WireConnection;15;0;12;0
WireConnection;104;0;103;0
WireConnection;35;0;34;0
WireConnection;151;0;95;0
WireConnection;34;0;30;0
WireConnection;17;0;16;0
WireConnection;145;0;91;0
WireConnection;145;2;92;0
WireConnection;145;3;105;0
WireConnection;145;4;93;0
WireConnection;145;6;151;0
WireConnection;145;7;152;0
ASEEND*/
//CHKSM=6C253555FB6ECAF1CC4EB7CF5896A16A5B6D6DC0
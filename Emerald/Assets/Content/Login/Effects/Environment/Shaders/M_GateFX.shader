// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_GateFX"
{
	Properties
	{
		_T_FX_DarkHit01_Mask01("T_FX_DarkHit01_Mask01", 2D) = "white" {}
		_T_purify_01("T_purify_01", 2D) = "white" {}
		_T_FX_LuoS_cirray02a("T_FX_LuoS_cirray02a", 2D) = "white" {}
		_T_FX_LuoS_cirray02("T_FX_LuoS_cirray02", 2D) = "white" {}
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_T_FX_LuoS_cirray02b("T_FX_LuoS_cirray02b", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _T_FX_LuoS_cirray02a;
		uniform sampler2D _T_FX_LuoS_cirray02b;
		uniform sampler2D _TextureSample0;
		uniform sampler2D _T_FX_LuoS_cirray02;
		uniform sampler2D _T_purify_01;
		uniform float4 _T_purify_01_ST;
		uniform sampler2D _T_FX_DarkHit01_Mask01;
		uniform float4 _T_FX_DarkHit01_Mask01_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord26 = i.uv_texcoord * float2( 1,4 );
			float2 panner23 = ( _Time.y * float2( 0.05,0.02 ) + uv_TexCoord26);
			float2 uv_TexCoord43 = i.uv_texcoord * float2( 1,2 );
			float2 panner39 = ( _Time.y * float2( -0.1,0.05 ) + uv_TexCoord43);
			float2 uv_TexCoord44 = i.uv_texcoord * float2( 0.7,1 );
			float2 panner40 = ( _Time.y * float2( 0.08,0.03 ) + uv_TexCoord44);
			float4 color62 = IsGammaSpace() ? float4(0.2735849,0.2088292,0.09678712,0) : float4(0.06083427,0.03592269,0.009531425,0);
			float4 temp_output_24_0 = ( float4( (( tex2D( _T_FX_LuoS_cirray02b, panner39 ) + tex2D( _TextureSample0, panner40 ) )).rg, 0.0 , 0.0 ) * color62 );
			float4 tex2DNode21 = tex2D( _T_FX_LuoS_cirray02a, ( float4( panner23, 0.0 , 0.0 ) + temp_output_24_0 ).rg );
			float4 color58 = IsGammaSpace() ? float4(0.9716981,0.9304467,0.9304467,0) : float4(0.9368213,0.8490117,0.8490117,0);
			float2 uv_TexCoord31 = i.uv_texcoord * float2( 1,6 );
			float2 panner29 = ( _Time.y * float2( -0.06,0.03 ) + uv_TexCoord31);
			float4 tex2DNode27 = tex2D( _T_FX_LuoS_cirray02, ( temp_output_24_0 + float4( panner29, 0.0 , 0.0 ) ).rg );
			float4 color59 = IsGammaSpace() ? float4(0.9433962,0.8944464,0.8944464,0) : float4(0.8760344,0.7764673,0.7764673,0);
			float4 color61 = IsGammaSpace() ? float4(0.4150943,0.4150943,0.4150943,0) : float4(0.1436938,0.1436938,0.1436938,0);
			float2 uv_T_purify_01 = i.uv_texcoord * _T_purify_01_ST.xy + _T_purify_01_ST.zw;
			float4 tex2DNode8 = tex2D( _T_purify_01, uv_T_purify_01 );
			float4 color60 = IsGammaSpace() ? float4(0.990566,0.9672036,0.9672036,0) : float4(0.9786729,0.9270089,0.9270089,0);
			float4 temp_output_11_0 = ( pow( ( ( ( pow( tex2DNode21 , color58 ) * color58 ) + tex2DNode21 ) + ( tex2DNode27 + ( pow( tex2DNode27 , color59 ) * color59 ) ) ) , color61 ) * ( tex2DNode8 + pow( tex2DNode8 , color60 ) ) );
			o.Emission = ( temp_output_11_0 * i.vertexColor ).rgb;
			float4 clampResult3 = clamp( ( i.vertexColor.a * temp_output_11_0 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float2 uv_T_FX_DarkHit01_Mask01 = i.uv_texcoord * _T_FX_DarkHit01_Mask01_ST.xy + _T_FX_DarkHit01_Mask01_ST.zw;
			o.Alpha = ( clampResult3 * tex2D( _T_FX_DarkHit01_Mask01, uv_T_FX_DarkHit01_Mask01 ) ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
97;132;1821;886;1015.385;634.0536;1.102791;True;True
Node;AmplifyShaderEditor.SimpleTimeNode;41;-4447.5,-352.5;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;42;-4487.5,-54.5;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;43;-4493.5,-498.5;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;44;-4530.5,-200.5;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.7,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;40;-4210.5,-175.5;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.08,0.03;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;39;-4199.5,-470.5;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,0.05;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;37;-3875.39,-203.7827;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;e7fc6ac1973be0f4dafdbe3d95af029f;e7fc6ac1973be0f4dafdbe3d95af029f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;38;-3874.521,-499.9349;Inherit;True;Property;_T_FX_LuoS_cirray02b;T_FX_LuoS_cirray02b;5;0;Create;True;0;0;False;0;c31b8f277ec18da4783caba68522f88f;c31b8f277ec18da4783caba68522f88f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;35;-3479.327,-342.2413;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-3182.5,-606.5;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,4;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;31;-3358.5,-50.5;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,6;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;25;-3150.5,-460.5;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;62;-3060.639,-202.8401;Inherit;False;Constant;_Color4;Color 4;6;0;Create;True;0;0;False;0;0.2735849,0.2088292,0.09678712,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;55;-3299.201,-347.8171;Inherit;True;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;30;-3326.5,141.5;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;29;-3066.5,8.5;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.06,0.03;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;23;-2902.5,-605.5;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.05,0.02;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-2843.175,-341.7612;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-2639.5,-229.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-2654.5,-604.5;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;58;-2039.699,-544.8533;Inherit;False;Constant;_Color0;Color 0;6;0;Create;True;0;0;False;0;0.9716981,0.9304467,0.9304467,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;-2452.5,-633.5;Inherit;True;Property;_T_FX_LuoS_cirray02a;T_FX_LuoS_cirray02a;2;0;Create;True;0;0;False;0;e7fc6ac1973be0f4dafdbe3d95af029f;e7fc6ac1973be0f4dafdbe3d95af029f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;59;-2302.083,48.91028;Inherit;False;Constant;_Color1;Color 1;6;0;Create;True;0;0;False;0;0.9433962,0.8944464,0.8944464,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;27;-2446.5,-258.5;Inherit;True;Property;_T_FX_LuoS_cirray02;T_FX_LuoS_cirray02;3;0;Create;True;0;0;False;0;6d1f10d38571eeb4b98dc0c0710db796;6d1f10d38571eeb4b98dc0c0710db796;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;46;-1992.5,-162.5;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;19;-1791.5,-832.5;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1540.5,-756.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1688.5,-162.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;8;-1262.479,-33.06534;Inherit;True;Property;_T_purify_01;T_purify_01;1;0;Create;True;0;0;False;0;881a46859c9d28d42b5ba4d7d26fa396;881a46859c9d28d42b5ba4d7d26fa396;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1263.5,-653.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-1384.601,-251.36;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;60;-1234.915,294.7797;Inherit;False;Constant;_Color2;Color 2;6;0;Create;True;0;0;False;0;0.990566,0.9672036,0.9672036,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;6;-895.5,77.5;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-984.5,-544.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;61;-1136.915,-260.2203;Inherit;False;Constant;_Color3;Color 3;6;0;Create;True;0;0;False;0;0.4150943,0.4150943,0.4150943,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;12;-780.5,-435.5;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-726.2869,-26.72675;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;9;-794.5,-290.5;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-450.5,-544.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-454.5,-122.5;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-498.5,212.5;Inherit;True;Property;_T_FX_DarkHit01_Mask01;T_FX_DarkHit01_Mask01;0;0;Create;True;0;0;False;0;15d02e97a7dc6a14495e8986ee99ff0b;15d02e97a7dc6a14495e8986ee99ff0b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;3;-179.2785,-122.9133;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1;29.5,-51.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-153.5,-313.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;173;231.7944,-303.8312;Float;False;True;2;ASEMaterialInspector;0;0;Standard;M_GateFX;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;44;0
WireConnection;40;1;42;0
WireConnection;39;0;43;0
WireConnection;39;1;41;0
WireConnection;37;1;40;0
WireConnection;38;1;39;0
WireConnection;35;0;38;0
WireConnection;35;1;37;0
WireConnection;55;0;35;0
WireConnection;29;0;31;0
WireConnection;29;1;30;0
WireConnection;23;0;26;0
WireConnection;23;1;25;0
WireConnection;24;0;55;0
WireConnection;24;1;62;0
WireConnection;28;0;24;0
WireConnection;28;1;29;0
WireConnection;22;0;23;0
WireConnection;22;1;24;0
WireConnection;21;1;22;0
WireConnection;27;1;28;0
WireConnection;46;0;27;0
WireConnection;46;1;59;0
WireConnection;19;0;21;0
WireConnection;19;1;58;0
WireConnection;17;0;19;0
WireConnection;17;1;58;0
WireConnection;47;0;46;0
WireConnection;47;1;59;0
WireConnection;16;0;17;0
WireConnection;16;1;21;0
WireConnection;48;0;27;0
WireConnection;48;1;47;0
WireConnection;6;0;8;0
WireConnection;6;1;60;0
WireConnection;14;0;16;0
WireConnection;14;1;48;0
WireConnection;12;0;14;0
WireConnection;12;1;61;0
WireConnection;5;0;8;0
WireConnection;5;1;6;0
WireConnection;11;0;12;0
WireConnection;11;1;5;0
WireConnection;4;0;9;4
WireConnection;4;1;11;0
WireConnection;3;0;4;0
WireConnection;1;0;3;0
WireConnection;1;1;2;0
WireConnection;10;0;11;0
WireConnection;10;1;9;0
WireConnection;173;2;10;0
WireConnection;173;9;1;0
ASEEND*/
//CHKSM=DB93138D36E460575BC9DC1769DA646D3BDAE124
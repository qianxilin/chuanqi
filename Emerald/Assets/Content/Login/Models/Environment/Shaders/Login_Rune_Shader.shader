// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/NewSurfaceShader"
{
	Properties
	{
		_EmissiveTexture_2("EmissiveTexture_2", 2D) = "white" {}
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_EmissiveTexture_1("EmissiveTexture_1", 2D) = "white" {}
		_EmissiveTextureTiling_1("EmissiveTextureTiling_1", Int) = 2
		_EmissiveTextureTiling_("EmissiveTextureTiling_", Int) = 2
		_Specular("Specular", 2D) = "white" {}
		_Emissive_Color("Emissive_Color", Color) = (0.4784314,0.6588235,1,1)
		_Emissive_Power("Emissive_Power", Int) = 1
		_Diffuse("Diffuse", 2D) = "white" {}
		_TexSpeed_1("TexSpeed_1", Vector) = (1,1,0,0)
		_TexSpeed_2("TexSpeed_2", Vector) = (1,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform sampler2D _Specular;
		uniform float4 _Specular_ST;
		uniform sampler2D _EmissiveTexture_1;
		uniform float2 _TexSpeed_1;
		uniform int _EmissiveTextureTiling_1;
		uniform sampler2D _EmissiveTexture_2;
		uniform float2 _TexSpeed_2;
		uniform int _EmissiveTextureTiling_;
		uniform float4 _Emissive_Color;
		uniform int _Emissive_Power;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			o.Albedo = tex2D( _Diffuse, uv_Diffuse ).rgb;
			float2 uv_Specular = i.uv_texcoord * _Specular_ST.xy + _Specular_ST.zw;
			float4 tex2DNode33 = tex2D( _Specular, uv_Specular );
			float mulTime6 = _Time.y * 5.0;
			float2 panner12 = ( mulTime6 * _TexSpeed_1 + i.uv_texcoord);
			float2 panner15 = ( _Time.y * _TexSpeed_2 + i.uv_texcoord);
			o.Emission = ( tex2DNode33.b * ( ( ( tex2D( _EmissiveTexture_1, ( panner12 * _EmissiveTextureTiling_1 ) ) * tex2D( _EmissiveTexture_2, ( panner15 * _EmissiveTextureTiling_ ) ) ) * _Emissive_Color ) * _Emissive_Power ) ).rgb;
			o.Specular = (float)10;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
-1870;40;1821;989;-167.1219;678.4696;1;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;-1333.878,-279.4696;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;44;-1332.878,246.5304;Inherit;False;Property;_TexSpeed_2;TexSpeed_2;11;0;Create;True;0;0;False;0;1,1;0.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;13;-1334.5,392.5;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;45;-1333.878,-124.4696;Inherit;False;Property;_TexSpeed_1;TexSpeed_1;10;0;Create;True;0;0;False;0;1,1;0.01,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;6;-1335.5,5.5;Inherit;False;1;0;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;43;-1341.878,95.5304;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;19;-1039.5,379.5;Inherit;False;Property;_EmissiveTextureTiling_;EmissiveTextureTiling_;4;0;Create;True;0;0;False;0;2;2;0;1;INT;0
Node;AmplifyShaderEditor.PannerNode;15;-1036.5,229.5;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.0074,0.0028;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.IntNode;16;-1052.5,10.5;Inherit;False;Property;_EmissiveTextureTiling_1;EmissiveTextureTiling_1;3;0;Create;True;0;0;False;0;2;2;0;1;INT;0
Node;AmplifyShaderEditor.PannerNode;12;-1055.5,-142.5;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.015,-0.0066;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-764.5,108.5;Inherit;False;2;2;0;FLOAT2;0,0;False;1;INT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-776.5,-7.5;Inherit;False;2;2;0;FLOAT2;1,0;False;1;INT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;25;-557.8785,-116.4696;Inherit;True;Property;_EmissiveTexture_1;EmissiveTexture_1;2;0;Create;True;0;0;False;0;4ff0eb56f29525344886052b43170838;09a1ae27103a3e845b7c7d4f955408f9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;24;-558.8785,151.5304;Inherit;True;Property;_EmissiveTexture_2;EmissiveTexture_2;0;0;Create;True;0;0;False;0;4ff0eb56f29525344886052b43170838;412fc329f5c3f14478f39a218cf7833a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-180.8785,21.53036;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;28;-177.8785,165.5304;Inherit;False;Property;_Emissive_Color;Emissive_Color;6;0;Create;True;0;0;False;0;0.4784314,0.6588235,1,1;0.2216981,0.3635382,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;30;87.12146,166.5304;Inherit;False;Property;_Emissive_Power;Emissive_Power;7;0;Create;True;0;0;False;0;1;3;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;85.12146,21.53036;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;320.1215,21.53036;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;33;151.1215,-492.4696;Inherit;True;Property;_Specular;Specular;5;0;Create;True;0;0;False;0;82848d61deff960419cdc83ba529c6f7;900d272a9ae5c7f40ad6eece9380e687;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;40;693.1221,-772.4696;Inherit;True;Property;_Diffuse;Diffuse;9;0;Create;True;0;0;False;0;82848d61deff960419cdc83ba529c6f7;ec9d09e983b19434ab9fcd68aeb9fde3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;36;786.1221,65.5304;Inherit;False;Constant;_Int4;Int 4;5;0;Create;True;0;0;False;0;10;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;549.1215,-2.469641;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;35;529.1215,-324.4696;Inherit;False;Property;_Specular_Level;Specular_Level;8;0;Create;True;0;0;False;0;50;1;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;741.1215,-460.4696;Inherit;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;38;1192,-414;Float;False;True;2;ASEMaterialInspector;0;0;BlinnPhong;Custom/NewSurfaceShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;43;0
WireConnection;15;2;44;0
WireConnection;15;1;13;0
WireConnection;12;0;42;0
WireConnection;12;2;45;0
WireConnection;12;1;6;0
WireConnection;18;0;15;0
WireConnection;18;1;19;0
WireConnection;17;0;12;0
WireConnection;17;1;16;0
WireConnection;25;1;17;0
WireConnection;24;1;18;0
WireConnection;26;0;25;0
WireConnection;26;1;24;0
WireConnection;27;0;26;0
WireConnection;27;1;28;0
WireConnection;29;0;27;0
WireConnection;29;1;30;0
WireConnection;31;0;33;3
WireConnection;31;1;29;0
WireConnection;34;0;33;1
WireConnection;34;1;35;0
WireConnection;38;0;40;0
WireConnection;38;2;31;0
WireConnection;38;3;36;0
ASEEND*/
//CHKSM=3F17B9D719B0A9403C6D564336E031F92363E4CA
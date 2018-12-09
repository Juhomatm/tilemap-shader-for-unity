// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Tiled/Opaque"
{
	Properties
	{
		_MainTex("Map Texture", 2D) = "white" {}
		_TilemapTex("Tilemap Texture", 2D) = "white" {}

		_TilesetHorizontalCount("Tileset Horizontal Count", int) = 10
		_TilesetVerticalCount("Tileset Vertical Count", int) = 10

		_MapWidth("Map Width", int) = 10
		_MapHeight("Map Height", int) = 10
		_PaddingFraction("Padding Fraction", float) = 0.07
		_AltasSizeX("Altas Size X", int) = 100
		_AltasSizeY("Altas Size Y", int) = 100

	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				//sampler2D _MainTex2;
				//sampler2D _MainTex3;
				//sampler2D _MainTex4;
				sampler2D _TilemapTex;
				float4 _MainTex_ST;

				int _TilesetHorizontalCount;
				int _TilesetVerticalCount;

				int _MapWidth;
				int _MapHeight;
				float _PaddingFraction;
				int _AtlasSizeX;
				int _AtlasSizeY;

				float GetMipMapLevel(float2 iUV, float2 iTextureSize)
				{
					float2 dx = ddx(iUV * iTextureSize.x);
					float2 dy = ddy(iUV * iTextureSize.y);
					float d = max(dot(dx, dx), dot(dy, dy));
					return 0.5 * log2(d);
				}

				//float SampleFromLayers(float2 uv, sampler2D sampler1, sampler2D sampler2, sampler2D sampler3, sampler2D sampler4)
				//{
				//	fixed4 col = tex2D(sampler4, uv);
				//	if (col.a > 0.5)
				//	{
				//		return col;
				//	}
				//	col = tex2D(sampler3, uv);
				//	if (col.a > 0.5)
				//	{
				//		return col;
				//	}
				//	col = tex2D(sampler2, uv);
				//	if (col.a > 0.5)
				//	{
				//		return col;
				//	}

				//	return tex2D(sampler1, uv).x;
				//}

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//float tileCode = SampleFromLayers(i.uv, _MainTex, _MainTex2, _MainTex3, _MainTex4);
					//int tileIndex = floor(tileCode * (_TilesetHorizontalCount * _TilesetVerticalCount));
					int tileIndex = floor(tex2D(_MainTex, i.uv).x * (_TilesetHorizontalCount * _TilesetVerticalCount));

					float paddingX = _PaddingFraction / _TilesetHorizontalCount;
					float paddingY = _PaddingFraction / _TilesetVerticalCount;

					float maxYIndex = _TilesetVerticalCount - 1;
					float tileXpos = (tileIndex % _TilesetHorizontalCount) / (float)_TilesetHorizontalCount;
					float tileYpos = (maxYIndex - (tileIndex / _TilesetHorizontalCount)) / (float)_TilesetVerticalCount;
					float2 uv = float2(tileXpos + paddingX, tileYpos + paddingY);
					float xoffset = frac(i.uv.x * _MapWidth) / _TilesetHorizontalCount;
					float yoffset = (frac(i.uv.y * _MapHeight) / _TilesetVerticalCount);
					float distanceToMaxOffsetX = ((float)xoffset / (1 / (float)_TilesetHorizontalCount));
					float distanceToMaxOffsetY = ((float)yoffset / (1 / (float)_TilesetVerticalCount));

					float offsetOffsetX = distanceToMaxOffsetX * -2 * paddingX;
					float offsetOffsetY = distanceToMaxOffsetY * -2 * paddingY;

					uv += float2(xoffset + offsetOffsetX, /*-*/(yoffset + offsetOffsetY));
					float2 atlasSize = float2(_AtlasSizeX, _AtlasSizeX);
					float mipLevel = GetMipMapLevel(i.uv, atlasSize);
					float2 sampleSpot = float2(uv.x, uv.y);
					return tex2Dlod(_TilemapTex, float4(sampleSpot, 0, mipLevel));
				}

				ENDCG
			}
		}
}

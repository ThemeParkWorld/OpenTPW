vertex {
  //
  // Layout
  //
  layout( location = 0 ) in vec3 position;
  layout( location = 1 ) in vec3 normal;
  layout( location = 2 ) in vec2 texCoords;

  //
  // Uniforms
  //
  layout( set = 0, binding = 0 ) uniform ObjectUniformBuffer {
      mat4 g_mModel;
      mat4 g_mView;
      mat4 g_mProj;

      vec3 g_vLightPos;
      vec3 g_vLightColor;
      vec3 g_vCameraPos;
  } g_oUbo;

  //
  // Out
  //
  layout( location = 0 ) out struct VS_OUT {
      vec2 vTexCoords;
      vec3 vNormal;
      vec3 vPosition;
  } vs_out;

  void main() {
      vs_out.vTexCoords = texCoords;
      vs_out.vNormal = normal;
      vs_out.vPosition = vec3( g_oUbo.g_mModel * vec4( position, 1.0 ));

      vec4 pos = g_oUbo.g_mModel * vec4( position, 1.0 );
      gl_Position = g_oUbo.g_mProj * g_oUbo.g_mView * pos;
  }
}

fragment {
  //
  // In
  //
  layout( location = 0 ) in struct VS_OUT {
      vec2 vTexCoords;
      vec3 vNormal;
      vec3 vPosition;
  } vs_out;

  //
  // Out
  //
  layout( location = 0 ) out vec4 fragColor;

  //
  // Uniforms
  //
  layout( set = 0, binding = 0 ) uniform ObjectUniformBuffer {
      mat4 g_mModel;
      mat4 g_mView;
      mat4 g_mProj;

      vec3 g_vLightPos;
      vec3 g_vLightColor;
      vec3 g_vCameraPos;
  } g_oUbo;
  
  layout( set = 1, binding = 0 ) uniform texture2D Color;
  layout( set = 1, binding = 1 ) uniform sampler s_Color;

  vec3 lambert( vec3 vLightDir, vec3 vNormal ) 
  {
      return dot( normalize( vLightDir ), normalize( vNormal ) ) * g_oUbo.g_vLightColor;
  }

  vec3 ambient( vec3 vLightColor ) 
  {
      return vLightColor * 0.2;
  }

  vec3 specular( vec3 vLightDir, vec3 vNormal, vec3 vCameraDir, float fShininess ) 
  {
      vec3 vReflection = reflect( normalize( -vLightDir ), normalize( vNormal ) );
      return pow( max( dot( normalize( vCameraDir ), vReflection ), 0.0 ), fShininess ) * g_oUbo.g_vLightColor;
  }

  void main() 
  {
      vec3 vLightDir = normalize( g_oUbo.g_vLightPos - vs_out.vPosition );

      vec3 vLambert = lambert( vLightDir, vs_out.vNormal );
      vec3 vAmbient = ambient( g_oUbo.g_vLightColor );
      
      vec3 vCameraDir = normalize( g_oUbo.g_vCameraPos - vs_out.vPosition );
      vec3 vSpecular = specular( vLightDir, vs_out.vNormal, vCameraDir, 32.0 );

      vec4 vColor = texture( sampler2D( Color, s_Color ), vs_out.vTexCoords );

      fragColor = vec4( vLambert + vAmbient, 1.0 ) * vColor;
  }
}
﻿using System.Diagnostics;
using Veldrid;

namespace OpenTPW;

[Flags]
public enum MaterialFlags
{
	None,

	DisableDepthTest,
	DisableDepthWrite,

	DisableDepth = DisableDepthTest | DisableDepthWrite
}

public partial class Material : Asset
{
	public Shader Shader { get; set; }

	public Type UniformBufferType { get; } = typeof( ObjectUniformBuffer );
	public Pipeline Pipeline { get; private set; } = null!;

	private Dictionary<string, BindableResource> _boundResources = new();

	private ResourceLayout[] _resourceLayouts;
	private ResourceSet[] _resourceSets;

	public Material( string shaderPath, MaterialFlags flags = MaterialFlags.None )
	{
		Shader = new Shader( shaderPath );

		All.Add( this );
		SetupResources( flags );
	}

	protected Material( string shaderPath, Type uniformBufferType )
	{
		Shader = new Shader( shaderPath );
		UniformBufferType = uniformBufferType;

		All.Add( this );
		SetupResources( MaterialFlags.None );
	}

	private DeviceBuffer UniformBuffer;

	private static readonly Sampler[] Samplers =
	[
		CreateSampler( SamplerType.Anisotropic ),
		CreateSampler( SamplerType.Linear ),
		CreateSampler( SamplerType.Point )
	];

	private static Sampler CreateSampler( SamplerType type )
	{
		var samplerFilter = type switch
		{
			SamplerType.Anisotropic => SamplerFilter.Anisotropic,
			SamplerType.Linear => SamplerFilter.MinLinear_MagLinear_MipLinear,
			SamplerType.Point => SamplerFilter.MinPoint_MagPoint_MipPoint,
			_ => throw new NotImplementedException()
		};

		var samplerDescription = new SamplerDescription(
			SamplerAddressMode.Clamp,
			SamplerAddressMode.Clamp,
			SamplerAddressMode.Clamp,
			samplerFilter,
			ComparisonKind.Always,
			(type == SamplerType.Anisotropic) ? 16u : 0u,
			0,
			0,
			0,
			SamplerBorderColor.TransparentBlack
		);

		return Device.ResourceFactory.CreateSampler( samplerDescription );
	}

	public void Set<T>( string name, T obj ) where T : unmanaged
	{
		Render.ImmediateSubmit( cmd =>
		{
			cmd.UpdateBuffer( UniformBuffer, 0, [obj] );
			_boundResources[name] = UniformBuffer;
		} );
	}

	public void Set( string name, Texture texture )
	{
		_boundResources[name] = texture.NativeTexture;
		_boundResources["s_" + name] = Samplers[(int)texture.SamplerType];
	}

	internal ResourceLayout[] CreateResourceLayouts()
	{
		return Shader.ResourceLayouts.Select( x => Device.ResourceFactory.CreateResourceLayout( x ) ).ToArray();
	}

	internal ResourceSet[] CreateResourceSets()
	{
		Debug.Assert( _resourceLayouts != null );

		List<ResourceSetDescription> resourceSetDescriptions = new();

		for ( int i = 0; i < Shader.ResourceLayouts.Length; i++ )
		{
			ResourceLayoutDescription resourceLayout = Shader.ResourceLayouts[i];
			var sortedBoundResources = new List<BindableResource>();

			foreach ( var resource in resourceLayout.Elements )
			{
				if ( _boundResources.TryGetValue( resource.Name, out var boundResource ) )
				{
					sortedBoundResources.Add( boundResource );
				}
				else
				{
					throw new Exception( $"{resource.Name} wasn't bound at draw time!" );
				}
			}

			var resourceSetDescription = new ResourceSetDescription()
			{
				Layout = _resourceLayouts[i],
				BoundResources = [.. sortedBoundResources]
			};

			resourceSetDescriptions.Add( resourceSetDescription );
		}

		return resourceSetDescriptions.Select( x => Device.ResourceFactory.CreateResourceSet( x ) ).ToArray();
	}

	internal void CreateResources( out ResourceSet[] resourceSets, out ResourceLayout[] resourceLayouts )
	{
		// Resource layout doesn't change, but resource set does
		_resourceLayouts ??= CreateResourceLayouts();
		resourceLayouts = _resourceLayouts;

		_resourceSets = CreateResourceSets();
		resourceSets = _resourceSets;
	}

	private void SetupResources( MaterialFlags flags )
	{
		var vertexLayout = new VertexLayoutDescription( Vertex.VertexElementDescriptions );

		//
		// Create resource layout - but only from what we're using/need
		//
		_resourceLayouts ??= CreateResourceLayouts();

		//
		// Create pipeline
		//
		var pipelineDescription = new GraphicsPipelineDescription()
		{
			BlendState = BlendStateDescription.SingleAlphaBlend,

			DepthStencilState = new DepthStencilStateDescription(
				!flags.HasFlag( MaterialFlags.DisableDepthTest ),
				!flags.HasFlag( MaterialFlags.DisableDepthWrite ),
				flags.HasFlag( MaterialFlags.DisableDepthTest | MaterialFlags.DisableDepthWrite ) ? ComparisonKind.Always : ComparisonKind.Less
			),

			RasterizerState = new RasterizerStateDescription(
				FaceCullMode.Back,
				PolygonFillMode.Solid,
				FrontFace.Clockwise,
				true,
				false
			),

			PrimitiveTopology = PrimitiveTopology.TriangleList,
			ResourceLayouts = [.. _resourceLayouts],
			ShaderSet = new ShaderSetDescription( [vertexLayout], Shader.ShaderProgram ),
			Outputs = Device.SwapchainFramebuffer.OutputDescription
		};

		Pipeline = Device.ResourceFactory.CreateGraphicsPipeline( pipelineDescription );

		//
		// Create single uniform buffer
		// TODO: dynamic 'scratch' buffer system
		//
		var bufferDescription = new BufferDescription( 16 * 200, BufferUsage.UniformBuffer | BufferUsage.Dynamic );
		UniformBuffer = Device.ResourceFactory.CreateBuffer( bufferDescription );
	}
}

public class Material<T>( string shaderPath ) : Material( shaderPath, typeof( T ) );

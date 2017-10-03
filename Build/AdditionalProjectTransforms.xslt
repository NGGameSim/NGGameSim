<xsl:if test="/Input/Generation/ProjectName = 'SimRenderer'">
	<PropertyGroup>
		<MonoGamePlatform>DesktopGL</MonoGamePlatform>
	</PropertyGroup>
	<Import Project="$(MSBuildExtensionsPath)\MonoGame\v3.0\MonoGame.Common.props"/>
	<Import Project="$(MSBuildExtensionsPath)\MonoGame\v3.0\MonoGame.Content.Builder.targets"/>
	<!-- We have to include it though this, because Protobuild does not support MonoGameContentReference files directly. -->
	<ItemGroup>
		<MonoGameContentReference Include="..\Content\Content.mgcb">
			<Link>Content\Content.mgcb</Link>
		</MonoGameContentReference>
	</ItemGroup>
</xsl:if>
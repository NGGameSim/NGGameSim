<!-- Needed to get Monogame content projects running and building properly. -->
<xsl:if test="/Input/Generation/ProjectName = 'SimViewer'">
	<PropertyGroup>
		<!-- Fixes a bug in MonoDevelop 6.2 that prevents this project from building. -->
		<UseMSBuildEngine>false</UseMSBuildEngine>
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

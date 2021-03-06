﻿using System;
using TwistedLogik.Ultraviolet.Content;

namespace TwistedLogik.Ultraviolet.OpenGL.Graphics
{
    /// <summary>
    /// Loads vertex shader assets.
    /// </summary>
    [ContentProcessor]
    public sealed class OpenGLVertexShaderProcessor : ShaderProcessor<OpenGLVertexShader>
    {
        /// <inheritdoc/>
        public override OpenGLVertexShader Process(ContentManager manager, IContentProcessorMetadata metadata, String input)
        {
            var source = ReplaceIncludes(manager, metadata, input);
            return new OpenGLVertexShader(manager.Ultraviolet, new[] { source });
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TwistedLogik.Ultraviolet.Content;
using TwistedLogik.Ultraviolet.Graphics;

namespace TwistedLogik.Ultraviolet.OpenGL.Graphics
{
    /// <summary>
    /// Loads shader effect assets from JSON definition files.
    /// </summary>
    [ContentProcessor]
    public sealed partial class OpenGLEffectImplementationProcessorFromJObject : EffectImplementationProcessor<JObject>
    {
        /// <inheritdoc/>
        public override void ExportPreprocessed(ContentManager manager, IContentProcessorMetadata metadata, BinaryWriter writer, JObject input, Boolean delete)
        {
            var description = input.ToObject<EffectDescription>();

            const Byte FileVersion = 0;
            writer.Write(FileVersion);

            writer.Write(description.Parameters?.Count() ?? 0);
            foreach (var parameter in description.Parameters)
                writer.Write(parameter);

            if (!description.Techniques?.Any() ?? false)
                throw new ContentLoadException(OpenGLStrings.EffectMustHaveTechniques);

            writer.Write(description.Techniques.Count());

            foreach (var technique in description.Techniques)
            {
                writer.Write(technique.Name);

                if (!technique.Passes?.Any() ?? false)
                    throw new ContentLoadException(OpenGLStrings.EffectTechniqueMustHavePasses);

                writer.Write(technique.Passes.Count());

                foreach (var pass in technique.Passes)
                {
                    writer.Write(pass.Name);

                    if (String.IsNullOrEmpty(pass.Stages.VertexShader))
                        throw new ContentLoadException(OpenGLStrings.EffectMustHaveVertexAndFragmentShader);

                    writer.Write(pass.Stages.VertexShader);
                    writer.Write(pass.Stages.VertexShaderES ?? String.Empty);

                    if (String.IsNullOrEmpty(pass.Stages.FragmentShader))
                        throw new ContentLoadException(OpenGLStrings.EffectMustHaveVertexAndFragmentShader);

                    writer.Write(pass.Stages.FragmentShader);
                    writer.Write(pass.Stages.FragmentShaderES ?? String.Empty);
                }
            }
        }

        /// <inheritdoc/>
        public override EffectImplementation ImportPreprocessed(ContentManager manager, IContentProcessorMetadata metadata, BinaryReader reader)
        {
            // version
            reader.ReadByte();

            var parameters = new List<String>();
            var parameterCount = reader.ReadInt32();
            for (int i = 0; i < parameterCount; i++)
                parameters.Add(reader.ReadString());

            var techniques = new List<OpenGLEffectTechnique>();
            var techniqueCount = reader.ReadInt32();
            if (techniqueCount == 0)
                throw new ContentLoadException(OpenGLStrings.EffectMustHaveTechniques);

            for (int i = 0; i < techniqueCount; i++)
            {
                var techniqueName = reader.ReadString();

                var passes = new List<OpenGLEffectPass>();
                var passCount = reader.ReadInt32();
                if (passCount == 0)
                    throw new ContentLoadException(OpenGLStrings.EffectTechniqueMustHavePasses);

                for (int j = 0; j < passCount; j++)
                {
                    var passName = reader.ReadString();

                    var vertPathGL = reader.ReadString();
                    var vertPathES = reader.ReadString();
                    var vertPath = GetShaderForCurrentPlatform(vertPathGL, vertPathES);

                    if (String.IsNullOrEmpty(vertPath))
                        throw new ContentLoadException(OpenGLStrings.EffectMustHaveVertexAndFragmentShader);

                    vertPath = ResolveDependencyAssetPath(metadata, vertPath);

                    var fragPathGL = reader.ReadString();
                    var fragPathES = reader.ReadString();
                    var fragPath = GetShaderForCurrentPlatform(fragPathGL, fragPathES);

                    if (String.IsNullOrEmpty(fragPath))
                        throw new ContentLoadException(OpenGLStrings.EffectMustHaveVertexAndFragmentShader);

                    fragPath = ResolveDependencyAssetPath(metadata, fragPath);

                    var vertShader = manager.Load<OpenGLVertexShader>(vertPath);
                    var fragShader = manager.Load<OpenGLFragmentShader>(fragPath);

                    var programs = new[] { new OpenGLShaderProgram(manager.Ultraviolet, vertShader, fragShader, true) };
                    passes.Add(new OpenGLEffectPass(manager.Ultraviolet, passName, programs));
                }

                techniques.Add(new OpenGLEffectTechnique(manager.Ultraviolet, techniqueName, passes));
            }

            return new OpenGLEffectImplementation(manager.Ultraviolet, techniques, parameters);
        }

        /// <inheritdoc/>
        public override EffectImplementation Process(ContentManager manager, IContentProcessorMetadata metadata, JObject input)
        {
            var description = input.ToObject<EffectDescription>();

            var techniques = new List<OpenGLEffectTechnique>();
            if (!description.Techniques?.Any() ?? false)
                throw new ContentLoadException(OpenGLStrings.EffectMustHaveTechniques);

            foreach (var technique in description.Techniques)
            {
                var techniqueName = technique.Name;
                var techniquePasses = new List<OpenGLEffectPass>();

                if (!technique.Passes?.Any() ?? false)
                    throw new ContentLoadException(OpenGLStrings.EffectTechniqueMustHavePasses);

                foreach (var pass in technique.Passes)
                {
                    var passName = pass.Name;

                    var vertPath = GetShaderForCurrentPlatform(pass.Stages.VertexShader, pass.Stages.VertexShaderES);
                    if (String.IsNullOrEmpty(vertPath))
                        throw new ContentLoadException(OpenGLStrings.EffectMustHaveVertexAndFragmentShader);

                    vertPath = ResolveDependencyAssetPath(metadata, vertPath);

                    var fragPath = GetShaderForCurrentPlatform(pass.Stages.FragmentShader, pass.Stages.FragmentShaderES);
                    if (String.IsNullOrEmpty(fragPath))
                        throw new ContentLoadException(OpenGLStrings.EffectMustHaveVertexAndFragmentShader);

                    fragPath = ResolveDependencyAssetPath(metadata, fragPath);

                    var vertShader = manager.Load<OpenGLVertexShader>(vertPath);
                    var fragShader = manager.Load<OpenGLFragmentShader>(fragPath);

                    var programs = new[] { new OpenGLShaderProgram(manager.Ultraviolet, vertShader, fragShader, true) };
                    techniquePasses.Add(new OpenGLEffectPass(manager.Ultraviolet, passName, programs));
                }

                techniques.Add(new OpenGLEffectTechnique(manager.Ultraviolet, techniqueName, techniquePasses));
            }

            return new OpenGLEffectImplementation(manager.Ultraviolet, techniques, description.Parameters);
        }

        /// <inheritdoc/>
        public override Boolean SupportsPreprocessing => true;        
    }
}

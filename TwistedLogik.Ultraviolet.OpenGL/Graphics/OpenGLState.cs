﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TwistedLogik.Gluon;
using TwistedLogik.Nucleus.Collections;

namespace TwistedLogik.Ultraviolet.OpenGL.Graphics
{
    /// <summary>
    /// Represents a collection of states to apply to the OpenGL context.
    /// </summary>
    internal sealed class OpenGLState : IDisposable
    {
        /// <summary>
        /// Represents the type of a <see cref="OpenGLState"/> object, which determines how it modifies the OpenGL context.
        /// </summary>
        private enum OpenGLStateType
        {
            None,
            ActiveTexture,
            BindTexture2D,
            BindVertexArrayObject,
            BindArrayBuffer,
            BindElementArrayBuffer,
            BindFramebuffer,
            BindRenderbuffer,
            UseProgram,
            CreateTexture2D,
            CreateArrayBuffer,
            CreateElementArrayBuffer,
            CreateFramebuffer,
            CreateRenderbuffer,
        }

        /// <summary>
        /// Initializes the <see cref="OpenGLState"/> type.
        /// </summary>
        static OpenGLState()
        {
            glCachedIntegers = typeof(OpenGLState).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(x => x.FieldType == typeof(OpenGLStateInteger))
                .Select(x => (OpenGLStateInteger)x.GetValue(null)).ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLState"/> class.
        /// </summary>
        private OpenGLState()
        { }

        /// <summary>
        /// Resets the values that are stored in the OpenGL state cache to their defaults.
        /// </summary>
        public static void ResetCache()
        {
            foreach (var glCachedInteger in glCachedIntegers)
                glCachedInteger.Reset();

            glTextureBinding2DByTextureUnit.Clear();
        }

        /// <summary>
        /// Verifies that the values that are stored in the OpenGL state cache accurately
        /// reflect the current state of the OpenGL context.
        /// </summary>
        [Conditional("VERIFY_OPENGL_CACHE")]
        public static void VerifyCache()
        {
            foreach (var value in glCachedIntegers)
            {
                value.Verify();
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which sets the active texture unit.
        /// </summary>
        /// <param name="texture">The texture unit to activate.</param>
        public static OpenGLState ScopedActiveTexture(UInt32 texture)
        {
            var state = pool.Retrieve();
            
            state.stateType                = OpenGLStateType.ActiveTexture;
            state.disposed                 = false;
            state.newGL_ACTIVE_TEXTURE     = texture;

            state.Apply();

            return state;
        }

        /// <summary>
        /// Immediately sets the active texture unit and updates the state cache.
        /// </summary>
        /// <param name="texture">The texture unit to activate.</param>
        public static void ActiveTexture(UInt32 texture)
        {
            gl.ActiveTexture(texture);
            gl.ThrowIfError();

            var tb2d = 0u;
            glTextureBinding2DByTextureUnit.TryGetValue(texture, out tb2d);

            GL_ACTIVE_TEXTURE.Update(texture);
            GL_TEXTURE_BINDING_2D.Update(tb2d);

            VerifyCache();
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which binds a 2D texture to the context.
        /// </summary>
        /// <param name="texture">The texture to bind to the context.</param>
        /// <param name="force">A value indicating whether to force-bind the texture, even if DSA is available.</param>
        public static OpenGLState ScopedBindTexture2D(UInt32 texture, Boolean force = false)
        {
            var state = pool.Retrieve();

            state.stateType                = OpenGLStateType.BindTexture2D;
            state.disposed                 = false;
            state.forced                   = force;
            state.newGL_TEXTURE_BINDING_2D = texture;

            state.Apply();

            return state;
        }

        /// <summary>
        /// Immediately binds a 2D texture to the OpenGL context and updates the state cache.
        /// </summary>
        /// <param name="texture">The texture to bind to the context.</param>
        public static void Texture2D(UInt32 texture)
        {
            gl.BindTexture(gl.GL_TEXTURE_2D, texture);
            gl.ThrowIfError();

            GL_TEXTURE_BINDING_2D.Update(texture);
            glTextureBinding2DByTextureUnit[GL_ACTIVE_TEXTURE] = texture;

            VerifyCache();
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which binds a vertex array object to the context.
        /// </summary>
        /// <param name="vertexArrayObject">The vertex array object to bind to the context.</param>
        /// <param name="arrayBuffer">The vertex array's associated array buffer.</param>
        /// <param name="elementArrayBuffer">The vertex array's associated element array buffer.</param>
        /// <param name="force">A value indicating whether to force-bind the vertex array object, even if DSA is available.</param>
        public static OpenGLState ScopedBindVertexArrayObject(UInt32 vertexArrayObject, UInt32 arrayBuffer, UInt32 elementArrayBuffer, Boolean force = false)
        {
            var state = pool.Retrieve();

            state.stateType                          = OpenGLStateType.BindVertexArrayObject;
            state.disposed                           = false;
            state.forced                             = force;
            state.newGL_VERTEX_ARRAY_BINDING         = vertexArrayObject;
            state.newGL_ARRAY_BUFFER_BINDING         = arrayBuffer;
            state.newGL_ELEMENT_ARRAY_BUFFER_BINDING = elementArrayBuffer;

            state.Apply();

            return state;
        }

        /// <summary>
        /// Immediately binds a vertex array object to the OpenGL context and updates the state cache.
        /// </summary>
        /// <param name="vertexArrayObject">The vertex array object to bind to the context.</param>
        /// <param name="arrayBuffer">The vertex array's associated array buffer.</param>
        /// <param name="elementArrayBuffer">The vertex array's associated element array buffer.</param>
        public static void BindVertexArrayObject(UInt32 vertexArrayObject, UInt32 arrayBuffer, UInt32 elementArrayBuffer)
        {
            gl.BindVertexArray(vertexArrayObject);
            gl.ThrowIfError();

            GL_VERTEX_ARRAY_BINDING.Update(vertexArrayObject);
            GL_ARRAY_BUFFER_BINDING.Update(arrayBuffer);
            GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(elementArrayBuffer);
            VerifyCache();
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which binds an array buffer to the context.
        /// </summary>
        /// <param name="arrayBuffer">The array buffer to bind to the OpenGL context.</param>
        /// <param name="force">A value indicating whether to force-bind the array buffer, even if DSA is available.</param>
        public static OpenGLState ScopedBindArrayBuffer(UInt32 arrayBuffer, Boolean force = false)
        {
            var state = pool.Retrieve();

            state.stateType                  = OpenGLStateType.BindArrayBuffer;
            state.disposed                   = false;
            state.forced                     = force;
            state.newGL_ARRAY_BUFFER_BINDING = arrayBuffer;

            state.Apply();

            return state;
        }

        /// <summary>
        /// Immediately binds an array buffer to the OpenGL context and updates the state cache.
        /// </summary>
        /// <param name="arrayBuffer">The array buffer to bind to the OpenGL context.</param>
        public static void BindArrayBuffer(UInt32 arrayBuffer)
        {
            gl.BindBuffer(gl.GL_ARRAY_BUFFER, arrayBuffer);
            gl.ThrowIfError();

            GL_ARRAY_BUFFER_BINDING.Update(arrayBuffer);
            VerifyCache();
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which binds an element array buffer to the context.
        /// </summary>
        /// <param name="elementArrayBuffer">The element array buffer to bind to the OpenGL context.</param>
        /// <param name="force">A value indicating whether to force-bind the array buffer, even if DSA is available.</param>
        public static OpenGLState ScopedBindElementArrayBuffer(UInt32 elementArrayBuffer, Boolean force = false)
        {
            var state = pool.Retrieve();

            state.stateType                          = OpenGLStateType.BindElementArrayBuffer;
            state.disposed                           = false;
            state.forced                             = force;
            state.newGL_ELEMENT_ARRAY_BUFFER_BINDING = elementArrayBuffer;

            state.Apply();

            return state;
        }

        /// <summary>
        /// Immediately binds an element array buffer to the OpenGL context and updates the state cache.
        /// </summary>
        /// <param name="elementArrayBuffer">The element array buffer to bind to the OpenGL context.</param>
        public static void BindElementArrayBuffer(UInt32 elementArrayBuffer)
        {
            gl.BindBuffer(gl.GL_ELEMENT_ARRAY_BUFFER, elementArrayBuffer);
            gl.ThrowIfError();

            GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(elementArrayBuffer);
            VerifyCache();
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which binds a framebuffer to the context.
        /// </summary>
        /// <param name="framebuffer">The framebuffer to bind to the OpenGL context.</param>
        /// <param name="force">A value indicating whether to force-bind the framebuffer, even if DSA is available.</param>
        public static OpenGLState ScopedBindFramebuffer(UInt32 framebuffer, Boolean force = false)
        {
            var state = pool.Retrieve();

            state.stateType                 = OpenGLStateType.BindFramebuffer;
            state.disposed                  = false;
            state.forced                    = force;
            state.newGL_FRAMEBUFFER_BINDING = framebuffer;

            state.Apply();

            return state;
        }

        /// <summary>
        /// Immediately binds a framebuffer to the OpenGL context and updates the state cache.
        /// </summary>
        /// <param name="framebuffer">The framebuffer to bind to the OpenGL context.</param>
        public static void BindFramebuffer(UInt32 framebuffer)
        {
            gl.BindFramebuffer(gl.GL_FRAMEBUFFER, framebuffer);
            gl.ThrowIfError();

            GL_FRAMEBUFFER_BINDING.Update(framebuffer);
            VerifyCache();
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which binds a renderbuffer to the context.
        /// </summary>
        /// <param name="renderbuffer">The renderbuffer to bind to the OpenGL context.</param>
        /// <param name="force">A value indicating whether to force-bind the renderbuffer, even if DSA is available.</param>
        public static OpenGLState ScopedBindRenderbuffer(UInt32 renderbuffer, Boolean force = false)
        {
            var state = pool.Retrieve();

            state.stateType = OpenGLStateType.BindRenderbuffer;
            state.disposed = false;
            state.forced = force;
            state.newGL_RENDERBUFFER_BINDING = renderbuffer;

            state.Apply();

            return state;
        }

        /// <summary>
        /// Immediately binds a renderbuffer to the OpenGL context and updates the state cache.
        /// </summary>
        /// <param name="renderbuffer">The renderbuffer to bind to the OpenGL context.</param>
        public static void BindRenderbuffer(UInt32 renderbuffer)
        {
            gl.BindRenderbuffer(gl.GL_RENDERBUFFER, renderbuffer);
            gl.ThrowIfError();

            GL_RENDERBUFFER_BINDING.Update(renderbuffer);
            VerifyCache();
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which activates a shader program.
        /// </summary>
        /// <param name="program">The program to bind to the OpenGL context.</param>
        public static OpenGLState ScopedUseProgram(UInt32 program)
        {
            var state = pool.Retrieve();

            state.stateType             = OpenGLStateType.UseProgram;
            state.disposed              = false;
            state.newGL_CURRENT_PROGRAM = program;

            state.Apply();

            return state;
        }

        /// <summary>
        /// Immediately activates a shader program and updates the state cache.
        /// </summary>
        /// <param name="program">The program to bind to the OpenGL context.</param>
        public static void UseProgram(UInt32 program)
        {
            gl.UseProgram(program);
            gl.ThrowIfError();

            GL_CURRENT_PROGRAM.Update(program);
            VerifyCache();
        }

        /// <summary>
        /// Immediately creates an array buffer and updates the state cache.
        /// </summary>
        /// <param name="buffer">The identifier of the buffer that was created.</param>
        public static void CreateArrayBuffer(out UInt32 buffer)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                buffer = gl.CreateBuffer();
                gl.ThrowIfError();
            }
            else
            {
                buffer = gl.GenBuffer();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindBuffer(gl.GL_ARRAY_BUFFER, buffer);
                    gl.ThrowIfError();

                    GL_ARRAY_BUFFER_BINDING.Update(buffer);
                    VerifyCache();
                }
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which creates an array buffer.
        /// </summary>
        /// <param name="buffer">The identifier of the buffer that was created.</param>
        public static OpenGLState ScopedCreateArrayBuffer(out UInt32 buffer)
        {
            var state = pool.Retrieve();

            state.stateType             = OpenGLStateType.CreateArrayBuffer;
            state.disposed              = false;

            state.Apply_CreateArrayBuffer(out buffer);

            return state;
        }

        /// <summary>
        /// Immediately creates an element array buffer and updates the state cache.
        /// </summary>
        /// <param name="buffer">The identifier of the buffer that was created.</param>
        public static void CreateElementArrayBuffer(out UInt32 buffer)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                buffer = gl.CreateBuffer();
                gl.ThrowIfError();
            }
            else
            {
                buffer = gl.GenBuffer();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindBuffer(gl.GL_ELEMENT_ARRAY_BUFFER, buffer);
                    gl.ThrowIfError();

                    GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(buffer);
                    VerifyCache();
                }
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which creates an element array buffer.
        /// </summary>
        /// <param name="buffer">The identifier of the buffer that was created.</param>
        public static OpenGLState ScopedCreateElementArrayBuffer(out UInt32 buffer)
        {
            var state = pool.Retrieve();

            state.stateType             = OpenGLStateType.CreateElementArrayBuffer;
            state.disposed              = false;

            state.Apply_CreateElementArrayBuffer(out buffer);

            return state;
        }

        /// <summary>
        /// Immediately creates a 2D texture and updates the state cache.
        /// </summary>
        /// <param name="texture">The identifier of the texture that was created.</param>
        public static void CreateTexture2D(out UInt32 texture)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                texture = gl.CreateTexture(gl.GL_TEXTURE_2D);
                gl.ThrowIfError();
            }
            else
            {
                texture = gl.GenTexture();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindTexture(gl.GL_TEXTURE_2D, texture);
                    gl.ThrowIfError();

                    GL_TEXTURE_BINDING_2D.Update(texture);
                    glTextureBinding2DByTextureUnit[GL_ACTIVE_TEXTURE] = texture;

                    VerifyCache();
                }
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which creates a 2D texture.
        /// </summary>
        /// <param name="texture">The identifier of the texture that was created.</param>
        public static OpenGLState ScopedCreateTexture2D(out UInt32 texture)
        {
            var state = pool.Retrieve();

            state.stateType             = OpenGLStateType.CreateTexture2D;
            state.disposed              = false;

            state.Apply_CreateTexture2D(out texture);

            return state;
        }

        /// <summary>
        /// Immediately creates a framebuffer and updates the state cache.
        /// </summary>
        /// <param name="framebuffer">The identifier of the framebuffer that was created.</param>
        public static void CreateFramebuffer(out UInt32 framebuffer)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                framebuffer = gl.CreateFramebuffer();
                gl.ThrowIfError();
            }
            else
            {
                framebuffer = gl.GenFramebuffer();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindFramebuffer(gl.GL_FRAMEBUFFER, framebuffer);
                    gl.ThrowIfError();

                    GL_FRAMEBUFFER_BINDING.Update(framebuffer);
                    VerifyCache();
                }
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which creates a framebuffer.
        /// </summary>
        /// <param name="framebuffer">The identifier of the framebuffer that was created.</param>
        public static OpenGLState ScopedCreateFramebuffer(out UInt32 framebuffer)
        {
            var state = pool.Retrieve();

            state.stateType = OpenGLStateType.CreateFramebuffer;
            state.disposed  = false;

            state.Apply_CreateFramebuffer(out framebuffer);

            return state;
        }

        /// <summary>
        /// Immediately creates a renderbuffer and updates the state cache.
        /// </summary>
        /// <param name="renderbuffer">The identifier of the renderbuffer that was created.</param>
        public static void CreateRenderbuffer(out UInt32 renderbuffer)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                renderbuffer = gl.CreateRenderbuffer();
                gl.ThrowIfError();
            }
            else
            {
                renderbuffer = gl.GenRenderbuffer();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindRenderbuffer(gl.GL_RENDERBUFFER, renderbuffer);
                    gl.ThrowIfError();

                    GL_RENDERBUFFER_BINDING.Update(renderbuffer);
                    VerifyCache();
                }
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="OpenGLState"/> which creates a renderbuffer.
        /// </summary>
        /// <param name="renderbuffer">The identifier of the renderbuffer that was created.</param>
        public static OpenGLState ScopedCreateRenderbuffer(out UInt32 renderbuffer)
        {
            var state = pool.Retrieve();

            state.stateType = OpenGLStateType.CreateRenderbuffer;
            state.disposed = false;

            state.Apply_CreateRenderbuffer(out renderbuffer);

            return state;
        }

        /// <summary>
        /// Indicates that the specified texture has been deleted and updates the OpenGL state accordingly.
        /// </summary>
        /// <param name="texture">The texture to delete.</param>
        public static void DeleteTexture2D(UInt32 texture)
        {
            if (GL_TEXTURE_BINDING_2D == texture)
            {
                GL_TEXTURE_BINDING_2D.Update(0);
                glTextureBinding2DByTextureUnit[GL_ACTIVE_TEXTURE] = 0;

                VerifyCache();
            }
        }

        /// <summary>
        /// Indicates that the specified vertex array object has been deleted and updates the OpenGL state accordingly.
        /// </summary>
        /// <param name="vertexArrayObject">The vertex array object to delete.</param>
        /// <param name="arrayBuffer">The array buffer to delete.</param>
        /// <param name="elementArrayBuffer">The element array buffer to delete.</param>
        public static void DeleteVertexArrayObject(UInt32 vertexArrayObject, UInt32 arrayBuffer, UInt32 elementArrayBuffer)
        {
            if (GL_VERTEX_ARRAY_BINDING == vertexArrayObject)
            {
                GL_VERTEX_ARRAY_BINDING.Update(0);
                GL_ARRAY_BUFFER_BINDING.Update(0);
                GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(0);
                VerifyCache();
            }
        }

        /// <summary>
        /// Applies the state object's values to the OpenGL context.
        /// </summary>
        public void Apply()
        {
            switch (stateType)
            {
                case OpenGLStateType.None:
                    break;

                case OpenGLStateType.ActiveTexture:
                    Apply_ActiveTexture();
                    break;

                case OpenGLStateType.BindTexture2D:
                    Apply_BindTexture2D();
                    break;

                case OpenGLStateType.BindVertexArrayObject:
                    Apply_BindVertexArrayObject();
                    break;

                case OpenGLStateType.BindArrayBuffer:
                    Apply_BindArrayBuffer();
                    break;

                case OpenGLStateType.BindElementArrayBuffer:
                    Apply_BindElementArrayBuffer();
                    break;

                case OpenGLStateType.BindFramebuffer:
                    Apply_BindFramebuffer();
                    break;

                case OpenGLStateType.BindRenderbuffer:
                    Apply_BindRenderbuffer();
                    break;

                case OpenGLStateType.UseProgram:
                    Apply_UseProgram();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            OpenGLState.VerifyCache();
        }

        private void Apply_ActiveTexture()
        {
            gl.ActiveTexture(newGL_ACTIVE_TEXTURE);
            gl.ThrowIfError();

            var tb2d = 0u;
            glTextureBinding2DByTextureUnit.TryGetValue(newGL_ACTIVE_TEXTURE, out tb2d);

            oldGL_ACTIVE_TEXTURE = OpenGLState.GL_ACTIVE_TEXTURE.Update(newGL_ACTIVE_TEXTURE);
            oldGL_TEXTURE_BINDING_2D = GL_TEXTURE_BINDING_2D.Update(tb2d);
        }

        private void Apply_BindTexture2D()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindTexture(gl.GL_TEXTURE_2D, newGL_TEXTURE_BINDING_2D);
                gl.ThrowIfError();

                oldGL_TEXTURE_BINDING_2D = GL_TEXTURE_BINDING_2D.Update(newGL_TEXTURE_BINDING_2D);

                glTextureBinding2DByTextureUnit[GL_ACTIVE_TEXTURE] = newGL_TEXTURE_BINDING_2D;
            }
        }

        private void Apply_BindVertexArrayObject()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindVertexArray(newGL_VERTEX_ARRAY_BINDING);
                gl.ThrowIfError();

                oldGL_VERTEX_ARRAY_BINDING         = OpenGLState.GL_VERTEX_ARRAY_BINDING.Update(newGL_VERTEX_ARRAY_BINDING);
                oldGL_ARRAY_BUFFER_BINDING         = OpenGLState.GL_ARRAY_BUFFER_BINDING.Update(newGL_ARRAY_BUFFER_BINDING);
                oldGL_ELEMENT_ARRAY_BUFFER_BINDING = OpenGLState.GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(newGL_ELEMENT_ARRAY_BUFFER_BINDING);
            }
        }

        private void Apply_BindArrayBuffer()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindBuffer(gl.GL_ARRAY_BUFFER, newGL_ARRAY_BUFFER_BINDING);
                gl.ThrowIfError();

                oldGL_ARRAY_BUFFER_BINDING = OpenGLState.GL_ARRAY_BUFFER_BINDING.Update(newGL_ARRAY_BUFFER_BINDING);
            }
        }

        private void Apply_BindElementArrayBuffer()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindBuffer(gl.GL_ELEMENT_ARRAY_BUFFER, newGL_ELEMENT_ARRAY_BUFFER_BINDING);
                gl.ThrowIfError();

                oldGL_ELEMENT_ARRAY_BUFFER_BINDING = OpenGLState.GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(newGL_ELEMENT_ARRAY_BUFFER_BINDING);
            }
        }

        private void Apply_BindFramebuffer()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindFramebuffer(gl.GL_FRAMEBUFFER, newGL_FRAMEBUFFER_BINDING);
                gl.ThrowIfError();

                oldGL_FRAMEBUFFER_BINDING = OpenGLState.GL_FRAMEBUFFER_BINDING.Update(newGL_FRAMEBUFFER_BINDING);
            }
        }

        private void Apply_BindRenderbuffer()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindRenderbuffer(gl.GL_RENDERBUFFER, newGL_RENDERBUFFER_BINDING);
                gl.ThrowIfError();

                oldGL_RENDERBUFFER_BINDING = GL_RENDERBUFFER_BINDING.Update(newGL_RENDERBUFFER_BINDING);
            }
        }

        private void Apply_UseProgram()
        {
            gl.UseProgram(newGL_CURRENT_PROGRAM);
            gl.ThrowIfError();

            oldGL_CURRENT_PROGRAM = OpenGLState.GL_CURRENT_PROGRAM.Update(newGL_CURRENT_PROGRAM);
        }

        private void Apply_CreateElementArrayBuffer(out UInt32 buffer)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                buffer = gl.CreateBuffer();
                gl.ThrowIfError();
            }
            else
            {
                buffer = gl.GenBuffer();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindBuffer(gl.GL_ELEMENT_ARRAY_BUFFER, buffer);
                    gl.ThrowIfError();

                    newGL_ELEMENT_ARRAY_BUFFER_BINDING = buffer;
                    oldGL_ELEMENT_ARRAY_BUFFER_BINDING = OpenGLState.GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(buffer);
                }
            }
        }

        private void Apply_CreateArrayBuffer(out UInt32 buffer)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                buffer = gl.CreateBuffer();
                gl.ThrowIfError();
            }
            else
            {
                buffer = gl.GenBuffer();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindBuffer(gl.GL_ARRAY_BUFFER, buffer);
                    gl.ThrowIfError();

                    newGL_ARRAY_BUFFER_BINDING = buffer;
                    oldGL_ARRAY_BUFFER_BINDING = OpenGLState.GL_ARRAY_BUFFER_BINDING.Update(buffer);
                }
            }
        }

        private void Apply_CreateTexture2D(out UInt32 texture)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                texture = gl.CreateTexture(gl.GL_TEXTURE_2D);
                gl.ThrowIfError();
            }
            else
            {
                texture = gl.GenTexture();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindTexture(gl.GL_TEXTURE_2D, texture);
                    gl.ThrowIfError();

                    newGL_TEXTURE_BINDING_2D = texture;
                    oldGL_TEXTURE_BINDING_2D = GL_TEXTURE_BINDING_2D.Update(texture);
                    glTextureBinding2DByTextureUnit[GL_ACTIVE_TEXTURE] = texture;
                }
            }
        }

        private void Apply_CreateFramebuffer(out UInt32 framebuffer)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                framebuffer = gl.CreateFramebuffer();
            }
            else
            {
                framebuffer = gl.GenFramebuffer();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindFramebuffer(gl.GL_FRAMEBUFFER, framebuffer);
                    gl.ThrowIfError();

                    newGL_FRAMEBUFFER_BINDING = framebuffer;
                    oldGL_FRAMEBUFFER_BINDING = OpenGLState.GL_FRAMEBUFFER_BINDING.Update(framebuffer);
                }
            }            
        }

        private void Apply_CreateRenderbuffer(out UInt32 renderbuffer)
        {
            if (SupportsDirectStateAccessCreateFunctions)
            {
                renderbuffer = gl.CreateRenderbuffer();
            }
            else
            {
                renderbuffer = gl.GenRenderbuffer();
                gl.ThrowIfError();

                if (!gl.IsDirectStateAccessAvailable)
                {
                    gl.BindRenderbuffer(gl.GL_RENDERBUFFER, renderbuffer);
                    gl.ThrowIfError();

                    newGL_RENDERBUFFER_BINDING = renderbuffer;
                    oldGL_RENDERBUFFER_BINDING = GL_RENDERBUFFER_BINDING.Update(renderbuffer);
                }
            }
        }

        /// <summary>
        /// Resets the OpenGL context to its values prior to this object's application.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
                return;

            switch (stateType)
            {
                case OpenGLStateType.None:
                    break;

                case OpenGLStateType.ActiveTexture:
                    Dispose_ActiveTexture();
                    break;

                case OpenGLStateType.BindTexture2D:
                    Dispose_BindTexture2D();
                    break;

                case OpenGLStateType.BindVertexArrayObject:
                    Dispose_BindVertexArrayObject();
                    break;

                case OpenGLStateType.BindArrayBuffer:
                    Dispose_BindArrayBuffer();
                    break;

                case OpenGLStateType.BindElementArrayBuffer:
                    Dispose_BindElementArrayBuffer();
                    break;

                case OpenGLStateType.BindFramebuffer:
                    Dispose_BindFramebuffer();
                    break;

                case OpenGLStateType.BindRenderbuffer:
                    Dispose_BindRenderbuffer();
                    break;

                case OpenGLStateType.UseProgram:
                    Dispose_UseProgram();
                    break;

                case OpenGLStateType.CreateTexture2D:
                    Dispose_CreateTexture2D();
                    break;

                case OpenGLStateType.CreateArrayBuffer:
                    Dispose_CreateArrayBuffer();
                    break;

                case OpenGLStateType.CreateElementArrayBuffer:
                    Dispose_CreateElementArrayBuffer();
                    break;

                case OpenGLStateType.CreateFramebuffer:
                    Dispose_CreateFramebuffer();
                    break;

                case OpenGLStateType.CreateRenderbuffer:
                    Dispose_CreateRenderbuffer();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            OpenGLState.VerifyCache();

            disposed = true;
            pool.Release(this);
        }

        private void Dispose_ActiveTexture()
        {
            gl.ActiveTexture(oldGL_ACTIVE_TEXTURE);
            gl.ThrowIfError();

            var tb2d = 0u;
            glTextureBinding2DByTextureUnit.TryGetValue(oldGL_ACTIVE_TEXTURE, out tb2d);

            OpenGLState.GL_ACTIVE_TEXTURE.Update(oldGL_ACTIVE_TEXTURE);
            GL_TEXTURE_BINDING_2D.Update(tb2d);
        }

        private void Dispose_BindTexture2D()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindTexture(gl.GL_TEXTURE_2D, oldGL_TEXTURE_BINDING_2D);
                gl.ThrowIfError();

                GL_TEXTURE_BINDING_2D.Update(oldGL_TEXTURE_BINDING_2D);
                glTextureBinding2DByTextureUnit[GL_ACTIVE_TEXTURE] = oldGL_TEXTURE_BINDING_2D;
            }
        }

        private void Dispose_BindVertexArrayObject()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindVertexArray(oldGL_VERTEX_ARRAY_BINDING);
                gl.ThrowIfError();

                OpenGLState.GL_VERTEX_ARRAY_BINDING.Update(oldGL_VERTEX_ARRAY_BINDING);
                OpenGLState.GL_ARRAY_BUFFER_BINDING.Update(oldGL_ARRAY_BUFFER_BINDING);
                OpenGLState.GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(oldGL_ELEMENT_ARRAY_BUFFER_BINDING);
            }
        }

        private void Dispose_BindArrayBuffer()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindBuffer(gl.GL_ARRAY_BUFFER, oldGL_ARRAY_BUFFER_BINDING);
                gl.ThrowIfError();

                OpenGLState.GL_ARRAY_BUFFER_BINDING.Update(oldGL_ARRAY_BUFFER_BINDING);
            }
        }

        private void Dispose_BindElementArrayBuffer()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindBuffer(gl.GL_ELEMENT_ARRAY_BUFFER, oldGL_ELEMENT_ARRAY_BUFFER_BINDING);
                gl.ThrowIfError();

                OpenGLState.GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(oldGL_ELEMENT_ARRAY_BUFFER_BINDING);
            }
        }

        private void Dispose_BindFramebuffer()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindFramebuffer(gl.GL_FRAMEBUFFER, oldGL_FRAMEBUFFER_BINDING);
                gl.ThrowIfError();

                OpenGLState.GL_FRAMEBUFFER_BINDING.Update(oldGL_FRAMEBUFFER_BINDING);
            }
        }

        private void Dispose_BindRenderbuffer()
        {
            if (forced || !gl.IsDirectStateAccessAvailable)
            {
                gl.BindRenderbuffer(gl.GL_RENDERBUFFER, oldGL_RENDERBUFFER_BINDING);
                gl.ThrowIfError();

                GL_RENDERBUFFER_BINDING.Update(oldGL_RENDERBUFFER_BINDING);
            }
        }

        private void Dispose_UseProgram()
        {
            gl.UseProgram(oldGL_CURRENT_PROGRAM);
            gl.ThrowIfError();

            OpenGLState.GL_CURRENT_PROGRAM.Update(oldGL_CURRENT_PROGRAM);
        }

        private void Dispose_CreateArrayBuffer()
        {
            if (!gl.IsDirectStateAccessAvailable)
            {
                gl.BindBuffer(gl.GL_ARRAY_BUFFER, oldGL_ARRAY_BUFFER_BINDING);
                gl.ThrowIfError();

                OpenGLState.GL_ARRAY_BUFFER_BINDING.Update(oldGL_ARRAY_BUFFER_BINDING);
            }
        }

        private void Dispose_CreateElementArrayBuffer()
        {
            if (!gl.IsDirectStateAccessAvailable)
            {
                gl.BindBuffer(gl.GL_ELEMENT_ARRAY_BUFFER, oldGL_ELEMENT_ARRAY_BUFFER_BINDING);
                gl.ThrowIfError();

                OpenGLState.GL_ELEMENT_ARRAY_BUFFER_BINDING.Update(oldGL_ELEMENT_ARRAY_BUFFER_BINDING);
            }
        }

        private void Dispose_CreateTexture2D()
        {
            if (!gl.IsDirectStateAccessAvailable)
            {
                gl.BindTexture(gl.GL_TEXTURE_2D, oldGL_TEXTURE_BINDING_2D);
                gl.ThrowIfError();

                GL_TEXTURE_BINDING_2D.Update(oldGL_TEXTURE_BINDING_2D);
                glTextureBinding2DByTextureUnit[GL_ACTIVE_TEXTURE] = oldGL_TEXTURE_BINDING_2D;
            }
        }

        private void Dispose_CreateFramebuffer()
        {
            if (!gl.IsDirectStateAccessAvailable)
            {
                gl.BindFramebuffer(gl.GL_FRAMEBUFFER, oldGL_FRAMEBUFFER_BINDING);
                gl.ThrowIfError();

                OpenGLState.GL_FRAMEBUFFER_BINDING.Update(oldGL_FRAMEBUFFER_BINDING);
            }
        }

        private void Dispose_CreateRenderbuffer()
        {
            if (!gl.IsDirectStateAccessAvailable)
            {
                gl.BindRenderbuffer(gl.GL_RENDER, oldGL_RENDERBUFFER_BINDING);
                gl.ThrowIfError();

                GL_RENDERBUFFER_BINDING.Update(oldGL_RENDERBUFFER_BINDING);
            }
        }

        /// <summary>
        /// Gets the cached value of GL_ACTIVE_TEXTURE.
        /// </summary>
        public static OpenGLStateInteger GL_ACTIVE_TEXTURE { get { return glActiveTexture; } }

        /// <summary>
        /// Gets the cached value of GL_TEXTURE_BINDING_2D.
        /// </summary>
        public static OpenGLStateInteger GL_TEXTURE_BINDING_2D { get { return glTextureBinding2D; } }

        /// <summary>
        /// Gets the cached value of GL_VERTEX_ARRAY_BINDING.
        /// </summary>
        public static OpenGLStateInteger GL_VERTEX_ARRAY_BINDING { get { return glVertexArrayBinding; } }

        /// <summary>
        /// Gets the cached value of GL_ARRAY_BUFFER_BINDING.
        /// </summary>
        public static OpenGLStateInteger GL_ARRAY_BUFFER_BINDING { get { return glArrayBufferBinding; } }

        /// <summary>
        /// Gets the cached value of GL_ELEMENT_ARRAY_BUFFER_BINDING.
        /// </summary>
        public static OpenGLStateInteger GL_ELEMENT_ARRAY_BUFFER_BINDING { get { return glElementArrayBufferBinding; } }

        /// <summary>
        /// Gets the cached value of GL_FRAMEBUFFER_BINDING.
        /// </summary>
        public static OpenGLStateInteger GL_FRAMEBUFFER_BINDING { get { return glFramebufferBinding; } }

        /// <summary>
        /// Gets the cached value of GL_RENDERBUFFER_BINDING.
        /// </summary>
        public static OpenGLStateInteger GL_RENDERBUFFER_BINDING { get { return glRenderbufferBinding; } }

        /// <summary>
        /// Gets the cached value of GL_CURRENT_PROGRAM.
        /// </summary>
        public static OpenGLStateInteger GL_CURRENT_PROGRAM { get { return glCurrentProgram; } }

        /// <summary>
        /// Gets a value indicating whether the current OpenGL context has support for vertex array objects (VAOs).
        /// </summary>
        public static Boolean SupportsVertexArrayObjects { get { return !gl.IsGLES || gl.IsVersionAtLeast(3, 0); } }

        /// <summary>
        /// Gets a value indicating whether the current OpenGL context has support for the glCreateX() functions provided by
        /// ARB_direct_state_access or OpenGL 4.5.
        /// </summary>
        public static Boolean SupportsDirectStateAccessCreateFunctions { get { return gl.IsVersionAtLeast(4, 5) || gl.IsARBDirectStateAccessAvailable; } }

        // State values.
        private OpenGLStateType stateType;
        private Boolean disposed;
        private Boolean forced;

        private UInt32 newGL_ACTIVE_TEXTURE = gl.GL_TEXTURE0;
        private UInt32 newGL_TEXTURE_BINDING_2D;
        private UInt32 newGL_VERTEX_ARRAY_BINDING;
        private UInt32 newGL_ARRAY_BUFFER_BINDING;
        private UInt32 newGL_ELEMENT_ARRAY_BUFFER_BINDING;
        private UInt32 newGL_FRAMEBUFFER_BINDING;
        private UInt32 newGL_RENDERBUFFER_BINDING;
        private UInt32 newGL_CURRENT_PROGRAM;

        private UInt32 oldGL_ACTIVE_TEXTURE = gl.GL_TEXTURE0;
        private UInt32 oldGL_TEXTURE_BINDING_2D;
        private UInt32 oldGL_VERTEX_ARRAY_BINDING;
        private UInt32 oldGL_ARRAY_BUFFER_BINDING;
        private UInt32 oldGL_ELEMENT_ARRAY_BUFFER_BINDING;
        private UInt32 oldGL_FRAMEBUFFER_BINDING;
        private UInt32 oldGL_RENDERBUFFER_BINDING;
        private UInt32 oldGL_CURRENT_PROGRAM;
        
        // Cached OpenGL state values.
        private static readonly OpenGLStateInteger[] glCachedIntegers;
        private static readonly OpenGLStateInteger glActiveTexture             = new OpenGLStateInteger("GL_ACTIVE_TEXTURE", gl.GL_ACTIVE_TEXTURE, (int)gl.GL_TEXTURE0);
        private static readonly OpenGLStateInteger glTextureBinding2D          = new OpenGLStateInteger("GL_TEXTURE_BINDING_2D", gl.GL_TEXTURE_BINDING_2D);
        private static readonly OpenGLStateInteger glVertexArrayBinding        = new OpenGLStateInteger("GL_VERTEX_ARRAY_BINDING", gl.GL_VERTEX_ARRAY_BINDING);
        private static readonly OpenGLStateInteger glArrayBufferBinding        = new OpenGLStateInteger("GL_ARRAY_BUFFER_BINDING", gl.GL_ARRAY_BUFFER_BINDING);
        private static readonly OpenGLStateInteger glElementArrayBufferBinding = new OpenGLStateInteger("GL_ELEMENT_ARRAY_BUFFER_BINDING", gl.GL_ELEMENT_ARRAY_BUFFER_BINDING);
        private static readonly OpenGLStateInteger glFramebufferBinding        = new OpenGLStateInteger("GL_FRAMEBUFFER_BINDING", gl.GL_FRAMEBUFFER_BINDING);
        private static readonly OpenGLStateInteger glRenderbufferBinding       = new OpenGLStateInteger("GL_RENDERBUFFER_BINDING", gl.GL_RENDERBUFFER_BINDING);
        private static readonly OpenGLStateInteger glCurrentProgram            = new OpenGLStateInteger("GL_CURRENT_PROGRAM", gl.GL_CURRENT_PROGRAM);

        private static readonly Dictionary<UInt32, UInt32> glTextureBinding2DByTextureUnit = 
            new Dictionary<UInt32, UInt32>();

        // The pool of state objects.
        private static readonly IPool<OpenGLState> pool = new ExpandingPool<OpenGLState>(1, () => new OpenGLState());
    }
}

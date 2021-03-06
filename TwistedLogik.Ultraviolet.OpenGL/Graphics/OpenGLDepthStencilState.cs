﻿using System;
using TwistedLogik.Gluon;
using TwistedLogik.Nucleus;
using TwistedLogik.Ultraviolet.Graphics;

namespace TwistedLogik.Ultraviolet.OpenGL.Graphics
{
    /// <summary>
    /// Represents the OpenGL/SDL2 implementation of the DepthStencilState class.
    /// </summary>
    public class OpenGLDepthStencilState : DepthStencilState
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLDepthStencilState class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        public OpenGLDepthStencilState(UltravioletContext uv)
            : base(uv)
        {

        }

        /// <summary>
        /// Creates the Default depth/stencil state.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <returns>The depth/stencil state that was created.</returns>
        public static OpenGLDepthStencilState CreateDefault(UltravioletContext uv)
        {
            var state = new OpenGLDepthStencilState(uv);
            state.DepthBufferEnable = true;
            state.DepthBufferWriteEnable = true;
            state.MakeImmutable();
            return state;
        }
        
        /// <summary>
        /// Creates the DepthRead depth/stencil state.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <returns>The depth/stencil state that was created.</returns>
        public static OpenGLDepthStencilState CreateDepthRead(UltravioletContext uv)
        {
            var state = new OpenGLDepthStencilState(uv);
            state.DepthBufferEnable = true;
            state.DepthBufferWriteEnable = true;
            state.MakeImmutable();
            return state;
        }

        /// <summary>
        /// Creates the None depth/stencil state.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <returns>The depth/stencil state that was created.</returns>
        public static OpenGLDepthStencilState CreateNone(UltravioletContext uv)
        {
            var state = new OpenGLDepthStencilState(uv);
            state.DepthBufferEnable = false;
            state.DepthBufferWriteEnable = false;
            state.MakeImmutable();
            return state;
        }

        /// <summary>
        /// Applies the depth/stencil state to the device.
        /// </summary>
        internal void Apply()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            gl.Enable(gl.GL_DEPTH_TEST, DepthBufferEnable);
            gl.ThrowIfError();

            gl.DepthMask(DepthBufferWriteEnable);
            gl.ThrowIfError();

            gl.DepthFunc(GetCompareFunctionGL(DepthBufferFunction));
            gl.ThrowIfError();

            gl.Enable(gl.GL_STENCIL_TEST, StencilEnable);
            gl.ThrowIfError();

            if (TwoSidedStencilMode)
            {
                gl.StencilFuncSeparate(gl.GL_FRONT, GetCompareFunctionGL(StencilFunction),
                    ReferenceStencil, (uint)StencilMask);
                gl.ThrowIfError();

                gl.StencilFuncSeparate(gl.GL_BACK, GetCompareFunctionGL(CounterClockwiseStencilFunction),
                    ReferenceStencil, (uint)StencilMask);
                gl.ThrowIfError();

                gl.StencilOpSeparate(gl.GL_FRONT,
                    GetStencilOpGL(StencilFail),
                    GetStencilOpGL(StencilDepthBufferFail),
                    GetStencilOpGL(StencilPass));
                gl.ThrowIfError();

                gl.StencilOpSeparate(gl.GL_BACK,
                    GetStencilOpGL(CounterClockwiseStencilFail),
                    GetStencilOpGL(CounterClockwiseStencilDepthBufferFail),
                    GetStencilOpGL(CounterClockwiseStencilPass));
                gl.ThrowIfError();
            }
            else
            {
                gl.StencilFunc(GetCompareFunctionGL(StencilFunction), ReferenceStencil, (uint)StencilMask);
                gl.ThrowIfError();

                gl.StencilOp(
                    GetStencilOpGL(StencilFail),
                    GetStencilOpGL(StencilDepthBufferFail),
                    GetStencilOpGL(StencilPass));
                gl.ThrowIfError();
            }
        }

        /// <summary>
        /// Gets the OpenGL enum value that corresponds to the specified CompareFunction value.
        /// </summary>
        /// <param name="func">The CompareFunction value to convert.</param>
        /// <returns>The converted value.</returns>
        private static UInt32 GetCompareFunctionGL(CompareFunction func)
        {
            switch (func)
            {
                case CompareFunction.Always:
                    return gl.GL_ALWAYS;
                case CompareFunction.Never:
                    return gl.GL_NEVER;
                case CompareFunction.Equal:
                    return gl.GL_EQUAL;
                case CompareFunction.NotEqual:
                    return gl.GL_NOTEQUAL;
                case CompareFunction.Greater:
                    return gl.GL_GREATER;
                case CompareFunction.GreaterEqual:
                    return gl.GL_GEQUAL;
                case CompareFunction.Less:
                    return gl.GL_LESS;
                case CompareFunction.LessEqual:
                    return gl.GL_LEQUAL;
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the OpenGL enum value that corresponds to the specified StencilOperation value.
        /// </summary>
        /// <param name="op">The StencilOperation value to convert.</param>
        /// <returns>The converted value.</returns>
        private static UInt32 GetStencilOpGL(StencilOperation op)
        {
            switch (op)
            {
                case StencilOperation.Decrement:
                    return gl.GL_DECR_WRAP;
                case StencilOperation.DecrementSaturation:
                    return gl.GL_DECR;
                case StencilOperation.Increment:
                    return gl.GL_INCR_WRAP;
                case StencilOperation.IncrementSaturation:
                    return gl.GL_INCR;
                case StencilOperation.Invert:
                    return gl.GL_INVERT;
                case StencilOperation.Keep:
                    return gl.GL_KEEP;
                case StencilOperation.Replace:
                    return gl.GL_REPLACE;
                case StencilOperation.Zero:
                    return gl.GL_ZERO;
            }
            throw new NotSupportedException();
        }
    }
}

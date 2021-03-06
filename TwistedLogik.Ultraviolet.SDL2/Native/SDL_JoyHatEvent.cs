﻿using System;
using System.Runtime.InteropServices;
using SDL_JoystickID = System.Int32;

#pragma warning disable 1591

namespace TwistedLogik.Ultraviolet.SDL2.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_JoyHatEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public SDL_JoystickID which;
        public Byte hat;
        public Byte value;
        public Byte padding1;
        public Byte padding2;
    }
}

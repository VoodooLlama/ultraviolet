﻿using TwistedLogik.Ultraviolet.Content;
using TwistedLogik.Ultraviolet.Graphics;

namespace TwistedLogik.Ultraviolet.SDL2
{
    /// <summary>
    /// Represents a content processor that loads cursor collections.
    /// </summary>
    internal class CursorCollectionProcessor : ContentProcessor<CursorCollectionDescription, CursorCollection>
    {
        /// <inheritdoc/>
        public override CursorCollection Process(ContentManager manager, IContentProcessorMetadata metadata, CursorCollectionDescription input)
        {
            var collection = new CursorCollection(manager.Ultraviolet);
            var texture = ResolveDependencyAssetPath(metadata, input.Texture);
            using (var textureSurface = manager.Load<Surface2D>(texture, false))
            {
                if (input.Cursors != null)
                {
                    foreach (var cursorDesc in input.Cursors)
                    {
                        using (var cursorSurface = textureSurface.CreateSurface(cursorDesc.Area))
                        {
                            var cursor = Cursor.Create(cursorSurface, cursorDesc.Hotspot.X, cursorDesc.Hotspot.Y);
                            collection[cursorDesc.Name] = cursor;
                        }
                    }
                }
            }
            return collection;
        }
    }
}
